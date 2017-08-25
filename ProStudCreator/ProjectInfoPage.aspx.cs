using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Org.BouncyCastle.X509;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        public static bool OverRide = false;
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;

        private ProjectTypes type = ProjectTypes.NotDefined;
        private bool userCanEditAfterStart;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single(p => (int?)p.Id == id);
            }
            else
            {
                Response.Redirect("Projectlist.aspx");
                Response.End();
            }

            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == project.Id && !item.Deleted).Select(i => getProjectSingleAttachment(i));
            gridProjectAttachs.DataBind();

            if (Page.IsPostBack) return;
            

            //All Admins or Owners
            userCanEditAfterStart = project.UserCanEditAfterStart();

            //set the Semester if it isn't set already
            project.Semester = project.Semester ?? Semester.NextSemester(db);

            //Project title
            ProjectTitle.Text = project.Name;


            //Set the Students
            Student1Name.Text = !string.IsNullOrEmpty(project.LogStudent1Name)
                ? "<a href=\"mailto:" + project.LogStudent1Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Student2Name.Text = !string.IsNullOrEmpty(project.LogStudent2Name)
                ? "<a href=\"mailto:" + project.LogStudent2Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";

            //Set the Advisor
            Advisor1Name.Text = !string.IsNullOrEmpty(project.Advisor1Name)
                ? "<a href=\"mailto:" + project.Advisor1Mail + "\">" +
                  Server.HtmlEncode(project.Advisor1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Advisor2Name.Text = !string.IsNullOrEmpty(project.Advisor2Name)
                ? "<a href=\"mailto:" + project.Advisor2Mail + "\">" +
                  Server.HtmlEncode(project.Advisor2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";


            //Set the Expert
            if (project.LogProjectType != null)
                if (!project.LogProjectType.P5 && project.LogProjectType.P6)
                    ExpertName.Text = !string.IsNullOrEmpty(project.LogExpertID.ToString())
                        ? "<a href=\"mailto:" + project.Expert.Mail + "\">" +
                          Server.HtmlEncode(project.Expert.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "Wird noch organisiert.";
                else
                    ExpertName.Text = "Noch nicht entschieden.";
            else
                ExpertName.Text = "Noch nicht entschieden.";


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
                lblProjectDuration.Text = "2 Semester";
            else if (project?.LogProjectDuration == 1)
                lblProjectDuration.Text = "1 Semester";
            else
                lblProjectDuration.Text = "?";

            //Sets the DeliveryInfos
            SetProjectDeliveryLabels();

            //Set the Exhibition date? of the Bachelorthesis
            ProjectExhibition.Text = project.Semester.ExhibitionBachelorThesis;

            //Set the LogLanguage
            if (project.LogLanguageEnglish != null && project.LogLanguageGerman != null)
                if (project.LogLanguageEnglish.Value && !project.LogLanguageGerman.Value)
                    drpLogLanguage.SelectedValue = "1";
                else
                    drpLogLanguage.SelectedValue = "2";
            else
                drpLogLanguage.SelectedValue = "0";

            drpLogLanguage.Items[0].Text = userCanEditAfterStart ? "(Bitte Auswählen)" : "Noch nicht entschieden";

            //Set the Grades
            nbrGradeStudent1.Text = project?.LogGradeStudent1 == null
                ? ""
                : project?.LogGradeStudent1.Value.ToString("N1", CultureInfo.InvariantCulture);
            nbrGradeStudent2.Text = project?.LogGradeStudent2 == null
                ? ""
                : project?.LogGradeStudent2.Value.ToString("N1", CultureInfo.InvariantCulture);

            //set the Labels to the Grades
            lblGradeStudent1.Text = $"Note von {project?.LogStudent1Name ?? "Student/in 1"}:";
            lblGradeStudent2.Text = $"Note von {project?.LogStudent2Name ?? "Student/in 2"}:";


            //fill the Billingstatus dropdown with Data
            drpBillingstatus.DataSource = db.BillingStatus;
            drpBillingstatus.DataBind();
            drpBillingstatus.Items.Insert(0,
                new ListItem(userCanEditAfterStart ? "(Bitte Auswählen)" : "Noch nicht eingetragen",
                    "ValueWhichNeverWillBeGivenByTheDB"));
            drpBillingstatus.SelectedValue = project?.BillingStatusID?.ToString() ??
                                             "ValueWhichNeverWillBeGivenByTheDB";

            //Set the data from the addressform
            if (string.IsNullOrEmpty(project.ClientCompany))
            {
                radioClientType.SelectedValue = "PrivatePerson";
                divClientCompany.Visible = false;
            }
            else
            {
                radioClientType.SelectedValue = "Company";
                divClientCompany.Visible = true;
            }
            txtClientCompany.Text = project?.ClientCompany;
            drpClientTitle.SelectedValue = project?.ClientAddressTitle == "Herr" ? "1" : "2";
            txtClientName.Text = project?.ClientPerson;
            txtClientDepartment.Text = project?.ClientAddressDepartment;
            txtClientStreet.Text = project?.ClientAddressStreet;
            txtClientPLZ.Text = project?.ClientAddressPostcode;
            txtClientCity.Text = project?.ClientAddressCity;
            txtClientReference.Text = project?.ClientReferenceNumber;
            txtClientEmail.Text = project?.ClientMail;

            //disable for unauthorized Users
            ProjectTitle.Enabled = userCanEditAfterStart && project.CanEditTitle();

            drpLogLanguage.Enabled =
                nbrGradeStudent1.Enabled =
                    nbrGradeStudent2.Enabled =
                        BtnSaveBetween.Enabled =
                            BtnSaveChanges.Enabled =
                                drpBillingstatus.Enabled = userCanEditAfterStart;

            //set the visibility
            if (project.Department.ShowDefenseOnInfoPage) //i4ds
                divExpert.Visible = project?.LogProjectType?.P6 ?? false;
            else //imvs
                divExpert.Visible = false;

            divPresentation.Visible = project?.Department?.ShowDefenseOnInfoPage ?? false;

            divBachelor.Visible = project?.LogProjectType?.P6 ?? false;

            divGradeStudent1.Visible = !string.IsNullOrEmpty(project?.LogStudent1Name);
            divGradeStudent2.Visible = !string.IsNullOrEmpty(project?.LogStudent2Name);

            BillAddressPlaceholder.Visible = project?.BillingStatus?.ShowAddressOnInfoPage == true &&
                                             userCanEditAfterStart;
        }

        private ProjectSingleAttachment getProjectSingleAttachment(Attachements attach)
        {
            return new ProjectSingleAttachment()
            {
                Guid = attach.ROWGUID,
                ProjectId = attach.ProjectId,
                Name = attach.FileName,
                Size = FixupSize((long) (attach.UploadSize ?? 0)),
                UploadUser = "<a href=\"mailto:" + attach.UploadUserMail + "\">" + Server.HtmlEncode(attach.UploadUserName).Replace(" ", "&nbsp;") + "</a>",
                FileType = getFileTypeImgPath(attach.FileName)

            };
        }

        private string getFileTypeImgPath(string filename)
        {

            switch (filename.Split('.').Last())
            {
                case "pdf":
                    return "Content/pdf.png";
                case "pptx":
                    return "Content/ppt.png";
                case "docx":
                    return "Content/doc.png";
                case "xlsx":
                    return "Content/xls.png";
                case "zip":
                case "rar":
                case "7z":
                    return "Content/zip.png";
                case "png":
                case "jpg":
                case "jpeg":
                    return "Content/jpg.png";
                default:
                    return "Content/file.png";
            }
        }

        private string FixupSize(long size)
        {
            if (size < 1024) //bytes
                return size + " B";
            if (size / 1024 < 1024) //kilobytes
                return (size / 1024) + " KB";
            if (size / (1024*1024) < 1024)
                return (size / (1024*1024)) + " MB";
        
            return Math.Round((float)size / ((float)1024*1024*1024), 2) + " GB";
        }


        private void SetProjectDeliveryLabels()
        {
            ProjectEndPresentation.Text = "?";

            if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP5) //IP5 1 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5FullPartTime;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP5) //IP5 2 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5Accompanying;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";
            }
            else if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP6) //IP6 1 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Normal;
                lblProjectEndPresentation.Text = "Verteidigung:";
                ProjectEndPresentation.Text = project.Semester.DefenseIP6Start + " - " + project.Semester.DefenseIP6End;
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP6) //IP6 2 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Variant2;
                lblProjectEndPresentation.Text = "Verteidigung:";
                ProjectEndPresentation.Text = project.Semester.DefenseIP6BStart + " - " +
                                              project.Semester.DefenseIP6BEnd;
            }
            else
            {
                ProjectDelivery.Text = "?";
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
            }

            if (project.LogDefenceDate != null)
                ProjectEndPresentation.Text = project.LogDefenceDate.ToString();
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
            ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
        }

        protected void BtnSaveChanges_OnClick(object sender, EventArgs e)
        {
            SaveChanges("Projectlist");
        }

        protected void BtnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projectlist");
        }

        protected void FileExplorer_OnItemCommand(object sender, EventArgs e)
        {
            //OverRide = FileExplorer.OverwriteExistingFiles;
        }

        protected void DrpBillingstatusChanged(object sender, EventArgs e)
        {
            userCanEditAfterStart = project.UserCanEditAfterStart();

            if (ShowAddressForm(drpBillingstatus.SelectedValue == "ValueWhichNeverWillBeGivenByTheDB"
                ? 0
                : int.Parse(drpBillingstatus.SelectedValue)))
            {
                BillAddressPlaceholder.Visible = userCanEditAfterStart;
                txtClientCompany.Text = project?.ClientCompany;
                drpClientTitle.SelectedValue = project?.ClientAddressTitle == "Herr" ? "1" : "2";
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

            return allBillingstatuswhichShowForm.Any(billingStatus => billingStatus.Id == id);
        }

        protected void BtnSaveBetween_OnClick(object sender, EventArgs e)
        {
            SaveChanges("ProjectInfoPage?id=" + project.Id);
        }

        private void SaveChanges(string redirectTo)
        {
            userCanEditAfterStart = project.UserCanEditAfterStart();

            string validationMessage = null;
            if (userCanEditAfterStart)
            {
                project.Name = ProjectTitle.Text.FixupParagraph();

                if (nbrGradeStudent1.Text != "")
                    project.LogGradeStudent1 = float.Parse(nbrGradeStudent1.Text.Replace(",", "."),
                        CultureInfo.InvariantCulture);

                if (nbrGradeStudent2.Text != "")
                    project.LogGradeStudent2 = float.Parse(nbrGradeStudent2.Text.Replace(",", "."),
                        CultureInfo.InvariantCulture);

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

                project.BillingStatusID = drpBillingstatus.SelectedIndex == 0
                    ? (int?)null
                    : int.Parse(drpBillingstatus.SelectedValue);

                //this sould always be under the project.BillingstatusId statement
                if (project?.BillingStatus?.ShowAddressOnInfoPage == true)
                    if (radioClientType.SelectedValue == "Company" && txtClientCompany.Text == "" ||
                        txtClientName.Text == "" || txtClientStreet.Text == "" ||
                        txtClientPLZ.Text == "" || txtClientCity.Text == "")
                    {
                        validationMessage = "Bitte füllen Sie alle Pflichtfelder aus.";
                    }
                    else
                    {
                        project.ClientCompany = txtClientCompany.Text;
                        project.ClientAddressTitle = drpClientTitle.SelectedValue == "1" ? "Herr" : "Frau";
                        project.ClientPerson = txtClientName.Text;
                        project.ClientMail = txtClientEmail.Text == "" ? null : txtClientEmail.Text;
                        project.ClientAddressDepartment = txtClientDepartment.Text == ""
                            ? null
                            : txtClientDepartment.Text;
                        project.ClientAddressStreet = txtClientStreet.Text == "" ? null : txtClientStreet.Text;
                        project.ClientAddressPostcode = txtClientPLZ.Text == "" ? null : txtClientPLZ.Text;
                        project.ClientAddressCity = txtClientCity.Text == "" ? null : txtClientCity.Text;
                        project.ClientReferenceNumber = txtClientReference.Text == ""
                            ? null
                            : txtClientReference.Text;
                    }

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
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected void radioClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            divClientCompany.Visible = radioClientType.SelectedValue == "Company";
        }

        protected void OnUploadComplete(object sender, AjaxFileUploadEventArgs e)
        {
            var attachement = CreateNewAttach(e.FileSize, e.FileName);
            SaveFileInDb(attachement, e.GetStreamContents());

            e.GetStreamContents().Close();


            var di = new DirectoryInfo(Path.GetTempPath() + "_AjaxFileUpload");

            foreach (var dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private Attachements CreateNewAttach(long fileSize, string fileName)
        {

            var attach = new Attachements
            {
                ProjectId = int.Parse(Request.QueryString["id"]),
                UploadUserMail = ShibUser.GetEmail(),
                UploadUserName = ShibUser.GetFullName(),
                UploadDate = DateTime.Now,
                UploadSize = fileSize,
                ProjectAttachement = new Binary(new byte[0]),
                FileName = fileName
            };
            db.Attachements.InsertOnSubmit(attach);
            db.SubmitChanges();

            return attach;

        }


        private void SaveFileInDb(Attachements attach, Stream file)
        {
            using (var connection = new SqlConnection(db.Connection.ConnectionString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        "SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ROWGUID = @ROWGUID",
                        connection);
                command.Parameters.AddWithValue("@ROWGUID", attach.ROWGUID.ToString());

                using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    command.Transaction = tran;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the pointer for file
                            var pathfile = reader.GetString(0);
                            var transactionContext = reader.GetSqlBytes(1).Buffer;

                            using (
                                Stream fileStream = new SqlFileStream(pathfile, transactionContext, FileAccess.Write,
                                    FileOptions.SequentialScan, 0))
                            {
                                file.CopyTo(fileStream, 65536);
                            }
                        }
                    }
                    tran.Commit();
                }
            }
        }

        protected void gridProjectAttachs_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var project =
                db.Projects.Single(item => item.Id == ((ProjectSingleAttachment)e.Row.DataItem).ProjectId);

            if (!project.UserIsOwner() && !ShibUser.CanEditAllProjects())
                e.Row.Cells[e.Row.Cells.Count - 2].Visible = false;
        }

        protected void gridProjectAttachs_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "deleteProjectAttach") return;
            var guid = new Guid(e.CommandArgument.ToString());
            var attach = db.Attachements.Single(a => a.ROWGUID == guid);
            attach.DeletedDate = DateTime.Now;
            attach.Deleted = true;
            attach.DeletedUser = ShibUser.GetEmail();
            db.SubmitChanges();


            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == project.Id && !item.Deleted).Select(i => getProjectSingleAttachment(i));
            gridProjectAttachs.DataBind();

            updateProjectAttachements.Update();
            
        }

        private enum ProjectTypes
        {
            IP5,
            IP6,
            NotDefined
        }
    }

    public class ProjectSingleAttachment
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string UploadUser { get; set; }
        public int ProjectId { get; set; }
        public string FileType { get; set; }
    }
}