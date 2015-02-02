using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.Net.Mail;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;

namespace ProStudCreator
{

    public class ProjectSingleElement
    {
        public int id { get; set; }
        public string advisorName { get; set; }
        public string advisorEmail { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
    }


    public partial class projectlist : System.Web.UI.Page
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        bool[] projectFilter = new bool[5];
        DateTime today = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (IsPostBack)
            {
                projectFilter = (bool[])ViewState["Filter"];
            }
            else
            {
                ViewState["Filter"] = projectFilter;
                ProjectsFilterAllProjects.Items[2].Selected = true;
            }

            if (User.IsInRole("Admin"))
            {
                AdminView.Visible = true;
                AdminViewPDF.Visible = true;

                int counter = 0;
                foreach (System.Web.UI.WebControls.ListItem item in ProjectsFilterAllProjects.Items)
                {
                    projectFilter[counter] = item.Selected;
                    counter++;
                }

                var projects = db.Projects.Where(i => true);

                CheckProjects.DataSource = projects.Where(item => !item.Published && !item.InProgress && !item.StateDeleted).Select(i => getProjectSingleElement(i));


                bool isFruehlingSemester = false;

                if (today.Month > 1 && today.Month < 9)
                {
                    isFruehlingSemester = true;
                }


                DateTime vonFilter;
                DateTime bisFilter;

                if (projectFilter[0])
                {
                    if (isFruehlingSemester)
                    {
                        vonFilter = new DateTime(today.Year, 2, 1);
                        bisFilter = new DateTime(today.Year, 8, 1);

                        projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                    }
                    else
                    {
                        if (today.Month > 6)
                        {
                            vonFilter = new DateTime(today.Year, 8, 1);
                            bisFilter = new DateTime(today.Year + 1, 2, 1);

                            projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                        }
                        else
                        {
                            vonFilter = new DateTime(today.Year - 1, 8, 1);
                            bisFilter = new DateTime(today.Year, 2, 1);

                            projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                        }
                    }
                    projects = projects.Where(item => !item.InProgress && !item.StateDeleted && item.Published);


                }

                else if (projectFilter[1])
                {
                    if (isFruehlingSemester)
                    {
                        vonFilter = new DateTime(today.Year - 1, 8, 1);
                        bisFilter = new DateTime(today.Year, 2, 1);

                        projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                    }
                    else
                    {
                        if (today.Month > 6)
                        {
                            vonFilter = new DateTime(today.Year, 2, 1);
                            bisFilter = new DateTime(today.Year, 8, 1);

                            projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                        }
                        else
                        {
                            vonFilter = new DateTime(today.Year - 1, 2, 1);
                            bisFilter = new DateTime(today.Year - 1, 8, 1);

                            projects = projects.Where(p => p.PublishedDate >= vonFilter && p.PublishedDate <= bisFilter);
                        }
                    }
                    projects = projects.Where(item => !item.InProgress && !item.StateDeleted && item.Published);
                }

                else if (projectFilter[2])
                {
                    projects = projects.Where(item => item.Creator == User.Identity.Name && !item.Published && !item.StateDeleted && item.InProgress);
                }
                else if (projectFilter[3])
                {
                    projects = projects.Where(item => item.Creator == User.Identity.Name && !item.Published && !item.InProgress && !item.StateDeleted);

                }
                else
                {
                    projects = projects.Where(item => item.Published && item.Creator == User.Identity.Name && !item.StateDeleted);
                }

                AllProjects.DataSource = projects.Select(i => getProjectSingleElement(i));
            }

            else
            {
                ProjectsFilterAllProjects.Items[0].Attributes.CssStyle.Add("display", "none");
                ProjectsFilterAllProjects.Items[1].Attributes.CssStyle.Add("display", "none");
                
                int counter = 0;
                foreach (System.Web.UI.WebControls.ListItem item in ProjectsFilterAllProjects.Items)
                {
                    projectFilter[counter] = item.Selected;
                    counter++;
                }

                var projects = db.Projects.Where(i => true);

                if (projectFilter[2])
                {
                    projects = projects.Where(item => item.Creator == User.Identity.Name && !item.Published && !item.StateDeleted && item.InProgress);
                }
                else if (projectFilter[3])
                {
                    projects = projects.Where(item => item.Creator == User.Identity.Name && !item.Published && !item.InProgress && !item.StateDeleted);

                }
                else
                {
                    projects = projects.Where(item => item.Published && item.Creator == User.Identity.Name && !item.StateDeleted);
                }

                AllProjects.DataSource = projects.Select(i => getProjectSingleElement(i));
            }
            CheckProjects.DataBind();
            AllProjects.DataBind();
        }

