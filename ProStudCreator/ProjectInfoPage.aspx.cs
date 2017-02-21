using System;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using Telerik.Web.UI.AsyncUpload;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;
        private DateTime today = DateTime.Now;
        private DateTime deliveryDate;
        private bool canPostEdit = false;

        private enum ProjectTypes
        {
            IP5,
            IP6,
            NotDefined
        }

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


            //If the User can edit the Project after it has started
            canPostEdit =
            (ShibUser.IsAdmin() || ShibUser.GetEmail() == project.Advisor1Mail ||
             ShibUser.GetEmail() == project.Advisor2Mail);

            if (Page.IsPostBack) return;

            //set the Semester if it isn't set already
            project.Semester = project.Semester ?? Semester.NextSemester(db);

            //Project title
            ProjectTitle.Text = project.Name;


            //Set the Students
            Student1Name.Text = (!string.IsNullOrEmpty(project.LogStudent1Name))
                ? "<a href=\"mailto:" + project.LogStudent1Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Student2Name.Text = (!string.IsNullOrEmpty(project.LogStudent2Name))
                ? "<a href=\"mailto:" + project.LogStudent2Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";

            //Set the Advisor
            Advisor1Name.Text = (!string.IsNullOrEmpty(project.Advisor1Name))
                ? "<a href=\"mailto:" + project.Advisor1Mail + "\">" +
                  Server.HtmlEncode(project.Advisor1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Advisor2Name.Text = (!string.IsNullOrEmpty(project.Advisor2Name))
                ? "<a href=\"mailto:" + project.Advisor2Mail + "\">" +
                  Server.HtmlEncode(project.Advisor2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";


            //Set the Expert
            if (project.LogProjectType != null)
            {
                if (!project.LogProjectType.P5 && project.LogProjectType.P6)
                {
                    ExpertName.Text = (!string.IsNullOrEmpty(project.LogExpertID.ToString()))
                        ? "<a href=\"mailto:" + project.Expert.Mail + "\">" +
                          Server.HtmlEncode(project.Expert.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "Wird noch organisiert.";
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


            //Set the Projecttype
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

            //set the Project duration
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

            //Sets the DeliveryDate / dont call further up!
            deliveryDate = SetDates();


            //set the date of the EndPresentation
            ProjectEndPresentation.Text = project.LogDefenceDate?.ToString() ?? "?";

            //Set the Exhibition date? of the Bachelorthesis
            ProjectExhibition.Text = project.Semester.ExhibitionBachelorThesis;

            //Set the LogLanguage
            if (project.LogLanguageEnglish != null && project.LogLanguageGerman != null)
            {
                if (project.LogLanguageEnglish.Value && !project.LogLanguageGerman.Value)
                {
                    drpLogLanguage.SelectedValue = "1";
                }
                else
                {
                    drpLogLanguage.SelectedValue = "2";
                }
            }
            else
            {
                drpLogLanguage.SelectedValue = "0";
            }
            drpLogLanguage.Items[0].Text = (canPostEdit) ? "(Bitte Auswählen)" : "Noch nicht entschieden";

            //Set the Grades
            nbrGradeStudent1.Text = (project?.LogGradeStudent1 == null) ? "" : project?.LogGradeStudent1.Value.ToString("N1", CultureInfo.InvariantCulture);
            nbrGradeStudent2.Text = (project?.LogGradeStudent2 == null) ? "" : project?.LogGradeStudent2.Value.ToString("N1", CultureInfo.InvariantCulture);

            //set the Labels to the Grades
            lblGradeStudent1.Text = $"Note von {project?.LogStudent1Name ?? "Student/in 1"}:";
            lblGradeStudent2.Text = $"Note von {project?.LogStudent2Name ?? "Student/in 2"}:";



            //fill the Billingstatus dropdown with Data
            drpBillingstatus.DataSource = db.BillingStatus;
            drpBillingstatus.DataBind();
            drpBillingstatus.Items.Insert(0, new ListItem((canPostEdit) ? "(Bitte Auswählen)" : "Noch nicht eingetragen", "ValueWithNeverWillBeGivenByTheDB"));
            drpBillingstatus.SelectedValue = project?.BillingStatusID?.ToString() ?? "ValueWithNeverWillBeGivenByTheDB";

            //Set the data from the addressform
            txtClientCompany.Text = project?.ClientCompany;
            drpClientTitle.SelectedValue = (project?.ClientAddressTitle == "Herr") ? "1" : "2";
            txtClientName.Text = project?.ClientPerson;
            txtClientDepartment.Text = project?.ClientAddressDepartment;
            txtClientStreet.Text = project?.ClientAddressStreet;
            txtClientPLZ.Text = project?.ClientAddressPostcode;
            txtClientCity.Text = project?.ClientAddressCity;
            txtClientReference.Text = project?.ClientReferenceNumber;

            //disable for the unauthorized Users
            ProjectTitle.ReadOnly = !(canPostEdit && deliveryDate.AddDays(-28) < today);
            drpLogLanguage.Enabled =
                nbrGradeStudent1.Enabled =
                    nbrGradeStudent2.Enabled =
                        drpBillingstatus.Enabled = canPostEdit;

            //set the visibility
            if (project.Department.ShowDefenseOnInfoPage) //i4ds
            {
                divExpert.Visible = project?.LogProjectType?.P6 ?? false;
            }
            else //imvs
            {
                divExpert.Visible = false;
            }

            divPresentation.Visible = project?.Department?.ShowDefenseOnInfoPage ?? false;

            divBachelor.Visible = project?.LogProjectType?.P6 ?? false;

            divGradeStudent1.Visible = !string.IsNullOrEmpty(project?.LogStudent1Name);
            divGradeStudent2.Visible = !string.IsNullOrEmpty(project?.LogStudent2Name);

            BillAddressPlaceholder.Visible = (project?.BillingStatus?.ShowAddressOnInfoPage == true && canPostEdit);
        }

        private DateTime SetDates()
        {
            DateTime dbDate;
            if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP5) //IP5 Projekt Voll/TeilZeit
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5FullPartTime;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";

                return DateTime.TryParseExact(project.Semester.SubmissionIP5FullPartTime, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate
                    : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP5) //IP5 Berufsbegleitend
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5Accompanying;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";

                return DateTime.TryParseExact(project.Semester.SubmissionIP5Accompanying, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate
                    : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP6) //IP6 Variante 1 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Normal;
                lblProjectEndPresentation.Text = "Verteidigung:";

                return DateTime.TryParseExact(project.Semester.SubmissionIP6Normal, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate
                    : Semester.NextSemester(db).EndDate;
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP6) //IP6 Variante 2 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Variant2;
                lblProjectEndPresentation.Text = "Verteidigung:";

                return DateTime.TryParseExact(project.Semester.SubmissionIP6Variant2, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate
                    : Semester.NextSemester(db).EndDate;
            }
            else
            {
                ProjectDelivery.Text = "?";
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text = "?";
                return Semester.NextSemester(db).EndDate;
            }
        }

        private void ReturnAlert(string message)
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
            SaveChanges("Projectlist");
        }

        protected void BtnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projectlist");
        }

        protected void btnDeleteDoc_OnClick(object sender, EventArgs e)
        {
            DeleteFile("ProjectDocument");
            Response.Redirect("ProjectInfoPage?id=" + project.Id);
        }

        protected void btnDeletePresentation_OnClick(object sender, EventArgs e)
        {
            DeleteFile("ProjectPresentation");
            Response.Redirect("ProjectInfoPage?id=" + project.Id);
        }

        protected void btnDeleteCode_OnClick(object sender, EventArgs e)
        {
            DeleteFile("ProjectCode");
            Response.Redirect("ProjectInfoPage?id=" + project.Id);
        }

        private void StreamAllFilesToDb()
        {
            //for (var i = 0; AsyncUploadProject.UploadedFiles[i] != null; i++)
            //{
            //}
        }

        private void StreamFiletoDb(Stream input, string columnname)
        {
            using (var connection = new SqlConnection())
            {
                var cmd =
                    new SqlCommand(
                        $"INSERT INTO Projects(ROWGUID, {columnname}) VALUES(newsequentialid(),CAST('' AS varbinary(max))) WHERE Id = {project.Id};")
                    {
                        CommandType = CommandType.Text,
                        Connection = connection
                    };
                connection.Open();
                cmd.ExecuteNonQuery();
            }


            using (var connection = new SqlConnection())
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        $"SELECT TOP(1) {columnname}.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM BothTable WHERE Id = {project.Id}",
                        connection);

                var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = tran;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Get the pointer for file
                        var path = reader.GetString(0);
                        var transactionContext = reader.GetSqlBytes(1).Buffer;

                        using (
                            Stream fileStream = new SqlFileStream(path, transactionContext, FileAccess.Write,
                                FileOptions.SequentialScan, allocationSize: 0))
                        {
                            input.CopyTo(fileStream, 65536);
                        }
                    }
                }
                tran.Commit();
            }
        }

        private void DeleteFile(string columnname)
        {
            using (var connection = new SqlConnection())
            {
                connection.Open();

                var command =
                    new SqlCommand($"INSERT INTO Prostud.dbo.Projects({columnname}) VALUES(NULL) WHERE Id = {project.Id}", connection);

                var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = tran;
                tran.Commit();
            }
        }

        protected void DrpBillingstatusChanged(object sender, EventArgs e)
        {
            if (ShowAddressForm(drpBillingstatus.SelectedValue == "ValueWithNeverWillBeGivenByTheDB" ? 0 : int.Parse(drpBillingstatus.SelectedValue)))
            {
                BillAddressPlaceholder.Visible = canPostEdit;
                txtClientCompany.Text = project?.ClientCompany;
                drpClientTitle.SelectedValue = (project?.ClientAddressTitle == "Herr") ? "1" : "2";
                txtClientName.Text = project?.ClientPerson;
                txtClientDepartment.Text = project?.ClientAddressDepartment;
                txtClientStreet.Text = project?.ClientAddressStreet;
                txtClientPLZ.Text = project?.ClientAddressPostcode;
                txtClientCity.Text = project?.ClientAddressCity;
                txtClientReference.Text = project?.ClientReferenceNumber;
            }
            else
            {
                BillAddressPlaceholder.Visible = false;
            }
            BillAddressForm.Update();
        }

        private bool ShowAddressForm(int id)
        {
            var allBillingstatuswhichShowForm = db.BillingStatus.Where(s => s.ShowAddressOnInfoPage).ToArray();

            foreach (var billingStatus in allBillingstatuswhichShowForm)
            {
                if (billingStatus.id == id)
                    return true;
            }
            return false;
        }

        protected void BtnSaveBetween_OnClick(object sender, EventArgs e)
        {
            SaveChanges("ProjectInfoPage?id=" + project.Id);
        }

        private void SaveChanges(string redirectTo)
        {
            var oldTitle = project.Name;
            string validationMessage = null;
            if (canPostEdit)
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
                    if (nbrGradeStudent1.Text != "")
                    {
                        project.LogGradeStudent1 = float.Parse(nbrGradeStudent1.Text.Replace(",","."),System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (nbrGradeStudent2.Text != "")
                    {
                        project.LogGradeStudent2 = float.Parse(nbrGradeStudent2.Text.Replace(",","."), System.Globalization.CultureInfo.InvariantCulture);
                    }

                    switch (drpLogLanguage.SelectedValue)
                    {
                        case "1":
                            project.LogLanguageEnglish = true;
                            project.LogLanguageGerman = false;
                            break;
                        case "2":
                            project.LogLanguageEnglish = false;
                            project.LogLanguageGerman = true;
                            break;
                        default:
                            project.LogLanguageGerman = null;
                            project.LogLanguageEnglish = null;
                            break;
                    }

                    project.BillingStatusID = (drpBillingstatus.SelectedIndex == 0)
                        ? (int?)null
                        : int.Parse(drpBillingstatus.SelectedValue);

                    //this sould always be under the project.BillingstatusId statement
                    if (project?.BillingStatus?.ShowAddressOnInfoPage == true)
                    {
                        if (txtClientCompany.Text == "" || txtClientName.Text == "" || txtClientStreet.Text == "" ||
                            txtClientPLZ.Text == "" || txtClientCity.Text == "")
                        {
                            validationMessage = "Bitte füllen Sie alle Pflichtfelder aus.";
                        }
                        else
                        {
                            project.ClientCompany = txtClientCompany.Text;
                            project.ClientAddressTitle = (drpClientTitle.SelectedValue == "1") ? "Herr" : "Frau";
                            project.ClientPerson = txtClientName.Text;
                            project.ClientAddressDepartment = (txtClientDepartment.Text == "")
                                ? null
                                : txtClientDepartment.Text;
                            project.ClientAddressStreet = (txtClientStreet.Text == "") ? null : txtClientStreet.Text;
                            project.ClientAddressPostcode = (txtClientPLZ.Text == "") ? null : txtClientPLZ.Text;
                            project.ClientAddressCity = (txtClientCity.Text == "") ? null : txtClientCity.Text;
                            project.ClientReferenceNumber = (txtClientReference.Text == "")
                                ? null
                                : txtClientReference.Text;
                        }

                    }
                    StreamAllFilesToDb();

                    if (validationMessage == null)
                    {
                        project.Name = ProjectTitle.Text.FixupParagraph();
                        project.ModificationDate = DateTime.Now;
                        project.LastEditedBy = ShibUser.GetEmail();
                        db.SubmitChanges();
                        Response.Redirect(redirectTo);
                    }
                    else
                    {
                        ReturnAlert(validationMessage);
                    }

                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

    }
}