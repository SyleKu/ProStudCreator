using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected PlaceHolder AdminView;
        protected GridView CheckProjects;
        protected GridView AllProjects;
        protected Button newProject;

        // SR test
        IQueryable<Project> projects;
        //~SR test

        protected void Page_Init(object sender, EventArgs e)
        {
            Semester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            Semester.DataBind();
            var currentSemester = db.Semester.Where(s => s.StartDate > DateTime.Now).OrderBy(s => s.StartDate).First().Id;
            Semester.SelectedValue = currentSemester.ToString();
            Semester.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Alle Semester", ""));
            Semester.Items.Insert(1, new System.Web.UI.WebControls.ListItem("――――――――――――――――", "."));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            projects = db.Projects.Select(i => i);

            // Display project approval list if user is admin
            if (ShibUser.IsAdmin())
            {
                AdminView.Visible = true;
                CheckProjects.DataSource =
                    from item in projects
                    where item.State == ProjectState.Submitted && (int?)item.DepartmentId == ShibUser.GetDepartmentId()
                    select item into i
                    select getProjectSingleElement(i);
            }


            if (!base.IsPostBack && Session["listFilter"] != null)
            {
                whichOwner.SelectedValue = (string)Session["listFilter"];
                Semester.SelectedValue = (string)Session["whichSemester"];
            }
            else
            {
                Session["listFilter"] = whichOwner.SelectedValue;
                Session["whichSemester"] = Semester.SelectedValue;
            }

            AllProjects.DataSource =
                from i in filterRelevantProjects(projects, (string)Session["listFilter"])
                select getProjectSingleElement(i);
            AllProjects.DataBind();
            CheckProjects.DataBind();

            //Disabling the "-----" element in the Dropdownlist. So the item "Alle Semester" is separated from the rest
            Semester.Items.FindByValue(".").Attributes.Add("disabled", "disabled");
        }





        private IQueryable<Project> filterRelevantProjects(IQueryable<Project> allProjects, string filter)
        {

            IQueryable<Project> projects = allProjects;
            switch (filter)
            {
                case "OwnProjects":
                    if (Semester.SelectedValue == "")
                    {
                        projects =
                            from p in projects
                            where (p.Creator == ShibUser.GetEmail() || p.Advisor2Mail == ShibUser.GetEmail() || p.Advisor1Mail == ShibUser.GetEmail()) && p.State != ProjectState.Deleted
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    else
                    {
                        projects =
                            from p in projects
                            where (p.Creator == ShibUser.GetEmail() || p.Advisor1Mail == ShibUser.GetEmail() || p.Advisor2Mail == ShibUser.GetEmail())
                                && (p.State != ProjectState.Deleted)
                                && (((p.Semester.Id == int.Parse(Semester.SelectedValue) && p.State==ProjectState.Published) || (int.Parse(Semester.SelectedValue) == ProStudCreator.Semester.NextSemester.Id && p.Semester == null) || ((p.State !=ProjectState.Deleted && p.State != ProjectState.Published) && int.Parse(Semester.SelectedValue) == ProStudCreator.Semester.NextSemester.Id) ))
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    break;
                case "AllProjects":
                    if (Semester.SelectedValue == "")
                    {
                        projects =
                            from p in projects
                            where p.State == ProjectState.Published
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    else
                    {
                        projects =
                            from p in projects
                            where p.State == ProjectState.Published && p.Semester.Id == int.Parse(Semester.SelectedValue)
                            orderby p.Department.DepartmentName, p.ProjectNr
                            select p;
                    }
                    break;
            }
            return projects;
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
        protected void AllProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Project project = db.Projects.Single((Project item) => item.Id == ((ProjectSingleElement)e.Row.DataItem).id);

                if (project.State == ProjectState.Published)
                {
                    if (!ShibUser.IsAdmin())
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
                {
                    col = new Color?(ColorTranslator.FromHtml("#A9F5A9"));
                }
                else if (project.State == ProjectState.Rejected)
                {
                    col = new Color?(ColorTranslator.FromHtml("#F5A9A9"));
                }
                else if (project.State == ProjectState.Submitted)
                {
                    col = new Color?(ColorTranslator.FromHtml("#ffcc99"));
                }
                else if (project.OverOnePage)
                {
                    col = new Color?(ColorTranslator.FromHtml("#F3F781"));
                }
                if (col.HasValue)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        cell.BackColor = col.Value;
                    }
                }
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

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (AllProjects.Rows.Count != 0)
            {
                byte[] bytesInStream;
                using (var output = new MemoryStream())
                {
                    using (var document = PdfCreator.CreateDocument())
                    {
                        PdfCreator pdfCreator = new PdfCreator();
                        pdfCreator.AppendToPDF(document, output,
                            ((IEnumerable<ProjectSingleElement>)AllProjects.DataSource)
                                .Select(p => db.Projects.Single(pr => pr.Id == p.id))
                                .OrderBy(p => p.Reservation1Name != "")
                                .ThenBy(p => p.Department.DepartmentName)
                                .ThenBy(p => p.ProjectNr)
                        );
                    }
                    bytesInStream = output.ToArray();
                }
                Response.Clear();
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=AllProjects.pdf");
                Response.BinaryWrite(bytesInStream);
                Response.End();
            }
            else
            {
                string message = "In dieser Kategorie sind keine Projekte vorhanden!";
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(base.GetType(), "alert", sb.ToString());
            }
        }

        protected void AllProjectsAsExcel_Click(object sender, EventArgs e)
        {
            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                ExcelCreator.GenerateProjectList(output, ((IEnumerable<ProjectSingleElement>)AllProjects.DataSource)
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
