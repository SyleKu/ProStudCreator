using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using NPOI.Util;

namespace ProStudCreator
{
    public class TaskHandler
    {

        public static void CheckAllTasks() //register all Methods which check for tasks here.
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                CheckGradesRegistered(db);


                //Write all the Mails
                WriteAllMails(db);

            }

        }


        private static void CheckGradesRegistered(ProStudentCreatorDBDataContext db)
        {
            var allActiveGradeTasks = db.Tasks.Where(t => !t.Done && t.Project != null && t.TaskType.GradesRegistered)
                .Select(i => i.ProjectId);

            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published).ToList();

            foreach (var project in allPublishedProjects.Where(p => p.GetProjectRelevantForGradeTasks(db)))
            {
                if ((project.LogGradeStudent1 == null && !string.IsNullOrEmpty(project.LogStudent1Mail)) ||
                    (!string.IsNullOrEmpty(project.LogStudent2Mail) && project.LogGradeStudent2 == null))
                {
                    if (!allActiveGradeTasks.Contains(project.Id))
                    {
                        db.Tasks.InsertOnSubmit(new Task
                        {
                            ProjectId = project.Id,
                            ResponsibleUser =
                                db.UserDepartmentMap.Single(
                                    p => p.Mail == (!project.Advisor1Id.HasValue
                                             ? project.Advisor2.Mail
                                             : project.Advisor1.Mail)),
                            TaskType = db.TaskTypes.Single(t => t.GradesRegistered),
                            Supervisor =
                                db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                        });
                    }
                }
            }

            checkForFinishedGradesRegistered(db);

            db.SubmitChanges();
        }

        private static void checkForFinishedGradesRegistered(ProStudentCreatorDBDataContext db)
        {
            var openGradeTasks =
                db.Tasks.Where(i => !i.Done && i.TaskType == db.TaskTypes.Single(t => t.GradesRegistered));

            foreach (var openTask in openGradeTasks)
            {
                if (openTask.Project.LogGradeStudent1.HasValue &&
                    (openTask.Project.LogGradeStudent2.HasValue ||
                     string.IsNullOrEmpty(openTask.Project.LogStudent2Mail)))
                {
                    openTask.Done = true;
                }
            }
            db.SubmitChanges();
        }


        #region Mailing
        private static void WriteAllMails(ProStudentCreatorDBDataContext db)
        {
            sendMails(GenerateEmails(GetAllTasksToMail(db)));
        }


        private static IEnumerable<Task> GetAllTasksToMail(ProStudentCreatorDBDataContext db)
        {
            var openTasks = db.Tasks.Where(i => !i.Done);
            var tasksToMail = new List<Task>();


            foreach (var task in openTasks)
            {
                if (task.TaskType.RemindType.RemindOnce)
                {
                    tasksToMail.Add(task);
                    task.Done = true;
                }
                else if (DateTime.Now.Subtract(task.LastReminded ?? DateTime.Now).Ticks > task.TaskType.TicksBetweenReminds || task.LastReminded == null)
                {
                    tasksToMail.Add(task);
                    task.LastReminded = DateTime.Now;
                }
            }

            db.SubmitChanges();

            return tasksToMail;
        }



        private static MailMessage[] GenerateEmails(IEnumerable<Task> taskToMail)
        {
            var mailsToSend = new List<MailMessage>();

            Task[] copyTasksToMail = new Task[taskToMail.Count()];
            Array.Copy(taskToMail.ToArray(), copyTasksToMail, taskToMail.Count());

            foreach (var task in copyTasksToMail)
            {
                var mailMessage = new StringBuilder();

                if (!task.AlreadyChecked)
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(task.ResponsibleUser.Mail));
                    mail.Subject = "Offene Aufgaben bei Prostud";
                    mail.IsBodyHtml = true;
                    mailMessage.Append("<div style=\"font-family: Arial\">");
                    mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {task.ResponsibleUser.Name.Split(' ')[0]}<p>"
                                        + "<p>Du hast noch folgende offene Aufgaben:</p><ul>");

                    foreach (var underTask in copyTasksToMail)
                    {
                        if (underTask.ResponsibleUserId == task.ResponsibleUserId)
                        {
                            if (underTask.DueDate != null && DateTime.Now.AddDays(3) > underTask.DueDate)
                            {
                                mail.CC.Add(underTask.Supervisor?.Mail ?? "");
                            }
                            underTask.AlreadyChecked = true;
                            mailMessage.Append(task.Project != null ? "<li>" + $"{underTask.TaskType.Description} beim Projekt <a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectInfoPage?id={underTask.ProjectId}\">{underTask.Project.Name}</a></li>" : "<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ \">{task.TaskType.Description}</a></li>");
                        }
                    }

                    mailMessage.Append("</ul>"
                        + "<p>Bitte erledige diese so schnell wie möglich, Danke.</p>"
                        + "<br/>"
                        + "<p>Freundliche Grüsse</p>"
                        + "Dein ProStud-Team"
                        );

                    mailMessage.Append("</div>");
                    mail.Body = mailMessage.ToString();
                    mailsToSend.Add(mail);
                }
            }
            foreach (var task in copyTasksToMail)
            {
                task.AlreadyChecked = false;
            }

            return mailsToSend.ToArray();
        }


        private static void sendMails(MailMessage[] mails) //can be enhanced with buffering all mails for ex. 3 days
        {
            var smtpClient = new SmtpClient();

            foreach (var mail in mails)
            {
#if !DEBUG
                smtpClient.Send(mail);
#endif
            }
        }
#endregion
    }

}