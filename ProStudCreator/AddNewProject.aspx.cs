using System;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
namespace ProStudCreator
{
    public partial class AddNewProject : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private bool[] projectType = new bool[6];
        private int? id;
        private Project project;
        private ProjectType projectPriority = new ProjectType();
        private DateTime today = DateTime.Now;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShibUser.IsAdmin())
            {
                AdminView.Visible = true;
            }

            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = new int?(int.Parse(Request.QueryString["id"]));
                project = db.Projects.Single((Project p) => (int?)p.Id == id);

                if(!ShibUser.IsAdmin() && project.Creator != ShibUser.GetEmail() && project.ClientMail != ShibUser.GetEmail() && project.Advisor1Mail != ShibUser.GetEmail() && project.Advisor2Mail != ShibUser.GetEmail())
                    throw new UnauthorizedAccessException();
            }

            // Project picture
            if (project != null && project.Picture != null)
            {
                Image1.Visible = true;
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
            }
            else
            {
                ImageLabel.Visible = false;
                Image1.Visible = false;
            }            

            if (base.IsPostBack)
            {
                projectType = (bool[])ViewState["Types"];
            }
            else
            {
                InitialPositionContent.Attributes.Add("placeholder", "Beispiel: Die Open-Source WebGL-Library three.js stellt Benutzern einen einfachen Editor zum Erstellen von 3D-Szenen zur Verfügung. Eine Grundversion dieses Editors ist unter http://threejs.org/editor abrufbar. Dieser Editor wird als Basis für die hochschulübergreifende strategische Initiative „Playful Media Practices“ verwendet, wo er zum Design audiovisueller Szenen verwendet wird. Diesem Editor fehlt jedoch eine Undo-/Redo-Funktion, welche in diesem Projekt hinzuzufügen ist.");
                ObjectivContent.Attributes.Add("placeholder", "Beispiel: Das Ziel dieser Arbeit ist die Erarbeitung eines Undo-/Redo-Konzepts für den three.js-Editor sowie dessen Implementation. Da three.js eine Library für die cutting-edge-Technologie WebGL ist, nutzt auch der three.js-Editor modernste Browsermittel wie LocalStorage oder FileAPI. Deshalb gilt es nicht, die Implementation kompatibel zu alten Browsern zu halten, sondern das Maximum aus aktuellen Browsern zu holen.");
                ProblemStatementContent.Attributes.Add("placeholder", "Beispiel: Der three.js-Editor hat mittlerweile eine beachtliche Komplexität erreicht, entsprechend muss für verschiedene Bereiche anders mit Undo&Redo umgegangen werden. Wenn beispielsweise jemand neue Texturen hochlädt, müssen die vorherigen Texturen im Speicher behalten werden.");
                ReferencesContent.Attributes.Add("placeholder", "Beispiel:\n- JavaScript\n- Komplexe Datenstrukturen\n- Three.js/WebGL");
                RemarksContent.Attributes.Add("placeholder", "Beispiel: Ein Pullrequest der Implementation wird diese Erweiterung einem weltweiten Publikum öffentlich zugänglich machen. Sie leisten damit einen entscheidenden Beitrag für die Open-Source Community von three.js!");
                POneType.DataSource = db.ProjectTypes;
                POneTeamSize.DataSource = db.ProjectTeamSizes;
                PTwoType.DataSource = Enumerable.Repeat<ProjectType>(new ProjectType
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTypes);
                PTwoTeamSize.DataSource = Enumerable.Repeat<ProjectTeamSize>(new ProjectTeamSize
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTeamSizes);
                Department.DataSource = db.Departments;
                int? dep = ShibUser.GetDepartmentId();
                if (dep.HasValue)
                {
                    Department.SelectedValue = dep.Value.ToString();
                }

                NameBetreuer2.Text = ShibUser.GetFullName();
                EMail2.Text = ShibUser.GetEmail();
                DataBind();

                POneTeamSize.SelectedIndex = 1;
                PTwoTeamSize.SelectedIndex = 0;
                toggleReservationTwoVisible();

                ViewState["Types"] = projectType;
                AddPictureLabel.Text = "Bild hinzufügen:";
                
                if (id.HasValue)
                {
                    Page.Title = "Projekt bearbeiten";
                    SiteTitle.Text = "Projekt bearbeiten";

                    RetrieveProjectToEdit();
                }
                else
                {
                    Page.Title = "Neues Projekt";
                    SiteTitle.Text = "Neues Projekt anlegen";
                }
            }
        }
        private void RetrieveProjectToEdit()
        {
            CreatorID.Text = project.Creator + "/" + project.CreateDate.ToString("yyyy-MM-dd");
            saveProject.Visible = true;
            saveProject.Text = "Speichern";
            AddPictureLabel.Text = "Bild ändern:";
            saveProject.Width = 120;
            if (ShibUser.IsAdmin() && project.State == ProjectState.Submitted)
            {
                publishProject.Visible = true;
                refuseProject.Visible = true;
            }

            ProjectName.Text = project.Name;
            Employer.Text = project.ClientCompany;
            EmployerPerson.Text = project.ClientPerson;
            EmployerMail.Text = project.ClientMail;
            NameBetreuer1.Text = project.Advisor1Name;
            EMail1.Text = project.Advisor1Mail;
            NameBetreuer2.Text = project.Advisor2Name;
            EMail2.Text = project.Advisor2Mail;

            if (project.TypeDesignUX)
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }
            if (project.TypeHW)
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectType[1] = true;
            }
            if (project.TypeCGIP)
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectType[2] = true;
            }
            if (project.TypeMathAlg)
            {
                MathAlg.ImageUrl = "pictures/projectTypMathAlg.png";
                projectType[3] = true;
            }
            if (project.TypeAppWeb)
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }
            if (project.TypeDBBigData)
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }

            POneType.SelectedValue = project.POneType.Id.ToString();
            PTwoType.SelectedValue = ((project.PTwoType == null) ? null : project.PTwoType.Id.ToString());
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();
            PTwoTeamSize.SelectedValue = ((project.PTwoTeamSize == null) ? null : project.PTwoTeamSize.Id.ToString());

            InitialPositionContent.Text = project.InitialPosition;
            Image1.Visible = true;
            if (project.Picture != null)
            {
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
            }
            else
            {
                ImageLabel.Visible = false;
                Image1.Visible = false;
            }

            ObjectivContent.Text = project.Objective;
            ProblemStatementContent.Text = project.ProblemStatement;
            ReferencesContent.Text = project.References;
            RemarksContent.Text = project.Remarks;
            submitProject.Visible = (id.HasValue && project.State == ProjectState.InProgress);

            Reservation1Name.Text = project.Reservation1Name;
            Reservation1Mail.Text = project.Reservation1Mail;
            Reservation2Name.Text = project.Reservation2Name;
            Reservation2Mail.Text = project.Reservation2Mail;
            
            Department.SelectedValue = project.Department.Id.ToString();
            if (project.State == ProjectState.Published && ShibUser.IsAdmin())
            {
                if (project.PublishedDate == Semester.CurrentSemester - 1)
                    moveProjectToTheNextSemester.Visible = true;

                rollbackProject.Visible = true;
            }

            if(project.State == ProjectState.Submitted)
            {
                rollbackProject.Visible = true;
            }
        }
        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (DesignUX.ImageUrl == "pictures/projectTypDesignUXUnchecked.png")
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }
            else
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUXUnchecked.png";
                projectType[0] = false;
            }
            ViewState["Types"] = projectType;
        }
        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (HW.ImageUrl == "pictures/projectTypHWUnchecked.png")
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectType[1] = true;
            }
            else
            {
                HW.ImageUrl = "pictures/projectTypHWUnchecked.png";
                projectType[1] = false;
            }
            ViewState["Types"] = projectType;
        }
        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (CGIP.ImageUrl == "pictures/projectTypCGIPUnchecked.png")
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectType[2] = true;
            }
            else
            {
                CGIP.ImageUrl = "pictures/projectTypCGIPUnchecked.png";
                projectType[2] = false;
            }
            ViewState["Types"] = projectType;
        }
        protected void MathAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (MathAlg.ImageUrl == "pictures/projectTypMathAlgUnchecked.png")
            {
                MathAlg.ImageUrl = "pictures/projectTypMathAlg.png";
                projectType[3] = true;
            }
            else
            {
                MathAlg.ImageUrl = "pictures/projectTypMathAlgUnchecked.png";
                projectType[3] = false;
            }
            ViewState["Types"] = projectType;
        }
        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (AppWeb.ImageUrl == "pictures/projectTypAppWebUnchecked.png")
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }
            else
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWebUnchecked.png";
                projectType[4] = false;
            }
            ViewState["Types"] = projectType;
        }
        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (DBBigData.ImageUrl == "pictures/projectTypDBBigDataUnchecked.png")
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }
            else
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigDataUnchecked.png";
                projectType[5] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void saveProjectButton(object sender, EventArgs e)
        {
            SaveProject();
            Response.Redirect("AddNewProject?id=" + project.Id);
        }

        protected void saveCloseProjectButton(object sender, EventArgs e)
        {
            SaveProject();
            Response.Redirect("projectlist");
        }

        private void SaveProject()
        {
            if (project == null)    // New project
            {
                project = new Project();
                project.Creator = ShibUser.GetEmail();
                project.State = ProjectState.InProgress;
                project.CreateDate = (project.PublishedDate = DateTime.Now);
                db.Projects.InsertOnSubmit(project);
            }
            else
            {
                // Allow edit for authorised users only
                if (!ShibUser.IsAdmin() && project.Creator != ShibUser.GetEmail() && project.ClientMail != ShibUser.GetEmail() && project.Advisor1Mail != ShibUser.GetEmail() && project.Advisor2Mail != ShibUser.GetEmail())
                    throw new UnauthorizedAccessException();
            }
            project.ModificationDate = DateTime.Now;
            project.LastEditedBy = ShibUser.GetEmail();

            project.Name = ProjectName.Text.FixupParagraph();
            project.ClientCompany = Employer.Text.FixupParagraph();
            project.ClientPerson = EmployerPerson.Text.FixupParagraph();
            project.ClientMail = EmployerMail.Text.Trim().ToLowerInvariant();
            project.Advisor1Name = NameBetreuer1.Text.FixupParagraph();
            project.Advisor1Mail = EMail1.Text.Trim().ToLowerInvariant();
            project.Advisor2Name = NameBetreuer2.Text.FixupParagraph();
            project.Advisor2Mail = EMail2.Text.Trim().ToLowerInvariant();

            // Project types
            project.TypeDesignUX = projectType[0];
            project.TypeHW = projectType[1];
            project.TypeCGIP = projectType[2];
            project.TypeMathAlg = projectType[3];
            project.TypeAppWeb = projectType[4];
            project.TypeDBBigData = projectType[5];

            // Team size
            project.P1TypeId = int.Parse(POneType.SelectedValue);
            project.P1TeamSizeId = int.Parse(POneTeamSize.SelectedValue);
            if (PTwoType.SelectedIndex == 0 || PTwoTeamSize.SelectedIndex == 0)
            {
                project.P2TypeId = null;
                project.P2TeamSizeId = null;
            }
            else
            {
                project.P2TypeId = new int?(int.Parse(PTwoType.SelectedValue));
                project.P2TeamSizeId = new int?(int.Parse(PTwoTeamSize.SelectedValue));
            }
            if (project.P1TeamSizeId == project.P2TeamSizeId && project.P1TypeId == project.P2TypeId)
            {
                project.P2TeamSizeId = null;
                project.P2TypeId = null;
            }

            // Long texts (description etc.)
            project.InitialPosition = InitialPositionContent.Text.FixupParagraph();
            project.Objective = ObjectivContent.Text.FixupParagraph();
            project.ProblemStatement = ProblemStatementContent.Text.FixupParagraph();
            project.References = ReferencesContent.Text.FixupParagraph();
            project.Remarks = RemarksContent.Text.FixupParagraph();

            // Student reservations
            project.Reservation1Name = Reservation1Name.Text.FixupParagraph();
            project.Reservation1Mail = Reservation1Mail.Text.Trim().ToLowerInvariant();

            if (Reservation2Name.Visible)   // TODO Check team size instead of visibility (just because it makes more sense)
            {
                project.Reservation2Name = Reservation2Name.Text.FixupParagraph();
                project.Reservation2Mail = Reservation2Mail.Text.Trim().ToLowerInvariant();
            }
            else
            {
                project.Reservation2Name = "";
                project.Reservation2Mail = "";
            }

            // Move reservation 2 to 1 if 1 isn't specified
            if (project.Reservation1Name == "" && project.Reservation2Name != "")
            {
                project.Reservation1Name = project.Reservation2Name;
                project.Reservation2Name = "";
            }

            
            int oldDepartmentId = project.DepartmentId;
            project.DepartmentId = int.Parse(Department.SelectedValue);
            
            // If project changed departments & already has a ProjectNr, generate a new one
            if (project.DepartmentId != oldDepartmentId && project.ProjectNr > 0)
            {
                project.ProjectNr = 0;  // 'Remove' project number to allow finding a new one.
                project.GenerateProjectNr();
            }
            

            if (AddPicture.HasFile)
            {
                using (var input = AddPicture.PostedFile.InputStream)
                {
                    byte[] data = new byte[AddPicture.PostedFile.ContentLength];
                    int offset = 0;
                    for(; ;)
                    {
                        int read = input.Read(data, offset, data.Length - offset);
                        if (read == 0)
                            break;

                        offset += read;
                    }
                    project.Picture = new Binary(data);
                }
            }

            db.SubmitChanges();
            project.OverOnePage = (new PdfCreator().CalcNumberOfPages(project.Id, Request) > 1);
            db.SubmitChanges();
        }
        protected void cancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("projectlist");
        }
        protected void publishProject_Click(object sender, EventArgs e)
        {
            project.State = ProjectState.Published;
            project.PublishedDate = DateTime.Now;
            db.SubmitChanges();

            // Notification e-mail
            var mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if(project.Advisor1Mail!=null && project.Advisor1Mail.IsValidEmail() && project.Advisor1Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor1Mail);
            if(project.Advisor2Mail!=null && project.Advisor2Mail.IsValidEmail() && project.Advisor2Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor2Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = "Projekt '" + project.Name + "' veröffentlicht";
            mailMessage.Body = string.Concat(new string[]
            {
                "Ihr Projekt '",
                project.Name,
                "' wurde von ",
                ShibUser.GetFullName(),
                " veröffentlicht.\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttps://www.cs.technik.fhnw.ch/prostud/"
            });
            #if !DEBUG
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
            #endif

            Response.Redirect("projectlist");
        }
        protected void refuseProject_Click(object sender, EventArgs e)
        {
            refusedReason.Visible = true;
            refuseProject.Visible = false;
            publishProject.Visible = false;
            saveProject.Visible = false;
            refusedReasonText.Text = string.Concat(new string[]
            {
                "Ihr Projekt '",
                project.Name,
                "' wurde von ",
                ShibUser.GetFullName(),
                " abgelehnt.\n\nDies sind die Gründe dafür:\n\n\n\nFreundliche Grüsse\n",
                ShibUser.GetFullName()
            });
        }
        protected void refuseDefinitiveNewProject_Click(object sender, EventArgs e)
        {
            project.Reject();
            db.SubmitChanges();

            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if (project.Advisor1Mail!=null && project.Advisor1Mail.IsValidEmail() && project.Advisor1Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor1Mail);
            if (project.Advisor2Mail!=null && project.Advisor2Mail.IsValidEmail() && project.Creator!=project.Advisor2Mail)
                mailMessage.To.Add(project.Advisor2Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = "Projekt '" + project.Name + "' abgelehnt";
            mailMessage.Body = refusedReasonText.Text + "\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttps://www.cs.technik.fhnw.ch/prostud/";
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            Response.Redirect("projectlist");
        }
        protected void submitProject_Click(object sender, EventArgs e)
        {
            SaveProject();

            // Perform input validation & generate error messages
            string message = null;
            if (message == null && !project.Advisor1Name.IsValidName())
                message = "Bitte geben Sie den Namen des Hauptbetreuers an (Vorname Nachname).";
            if (message == null && !project.Advisor1Mail.IsValidEmail())
                message = "Bitte geben Sie die E-Mail-Adresse des Hauptbetreuers an.";
            if (message == null && project.Advisor2Name != "" && !project.Advisor2Name.IsValidName())
                message = "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";
            if (message == null && project.Advisor2Name != "" && !project.Advisor2Mail.IsValidEmail())
                message = "Bitte geben Sie die E-Mail-Adresse des Zweitbetreuers an.";
            if (message == null && project.Advisor2Name == "" && project.Advisor2Mail != "")
                message = "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";
            if (message == null && project.ClientPerson != "" && !project.ClientPerson.IsValidName())
                message = "Bitte geben Sie den Namen des Kundenkontakts an (Vorname Nachname).";
            if (message == null && project.ClientMail != "" && !project.ClientMail.IsValidEmail())
                message = "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts an.";

            var fileExt = Path.GetExtension(AddPicture.FileName.ToUpper());
            if (fileExt != ".JPEG" && fileExt != ".JPG" && fileExt != ".PNG" && fileExt != "")
                message = "Es werden nur JPEGs und PNGs als Bildformat unterstützt.";

            bool validTopicAssignment;
            if (message == null)
            {
                if (projectType.Count((bool a) => a) >= 1)
                    validTopicAssignment = (projectType.Count((bool a) => a) <= 2);
                else
                    validTopicAssignment = false;
            }
            else
                validTopicAssignment = true;

            if (!validTopicAssignment)
                message = "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";

            if (message == null && project.OverOnePage)
                message = "Der Projektbeschrieb passt nicht auf eine A4-Seite. Bitte kürzen Sie die Beschreibung.";

            if (message != null)
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
            else
            {
                project.Submit();
                db.SubmitChanges();
                Response.Redirect("projectlist");
            }
        }

        protected void deleteImage_Click(object sender, EventArgs e)
        {
            project.Picture = null;
            db.SubmitChanges();
            Response.Redirect("AddNewProject?id="+project.Id);
        }

        protected void TeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            toggleReservationTwoVisible();
        }

        private void toggleReservationTwoVisible()
        {
            bool showResTwo = (POneTeamSize.SelectedIndex != 0 || (PTwoTeamSize.SelectedIndex != 0 && PTwoTeamSize.SelectedIndex != 1));
            Reservation2Name.Visible = showResTwo;
            Reservation2Mail.Visible = showResTwo;
        }

        protected void rollbackProject_Click(object sender, EventArgs e)
        {
            if (project.State == ProjectState.Published && ShibUser.IsAdmin())
            {
                project.Unpublish();
                db.SubmitChanges();
            }
            else
            {
                project.Unsubmit();
                db.SubmitChanges();
            }
            Response.Redirect("projectlist");
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
            project.State = ProjectState.InProgress;
            project.PublishedDate = (project.ModificationDate = DateTime.Now);
            db.SubmitChanges();
            Response.Redirect("projectlist");
        }
    }
}
