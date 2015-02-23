using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Net.Mail;

namespace ProStudCreator
{
    public partial class AddNewProject : System.Web.UI.Page
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        bool[] projectType = new bool[6];
        private static int id;
        private Project projects;
        ProjectType projectPriority = new ProjectType();
        DateTime today = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.IsInRole("Admin"))
            {
                AdminView.Visible = true;
            }
            if (IsPostBack)
            {
                projectType = (bool[])ViewState["Types"];
            }
            else
            {
                InitialPositionContent.Attributes.Add("placeholder", "Beispiel: Die Open-Source WebGL-Library three.js stellt Benutzern einen einfachen Editor zum Erstellen von 3D-Szenen zur Verfügung. Eine Grundversion dieses Editors ist unter http://threejs.org/editor abrufbar. Dieser Editor wird als Basis für die hochschulübergreifende strategische Initiative „Playful Media Practices“ verwendet, wo er zum Design audiovisueller Szenen verwendet wird. Diesem Editor fehlt jedoch eine Undo-/Redo-Funktion, welche in diesem Projekt hinzuzufügen ist.");
                ObjectivContent.Attributes.Add("placeholder", "Beispiel: Das Ziel dieser Arbeit ist die Erarbeitung eines Undo-/Redo-Konzepts für den three.js-Editor sowie dessen Implementation. Da three.js eine Library für die cutting-edge-Technologie WebGL ist, nutzt auch der three.js-Editor modernste Browsermittel wie LocalStorage oder FileAPI. Deshalb gilt es nicht, die Implementation kompatibel zu alten Browsern zu halten, sondern das Maximum aus aktuellen Browsern zu holen.");
                ProblemStatementContent.Attributes.Add("placeholder", "Beispiel: Der three.js-Editor hat mittlerweile eine beachtliche Komplexität erreicht, entsprechend muss für verschiedene Bereiche anders mit Undo&Redo umgegangen werden. Wenn beispielsweise jemand neue Texturen hochlädt, müssen die vorherigen Texturen im Speicher behalten werden.");
                ReferencesContent.Attributes.Add("placeholder", "Beispiel: JavaScript, Komplexe Datenstrukturen, Three.js/WebGL");
                RemarksContent.Attributes.Add("placeholder", "Beispiel: Ein Pullrequest der Implementation wird diese Erweiterung einem weltweiten Publikum öffentlich zugänglich machen. Sie leisten damit einen entscheidenden Beitrag für die Open-Source Community von three.js!");

                POneType.DataSource = db.ProjectTypes;
                POneTeamSize.DataSource = db.ProjectTeamSizes;

                PTwoType.DataSource = Enumerable.Repeat(new ProjectType() { Description="-",Id=-1 }, 1).Concat(db.ProjectTypes);
                PTwoTeamSize.DataSource = Enumerable.Repeat(new ProjectTeamSize() { Description="-",Id=-1 }, 1).Concat(db.ProjectTeamSizes);

                Department.DataSource = db.Departments.Select(i => i.DepartmentName);
                DataBind();

                ViewState["Types"] = projectType;
                AddPictureLabel.Text = "Add image";
                SiteTitle.Text = "Create new project";
                saveProject.Text = "Save";

                if (Request.QueryString["id"] != null)
                {
                    id = int.Parse(Request.QueryString["id"]);
                    getDataToEdit();
                }

                if (Request.QueryString["show"] != null)
                {
                    showOnlyContent();
                }
            }
        }

        private void getDataToEdit()
        {
            var proj = db.Projects.Single(i => i.Id == id);

            Page.Title = "Edit project";
            SiteTitle.Text = "Edit project";
            CreatorID.Text = proj.Creator + "/" + proj.CreateDate.ToString("dd.MM.yyyy");
            saveProject.Visible = true;
            saveProject.Text = "Save changes";
            AddPictureLabel.Text = "Change image:";
            saveProject.Width = 120;

            if (User.IsInRole("Admin") && !proj.Published && Request.QueryString["show"] != null)
            {
                publishProject.Visible = true;
                refuseProject.Visible = true;
            }

            ProjectName.Text = proj.Name;
            Employer.Text = proj.Employer;
            EmployerMail.Text = proj.EmployerEmail;
            NameBetreuer1.Text = proj.Advisor;
            EMail1.Text = proj.AdvisorMail;
            NameBetreuer2.Text = proj.Advisor2;
            EMail2.Text = proj.AdvisorMail2;

            if (proj.TypeDesignUX)
            {
                DesignUX.ImageUrl = "/pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }

            if (proj.TypeHW)
            {
                HW.ImageUrl = "/pictures/projectTypHW.png";
                projectType[1] = true;
            }

            if (proj.TypeCGIP)
            {
                CGIP.ImageUrl = "/pictures/projectTypCGIP.png";
                projectType[2] = true;
            }

            if (proj.TypeMathAlg)
            {
                MathAlg.ImageUrl = "/pictures/projectTypMathAlg.png";
                projectType[3] = true;
            }

            if (proj.TypeAppWeb)
            {
                AppWeb.ImageUrl = "/pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }

            if (proj.TypeDBBigData)
            {
                DBBigData.ImageUrl = "/pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }

            // Priority 1
            POneType.Text = proj.POneType.Description;

            //  Priority 2
            PTwoType.Text = proj.PTwoType.Description;

            // Teamsize Priority 1
            POneTeamSize.Text = proj.POneTeamSize.Description;

            // Teamsize Priority 2
            PTwoTeamSize.Text = proj.PTwoTeamSize.Description;

            InitialPositionContent.Text = proj.InitialPosition;

            Image1.Visible = true;

            if (proj.Picture != null)
            {
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(proj.Picture.ToArray());
                DeleteImageButton.Visible = true;
            }
            else
            {
                ImageLabel.Visible = false;
                Image1.Visible = false;
            }

            ObjectivContent.Text = proj.Objective;
            ProblemStatementContent.Text = proj.ProblemStatement;
            ReferencesContent.Text = proj.References;
            RemarksContent.Text = proj.Remarks;

            if (Request.QueryString["id"] != null && proj.InProgress)
            {
                submitProject.Visible = true;
            }
            else
            {
                submitProject.Visible = false;
            }

            /* CANCELLED PART!
            if (proj.Importance)
            {
                ImportanceContent.Text = "wichtig aus Sicht Institut oder FHNW";
            }
            else
            {
                ImportanceContent.Text = "Normal";
            }
            */
            ReservationNameOne.Text = proj.ReservationNameOne;
            if (proj.ReservationNameTwo != "")
            {
                ReservationNameTwo.Text = proj.ReservationNameTwo;
                ReservationNameTwo.Visible = true;
            }

            Department.Text = proj.Department.DepartmentName;
        }

        private void showOnlyContent()
        {
            var proj = db.Projects.Single(i => i.Id == id);

            Page.Title = "View project";
            SiteTitle.Text = "View project";
            saveProject.Visible = false;
            editProject.Visible = true;

            if (User.IsInRole("Admin") && !proj.Published && !proj.InProgress)
            {
                publishProject.Visible = true;
                refuseProject.Visible = true;
                submitProject.Visible = false;
            }
            else if (User.IsInRole("Admin") && !proj.Published && proj.InProgress)
            {
                publishProject.Visible = false;
                refuseProject.Visible = false;
                submitProject.Visible = true;
            }
            else if (!proj.Published && proj.InProgress)
            {
                submitProject.Visible = true;
            }
            else
            {
                saveProject.Visible = false;
                publishProject.Visible = false;
                refuseProject.Visible = false;
                submitProject.Visible = false;
            }

            if (proj.OverOnePage)
            {
                submitProject.Enabled = false;
            }

            ProjectName.ReadOnly = true;
            Employer.ReadOnly = true;
            EmployerMail.ReadOnly = true;
            NameBetreuer1.ReadOnly = true;
            EMail1.ReadOnly = true;
            NameBetreuer2.ReadOnly = true;
            EMail2.ReadOnly = true;

            DesignUX.Enabled = false;
            HW.Enabled = false;
            CGIP.Enabled = false;
            MathAlg.Enabled = false;
            AppWeb.Enabled = false;
            DBBigData.Enabled = false;

            POneType.Enabled = false;
            POneTeamSize.Enabled = false;
            PTwoType.Enabled = false;
            PTwoTeamSize.Enabled = false;
            InitialPositionContent.ReadOnly = true;

            ObjectivContent.ReadOnly = true;
            AddPictureLabel.Visible = false;
            ImageLabel.Text = "Image:";
            AddPicture.Visible = false;
            DeleteImageButton.Visible = false;

            ObjectivContent.ReadOnly = true;
            ProblemStatementContent.ReadOnly = true;
            ReferencesContent.ReadOnly = true;
            RemarksContent.ReadOnly = true;
            ImportanceContent.Enabled = false;
            ReservationNameOne.Enabled = false;
            ReservationNameTwo.Enabled = false;
            Department.Enabled = false;

            if (proj.Published)
            {
                newProjectDiv.Attributes.Add("class", "publishedProjectBackground well newProjectSettings non-selectable");

                if (User.IsInRole("Admin"))
                {
                    rollbackProject.Visible = true;
                    saveProject.Visible = true;
                }
                else
                {
                    saveProject.Visible = false;
                }

                moveProjectToTheNextSemester.Visible = true;
            }

            if (proj.Refused)
            {
                newProjectDiv.Attributes.Add("class", "refusedProjectBackground well newProjectSettings non-selectable");
            }
        }

        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (DesignUX.ImageUrl == "/pictures/projectTypDesignUXUnchecked.png")
            {
                DesignUX.ImageUrl = "/pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }
            else
            {
                DesignUX.ImageUrl = "/pictures/projectTypDesignUXUnchecked.png";
                projectType[0] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (HW.ImageUrl == "/pictures/projectTypHWUnchecked.png")
            {
                HW.ImageUrl = "/pictures/projectTypHW.png";
                projectType[1] = true;
            }
            else
            {
                HW.ImageUrl = "/pictures/projectTypHWUnchecked.png";
                projectType[1] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (CGIP.ImageUrl == "/pictures/projectTypCGIPUnchecked.png")
            {
                CGIP.ImageUrl = "/pictures/projectTypCGIP.png";
                projectType[2] = true;
            }
            else
            {
                CGIP.ImageUrl = "/pictures/projectTypCGIPUnchecked.png";
                projectType[2] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void MathAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (MathAlg.ImageUrl == "/pictures/projectTypMathAlgUnchecked.png")
            {
                MathAlg.ImageUrl = "/pictures/projectTypMathAlg.png";
                projectType[3] = true;
            }
            else
            {
                MathAlg.ImageUrl = "/pictures/projectTypMathAlgUnchecked.png";
                projectType[3] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (AppWeb.ImageUrl == "/pictures/projectTypAppWebUnchecked.png")
            {
                AppWeb.ImageUrl = "/pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }
            else
            {
                AppWeb.ImageUrl = "/pictures/projectTypAppWebUnchecked.png";
                projectType[4] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (DBBigData.ImageUrl == "/pictures/projectTypDBBigDataUnchecked.png")
            {
                DBBigData.ImageUrl = "/pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }
            else
            {
                DBBigData.ImageUrl = "/pictures/projectTypDBBigDataUnchecked.png";
                projectType[5] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void editProject_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            Response.Redirect("/AddNewProject?id=" + id);
        }

        protected void saveProjectButton(object sender, EventArgs e)
        {
            string fileExt = System.IO.Path.GetExtension(AddPicture.FileName.ToUpper());
            if (fileExt == ".JPEG" || fileExt == ".JPG" || fileExt == ".PNG" || fileExt == "")
            {
                saveCurrentProject();
                Response.Redirect("/projectlist");
            }
            else
            {
                string message = "Wrong File-Extensions! Only .JPEG, .JPG and .PNG allowed.";
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

        private void saveCurrentProject()
        {
            if (Request.QueryString["id"] != null)
            {
                projects = db.Projects.Single(i => i.Id == id);
                projects.ModificationDate = DateTime.Now;
                projects.LastEditedBy = User.Identity.Name;
            }
            else
            {
                projects = new Project();
                projects.Creator = User.Identity.Name;
                projects.InProgress = true;
                projects.Published = false;
                projects.CreateDate = DateTime.Now;
                projects.ModificationDate = DateTime.Now;
                projects.PublishedDate = null;
                projects.LastEditedBy = User.Identity.Name;
                projects.StateDeleted = false;
                projects.Refused = false;
            }

            projects.Name = ProjectName.Text;
            projects.Employer = Employer.Text;
            projects.EmployerEmail = EmployerMail.Text;
            projects.Advisor = NameBetreuer1.Text;
            projects.AdvisorMail = EMail1.Text;

            if (NameBetreuer2.Text != "" && EMail2.Text != "")
            {
                projects.Advisor2 = NameBetreuer2.Text;
                projects.AdvisorMail2 = EMail2.Text;
            }
            else
            {
                projects.Advisor2 = "";
                projects.AdvisorMail2 = "";
            }

            applyProjectType(projects);

            projects.POneTypeId = int.Parse(POneType.SelectedValue);
            projects.POneTeamSizeId = POneTeamSize.SelectedIndex + 1;

            if (PTwoType.SelectedIndex == 0 || PTwoTeamSize.SelectedIndex == 0)
            {
                projects.PTwoTypeId = null;
                projects.PTwoTeamSizeId = null;
            }
            else
            {
                projects.PTwoTypeId = PTwoType.SelectedIndex;
                projects.PTwoTeamSizeId = PTwoTeamSize.SelectedIndex;
            }

            projects.InitialPosition = InitialPositionContent.Text;
            projects.Objective = ObjectivContent.Text;
            projects.ProblemStatement = ProblemStatementContent.Text;
            projects.References = ReferencesContent.Text;
            projects.Remarks = RemarksContent.Text;
            projects.ReservationNameOne = ReservationNameOne.Text;

            if (ReservationNameTwo.Visible)
            {
                projects.ReservationNameTwo = ReservationNameTwo.Text;
            }
            else
            {
                projects.ReservationNameTwo = "";
            }

            /* CANCELLED PART!
            applyImportance(projects);
            */

            projects.DepartmentId = Department.SelectedIndex;

            if (AddPicture.HasFile)
            {
                using (var input = AddPicture.PostedFile.InputStream)
                {
                    var data = new byte[AddPicture.PostedFile.ContentLength];
                    for (var offset = 0; ; )
                    {
                        var read = input.Read(data, offset, data.Length - offset);
                        if (read == 0)
                            break;

                        offset += read;
                    }
                    projects.Picture = new System.Data.Linq.Binary(data);
                }
            }

            if (Request.QueryString["id"] == null)
            {
                db.Projects.InsertOnSubmit(projects);
            }
            db.SubmitChanges();

            int lastinsertedId;

            if (Request.QueryString["id"] == null)
            {
                lastinsertedId = projects.Id;
            }
            else
            {
                lastinsertedId = id;
            }

            PdfCreator pdfCreator = new PdfCreator();
            int amountPages = pdfCreator.getNumberOfPDFPages(lastinsertedId, Request);

            if (amountPages > 1)
            {
                projects.OverOnePage = true;
            }
            else
            {
                projects.OverOnePage = false;
            }

            db.SubmitChanges();
        }

        private void applyProjectType(Project _is)
        {
            for (int i = 0; i < 6; i++)
            {
                if (projectType[i])
                {
                    switch (i)
                    {
                        case 0:
                            _is.TypeDesignUX = true;
                            break;
                        case 1:
                            _is.TypeHW = true;
                            break;
                        case 2:
                            _is.TypeCGIP = true;
                            break;
                        case 3:
                            _is.TypeMathAlg = true;
                            break;
                        case 4:
                            _is.TypeAppWeb = true;
                            break;
                        case 5:
                            _is.TypeDBBigData = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            _is.TypeDesignUX = false;
                            break;
                        case 1:
                            _is.TypeHW = false;
                            break;
                        case 2:
                            _is.TypeCGIP = false;
                            break;
                        case 3:
                            _is.TypeMathAlg = false;
                            break;
                        case 4:
                            _is.TypeAppWeb = false;
                            break;
                        case 5:
                            _is.TypeDBBigData = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /* CANCELLED PART!
        private void applyImportance(Project _is)
        {
            if (ImportanceContent.Text == "Normal")
            {
                _is.Importance = false;
            }
            else
                _is.Importance = true;
        }
        */
        protected void cancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("/projectlist.aspx");
        }

        protected void publishProject_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);
            proj.Published = true;
            proj.PublishedDate = DateTime.Now;
            db.SubmitChanges();

            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(proj.Creator);
                mailMessage.From = new MailAddress(User.Identity.Name);
                mailMessage.Subject = "Projekt '" + proj.Name + "' veröffentlicht";
                mailMessage.Body = "Ihr Projekt '" + proj.Name + "' wurde von " + User.Identity.Name + " veröffentlicht.\r\n\n----------------------\n Automatische Antwort von ProStudCreator\n\n http://prostudcreator.cs.technik.fhnw.ch/";
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
                Response.Write("E-mail sent!");
            }
            catch (Exception ex)
            {
                Response.Write("Could not send the e-mail - error: " + ex.Message);
            }


            Response.Redirect("/projectlist");
        }

        protected void refuseProject_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);

            refusedReason.Visible = true;
            refuseProject.Visible = false;
            publishProject.Visible = false;
            saveProject.Visible = false;
            refusedReasonText.Text = "Ihr Projekt '" + proj.Name + "' wurde von " + User.Identity.Name + " abgelehnt.\r\n\nDies sind die Gründe dafür:\n\n\n\nFreundliche Grüsse\n" + User.Identity.Name;
        }

        protected void refuseDefinitiveNewProject_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);

            proj.Published = false;
            proj.InProgress = true;
            proj.Refused = true;
            db.SubmitChanges();

            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(proj.Creator);
                mailMessage.From = new MailAddress(User.Identity.Name);
                mailMessage.Subject = "Projekt '" + proj.Name + "' abgelehnt";
                mailMessage.Body = refusedReasonText.Text + "\n\n----------------------\n Automatische Antwort von ProStudCreator\n\n http://prostudcreator.cs.technik.fhnw.ch/";
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
                Response.Write("E-mail sent!");
            }
            catch (Exception ex)
            {
                Response.Write("Could not send the e-mail - error: " + ex.Message);
            }

            Response.Redirect("/projectlist");
        }

        protected void submitProject_Click(object sender, EventArgs e)
        {
            if (!(projectType.Count(a => a) < 1) && !(projectType.Count(a => a) > 2))
            {
                saveCurrentProject();
                var id = int.Parse(Request.QueryString["id"]);
                var proj = db.Projects.Single(i => i.Id == id);
                proj.Published = false;
                proj.InProgress = false;
                proj.Refused = false;
                db.SubmitChanges();
                Response.Redirect("/projectlist");
            }
            else
            {
                string message = "You have to choose 1 or 2 projecttypes, before you can submit this project.";
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

        protected void deleteImage_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);
            proj.Picture = null;
            db.SubmitChanges();
            Response.Redirect(Request.RawUrl);
        }

        protected void POneTeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (POneTeamSize.SelectedIndex == 0 && PTwoTeamSize.SelectedIndex == 1 || POneTeamSize.SelectedIndex == 0 && PTwoTeamSize.SelectedIndex == 0)
            {
                ReservationNameTwo.Visible = false;
            }
            else
            {
                ReservationNameTwo.Visible = true;
            }
        }

        protected void PTwoTeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (POneTeamSize.SelectedIndex == 0 && PTwoTeamSize.SelectedIndex == 1 || POneTeamSize.SelectedIndex == 0 && PTwoTeamSize.SelectedIndex == 0)
            {
                ReservationNameTwo.Visible = false;
            }
            else
            {
                ReservationNameTwo.Visible = true;
            }
        }

        protected void rollbackProject_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);
            proj.Published = false;
            proj.InProgress = false;
            proj.Refused = false;
            db.SubmitChanges();
            Response.Redirect("/projectlist");
        }



        protected void cancelRefusion_Click(object sender, EventArgs e)
        {
            saveProject.Visible = true;
            refusedReason.Visible = false;
            refuseProject.Visible = true;
            publishProject.Visible = true;
        }

        protected void moveProjectToTheNextSemester_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);
            proj.Published = false;
            proj.PublishedDate = null;
            proj.InProgress = false;
            proj.Refused = false;
            db.SubmitChanges();
            Response.Redirect("/projectlist");
        }

    }
}