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
        enum Type : int
        {
            RegisterGrades = 1,
            CheckWebsummary = 2,
            StartProject = 4,
            FinishProject = 5,
            UploadResults = 6,
            PlanDefenses = 7, //TODO
            UpdateDefenseDates = 8, //TODO
            PayExperts = 9,
            InsertNewSemesters = 10,
            SendGrades = 11,
            SendMarKomBrochure = 12,
            InvoiceCustomers = 13, //TODO
            EnterAssignedStudents = 14
        }


        public static void CheckAllTasks() //register all Methods which check for tasks here.
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                CheckGradesRegistered(db);
                CheckWebsummaryChecked(db);
                CheckUploadResults(db);

                InfoStartProject(db);
                InfoFinishProject(db);

                InfoInsertNewSemesters(db);

                EnterAssignedStudents(db);

                SendMailsToResponsibleUsers(db);

                SendGradesToAdmin(db);
                SendPayExperts(db);
                //SendInvoiceCustomers(db);
            }
        }




        private static void EnterAssignedStudents(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.EnterAssignedStudents);

            var cutoffDate = new DateTime(2018, 1, 1);

            //add new tasks for projects
            var allActiveAssignStudentsTasks = db.Tasks.Where(t => !t.Done && t.TaskType == type).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && (p.LogStudent1Mail == null || p.LogStudent1Name == null || p.LogProjectDuration==null || p.LogProjectType==null) && p.Semester.StartDate <= DateTime.Now && p.Semester.StartDate >= cutoffDate).ToList();

            foreach (var project in allPublishedProjects)
                if (!allActiveAssignStudentsTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department),
                        TaskType = type,
                        Supervisor = db.UserDepartmentMap.Single(i => i.Mail == Global.WebAdmin)
                    });

            //mark registered tasks as done
            var openAssignStudentsTasks = db.Tasks.Where(i => !i.Done && i.TaskType == type).ToList();
            foreach (var openTask in openAssignStudentsTasks)
                if ((!string.IsNullOrEmpty(openTask.Project.LogStudent1Mail) && !string.IsNullOrEmpty(openTask.Project.LogStudent1Name) && openTask.Project.LogProjectDuration!=null && openTask.Project.LogProjectType!=null) || openTask.Project.State!=ProjectState.Published)
                    openTask.Done = true;

            db.SubmitChanges();
        }





        //public static void SendInvoiceCustomers(ProStudentCreatorDBDataContext db)
        //{
        //    var type = db.TaskTypes.Single(t => t.Type == (int)Type.InvoiceCustomers);

        //    var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && && t.TaskType == type);
        //    if (activeTask == null)
        //    {
        //        activeTask = new Task()
        //        {
        //            TaskType = type,
        //        };
        //        db.Tasks.InsertOnSubmit(activeTask);
        //        db.SubmitChanges();
        //    }

        //    if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).Ticks > type.TicksBetweenReminds)
        //    {
        //        activeTask.LastReminded = DateTime.Now;

        //        var unpaidExperts = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && p.WebSummaryChecked && !p.LogExpertPaid && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && p.Expert != null).OrderBy(p => p.Expert.Name).ThenBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr).ToList();

        //        unpaidExperts = unpaidExperts.Where(p => p.WasDefenseHeld()).ToList();
        //        if (unpaidExperts.Any())
        //            using (var smtpClient = new SmtpClient())
        //            {
        //                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
        //                mail.To.Add(new MailAddress(Global.PayExpertAdmin));
        //                mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
        //                mail.IsBodyHtml = true;

        //                var mailMessage = new StringBuilder();
        //                mailMessage.Append(
        //                    "<div style=\"font-family: Arial\">" +
        //                    "<p>Liebe Administration<p>" +
        //                    "<p>Bitte die Auszahlung von den folgenden Expertenhonoraren veranlassen:</p>" +
        //                    "<table>" +
        //                    "<tr>" +
        //                        "<th>Experte</th>" +
        //                        "<th>Semester</th>" +
        //                        "<th>Studierende</th>" +
        //                        "<th>Betreuer</th>" +
        //                        "<th>Projekttitel</th>" +
        //                    "</tr>");

        //                foreach (var p in unpaidExperts)
        //                {
        //                    p.LogExpertPaid = true;

        //                    mailMessage.Append(
        //                    "<tr>" +
        //                        $"<td>{p.Expert.Name}</td>" +
        //                        $"<td>{p.Semester.Name}</td>" +
        //                        $"<td>{p.LogStudent1Mail + (p.LogStudent2Mail != null ? ", " + p.LogStudent2Mail : "")}</td>" +
        //                        $"<td>{p.Advisor1.Mail}</td>" +
        //                        $"<td>{p.GetFullTitle()}</td>" +
        //                    "</tr>"
        //                    );
        //                }

        //                mailMessage.Append(
        //                    "</table>" +
        //                    "<br/>" +
        //                    "<p>Herzliche Grüsse,<br/>" +
        //                    "Dein ProStud-Team</p>" +
        //                    $"<p>Feedback an {Global.WebAdmin}</p>" +
        //                    "</div>"
        //                    );

        //                mail.Body = mailMessage.ToString();
        //                smtpClient.Send(mail);
        //            }
        //    }

        //    db.SubmitChanges();
        //}


        public static void SendPayExperts(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.PayExperts);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type);
            if (activeTask == null)
            {
                activeTask = new Task()
                {
                    TaskType = type,
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var unpaidExperts = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && p.WebSummaryChecked && !p.LogExpertPaid && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && p.Expert != null).OrderBy(p => p.Expert.Name).ThenBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr).ToList();

                unpaidExperts = unpaidExperts.Where(p => p.WasDefenseHeld()).ToList();
                if (unpaidExperts.Any())
                    using (var smtpClient = new SmtpClient())
                    {
                        var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                        mail.To.Add(new MailAddress(Global.PayExpertAdmin));
                        mail.CC.Add(new MailAddress(Global.WebAdmin));
                        mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
                        mail.IsBodyHtml = true;

                        var mailMessage = new StringBuilder();
                        mailMessage.Append(
                            "<div style=\"font-family: Arial\">" +
                            "<p>Liebe Administration<p>" +
                            "<p>Bitte die Auszahlung von den folgenden Expertenhonoraren veranlassen:</p>" +
                            "<table>" +
                            "<tr>" +
                                "<th>Experte</th>" +
                                "<th>Semester</th>" +
                                "<th>Studierende</th>" +
                                "<th>Betreuer</th>" +
                                "<th>Projekttitel</th>" +
                            "</tr>");

                        foreach (var p in unpaidExperts)
                        {
                            p.LogExpertPaid = true;

                            mailMessage.Append(
                            "<tr>" +
                                $"<td>{p.Expert.Name}</td>" +
                                $"<td>{p.Semester.Name}</td>" +
                                $"<td>{p.LogStudent1Mail + (p.LogStudent2Mail != null ? ", " + p.LogStudent2Mail : "")}</td>" +
                                $"<td>{p.Advisor1.Mail}</td>" +
                                $"<td>{p.GetFullTitle()}</td>" +
                            "</tr>"
                            );
                        }

                        mailMessage.Append(
                            "</table>" +
                            "<br/>" +
                            "<p>Herzliche Grüsse,<br/>" +
                            "Dein ProStud-Team</p>" +
                            $"<p>Feedback an {Global.WebAdmin}</p>" +
                            "</div>"
                            );

                        mail.Body = mailMessage.ToString();
                        smtpClient.Send(mail);
                    }
            }

            db.SubmitChanges();
        }

        public static void SendGradesToAdmin(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendGrades);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type);
            if (activeTask == null)
            {
                activeTask = new Task()
                {
                    TaskType = type,
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var updatedProjects = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && p.WebSummaryChecked && !p.GradeSentToAdmin && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && (p.LogLanguageEnglish == true || p.LogLanguageGerman == true)).OrderBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

                if (updatedProjects.Any())
                    using (var smtpClient = new SmtpClient())
                    {
                        var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                        mail.To.Add(new MailAddress(Global.GradeAdmin));
                        mail.CC.Add(new MailAddress(Global.WebAdmin));
                        mail.Subject = "Informatikprojekte P5/P6: Neue Noten";
                        mail.IsBodyHtml = true;

                        var mailMessage = new StringBuilder();
                        mailMessage.Append(
                            "<div style=\"font-family: Arial\">" +
                            "<p>Liebe Ausbildungsadministration<p>" +
                            "<p>Es wurden neue Noten für die Informatikprojekte erfasst:</p>" +
                            "<table>" +
                            "<tr>" +
                                "<th>Mail</th>" +
                                "<th>Note</th>" +
                                "<th>Art</th>" +
                                "<th>Sprache</th>" +
                                "<th>Betreuer</th>" +
                                "<th>Projekttitel</th>" +
                            "</tr>");

                        foreach (var p in updatedProjects)
                        {
                            p.GradeSentToAdmin = true;

                            if (p.LogStudent1Mail != null && p.LogGradeStudent1 != null)
                                mailMessage.Append(
                                "<tr>" +
                                    $"<td>{p.LogStudent1Mail}</td>" +
                                    $"<td>{p.LogGradeStudent1.Value.ToString("F1")}</td>" +
                                    $"<td>{p.LogProjectType.ExportValue}</td>" +
                                    $"<td>{(p.LogLanguageGerman.Value ? "Deutsch" : "Englisch")}</td>" +
                                    $"<td>{p.Advisor1.Mail}</td>" +
                                    $"<td>{p.GetFullTitle()}</td>" +
                                "</tr>"
                                );

                            if (p.LogStudent2Mail != null && p.LogGradeStudent2 != null)
                                mailMessage.Append(
                                    "<tr>" +
                                        $"<td>{p.LogStudent2Mail}</td>" +
                                        $"<td>{p.LogGradeStudent2.Value.ToString("F1")}</td>" +
                                        $"<td>{p.LogProjectType.ExportValue}</td>" +
                                        $"<td>{(p.LogLanguageGerman.Value ? "Deutsch" : "Englisch")}</td>" +
                                        $"<td>{p.Advisor1.Mail}</td>" +
                                        $"<td>{p.GetFullTitle()}</td>" +
                                    "</tr>"
                                    );
                        }

                        mailMessage.Append(
                            "</table>" +
                            "<br/>" +
                            "<p>Herzliche Grüsse,<br/>" +
                            "Dein ProStud-Team</p>" +
                            $"<p>Feedback an {Global.WebAdmin}</p>" +
                            "</div>"
                            );

                        mail.Body = mailMessage.ToString();
                        smtpClient.Send(mail);
                    }
            }

            db.SubmitChanges();
        }

        private static void InfoInsertNewSemesters(ProStudentCreatorDBDataContext db)
        {
            var targetDate = DateTime.Now.AddMonths(18);

            var activeTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.InsertNewSemesters);
            var semesterMissing = !db.Semester.Any(s => s.StartDate <= targetDate && targetDate <= s.EndDate);

            //add new tasks for projects
            if (semesterMissing && !activeTasks.Any())
                db.Tasks.InsertOnSubmit(new Task
                {
                    ResponsibleUser = db.UserDepartmentMap.Single(i => i.Mail == Global.WebAdmin),
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.InsertNewSemesters),
                });

            if (!semesterMissing)
                foreach (var t in activeTasks)
                    t.Done = true;

            db.SubmitChanges();
        }

        private static void InfoStartProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.StartProject)).ToList();

            foreach (var project in allPublishedProjects)
                db.Tasks.InsertOnSubmit(new Task
                {
                    ProjectId = project.Id,
                    ResponsibleUser = project.Advisor1,
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.StartProject),
                    Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                });

            db.SubmitChanges();
        }

        private static void InfoFinishProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.FinishProject)).ToList();

            foreach (var project in allPublishedProjects)
            {
                var deliveryDate = project.GetDeliveryDate();

                if (deliveryDate.HasValue && deliveryDate.Value.AddDays(4 * 7) <= DateTime.Now)
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.FinishProject),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });
            }

            db.SubmitChanges();
        }

        private static void CheckGradesRegistered(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveGradeTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.RegisterGrades).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if ((project.LogGradeStudent1 == null && !string.IsNullOrEmpty(project.LogStudent1Mail)) || (!string.IsNullOrEmpty(project.LogStudent2Mail) && project.LogGradeStudent2 == null))
                    if (!allActiveGradeTasks.Contains(project.Id))
                        db.Tasks.InsertOnSubmit(new Task
                        {
                            ProjectId = project.Id,
                            ResponsibleUser = project.Advisor1,
                            TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.RegisterGrades),
                            Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                        });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.RegisterGrades).ToList();
            foreach (var openTask in openGradeTasks)
                if (openTask.Project.State!=ProjectState.Published || (openTask.Project.LogGradeStudent1.HasValue && (openTask.Project.LogGradeStudent2.HasValue || string.IsNullOrEmpty(openTask.Project.LogStudent2Mail))))
                    openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckWebsummaryChecked(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveWebsummaryTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.CheckWebsummary).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && !p.WebSummaryChecked).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if (!allActiveWebsummaryTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.CheckWebsummary),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.CheckWebsummary && (i.Project.WebSummaryChecked || i.Project.State!=ProjectState.Published)).ToList();
            foreach (var openTask in openGradeTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }



        private static void CheckUploadResults(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var activeUploadTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.UploadResults).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && !p.Attachements.Any(a => !a.Deleted) && p.BillingStatus.RequiresProjectResults).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2018, 4, 1))))
                if (!activeUploadTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.UploadResults),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openUploadTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.UploadResults &&
            (i.Project.Attachements.Any(a => !a.Deleted) || !i.Project.BillingStatus.RequiresProjectResults || i.Project.State!=ProjectState.Published)).ToList();
            foreach (var openTask in openUploadTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }


        private static void SendMailsToResponsibleUsers(ProStudentCreatorDBDataContext db)
        {
            //which tasks must be sent?
            var tasksToMail = new List<Task>();
            foreach (var task in db.Tasks.Where(i => !i.Done && i.ResponsibleUser != null))
                if ((DateTime.Now.Subtract(task.LastReminded ?? DateTime.Now).TotalDays > task.TaskType.DaysBetweenReminds) || task.LastReminded == null)
                    tasksToMail.Add(task);


            //generate emails
            var emails = new List<Tuple<Task, MailMessage>>();
            foreach (var task in tasksToMail)
            {
                var mailMessage = new StringBuilder();

                if (!task.AlreadyChecked)
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(task.ResponsibleUser.Mail));
                    mail.Subject = "Erinnerung von ProStud";
                    mail.IsBodyHtml = true;
                    mailMessage.Append("<div style=\"font-family: Arial\">");
                    mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {task.ResponsibleUser.Name.Split(' ')[0]}<p>"
                                        + "<p>Es stehen folgende Aufgaben im ProStud an:</p><ul>");

                    foreach (var underTask in tasksToMail.Where(st => st.ResponsibleUser == task.ResponsibleUser))
                    {
                        if (underTask.DueDate != null && DateTime.Now.AddDays(3) > underTask.DueDate && underTask.Supervisor != null)
                            mail.CC.Add(underTask.Supervisor.Mail);

                        underTask.AlreadyChecked = true;
                        mailMessage.Append(task.Project != null ? "<li>" + $"{underTask.TaskType.Description} beim Projekt <a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectInfoPage?id={underTask.ProjectId}\">{underTask.Project.Name}</a></li>" : $"<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ \">{task.TaskType.Description}</a></li>");
                    }

                    mailMessage.Append("</ul>"
                        + "<br/>"
                        + "<p>Freundliche Grüsse</p>"
                        + "Dein ProStud-Team"
                        );

                    mailMessage.Append("</div>");
                    mail.Body = mailMessage.ToString();
                    emails.Add(Tuple.Create<Task, MailMessage>(task, mail));
                }
            }

            foreach (var task in tasksToMail)
                task.AlreadyChecked = false;

#if !DEBUG
            //send mails
            using (var smtpClient = new SmtpClient())
                foreach (var mailTaskTuple in emails)
                {
                    var mail = mailTaskTuple.Item2;
                    var task = mailTaskTuple.Item1;
                    smtpClient.Send(mail);

                    task.LastReminded = DateTime.Now;
                    if (task.TaskType.DaysBetweenReminds == 0)
                        task.Done = true;

                    db.SubmitChanges();
                }
#endif
        }
    }

}