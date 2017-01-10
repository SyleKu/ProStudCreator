using System;
using System.Data.Linq;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.OpenXml4Net.Exceptions;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;
        private ProjectType projectPriority = new ProjectType();
        private DateTime today = DateTime.Now;
        private DateTime deliveryDate;
        private enum ProjectTypes { IP5, IP6, NotDefined }
        private ProjectTypes type = ProjectTypes.NotDefined;

        protected void Page_Load(object sender, EventArgs e)
        {

            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single((Project p) => (int?)p.Id == id);
            }
            else
            {
                Response.Redirect("Projectlist.aspx");
                Response.End();
            }

            if (Page.IsPostBack) return;
            project.Semester = project.Semester ?? Semester.NextSemester(db);

            ProjectTitle.Text = project.Name;


            Student1Name.Text = (!string.IsNullOrEmpty(project.LogStudent1Name))
                ? "<a href=\"mailto:" + project.LogStudent1Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Student2Name.Text = (!string.IsNullOrEmpty(project.LogStudent2Name))
                ? "<a href=\"mailto:" + project.LogStudent2Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";

            Advisor1Name.Text = (!string.IsNullOrEmpty(project.Advisor1Name))
                ? "<a href=\"mailto:" + project.Advisor1Mail + "\">" +
                  Server.HtmlEncode(project.Advisor1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Advisor2Name.Text = (!string.IsNullOrEmpty(project.Advisor2Name))
                ? "<a href=\"mailto:" + project.Advisor2Mail + "\">" +
                  Server.HtmlEncode(project.Advisor2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";

            if (project.LogProjectType != null)
            {
                if (project.LogProjectType.P5 && !project.LogProjectType.P6)
                {
                    ExpertName.Text = "Bei IP5 Projekte gibt es keine Experten.";
                }
                else if (!project.LogProjectType.P5 && project.LogProjectType.P6)
                {
                    ExpertName.Text = (!string.IsNullOrEmpty(project.LogExpertID.ToString()))
                        ? "<a href=\"mailto:" + project.Expert.Mail + "\">" +
                          Server.HtmlEncode(project.Expert.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "Wird noch organisiert";
                }
                else
                {
                    ExpertName.Text = "Noch nicht entschieden.";
                }
            }
            else
            {
                ExpertName.Text = "Noch nicht entschieden.";
            }




            if (project.LogProjectTypeID == null)
            {
                lblProjectType.Text = "?";
                type = ProjectTypes.NotDefined;
            }
            else if (project.LogProjectType.P5 && !project.LogProjectType.P6)
            {
                lblProjectType.Text = "IP5";
                type = ProjectTypes.IP5;
            }
            else if (!project.LogProjectType.P5 && project.LogProjectType.P6)
            {
                lblProjectType.Text = "IP6";
                type = ProjectTypes.IP6;
            }
            else
            {
                lblProjectType.Text = "?";
                type = ProjectTypes.NotDefined;
            }


            if (project?.LogProjectDuration == 2)
            {
                lblProjectDuration.Text = "2 Semester";
            }
            else if (project?.LogProjectDuration == 1)
            {
                lblProjectDuration.Text = "1 Semester";
            }
            else
            {
                lblProjectDuration.Text = "?";
            }

            ProjectEndPresentation.Text = project.LogDefenceDate?.ToString() ?? "?";

            //darf erst Hier aufgerufen werden.
            deliveryDate = SetDates();

            ProjectExhibition.Text = project.Semester.ExhibitionBachelorThesis;

            //Disable the Textbox if Title can't be changed anymore
            ProjectTitle.Enabled =
                (ShibUser.IsAdmin() || ShibUser.GetEmail() == project.Advisor1Mail ||
                 ShibUser.GetEmail() == project.Advisor2Mail)
                && deliveryDate.AddDays(-28) > today;

            SemesterDropdown.DataSource = db.BillingStatus;
            SemesterDropdown.DataBind();
            SemesterDropdown.Items.Insert(0, new ListItem("Unbekannt"));
            SemesterDropdown.SelectedIndex = project.BillingStatusID ?? 0;

            SemesterDropdown.Enabled = project.UserCanEdit();
        }

        private DateTime SetDates()
        {
            DateTime dbDate;
            if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP5) //IP5 Projekt Voll/TeilZeit
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5FullPartTime;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text = "Selber mit den Studierenden beschprechen.";

                return DateTime.TryParseExact(project.Semester.SubmissionIP5FullPartTime, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate) ? dbDate : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP5) //IP5 Berufsbegleitend
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5Accompanying;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text = "Selber mit den Studierenden beschprechen.";

                return DateTime.TryParseExact(project.Semester.SubmissionIP5Accompanying, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate) ? dbDate : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP6) //IP6 Variante 1 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Normal;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";

                return DateTime.TryParseExact(project.Semester.SubmissionIP6Normal, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate) ? dbDate : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP6) //IP6 Variante 2 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Variant2;
                lblProjectEndPresentation.Text = "Verteidigung:";

                return DateTime.TryParseExact(project.Semester.SubmissionIP6Variant2, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate) ? dbDate : Semester.NextSemester(db).EndDate;
            }
            else
            {
                ProjectDelivery.Text = "?";
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text = "?";
                return Semester.NextSemester(db).EndDate;
            }
        }

        private void ReturnAlert(String message)
        {
            var sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("alert('");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(base.GetType(), "alert", sb.ToString());
        }

        protected void BtnSaveChanges_OnClick(object sender, EventArgs e)
        {
            var oldTitle = project.Name;
            if (project.UserCanEdit())
            {
                project.Name = ProjectTitle.Text.FixupParagraph();
                db.SubmitChanges();
                project.OverOnePage = (new PdfCreator().CalcNumberOfPages(project) > 1);
                if (project.OverOnePage)
                {
                    project.Name = oldTitle;
                    db.SubmitChanges();
                    ReturnAlert("Die Änderung des Titels ist nicht möglich, weil das PDF zu lang werden würde.");
                    Response.Redirect("ProjectInfoPage?id=" + project.Id);
                }
                else
                {
                    //if (SemesterDropdown.SelectedIndex != 0)
                    //{
                    //    project.BillingStatusID = int.Parse(SemesterDropdown.SelectedValue);
                    //}
                    //else
                    //{
                    //    project.BillingStatusID = null;
                    //}

                    project.BillingStatusID = (SemesterDropdown.SelectedIndex == 0) ? (int?)null : int.Parse(SemesterDropdown.SelectedValue);
                    project.Name = ProjectTitle.Text.FixupParagraph();
                    project.ModificationDate = DateTime.Now;
                    project.LastEditedBy = ShibUser.GetEmail();
                    db.SubmitChanges();
                    Response.Redirect("ProjectInfoPage?id=" + project.Id);
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected void BtnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projectlist");
        }
    }
}
