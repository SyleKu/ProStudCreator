﻿using System;
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
            EnterAssignedStudents = 14,
            DoubleCheckMarKomBrochureData = 15,
            CheckBillingStatus = 16
        }


        public static void CheckAllTasks() //register all Methods which check for tasks here.
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                CheckGradesRegistered(db);
                CheckWebsummaryChecked(db);
                CheckBillingStatus(db);
                CheckUploadResults(db);

                InfoStartProject(db);
                InfoFinishProject(db);

                InfoInsertNewSemesters(db);

                EnterAssignedStudents(db);

                SendGradesToAdmin(db);
                SendPayExperts(db);



                //vvvvvvvvvvvvv NOT YET IMPLEMENTED
                //SendInvoiceCustomers(db); //<-- not yet implemented

                SendDoubleCheckMarKomBrochureData(db);
                SendMarKomBrochure(db);

                SendMailsToResponsibleUsers(db);
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
                        FirstReminded = DateTime.Now,
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
        //                    "ProStud-Team</p>" +
        //                    $"<p>Feedback an {Global.WebAdmin}</p>" +
        //                    "</div>"
        //                    );

        //                mail.Body = mailMessage.ToString();
        //                smtpClient.Send(mail);
        //            }
        //    }

        //    db.SubmitChanges();
        //}




        public static void SendMarKomBrochure(ProStudentCreatorDBDataContext db)
        {
            var lastSendDate = new DateTime(DateTime.Now.Year, 6, 1);
            if (DateTime.Now < lastSendDate)
                lastSendDate = lastSendDate.AddYears(-1);

            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendMarKomBrochure);
            if(db.Tasks.Any(t => t.Done && t.TaskType == type && t.LastReminded>=lastSendDate))
                return;

            
            var activeTask = new Task()
            {
                TaskType = type,
                LastReminded = DateTime.Now,
                FirstReminded = DateTime.Now,
            };
            db.Tasks.InsertOnSubmit(activeTask);
            db.SubmitChanges();

            using (var smtpClient = new SmtpClient())
            {
                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                mail.To.Add(new MailAddress(Global.MarKomAdmin));
                mail.CC.Add(new MailAddress(Global.WebAdmin));
                mail.Subject = "Informatikprojekte P6: Projektliste für Broschüre";
                mail.IsBodyHtml = true;

                var mailMessage = new StringBuilder();
                mailMessage.Append(
                    "<div style=\"font-family: Arial\">" +
                    "<p>Liebes MarKom<p>" +
                    $"<p>Hier die Liste aller Informatik-Bachelorarbeiten im { Semester.ActiveSemester(lastSendDate,db).Name }:</p>" +
                    "<table>" +
                    "<tr>" +
                        "<th>Nachname</th>" +
                        "<th>Vorname</th>" +
                        "<th>Titel</th>" +
                        "<th>Unternehmen</th>" +
                        "<th>Unternehmensort</th>" +
                        "<th>Nummer</th>" +
                    "</tr>");

                var projs = db.Projects.Where(p => p.IsMainVersion && p.LogProjectType.P6 && p.Semester.StartDate <= lastSendDate && p.Semester.EndDate >= lastSendDate && p.State == (int)ProjectState.Published && !p.UnderNDA).ToArray();

                foreach (var p in projs.OrderBy(p => p.Student1LastName()))
                    mailMessage.Append(
                    "<tr>" +
                        $"<td>{HttpUtility.HtmlEncode(p.Student1LastName()) + (p.LogStudent2Name!=null ? "<br/>"+HttpUtility.HtmlEncode(p.Student2LastName()) : "")}</td>" +
                        $"<td>{HttpUtility.HtmlEncode(p.Student1FirstName()) + (p.LogStudent2Name != null ? "<br/>" + HttpUtility.HtmlEncode(p.Student2FirstName()) : "")}</td>" +
                        $"<td>{HttpUtility.HtmlEncode(p.Name)}</td>" +
                        $"<td>{(p.ClientType==(int)ClientType.Company ? HttpUtility.HtmlEncode(p.ClientCompany) : "")}</td>" +
                        $"<td>{(p.ClientType == (int)ClientType.Company ? HttpUtility.HtmlEncode(p.ClientAddressCity) : "")}</td>" +
                        $"<td>{HttpUtility.HtmlEncode(p.GetFullNr())}</td>" +
                    "</tr>"
                    );

                mailMessage.Append(
                    "</table>" +
                    "<br/>" +
                    "<p>Herzliche Grüsse,<br/>" +
                    "ProStud-Team</p>" +
                    $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                    "</div>"
                    );

                mail.Body = mailMessage.ToString();
                smtpClient.Send(mail);
            }

            activeTask.Done = true;
            db.SubmitChanges();
        }








        public static void SendPayExperts(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.PayExperts);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type);
            if (activeTask == null)
            {
                activeTask = new Task()
                {
                    TaskType = type,
                    FirstReminded = DateTime.Now
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
                                $"<td>{HttpUtility.HtmlEncode(p.Expert.Name)}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.Semester.Name)}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.LogStudent1Mail + (p.LogStudent2Mail != null ? ", " + p.LogStudent2Mail : ""))}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.Advisor1.Mail)}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.GetFullTitle())}</td>" +
                            "</tr>"
                            );
                        }

                        mailMessage.Append(
                            "</table>" +
                            "<br/>" +
                            "<p>Herzliche Grüsse,<br/>" +
                            "ProStud-Team</p>" +
                            $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
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
                    FirstReminded = DateTime.Now
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var updatedProjects = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && !p.GradeSentToAdmin && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && (p.LogLanguageEnglish == true || p.LogLanguageGerman == true)).OrderBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

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
                                    $"<td>{HttpUtility.HtmlEncode(p.LogStudent1Mail)}</td>" +
                                    $"<td>{HttpUtility.HtmlEncode(p.LogGradeStudent1.Value.ToString("F1"))}</td>" +
                                    $"<td>{HttpUtility.HtmlEncode(p.LogProjectType.ExportValue)}</td>" +
                                    $"<td>{HttpUtility.HtmlEncode((p.LogLanguageGerman.Value ? "Deutsch" : "Englisch"))}</td>" +
                                    $"<td>{HttpUtility.HtmlEncode(p.Advisor1.Mail)}</td>" +
                                    $"<td>{HttpUtility.HtmlEncode(p.GetFullTitle())}</td>" +
                                "</tr>"
                                );

                            if (p.LogStudent2Mail != null && p.LogGradeStudent2 != null)
                                mailMessage.Append(
                                    "<tr>" +
                                        $"<td>{HttpUtility.HtmlEncode(p.LogStudent2Mail)}</td>" +
                                        $"<td>{HttpUtility.HtmlEncode(p.LogGradeStudent2.Value.ToString("F1"))}</td>" +
                                        $"<td>{HttpUtility.HtmlEncode(p.LogProjectType.ExportValue)}</td>" +
                                        $"<td>{HttpUtility.HtmlEncode((p.LogLanguageGerman.Value ? "Deutsch" : "Englisch"))}</td>" +
                                        $"<td>{HttpUtility.HtmlEncode(p.Advisor1.Mail)}</td>" +
                                        $"<td>{HttpUtility.HtmlEncode(p.GetFullTitle())}</td>" +
                                    "</tr>"
                                    );
                        }

                        mailMessage.Append(
                            "</table>" +
                            "<br/>" +
                            "<p>Herzliche Grüsse,<br/>" +
                            "ProStud-Team</p>" +
                            $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
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
            var targetDate = DateTime.Now.Date.AddMonths(18);

            var activeTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.InsertNewSemesters);
            var semesterMissing = !db.Semester.Any(s => targetDate >= s.StartDate && targetDate < s.DayBeforeNextSemester);

            //add new tasks for projects
            if (semesterMissing && !activeTasks.Any())
                db.Tasks.InsertOnSubmit(new Task
                {
                    ResponsibleUser = db.UserDepartmentMap.Single(i => i.Mail == Global.WebAdmin),
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.InsertNewSemesters),
                    FirstReminded = DateTime.Now
                });

            if (!semesterMissing)
                foreach (var t in activeTasks)
                    t.Done = true;

            db.SubmitChanges();
        }





        private static void SendDoubleCheckMarKomBrochureData(ProStudentCreatorDBDataContext db)
        {
            var lastCheckDate = new DateTime(DateTime.Now.Year, 5, 1);
            if (lastCheckDate > DateTime.Now)
                lastCheckDate = lastCheckDate.AddYears(-1);

            if (lastCheckDate.Year <= 2018)
                return;

            //add new tasks for projects
            var allExportProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.LogProjectType.P6 && p.Semester.StartDate >= lastCheckDate && p.Semester.EndDate <= lastCheckDate
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.DoubleCheckMarKomBrochureData)).ToList();

            foreach (var project in allExportProjects)
                db.Tasks.InsertOnSubmit(new Task
                {
                    ProjectId = project.Id,
                    ResponsibleUser = project.Advisor1,
                    FirstReminded = DateTime.Now,
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.DoubleCheckMarKomBrochureData),
                    Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                });

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
                    FirstReminded = DateTime.Now,
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
                        FirstReminded = DateTime.Now,
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
                            FirstReminded = DateTime.Now,
                            TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.RegisterGrades),
                            Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                        });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.RegisterGrades).ToList();
            foreach (var openTask in openGradeTasks)
                if (openTask.Project.State!=ProjectState.Published || !openTask.Project.IsMainVersion || (openTask.Project.LogGradeStudent1.HasValue && (openTask.Project.LogGradeStudent2.HasValue || string.IsNullOrEmpty(openTask.Project.LogStudent2Mail))))
                    openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckWebsummaryChecked(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveWebsummaryTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.CheckWebsummary).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && !p.WebSummaryChecked && p.BillingStatus.RequiresProjectResults).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if (!allActiveWebsummaryTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.CheckWebsummary),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.CheckWebsummary && (i.Project.WebSummaryChecked || i.Project.State!=ProjectState.Published || !i.Project.BillingStatus.RequiresProjectResults || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in openGradeTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckBillingStatus(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveBillingTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.CheckBillingStatus).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && p.BillingStatus==null).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if (!allActiveBillingTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.CheckBillingStatus),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var completedBillingTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.CheckBillingStatus && (i.Project.BillingStatus!=null || i.Project.State != ProjectState.Published || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in completedBillingTasks)
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
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.UploadResults),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsSupervisor && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openUploadTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.UploadResults &&
            (i.Project.Attachements.Any(a => !a.Deleted) || !i.Project.BillingStatus.RequiresProjectResults || i.Project.State!=ProjectState.Published || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in openUploadTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }


        private static void SendMailsToResponsibleUsers(ProStudentCreatorDBDataContext db)
        {
            var usersToMail = db.Tasks.Where(t => !t.Done && t.ResponsibleUser!=null && (t.LastReminded==null || t.LastReminded<=DateTime.Now.AddDays(-t.TaskType.DaysBetweenReminds))).Select(t => t.ResponsibleUser).Distinct();

            foreach (var user in usersToMail)
            {
                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                mail.To.Add(new MailAddress(user.Mail));
                mail.CC.Add(new MailAddress(Global.WebAdmin));
                mail.Subject = "Erinnerung von ProStud";
                mail.IsBodyHtml = true;

                var mailMessage = new StringBuilder();
                mailMessage.Append("<div style=\"font-family: Arial\">");
                mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {HttpUtility.HtmlEncode(user.Name.Split(' ')[0])}<p>"
                                    + "<p>Es stehen folgende Aufgaben im ProStud an:</p><ul>");

                var tasks = db.Tasks.Where(t => t.ResponsibleUser == user && !t.Done).OrderBy(t => t.Project.ProjectNr).ToArray();
                foreach (var task in tasks)
                {
                    if (task.DueDate != null && DateTime.Now.AddDays(3) > task.DueDate && task.Supervisor != null)
                        mail.CC.Add(task.Supervisor.Mail);

                    mailMessage.Append(task.Project != null ? "<li>" + $"{HttpUtility.HtmlEncode(task.TaskType.Description)} beim Projekt <a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectInfoPage?id={task.ProjectId}\">{HttpUtility.HtmlEncode(task.Project.Name)}</a></li>" : $"<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ \">{HttpUtility.HtmlEncode(task.TaskType.Description)}</a></li>");
                }

                mailMessage.Append("</ul>"
                    + "<br/>"
                    + "<p>Freundliche Grüsse</p>"
                    + "ProStud-Team</p>"
                    + $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>"
                    + "</div>"
                    );

                mail.Body = mailMessage.ToString();

#if !DEBUG
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Send(mail);

                    foreach (var task in tasks)
                    {
                        task.LastReminded = DateTime.Now;
                        if (task.TaskType.DaysBetweenReminds == 0)
                            task.Done = true;

                        db.SubmitChanges();
                    }
                }
#endif
            }
        }
    }

}