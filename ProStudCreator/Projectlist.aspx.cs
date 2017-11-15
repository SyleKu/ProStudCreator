using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using iTextSharp.text;
using NPOI.OpenXmlFormats.Vml;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace ProStudCreator
{
    public class ProjectSingleElement
    {
        public int id { get; set; }
        public string Institute { get; set; }
        public string advisorName { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
    }

    public partial class Projectlist : Page
    {
        protected PlaceHolder AdminView;
        protected GridView AllProjects;
        protected GridView CheckProjects;
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected Button NewProject;
        
        // SR test
        private IQueryable<Project> projects;
        //~SR test

        protected void Page_Init(object sender, EventArgs e)
        {
            dropSemester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            dropSemester.DataBind();
            dropSemester.Items.Insert(0, new ListItem("Alle Semester", "allSemester"));
            dropSemester.Items.Insert(1, new ListItem("――――――――――――――――", "."));
            dropSemester.SelectedValue = db.Semester.Where(s => s.StartDate > DateTime.Now).OrderBy(s => s.StartDate)
                .First().Id.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            projects = db.Projects.Where(i => i.IsMainVersion);

            if (IsPostBack || Session["SelectedOwner"] == null || Session["SelectedSemester"] == null)
            {
                Session["SelectedOwner"] = whichOwner.SelectedValue;
                Session["SelectedSemester"] = dropSemester.SelectedValue;
            }
            else
            {
                whichOwner.SelectedValue = (string) Session["SelectedOwner"]; 
                dropSemester.SelectedValue = (string) Session["SelectedSemester"];
            }


            AllProjects.DataSource = filterRelevantProjects(projects)
                .Select(i => getProjectSingleElement(i));
            AllProjects.DataBind();

            //Disabling the "-----" element in the Dropdownlist. So the item "Alle Semester" is separated from the rest
            dropSemester.Items.FindByValue(".").Attributes.Add("disabled", "disabled");

            if (!ShibUser.CanSeeAllProjectsInProgress())
            {
                var item = whichOwner.Items.FindByValue("NotOwnEdited");
                if (item != null)
                    whichOwner.Items.Remove(item);
            }


            Session["LastPage"] = "projectlist";
        }


        private IQueryable<Project> filterRelevantProjects(IQueryable<Project> allProjects)
        {
            var projects = allProjects;
            switch (Session["SelectedOwner"])
            {
                case "OwnProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                    {
                        projects = db.Projects.Where(p => (p.Creator == ShibUser.GetEmail() ||
                                                           p.Advisor2.Mail == ShibUser.GetEmail() ||
                                                           p.Advisor1.Mail == ShibUser.GetEmail()) &&
                                                          p.State != ProjectState.Deleted && p.IsMainVersion)
                            .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
                    }

                    else
                    {
                        var nextSemesterSelected = int.Parse(dropSemester.SelectedValue) ==
                                                   Semester.NextSemester(db).Id;
                        projects = db.Projects.Where(p => (p.Creator == ShibUser.GetEmail() ||
                                                           p.Advisor1.Mail == ShibUser.GetEmail() ||
                                                           p.Advisor2.Mail == ShibUser.GetEmail())
                                                          && p.State != ProjectState.Deleted && p.IsMainVersion
                                                          && (p.Semester.Id == int.Parse(dropSemester.SelectedValue) &&
                                                              p.State == ProjectState.Published ||
                                                              nextSemesterSelected && p.Semester == null && p.IsMainVersion ||
                                                              p.State != ProjectState.Deleted && p.IsMainVersion && p.State !=
                                                              ProjectState.Published &&
                                                              nextSemesterSelected))
                            .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
                    }
                    break;
                case "AllProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                        projects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion)
                            .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
                    else
                        projects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion &&
                                                          p.Semester.Id == int.Parse(dropSemester.SelectedValue))
                            .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
                    break;
            }
            return projects;
        }

        private ProjectSingleElement getProjectSingleElement(Project i)
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

        protected void AllProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = db.Projects.Single(item => item.Id == ((ProjectSingleElement) e.Row.DataItem).id);

                if (project.State == ProjectState.Published)
                {
                    if (!ShibUser.CanEditAllProjects())
                    {
                        e.Row.Cells[e.Row.Cells.Count - 4].Visible = false; //edit
                        e.Row.Cells[e.Row.Cells.Count - 2].Visible = false; //delete
                    }
                }
                else if (!project.UserCanEdit())
                {
                    e.Row.Cells[e.Row.Cells.Count - 4].Visible = false; //edit
                    e.Row.Cells[e.Row.Cells.Count - 2].Visible = false; //delete
                }

                Color? col = null;
                if (project.State == ProjectState.Published)
                {
                    col = ColorTranslator.FromHtml("#A9F5A9");
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit
                }
                    

                else if (project.State == ProjectState.Rejected)
                {
                    col = ColorTranslator.FromHtml("#F5A9A9");
                }
                else if (project.State == ProjectState.Submitted)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit
                    col = ColorTranslator.FromHtml("#ffcc99");
                }
                if (col.HasValue)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.BackColor = col.Value;
            }
        }

        protected void newProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddNewProject");
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
                    einreichenButton_Click(id);
                    break;
                default:
                    throw new Exception("Unknown command " + e.CommandName);
            }
        }

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (AllProjects.Rows.Count != 0)
            {
                Response.Clear();
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=AllProjects.pdf");
                Response.Buffer = false;

                var output = Response.OutputStream;
                var document = PdfCreator.CreateDocument();
                try
                {
                    var pdfCreator = new PdfCreator();
                    pdfCreator.AppendToPDF(document, output,
                        ((IEnumerable<ProjectSingleElement>) AllProjects.DataSource)
                        .Select(p => db.Projects.Single(pr => pr.Id == p.id))
                        .OrderBy(p => p.Reservation1Name != "")
                        .ThenBy(p => p.Department.DepartmentName)
                        .ThenBy(p => p.ProjectNr)
                    );
                    document.Dispose();
                }
                catch (DocumentException documentException) when (documentException.Message.Contains("0x800704CD"))
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch
                    {
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch
                    {
                    }
                    throw;
                }
                Response.End();
            }
            else
            {
                var message = "In dieser Kategorie sind keine Projekte vorhanden!";
                var sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
            }
        }

        protected void AllProjectsAsExcel_Click(object sender, EventArgs e)
        {
            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                ExcelCreator.GenerateProjectList(output, ((IEnumerable<ProjectSingleElement>) AllProjects.DataSource)
                    .Select(p => db.Projects.Single(pr => pr.Id == p.id && pr.IsMainVersion))
                    .OrderBy(p => p.Reservation1Name != "")
                    .ThenBy(p => p.Department.DepartmentName)
                    .ThenBy(p => p.ProjectNr));
                bytesInStream = output.ToArray();
            }

            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition", "attachment; filename=Informatikprojekte.xlsx");
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }

        private void updateGridView()
        {
            if (filterText.Value=="")return;

            //var searchString =  filterText.Value;
            //projects = db.Projects.Select(i => i);
            //projects = db.Projects.Where(p => searchString == "" || p.LogStudent1Name.Contains(searchString) || p.LogStudent2Name.Contains(searchString))
            //    .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
            //AllProjects.DataSource = projects
            //    .Select(i => getProjectSingleElement(i));
            AllProjects.DataBind();
        }
       protected void filterButton_Click(object sender, EventArgs e)
        {
            updateGridView();
        }

        private bool[] getProjectTypeBools(Project project)
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
            var projectType = getProjectTypeBools(project);

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
        protected void einreichenButton_Click(int id)
        {
            Project project = db.Projects.Single(p => p.Id == id);
            var validationMessage = GenerateValidationMessage(project);
            if (validationMessage != null)
            {
                var sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
            }
            else
            {
                project.Submit();
                db.SubmitChanges();
                Response.Redirect("projectlist");
            }

        }
        protected void AllProjects_Sorting(object sender, GridViewSortEventArgs e)
        {
            //AllProjects.DataSource =
            //    from i in filterRelevantProjects(projects)
            //    orderby "department desc" // Doesn't allow sorting based on string param
            //    select getProjectSingleElement(i);
            //AllProjects.DataBind();
        }
    }
}