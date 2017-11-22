﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public class ProjectSingleTask
    {
        public string project { get; set; }
        public string TaskOrganiseExpert { get; set; }
        public string TaskOrganiseRoom { get; set; }
        public string TaskOrganiseDate { get; set; }
        public string TaskPayExpert { get; set; }
    }

    public partial class AdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            SelectedSemester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            SelectedSemester.DataBind();
            SelectedSemester.SelectedValue = Semester.CurrentSemester(db).Id.ToString();
            SelectedSemester.Items.Insert(0, new ListItem("Alle Semester", ""));
            SelectedSemester.Items.Insert(1, new ListItem("――――――――――――――――", ".", false));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShibUser.CanVisitAdminPage())
            {
                DivAdminProjects.Visible = ShibUser.CanPublishProject();
                DivExcelExport.Visible = ShibUser.CanExportExcel();


                if (!Page.IsPostBack)
                {
                    if (Session["SelectedAdminProjects"] == null)
                    {
                        radioSelectedProjects.SelectedIndex = 0;
                        Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
                    }
                    else
                    {
                        radioSelectedProjects.SelectedIndex = (int)Session["SelectedAdminProjects"];
                    }

                    if (Session["AdminProjectCollapsed"] == null)
                        CollapseAdminProjects(false);
                    else
                        CollapseAdminProjects((bool)Session["AdminProjectCollapsed"]);

                    if (Session["ExcelExportCollapsed"] == null)
                        CollapseExcelExport(false);
                    else
                        CollapseExcelExport((bool)Session["ExcelExportCollapsed"]);


                    if (Session["AddInfoCollapsed"] == null)
                        CollapseAddInfo(true);
                    else
                        CollapseAddInfo((bool)Session["AddInfoCollapsed"]);

                }

                CheckProjects.DataSource = GetSelectedProjects();
                CheckProjects.DataBind();
                //GVTasks.DataSource = AllTasks();
                //GVTasks.DataBind();

            }
            else
            {
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
            }

            Session["LastPage"] = "adminpage";
        }

        private ProjectSingleElement GetProjectSingleElement(Project i)
        {
            return new ProjectSingleElement
            {
                id = i.Id,
                advisorName = string.Concat(new[]
                {
                    i.Advisor1 != null
                        ? "<a href=\"mailto:" + i.Advisor1.Mail + "\">" +
                          Server.HtmlEncode(i.Advisor1.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "?",
                    i.Advisor2 != null
                        ? "<br /><a href=\"mailto:" + i.Advisor2.Mail + "\">" +
                          Server.HtmlEncode(i.Advisor2.Name).Replace(" ", "&nbsp;") + "</a>"
                        : ""
                }),
                projectName = (i.ProjectNr == 0 ? "" : i.ProjectNr.ToString("D2") + ": ") + i.Name,
                Institute = i.Department.DepartmentName,
                p5 = i.POneType.P5 || i.PTwoType != null && i.PTwoType.P5,
                p6 = i.POneType.P6 || i.PTwoType != null && i.PTwoType.P6,
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX
                                   ? "DesignUX"
                                   : (i.TypeHW
                                       ? "HW"
                                       : (i.TypeCGIP
                                           ? "CGIP"
                                           : (i.TypeMathAlg
                                               ? "MathAlg"
                                               : (i.TypeAppWeb
                                                   ? "AppWeb"
                                                   : (i.TypeDBBigData
                                                       ? "DBBigData"
                                                       : (i.TypeSysSec
                                                           ? "SysSec"
                                                           : (i.TypeSE ? "SE" : "Transparent")))))))) + ".png",
                projectType2 = "pictures/projectTyp" + (i.TypeHW && i.TypeDesignUX
                                   ? "HW"
                                   : (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)
                                       ? "CGIP"
                                       : (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)
                                           ? "MathAlg"
                                           : (i.TypeAppWeb &&
                                              (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)
                                               ? "AppWeb"
                                               : (i.TypeDBBigData &&
                                                  (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg ||
                                                   i.TypeAppWeb)
                                                   ? "DBBigData"
                                                   : (i.TypeSysSec &&
                                                      (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg ||
                                                       i.TypeAppWeb || i.TypeDBBigData)
                                                       ? "SysSec"
                                                       : (i.TypeSE && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP ||
                                                                       i.TypeMathAlg || i.TypeAppWeb ||
                                                                       i.TypeDBBigData || i.TypeSysSec)
                                                           ? "SE"
                                                           : "Transparent"))))))) + ".png"
            };
        }

        private IQueryable<ProjectSingleElement> GetSelectedProjects()
        {
            var depId = ShibUser.GetDepartmentId(db);

            if (radioSelectedProjects.SelectedValue == "inProgress")
            {
                var lastSemStartDate = Semester.LastSemester(db).StartDate;
                return db.Projects.Where(p =>
                    p.DepartmentId == depId && p.IsMainVersion &&
                    p.ModificationDate > lastSemStartDate &&
                    (p.State == ProjectState.InProgress || p.State == ProjectState.Rejected))
                    .OrderBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr)
                    .Select(i => GetProjectSingleElement(i));
            }
            else
            {
                return db.Projects
                    .Where(item => item.State == ProjectState.Submitted && item.IsMainVersion && (int?)item.DepartmentId == depId)
                    .OrderBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr)
                    .Select(i => GetProjectSingleElement(i));
            }
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
                return;

            var id = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "revokeSubmission":
                    var projectr = db.Projects.Single(i => i.Id == id);
                    projectr.State = ProjectState.InProgress;
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "deleteProject":
                    var project = db.Projects.Single(i => i.Id == id);
                    project.Delete();
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "editProject":
                    Response.Redirect("AddNewProject?id=" + id);
                    break;
                case "submitProject":
                    EinreichenButton_Click(id);
                    break;
                default:
                    throw new Exception("Unknown command " + e.CommandName);
            }
        }

        private void CollapseAdminProjects(bool collapse)
        {
            Session["AdminProjectCollapsed"] = collapse;
            DivAdminProjectsCollapsable.Visible = divRadioProjects.Visible = !collapse;
            btnAdminProjectsCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseExcelExport(bool collapse)
        {
            Session["ExcelExportCollapsed"] = collapse;
            DivExcelExportCollapsable.Visible = !collapse;
            btnExcelExportCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseAddInfo(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivAddInfoCollapsable.Visible = !collapse;
            btnAddInfoCollapse.Text = collapse ? "◄" : "▼";
        }

        protected void BtnMarketingExport_OnClick(object sender, EventArgs e)
        {
            IEnumerable<Project> projectsToExport = null;
            if (radioProjectStart.SelectedValue == "StartingProjects") //Projects which start in this Sem.
                if (SelectedSemester.SelectedValue == "") //Alle Semester
                {
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.IsMainVersion && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
                else
                {
                    var semesterId = int.Parse(SelectedSemester.SelectedValue);
                    projectsToExport = db.Projects
                        .Where(i => i.SemesterId == semesterId && i.IsMainVersion && i.State == ProjectState.Published &&
                                    i.LogStudent1Mail != null && i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
            else if (radioProjectStart.SelectedValue == "EndingProjects") //Projects which end in this Sem.
                if (SelectedSemester.SelectedValue == "") //Alle Semester
                {
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.IsMainVersion && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
                else
                {
                    var selectedSemester = db.Semester.Single(s => s.Id == int.Parse(SelectedSemester.SelectedValue));
                    var previousSemester = db.Semester.OrderByDescending(s => s.StartDate)
                        .FirstOrDefault(s => s.StartDate < selectedSemester.StartDate);
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.IsMainVersion && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != ""
                                    && (i.LogProjectDuration == 1 && i.Semester == selectedSemester ||
                                        i.LogProjectDuration == 2 && i.Semester == previousSemester))
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
            else
                throw new Exception($"Unexpected selection: {radioProjectStart.SelectedIndex}");

            //Response
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text.Replace(" ", "_")}_IP56_Informatikprojekte.xlsx");
            ExcelCreator.GenerateMarketingList(Response.OutputStream, projectsToExport, db,
                SelectedSemester.SelectedItem.Text);
            Response.End();
        }

        protected void RadioSelectedProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
            CheckProjects.DataSource = GetSelectedProjects();
            CheckProjects.DataBind();

        }
        protected void CheckProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var project = db.Projects.Single(item => item.Id == ((ProjectSingleElement)e.Row.DataItem).id);

            Color? col = null;
            switch (project.State)
            {
                case ProjectState.Published:
                    col = ColorTranslator.FromHtml("#A9F5A9");
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit
                    break;
                case ProjectState.Rejected:
                    col = ColorTranslator.FromHtml("#F5A9A9");
                    break;
                case ProjectState.Submitted:
                    col = ColorTranslator.FromHtml("#ffcc99");
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit
                    break;
            }
            if (!col.HasValue) return;
            foreach (TableCell cell in e.Row.Cells)
                cell.BackColor = col.Value;
        }

        protected void BtnAdminProjectsCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminProjects(!(bool)Session["AdminProjectCollapsed"]);
        }

        protected void BtnExcelExportCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseExcelExport(!(bool)Session["ExcelExportCollapsed"]);
        }

        protected void BtnAddInfoCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAddInfo(!(bool)Session["AddInfoCollapsed"]);
        }

        private bool[] GetProjectTypeBools(Project project)
        {
            bool[] projectType = new bool[8];
            if (project.TypeDesignUX)
            {
                projectType[0] = true;
            }
            if (project.TypeHW)
            {
                projectType[1] = true;
            }
            if (project.TypeCGIP)
            {
                projectType[2] = true;
            }
            if (project.TypeMathAlg)
            {
                projectType[3] = true;
            }
            if (project.TypeAppWeb)
            {
                projectType[4] = true;
            }
            if (project.TypeDBBigData)
            {
                projectType[5] = true;
            }
            if (project.TypeSysSec)
            {
                projectType[6] = true;
            }
            if (project.TypeSE)
            {
                projectType[7] = true;
            }
            return projectType;
        }

        private string GenerateValidationMessage(Project project)
        {

            if (project.Advisor1 == null)
                return "Bitte wählen Sie einen Hauptbetreuer aus.";

            var projectType = GetProjectTypeBools(project);

            if (project.ClientPerson != "" && !project.ClientPerson.IsValidName())
                return "Bitte geben Sie den Namen des Kundenkontakts an (Vorname Nachname).";

            if (project.ClientMail != "" && !project.ClientMail.IsValidEmail())
                return "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts an.";

            if ((!project.Advisor1?.Name.IsValidName()) ?? true)
                return "Bitte wählen Sie einen Hauptbetreuer aus.";

            var numAssignedTypes = projectType.Count(a => a);

            if (numAssignedTypes != 1 && numAssignedTypes != 2)
                return "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";

            if (project.OverOnePage)
                return "Der Projektbeschrieb passt nicht auf eine A4-Seite. Bitte kürzen Sie die Beschreibung.";

            if (!ShibUser.CanSubmitAllProjects() && ShibUser.GetEmail() != project.Advisor1?.Mail)
                return "Nur Hauptbetreuer können Projekte einreichen.";

            if (project.Reservation1Mail != "" && project.Reservation1Name == "")
                return
                    "Bitte geben Sie den Namen der ersten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (project.Reservation2Mail != "" && project.Reservation2Name == "")
                return
                    "Bitte geben Sie den Namen der zweiten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (project.Reservation1Name != "" && project.Reservation1Mail == "")
                return "Bitte geben Sie die E-Mail-Adresse der Person an, für die das Projekt reserviert ist.";

            if (project.Reservation2Name != "" && project.Reservation2Mail == "")
                return "Bitte geben Sie die E-Mail-Adresse der zweiten Person an, für die das Projekt reserviert ist.";

            return null;
        }

        protected void EinreichenButton_Click(int id)
        {
            Project project = db.Projects.Single(p => p.Id == id);
            var validationMessage = GenerateValidationMessage(project);

            if (validationMessage != null)
            {
                var sb = new StringBuilder();
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("');");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                    "key", sb.ToString(), true);
            }
            else
            {
                project.Submit();
                db.SubmitChanges();
                Response.Redirect("projectlist");
            }

        }
    }
}