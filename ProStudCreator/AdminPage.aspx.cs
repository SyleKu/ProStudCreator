﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ProStudCreator
{

    public class ProjectSingleTask
    {
        public string project { get; set; }
        public string taskOrganiseExpert { get; set; }
        public string taskOrganiseRoom { get; set; }
        public string taskOrganiseDate { get; set; }
        public string taskPayExpert { get; set; }
    }

    public partial class AdminPage : Page
    {

        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        // SR test
        IQueryable<Project> projects;
        //~SR test

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShibUser.IsAdmin())
            {
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
            }
            else
            {
                projects = db.Projects.Select(i => i);
                CheckProjects.DataSource =
                    from item in projects
                    where item.State == ProjectState.Submitted && (int?)item.DepartmentId == ShibUser.GetDepartmentId()
                    select item into i
                    select getProjectSingleElement(i);
                CheckProjects.DataBind();

                GVTasks.DataSource = AllTasks();
                GVTasks.DataBind();
            }
        }
        private ProjectSingleElement getProjectSingleElement(Project i)
        {
            return new ProjectSingleElement
            {
                id = i.Id,
                advisorName = string.Concat(new string[]
                {
                    (i.Advisor1Name!="") ? "<a href=\"mailto:" + i.Advisor1Mail + "\">"+Server.HtmlEncode(i.Advisor1Name).Replace(" ", "&nbsp;")+"</a>" : "?",
                    (i.Advisor2Name!="") ? "<br /><a href=\"mailto:" + i.Advisor2Mail + "\">" + Server.HtmlEncode(i.Advisor2Name).Replace(" ", "&nbsp;") + "</a>" : ""
                }),
                projectName = ((i.ProjectNr == 0) ? "" : i.ProjectNr.ToString("D2") + ": ") + i.Name,
                Institute = i.Department.DepartmentName,
                p5 = i.POneType.P5 || (i.PTwoType != null && i.PTwoType.P5),
                p6 = i.POneType.P6 || (i.PTwoType != null && i.PTwoType.P6),
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : (i.TypeMathAlg ? "MathAlg" : (i.TypeAppWeb ? "AppWeb" : (i.TypeDBBigData ? "DBBigData" : (i.TypeSysSec ? "SysSec" : (i.TypeSE ? "SE" : "Transparent")))))))) + ".png",
                projectType2 = "pictures/projectTyp" + ((i.TypeHW && i.TypeDesignUX) ? "HW" : ((i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" : ((i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" : ((i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" : ((i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb)) ? "DBBigData" : ((i.TypeSysSec && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb || i.TypeDBBigData)) ? "SysSec" : (i.TypeSE && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb || i.TypeDBBigData || i.TypeSysSec) ? "SE" : "Transparent"))))))) + ".png"
            };
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
                return;

            var id = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "revokeSubmission":
                    Project projectr = db.Projects.Single((Project i) => i.Id == id);
                    projectr.State = ProjectState.InProgress;
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "deleteProject":
                    Project project = db.Projects.Single((Project i) => i.Id == id);
                    project.Delete();
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "editProject":
                    Response.Redirect("AddNewProject?id=" + id);
                    break;
                default:
                    throw new Exception("Unknown command " + e.CommandName);
            }
        }

        private ProjectSingleTask CheckDefenseOrganised(Project _project)
        {
            var task = new ProjectSingleTask();
            var empty = true;
            DateTime defensestart;
            if (!DateTime.TryParseExact(Semester.CurrentSemester(db).DefenseIP6Start, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out defensestart))
                throw new Exception(
                    "Die Daten in der Datenbank entsprechen nicht dem richtigen Format. Bitte Melden Sie diesen Fehler einer Ansprechperson!");

            if (_project?.LogProjectTypeID == 2 && _project?.LogExpertID == null &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseExpert = "pictures/experte.png";
            }
            else
            {
                task.taskOrganiseExpert = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectTypeID == 2 && _project?.LogDefenceDate == null &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseDate = "pictures/datum.png";
            }
            else
            {
                task.taskOrganiseDate = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectTypeID == 2 && string.IsNullOrEmpty(_project.LogDefenceRoom) &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseRoom = "pictures/raum.png";
            }
            else
            {
                task.taskOrganiseRoom = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectTypeID == 2 && _project?.LogExpertPaid != true &&
                (_project?.LogDefenceDate > DateTime.Now) && _project.State == 3)
            {
                empty = false;
                task.taskPayExpert = "pictures/experte_zahlen.png";
            }
            else
            {
                task.taskPayExpert = "pictures/projectTypTransparent.png";
            }
            task.project = _project?.Name;
            return empty ? null : task;
        }

        public IQueryable<ProjectSingleTask> AllTasks()
        {
            if (projects == null)
            {
                projects = db.Projects.Select(i => i);
            }
            var allTaskList = new List<ProjectSingleTask>();
            foreach (var project in projects)
            {
                if (CheckDefenseOrganised(project) != null)
                {
                    allTaskList.Add(CheckDefenseOrganised(project));
                }
            }
            return allTaskList.AsQueryable();
        }

        protected void btnMarketingExport_OnClick(object sender, EventArgs e)
        {
            byte[] bytesInStream;
            var sem = Semester.CurrentSemester(db);
            using (var output = new MemoryStream())
            {
                ExcelCreator.GenerationMarketingList(output, db.Projects.Where(i => i.Semester == sem && i.State == ProjectState.Published && i.LogStudent1Name != null && i.LogStudent1Name != ""));
                bytesInStream = output.ToArray();
            }

            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition", "attachment; filename=Informatikprojekte.xlsx");
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }
    }
}