        private ProjectSingleElement getProjectSingleElement(Project i)
        {
            return new ProjectSingleElement()
               {
                   id = i.Id,
                   advisorName = Server.HtmlEncode(i.Advisor) + "<br />" + Server.HtmlEncode(i.Advisor2),
                   advisorEmail = Server.HtmlEncode(i.AdvisorMail) + "<br />" + Server.HtmlEncode(i.AdvisorMail2),
                   projectName = i.Name,
                   p5 = (i.POneID == 1 ? true : false || i.PTwoID == 3 ? true : false),
                   p6 = (i.POneID == 2 ? true : false || i.PTwoID == 2 ? true : false || i.PTwoID == 3 ? true : false),
                   projectType1 = "pictures/projectTyp" + 
                   (i.TypeDesignUX ? "DesignUX" : 
                   (i.TypeHW ? "HW" : 
                   (i.TypeCGIP ? "CGIP" : 
                   i.TypeMathAlg ? "MathAlg" : 
                   i.TypeAppWeb ? "AppWeb" :
                   i.TypeDBBigData ? "DBBigData" : "Transparent"))) + ".png",

                   projectType2 = "pictures/projectTyp" +
                   ((i.TypeHW && i.TypeDesignUX) ? "HW" :
                   (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" :
                   (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" :
                   (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" :
                   (i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb) ? "DBBigData" : "Transparent")) + ".png"
               };
        }

        protected void AllProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (projectFilter[3] || projectFilter[4])
            {
                e.Row.Cells[9].Visible = false;
                e.Row.Cells[10].Visible = false;
            }

            Project projects;
            foreach (GridViewRow row in AllProjects.Rows)
            {
                int rowProjectID = Int32.Parse(row.Cells[0].Text);
                projects = db.Projects.Single(item => item.Id == rowProjectID);
                if (projects.Published)
                {
                    for (int j = 0; j < AllProjects.Columns.Count; j++)
                    {
                        row.Cells[j].BackColor = System.Drawing.ColorTranslator.FromHtml("#A9F5A9");
                    }
                }
                else if (projects.Refused)
                {
                    for (int j = 0; j < AllProjects.Columns.Count; j++)
                    {
                        row.Cells[j].BackColor = System.Drawing.ColorTranslator.FromHtml("#F5A9A9");
                    }
                }
                else if (projects.OverOnePage)
                {
                    for (int j = 0; j < AllProjects.Columns.Count; j++)
                    {
                        row.Cells[j].BackColor = System.Drawing.ColorTranslator.FromHtml("#F3F781");
                    }
                }
                else
                {
                    // DO NOTHING !!
                }
            }
            e.Row.Cells[0].Visible = false;
        }

        protected void newProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("/AddNewProject");
        }


        protected void CheckProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "showProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument + "&show=true");
                    break;
                case "editProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument);
                    break;
                case "deleteProject":
                    var id = Convert.ToInt32(e.CommandArgument);
                    Project projects = db.Projects.Single(i => i.Id == id);
                    projects.StateDeleted = true;
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "SinglePDF":
                    var idPDF = Convert.ToInt32(e.CommandArgument);
                    CreateSinglePDF(idPDF);
                    break;
                default:
                    break;
            }
        }

        protected void AllProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "showProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument + "&show=true");
                    break;
                case "editProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument);
                    break;
                case "deleteProject":
                    var id = Convert.ToInt32(e.CommandArgument);
                    Project projects = db.Projects.Single(i => i.Id == id);
                    projects.StateDeleted = true;
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "SinglePDF":
                    var idPDF = Convert.ToInt32(e.CommandArgument);
                    CreateSinglePDF(idPDF);
                    break;
                default:
                    break;
            }
        }

        private void CreateSinglePDF(int idPDF)
        {
            float margin = Utilities.MillimetersToPoints(Convert.ToSingle(20));
            byte[] bytesInStream;

            using (var output = new MemoryStream())
            {
                using (var document = new Document(iTextSharp.text.PageSize.A4, margin, margin, margin, margin))
                {
                    PdfCreator pdfCreator = new PdfCreator();
                    pdfCreator.CreatePDF(document, output, false, idPDF, Request, null);
                }
                bytesInStream = output.ToArray();
            }

            Project projects = db.Projects.Single(i => i.Id == idPDF);

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename="+ projects.Department.DepartmentName + "01.pdf");
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (AllProjects.Rows.Count != 0)
            {
                float margin = Utilities.MillimetersToPoints(Convert.ToSingle(20));
                byte[] bytesInStream;
                List<int> gridViewProjectsId = new List<int>();

                foreach (GridViewRow row in AllProjects.Rows)
                {
                    gridViewProjectsId.Add(Int32.Parse(row.Cells[0].Text));
                }

                using (var output = new MemoryStream())
                {
                    using (var document = new Document(iTextSharp.text.PageSize.A4, margin, margin, margin, margin))
                    {
                        PdfCreator pdfCreator = new PdfCreator();
                        pdfCreator.CreatePDF(document, output, true, 0, Request, gridViewProjectsId);
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
                string message = "No projects are listed!";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
            }
        }
    }
}