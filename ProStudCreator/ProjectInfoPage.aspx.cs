using System;
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
using Telerik.Web.UI;
using Telerik.Web.UI.Widgets;
using System.Collections.Generic;
using System.Web.Services;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.SS.Formula.Functions;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;
        private bool userCanEditAfterStart = false;
        public static bool OverRide = false;

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
            

            if (Page.IsPostBack) return;

            //All Admins or Owners
            userCanEditAfterStart = project.UserCanEditAfterStart();

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

            //Sets the DeliveryInfos
            SetProjectDeliveryLabels();


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
            drpLogLanguage.Items[0].Text = (userCanEditAfterStart) ? "(Bitte Auswählen)" : "Noch nicht entschieden";

            //Set the Grades
            nbrGradeStudent1.Text = (project?.LogGradeStudent1 == null) ? "" : project?.LogGradeStudent1.Value.ToString("N1", CultureInfo.InvariantCulture);
            nbrGradeStudent2.Text = (project?.LogGradeStudent2 == null) ? "" : project?.LogGradeStudent2.Value.ToString("N1", CultureInfo.InvariantCulture);

            //set the Labels to the Grades
            lblGradeStudent1.Text = $"Note von {project?.LogStudent1Name ?? "Student/in 1"}:";
            lblGradeStudent2.Text = $"Note von {project?.LogStudent2Name ?? "Student/in 2"}:";



            //fill the Billingstatus dropdown with Data
            drpBillingstatus.DataSource = db.BillingStatus;
            drpBillingstatus.DataBind();
            drpBillingstatus.Items.Insert(0, new ListItem((userCanEditAfterStart) ? "(Bitte Auswählen)" : "Noch nicht eingetragen", "ValueWhichNeverWillBeGivenByTheDB"));
            drpBillingstatus.SelectedValue = project?.BillingStatusID?.ToString() ?? "ValueWhichNeverWillBeGivenByTheDB";

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
            ProjectTitle.Enabled = userCanEditAfterStart && project.CanEditTitle();

            drpLogLanguage.Enabled =
                nbrGradeStudent1.Enabled =
                    nbrGradeStudent2.Enabled =
                        BtnSaveBetween.Enabled =
                            BtnSaveChanges.Enabled =
                                drpBillingstatus.Enabled = userCanEditAfterStart;

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

            BillAddressPlaceholder.Visible = (project?.BillingStatus?.ShowAddressOnInfoPage == true && userCanEditAfterStart);

            //FileExplorer settings


            FileExplorer.Configuration.ContentProviderTypeName =
                    typeof(ExtendedFileProvider).AssemblyQualifiedName;
            FileExplorer.Configuration.EnableAsyncUpload = true;
            FileExplorer.Configuration.MaxUploadFileSize = int.MaxValue;
            FileExplorer.Language = "de-DE";
            FileExplorer.Grid.Columns.Remove(FileExplorer.Grid.Columns[1]);
            FileExplorer.Grid.Columns[0].HeaderText = "Datei";
            FileExplorer.FindControl("chkOverwrite").Visible = false;
        }

        private void SetProjectDeliveryLabels()
        {
            if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP5) //IP5 Projekt Voll/TeilZeit
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5FullPartTime;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";

            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP5) //IP5 Berufsbegleitend
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP5Accompanying;
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";
                                
            }
            else if (project?.LogProjectDuration == 1 && type == ProjectTypes.IP6) //IP6 Variante 1 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Normal;
                lblProjectEndPresentation.Text = "Verteidigung:";
                
            }
            else if (project?.LogProjectDuration == 2 && type == ProjectTypes.IP6) //IP6 Variante 2 Semester
            {
                ProjectDelivery.Text = project.Semester.SubmissionIP6Variant2;
                lblProjectEndPresentation.Text = "Verteidigung:";
            }
            else
            {
                ProjectDelivery.Text = "?";
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text = "?";
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

        protected void FileExplorer_OnItemCommand(object sender, EventArgs e)
        {
            OverRide = FileExplorer.OverwriteExistingFiles;
        }

        protected void DrpBillingstatusChanged(object sender, EventArgs e)
        {

            userCanEditAfterStart = project.UserCanEditAfterStart();

            if (ShowAddressForm(drpBillingstatus.SelectedValue == "ValueWhichNeverWillBeGivenByTheDB" ? 0 : int.Parse(drpBillingstatus.SelectedValue)))
            {
                BillAddressPlaceholder.Visible = userCanEditAfterStart;
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
                if (billingStatus.Id == id)
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
            userCanEditAfterStart = project.UserCanEditAfterStart();

            string validationMessage = null;
            if (userCanEditAfterStart)
            {
                project.Name = ProjectTitle.Text.FixupParagraph();

                if (nbrGradeStudent1.Text != "")
                {
                    project.LogGradeStudent1 = float.Parse(nbrGradeStudent1.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                }

                if (nbrGradeStudent2.Text != "")
                {
                    project.LogGradeStudent2 = float.Parse(nbrGradeStudent2.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
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

    }




    public class ExtendedFileProvider : FileBrowserContentProvider
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private readonly int _id;
        private Project project;

        public ExtendedFileProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag)
: base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            _id = int.Parse(HttpContext.Current.Request.QueryString["id"]);
            project = db.Projects.Single(i => i.Id == _id);
        }

        public override bool CanCreateDirectory => false;

        public override string DeleteFile(string path)
        {
            var fileName = GetFileName(path);
            var attach = db.Attachements.Single(i => i.ProjectId == _id && i.FileName == fileName);
            attach.Deleted = true;
            attach.DeletedDate = DateTime.Now;
            attach.DeletedUser = ShibUser.GetEmail();
            db.SubmitChanges();
            return "Die Datei wurde gelöscht!";
        }

        public override bool CheckDeletePermissions(string folderPath)
        {
            return (project.UserCanEditAfterStart());
        }

        public override bool CheckReadPermissions(string folderPath) => true;

        public override bool CheckWritePermissions(string folderPath)
        {
            return CheckDeletePermissions(folderPath);
        }

        public override string StoreFile(UploadedFile file, string path, string name, params string[] arguments)
        {

            var justifiedFileName = JusitfyFileName(file.FileName);

            var isDublication = false;

            //get All File to one Project out of the db
            var allFiles = db.Attachements.Where(i => i.ProjectId == _id && !i.Deleted);
            foreach (var attachement in allFiles)
            {
                if (attachement.FileName.Equals(justifiedFileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    isDublication = true;
                }
            }

            if (isDublication)
            {
                SaveFileInDb(file, db.Attachements.Single(i => i.FileName == justifiedFileName && i.ProjectId == _id && !i.Deleted));
                return string.Empty;
            }


            var attach = CreateNewAttach(file, justifiedFileName);
            try
            {
                SaveFileInDb(file, attach);
            }
            catch (Exception e)
            {
                attach.Deleted = true;
                attach.DeletedDate = DateTime.Now;
                attach.DeletedUser = null;
                attach.ProjectAttachement = Encoding.UTF8.GetBytes(e.ToString().ToCharArray());
                db.SubmitChanges();
            }
            return string.Empty;


        }

        private Attachements CreateNewAttach(UploadedFile file, string justifiedFileName)
        {
            var attach = new Attachements
            {
                ProjectId = _id,
                UploadUser = ShibUser.GetEmail(),
                UploadDate = DateTime.Now,
                UploadSize = file.ContentLength,
                ProjectAttachement = new Binary(new byte[0]),
                FileName = justifiedFileName,
            };
            db.Attachements.InsertOnSubmit(attach);
            db.SubmitChanges();

            return attach;
        }

        private void SaveFileInDb(UploadedFile file, Attachements attach)
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
                                    FileOptions.SequentialScan, allocationSize: 0))
                            {
                                file.InputStream.CopyTo(fileStream, 65536);
                            }
                        }
                    }
                    tran.Commit();
                }
            }
        }

        public override DirectoryItem ResolveRootDirectoryAsTree(string path)
        {
            return ResolveDirectory(path);
        }

        public override DirectoryItem ResolveDirectory(string path)
        {
            var attaches = db.Attachements.Where(i => i.ProjectId == _id && !i.Deleted);
            var files = new List<FileItem>();

            foreach (var attach in attaches)
            {
                files.Add(new FileItem(attach.FileName, "", (long)attach.UploadSize, $@"~\{_id}\{attach.FileName}", $@"\{_id}\{attach.FileName}", "", GetPermissions(path)));
            }

            var item = new DirectoryItem($"{_id}", @"\", @"\", "", GetPermissions(path), files.ToArray(), new DirectoryItem[0]);
            return item;
        }

        public override string GetFileName(string url)
        {
            return Path.GetFileName(url);
        }

        public override string GetPath(string url)
        {
            return url;
        }

        public override Stream GetFile(string url)
        {
            Stream returnStream = null;
            var fileName = GetFileName(url);
            using (SqlConnection connection = new SqlConnection(db.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ProjectId = @id AND FileName = '@filename';", connection);
                command.Parameters.AddWithValue("@id", _id);
                command.Parameters.AddWithValue("@filename", fileName);

                using (SqlTransaction tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    command.Transaction = tran;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the pointer for the file  
                            string path = reader.GetString(0);
                            byte[] transactionContext = reader.GetSqlBytes(1).Buffer;

                            // Create the SqlFileStream  
                            using (
                                Stream fileStream = new SqlFileStream(path, transactionContext, FileAccess.Read,
                                    FileOptions.SequentialScan, allocationSize: 0))
                            {
                                fileStream.CopyTo(returnStream);
                            }
                        }
                    }
                    tran.Commit();
                }
            }

            return returnStream;
        }

        public override string StoreBitmap(Bitmap bitmap, string url, ImageFormat format)
        {
            throw new NotImplementedException();
        }

        public override string DeleteDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override string CreateDirectory(string path, string name)
        {
            throw new NotImplementedException();
        }

        private PathPermissions GetPermissions(string folderPath)
        {
            PathPermissions permissions = PathPermissions.Read;
            if (CheckDeletePermissions(folderPath)) permissions = PathPermissions.Delete | permissions;
            if (CheckWritePermissions(folderPath)) permissions = PathPermissions.Upload | permissions;

            return permissions;
        }

        private string JusitfyFileName(string FileName)
        {

            var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            try
            {
                var stream = File.Create(Path.Combine(dir, FileName));
                stream.Dispose();
            }
            catch (IOException)
            {
                try
                {
                    File.Delete(Path.Combine(dir, FileName));
                }
                catch { }

                if (Path.GetInvalidFileNameChars().Any(i => FileName.Contains(i)))
                {
                    foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    {
                        if (FileName.Contains(invalidChar))
                        {
                            FileName = FileName.Replace(invalidChar, '_');
                        }
                    }
                    return JusitfyFileName(FileName);
                }
                else
                {
                    return JusitfyFileName("_" + FileName);
                }
            }
            finally
            {
                try
                {
                    File.Delete(Path.Combine(dir, FileName));
                }
                catch { }
                Directory.Delete(dir, true);
            }

            return FileName;
        }
    }
}