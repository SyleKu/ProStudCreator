using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
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
            projects = db.Projects.Select(i => i);

            if (IsPostBack)
            {
                if (whichOwner.SelectedValue == "NotOwnEdited") //Set Semester to allSemester
                {
                    Session["ComesFromNotOwn"] = true;
                    Session["SelectedSemesterBeforeNotOwn"] =
                        dropSemester.SelectedValue; //Save last semester if user changes Owner in filter
                    dropSemester.Enabled = false;
                    Session["SelectedSemester"] =
                        dropSemester.SelectedValue =
                            "allSemester"; //Save the current State if user switches tab after this
                    Session["SelectedOwner"] = whichOwner.SelectedValue;
                }
                else
                {
                    if ((bool?) Session["ComesFromNotOwn"] ?? false) //if user comes from NotOwn
                    {
                        dropSemester.SelectedValue = (string) Session["SelectedSemesterBeforeNotOwn"];
                        Session["ComesFromNotOwn"] = false;
                    }
                    Session["SelectedSemester"] = dropSemester.SelectedValue;
                    Session["SelectedOwner"] = whichOwner.SelectedValue;
                    dropSemester.Enabled = true;
                }
            }
            else if (Session["SelectedOwner"] != null) //User comes from another tab or realoaded the page
            {
                if ((string) Session["SelectedOwner"] == "NotOwnEdited") //If last filter was set to NotOwn
                    dropSemester.Enabled = false;

                whichOwner.SelectedValue = (string) Session["SelectedOwner"]; //Set Filter from the Sessionvars
                dropSemester.SelectedValue = (string) Session["SelectedSemester"];
            }
            else //Sessionvars don't exist yet
            {
                Session["SelectedOwner"] = whichOwner.SelectedValue;
                Session["SelectedSemester"] = dropSemester.SelectedValue;
            }


            AllProjects.DataSource =
                from i in filterRelevantProjects(projects, (string) Session["SelectedOwner"])
                select getProjectSingleElement(i);
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


        private IQueryable<Project> filterRelevantProjects(IQueryable<Project> allProjects, string filter)
        {
            var projects = allProjects;
            switch (filter)
            {
                case "OwnProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                    {
                        projects =
                            from p in projects
                            where (p.Creator == ShibUser.GetEmail() || p.Advisor2Mail == ShibUser.GetEmail() ||
                                   p.Advisor1Mail == ShibUser.GetEmail()) && p.State != ProjectState.Deleted
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    else
                    {
                        var nextSemesterSelected = int.Parse(dropSemester.SelectedValue) ==
                                                   Semester.NextSemester(db).Id;
                        projects =
                            from p in projects
                            where (p.Creator == ShibUser.GetEmail() || p.Advisor1Mail == ShibUser.GetEmail() ||
                                   p.Advisor2Mail == ShibUser.GetEmail())
                                  && p.State != ProjectState.Deleted
                                  && (p.Semester.Id == int.Parse(dropSemester.SelectedValue) &&
                                      p.State == ProjectState.Published || nextSemesterSelected && p.Semester == null ||
                                      p.State != ProjectState.Deleted && p.State != ProjectState.Published &&
                                      nextSemesterSelected)
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    break;
                case "AllProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                        projects =
                            from p in projects
                            where p.State == ProjectState.Published
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    else
                        projects =
                            from p in projects
                            where p.State == ProjectState.Published &&
                                  p.Semester.Id == int.Parse(dropSemester.SelectedValue)
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    break;
                case "NotOwnEdited":
                    var depId = ShibUser.GetDepartmentId(db);
                    var lastSemStartDate = Semester.LastSemester(db).StartDate;
                    projects = db.Projects.Where(p =>
                        p.DepartmentId == depId &&
                        p.ModificationDate > lastSemStartDate &&
                        (p.State == ProjectState.InProgress || p.State == ProjectState.Rejected ||
                         p.State == ProjectState.Submitted));
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
                    i.Advisor1Name != ""
                        ? "<a href=\"mailto:" + i.Advisor1Mail + "\">" +
                          Server.HtmlEncode(i.Advisor1Name).Replace(" ", "&nbsp;") + "</a>"
                        : "?",
                    i.Advisor2Name != ""
                        ? "<br /><a href=\"mailto:" + i.Advisor2Mail + "\">" +
                          Server.HtmlEncode(i.Advisor2Name).Replace(" ", "&nbsp;") + "</a>"
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
                        e.Row.Cells[e.Row.Cells.Count - 3].Visible = false; //edit
                        e.Row.Cells[e.Row.Cells.Count - 1].Visible = false; //delete
                    }
                }
                else if (!project.UserCanEdit())
                {
                    e.Row.Cells[e.Row.Cells.Count - 3].Visible = false; //edit
                    e.Row.Cells[e.Row.Cells.Count - 1].Visible = false; //delete
                }

                Color? col = null;
                if (project.State == ProjectState.Published)
                    col = ColorTranslator.FromHtml("#A9F5A9");
                else if (project.State == ProjectState.Rejected)
                    col = ColorTranslator.FromHtml("#F5A9A9");
                else if (project.State == ProjectState.Submitted)
                    col = ColorTranslator.FromHtml("#ffcc99");
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
                    .Select(p => db.Projects.Single(pr => pr.Id == p.id))
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