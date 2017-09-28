using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace ProStudCreator
{
    public class TaskHandler
    {

        private static ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        public static void CheckAllTasks() //register all Methods which check for tasks here.
        {
            CheckGradesRegistered();
            Debug.WriteLine("Checking for new Taksks: " + DateTime.Now);


            //Write all the Mails
            WriteAllMails();
        }


        private static void CheckGradesRegistered()
        {
            var allActiveGradeTasks = db.Tasks.Where(t => !t.Done && t.Project != null && t.TaskType.GradesRegistered).Select(i => i.ProjectId);
            var allProjects = db.Projects.Where(p => true).AsEnumerable();

            foreach (var project in allProjects.Where(p => p.GetEndSemester(db) == Semester.LastSemester(db)))
            {
                if ((project.LogGradeStudent1 == null) || (!string.IsNullOrEmpty(project.LogStudent2Mail) && project.LogGradeStudent2 == null))
                {
                    if (!allActiveGradeTasks.Contains(project.Id))
                    {
                        db.Tasks.InsertOnSubmit(new Task
                        {
                            ProjectId = project.Id,
                            ResponsibleUser =
                                db.UserDepartmentMap.Single(
                                    p => p.Mail == (string.IsNullOrEmpty(project.Advisor1Mail)
                                             ? project.Advisor2Mail
                                             : project.Advisor1Mail)),
                            TaskType = db.TaskTypes.Single(t => t.GradesRegistered),
                            Supervisor =
                                db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                        });
                    }
                }
            }

            checkForFinishedGradesRegistered();

            db.SubmitChanges();
        }

        private static void checkForFinishedGradesRegistered()
        {
            var openGradeTasks =
                db.Tasks.Where(i => !i.Done && i.TaskType == db.TaskTypes.Single(t => t.GradesRegistered));

            foreach (var openTask in openGradeTasks)
            {
                if (openTask.Project.LogGradeStudent1.HasValue &&
                    (openTask.Project.LogGradeStudent2.HasValue ||
                     !string.IsNullOrEmpty(openTask.Project.LogStudent2Mail)))
                {
                    openTask.Done = true;
                }
            }
            db.SubmitChanges();
        }


        #region Mailing
        private static void WriteAllMails()
        {
            sendMails(GenerateEmails(GetAllTasksToMail()));
        }


        private static IEnumerable<Task> GetAllTasksToMail() //gets all tasks which got last reminded more than 3 days ago
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
                else if (task.LastReminded?.AddDays(3) < DateTime.Now || task.LastReminded == null)
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

                if (!task.isAddedToList())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(task.ResponsibleUser.Mail));
                    mail.Subject = "Offene Aufgaben bei Prostud";
                    mail.IsBodyHtml = true;

                    mailMessage.Append("<h4>Hallo<h4>"
                                        + "<p>Du hast noch folgende offene Aufgaben:</p><ul>");

                    foreach (var underTask in copyTasksToMail)
                    {
                        if (underTask.ResponsibleUserId != -1 && underTask.ResponsibleUserId == task.ResponsibleUserId)
                        {
                            underTask.AddToList();
                            mailMessage.Append(task.Project != null ? "<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectInfoPage?id=" +
                                               task.ProjectId + $"\">{underTask.TaskType.Description} beim Projekt {underTask.Project.Name}</a><li>" : "<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ \">{task.TaskType.Description}</a><li>");
                        }
                    }

                    mailMessage.Append("</ul>"
                        + "<br/>"
                        + "<p>Freundliche Grüsse</p><br/>"
                        + "Dein ProStud-Team"
                        );

                    mail.Body = mailMessage.ToString();
                    mailsToSend.Add(mail);
                }
            }
            foreach (var task in copyTasksToMail)
            {
                task.RemoveFromList();
            }

            return mailsToSend.ToArray();
        }


        private static void sendMails(MailMessage[] mails) //can be enhanced with buffering all mails for ex. 3 days
        {
            var smtpClient = new SmtpClient();

            foreach (var mail in mails)
            {
                smtpClient.Send(mail);
            }
        }
        #endregion
    }
}