using System;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class AddNewProject : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;
        private ProjectType projectPriority = new ProjectType();
        private bool[] projectType = new bool[8];
        private DateTime today = DateTime.Now;

        #region Timer tick

        protected void Pdfupdatetimer_Tick(object sender, EventArgs e) //function for better workflow with long texts
        {
            if (project != null)
            {
                var pdfc = new PdfCreator();

                Fillproject(project);

                if (pdfc.CalcNumberOfPages(project) > 1)
                {
                    Pdfupdatelabel.Text = "Länge: Das PDF ist länger als eine Seite!";
                    Pdfupdatelabel.ForeColor = Color.Red;
                }
                else
                {
                    Pdfupdatelabel.Text = "Länge: OK (1 Seite)";
                    Pdfupdatelabel.ForeColor = Color.Green;
                }
            }
        }

        #endregion

        #region Application logic

        protected void Page_Load(object sender, EventArgs e)
        {
            AdminView.Visible = ShibUser.CanSeeCreationDetails();

            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single(p => (int?)p.Id == id);
                if (!project.UserCanEdit())
                {
                    Response.Redirect("error/AccessDenied.aspx");
                    Response.End();
                }
            }

            // Project picture
            if (project != null && project.Picture != null)
            {
                Image1.Visible = true;
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
                //imgdescription.Visible = true;
                //imgdescription.Text = project.ImgDescription;
            }
            else
            {
                ImageLabel.Visible = false;
                Image1.Visible = false;
            }

            if (IsPostBack)
            {
                projectType = (bool[])ViewState["Types"];
            }
            else
            {
                // Fix for unsupported maxlength on multi-line in ASP.NET
                // Note: Recursive search of all controls is possible but not worthwhile (yet)
                //InitialPositionContent.Attributes.Add("maxlength", InitialPositionContent.MaxLength.ToString());
                //ObjectivContent.Attributes.Add("maxlength", ObjectivContent.MaxLength.ToString());
                //ProblemStatementContent.Attributes.Add("maxlength", ProblemStatementContent.MaxLength.ToString());
                //ReferencesContent.Attributes.Add("maxlength", ReferencesContent.MaxLength.ToString());
                //RemarksContent.Attributes.Add("maxlength", RemarksContent.MaxLength.ToString());

                InitialPositionContent.Attributes.Add("placeholder",
                    "Beispiel: Die Open-Source WebGL-Library three.js stellt Benutzern einen einfachen Editor zum Erstellen von 3D-Szenen zur Verfügung. Eine Grundversion dieses Editors ist unter http://threejs.org/editor abrufbar. Dieser Editor wird als Basis für die hochschulübergreifende strategische Initiative „Playful Media Practices“ verwendet, wo er zum Design audiovisueller Szenen verwendet wird. Diesem Editor fehlt jedoch eine Undo-/Redo-Funktion, welche in diesem Projekt hinzuzufügen ist.");
                ObjectivContent.Attributes.Add("placeholder",
                    "Beispiel: Das Ziel dieser Arbeit ist die Erarbeitung eines Undo-/Redo-Konzepts für den three.js-Editor sowie dessen Implementation. Da three.js eine Library für die cutting-edge-Technologie WebGL ist, nutzt auch der three.js-Editor modernste Browsermittel wie LocalStorage oder FileAPI. Deshalb gilt es nicht, die Implementation kompatibel zu alten Browsern zu halten, sondern das Maximum aus aktuellen Browsern zu holen.");
                ProblemStatementContent.Attributes.Add("placeholder",
                    "Beispiel: Der three.js-Editor hat mittlerweile eine beachtliche Komplexität erreicht, entsprechend muss für verschiedene Bereiche anders mit Undo&Redo umgegangen werden. Wenn beispielsweise jemand neue Texturen hochlädt, müssen die vorherigen Texturen im Speicher behalten werden.");
                ReferencesContent.Attributes.Add("placeholder",
                    "Beispiel:\n- JavaScript\n- Komplexe Datenstrukturen\n- Three.js/WebGL");
                RemarksContent.Attributes.Add("placeholder",
                    "Beispiel: Ein Pullrequest der Implementation wird diese Erweiterung einem weltweiten Publikum öffentlich zugänglich machen. Sie leisten damit einen entscheidenden Beitrag für die Open-Source Community von three.js!");

                POneType.DataSource = db.ProjectTypes;
                POneTeamSize.DataSource = db.ProjectTeamSizes;
                PTwoType.DataSource = Enumerable.Repeat(new ProjectType
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTypes);
                PTwoTeamSize.DataSource = Enumerable.Repeat(new ProjectTeamSize
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTeamSizes);

                Department.DataSource = db.Departments;
                var dep = ShibUser.GetDepartmentId(db);
                if (dep.HasValue)
                    Department.SelectedValue = dep.Value.ToString();

                DataBind();

                POneTeamSize.SelectedIndex = 1;
                PTwoTeamSize.SelectedIndex = 0;

                ViewState["Types"] = projectType;
                AddPictureLabel.Text = "Bild hinzufügen:";

                if (id.HasValue)
                {
                    Page.Title = "Projekt bearbeiten";
                    SiteTitle.Text = "Projekt bearbeiten";

                    RetrieveProjectToEdit();
                    prepareClientForm(project);
                }
                else
                {
                    Page.Title = "Neues Projekt";
                    SiteTitle.Text = "Neues Projekt anlegen";

                    FillDropPreviousProject(Semester.CurrentSemester(db));
                    dropPreviousProject.SelectedIndex = 0;
                    radioClientType.SelectedValue = "Intern";
                    divClientForm.Visible = false;
                    FillDropAdvisors();
                }
                ToggleReservationTwoVisible();
            }
        }

        private void FillDropAdvisors()
        {
            var user = db.UserDepartmentMap.Single(u => u.Mail == ShibUser.GetEmail());
            dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1);
            dropAdvisor1.DataBind();
            dropAdvisor1.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor2.DataSource = db.UserDepartmentMap;
            dropAdvisor2.DataBind();
            dropAdvisor2.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            if (user.CanBeAdvisor1)
            {
                dropAdvisor1.SelectedValue = user.Id.ToString();
            }
            else
            {
                dropAdvisor1.SelectedIndex = 0;
                dropAdvisor2.SelectedValue = user.Id.ToString();
            } 
        }

        private void FillDropPreviousProject(Semester projectSemester)
        {
            var lastSem = Semester.LastSemester(projectSemester, db);
            var beforeLastSem = Semester.LastSemester(lastSem, db);
            dropPreviousProject.DataSource = db.Projects.Where(p =>
                p.LogProjectType.P5 && !p.LogProjectType.P6
                && p.State == ProjectState.Published
                && (p.SemesterId == lastSem.Id || p.SemesterId == beforeLastSem.Id && p.LogProjectDuration == 2));
            dropPreviousProject.DataBind();
            dropPreviousProject.Items.Insert(0, new ListItem("-", "dropPreviousProjectImpossibleValue"));
        }

        private void RetrieveProjectToEdit()
        {
            CreatorID.Text = project.Creator + "/" + project.CreateDate.ToString("yyyy-MM-dd");
            AddPictureLabel.Text = "Bild ändern:";

            ProjectName.Text = project.Name;
            dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1);
            dropAdvisor1.DataBind();
            dropAdvisor1.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor1.SelectedValue = project.Advisor1Id?.ToString() ?? "ImpossibleValue";
            dropAdvisor2.DataSource = db.UserDepartmentMap;
            dropAdvisor2.DataBind();
            dropAdvisor2.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor2.SelectedValue = project.Advisor2Id?.ToString() ?? "ImpossibleValue";

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
            if (project.TypeSysSec)
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectType[6] = true;
            }
            if (project.TypeSE)
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectType[7] = true;
            }

            POneType.SelectedValue = project.POneType.Id.ToString();
            PTwoType.SelectedValue = project.PTwoType?.Id.ToString();
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();
            PTwoTeamSize.SelectedValue = project.PTwoTeamSize?.Id.ToString();

            if (project.LanguageEnglish && !project.LanguageGerman)
                Language.SelectedIndex = 2;
            else if (!project.LanguageEnglish && project.LanguageGerman)
                Language.SelectedIndex = 1;
            else
                Language.SelectedIndex = 0;

            //LanguageGerman.Checked = project.LanguageGerman;
            //LanguageEnglish.Checked = project.LanguageEnglish;

            DurationOneSemester.Checked = project.DurationOneSemester;

            InitialPositionContent.Text = project.InitialPosition;
            Image1.Visible = true;
            if (project.Picture != null)
            {
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
                imgdescription.Text = project.ImgDescription;
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

            Reservation1Name.Text = project.Reservation1Name;
            Reservation1Mail.Text = project.Reservation1Mail;
            Reservation2Name.Text = project.Reservation2Name;
            Reservation2Mail.Text = project.Reservation2Mail;

            Department.SelectedValue = project.Department.Id.ToString();

            // Button visibility
            saveProject.Visible = true;
            submitProject.Visible = id.HasValue && project.UserCanSubmit();
            publishProject.Visible = project.UserCanPublish();
            refuseProject.Visible = project.UserCanReject();
            rollbackProject.Visible = project.UserCanUnpublish() || project.UserCanUnsubmit();

            POneType.SelectedValue = project?.POneType.Id.ToString();
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();

            Reservation1Mail.Text = project.Reservation1Mail;
            Reservation1Name.Text = project.Reservation1Name;
            Reservation2Mail.Text = project.Reservation2Mail;
            Reservation2Name.Text = project.Reservation2Name;

            DisplayClient(project);


            FillDropPreviousProject(project.Semester);

            if (project.PreviousProjectID == null)
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID.ToString();
                prepareForm(false);
            }
            else
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID?.ToString() ??
                                                    "dropPreviousProjectImpossibleValue";
                prepareForm(true);
            }
        }


        /// <summary>
        ///     Saves changes to the project in the database.
        /// </summary>
        private void SaveProject()
        {
            if (project == null) // New project
            {
                project = new Project();
                project.InitNew();
                db.Projects.InsertOnSubmit(project);
            }
            else
            {
                if (!project.UserCanEdit())
                    throw new UnauthorizedAccessException();
            }
            project.ModificationDate = DateTime.Now;
            project.LastEditedBy = ShibUser.GetEmail();
            Fillproject(project);

            db.SubmitChanges();
            project.OverOnePage = new PdfCreator().CalcNumberOfPages(project) > 1;
            db.SubmitChanges();
        }

        private void ToggleReservationTwoVisible()
        {
            var showResTwo = POneTeamSize.SelectedIndex != 0 ||
                             PTwoTeamSize.SelectedIndex != 0 && PTwoTeamSize.SelectedIndex != 1;
            Reservation2Name.Visible = showResTwo;
            Reservation2Mail.Visible = showResTwo;
        }

        #endregion

        #region Click handlers: Project categories

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

        protected void SysSec_Click(object sender, ImageClickEventArgs e)
        {
            if (SysSec.ImageUrl == "pictures/projectTypSysSecUnchecked.png")
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectType[6] = true;
            }
            else
            {
                SysSec.ImageUrl = "pictures/projectTypSysSecUnchecked.png";
                projectType[6] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void SE_Click(object sender, ImageClickEventArgs e)
        {
            if (SE.ImageUrl == "pictures/projectTypSEUnchecked.png")
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectType[7] = true;
            }
            else
            {
                SE.ImageUrl = "pictures/projectTypSEUnchecked.png";
                projectType[7] = false;
            }
            ViewState["Types"] = projectType;
        }

        #endregion

        #region Click handlers: Buttons (user)

        /// <summary>
        ///     Saves the current state of the form and continue editing.
        /// </summary>
        protected void saveProjectButton(object sender, EventArgs e)
        {
            SaveProject();
            Response.Redirect("AddNewProject?id=" + project.Id);
        }

        /// <summary>
        ///     Save the current state of the form and return to project list.
        /// </summary>
        protected void saveCloseProjectButton(object sender, EventArgs e)
        {
            SaveProject();
            Response.Redirect("projectlist");
        }


        protected void cancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("projectlist");
        }

        protected void submitProject_Click(object sender, EventArgs e)
        {
            SaveProject();

            var validationMessage = generateValidationMessage();

            // Generate JavaScript alert with error message
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

        /// <summary>
        ///     Validates the user's input and generates an error message for invalid input.
        ///     One message is returned at a time, processed top to bottom.
        /// </summary>
        /// <returns>First applicable error message from the validation.</returns>
        private string generateValidationMessage()
        {
            if (project?.ClientPerson != "" && !project.ClientPerson.IsValidName())
                return "Bitte geben Sie den Namen des Kundenkontakts an (Vorname Nachname).";
            if (project.ClientMail != "" && !project.ClientMail.IsValidEmail())
                return "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts an.";

            if ((!project.Advisor1?.Name.IsValidName()) ?? true)
                return "Bitte geben Sie den Namen des Hauptbetreuers an (Vorname Nachname).";
            if (!project.Advisor1?.Mail.IsValidEmail() ?? true)
                return "Bitte geben Sie die E-Mail-Adresse des Hauptbetreuers an.";
            if (project.Advisor2 != null && !String.IsNullOrEmpty(project.Advisor2.Name) && !project.Advisor2.Name.IsValidName())
                return "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";
            if (project.Advisor2 != null && !project.Advisor2.Mail.IsValidEmail())
                return "Bitte geben Sie die E-Mail-Adresse des Zweitbetreuers an.";
            if (project.Advisor2 == null && !String.IsNullOrEmpty(project.Advisor2?.Mail)) // null check oder "" check
                return "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";

            var numAssignedTypes = projectType.Count(a => a);
            if (numAssignedTypes != 1 && numAssignedTypes != 2)
                return "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";

            //if (! (project.LanguageGerman || project.LanguageEnglish))
            //    return "Bitte wählen Sie mindestens eine Sprache aus.";

            var fileExt = Path.GetExtension(AddPicture.FileName.ToUpper());
            if (fileExt != ".JPEG" && fileExt != ".JPG" && fileExt != ".PNG" && fileExt != "")
                return "Es werden nur JPEGs und PNGs als Bildformat unterstützt.";

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

            /*if (project.Picture != null && project.ImgDescription == "")
                return "Bitte beschriften Sie ihr Bild";
                */
            return null;
        }

        #endregion

        #region Click handlers: Buttons (admin only)

        protected void publishProject_Click(object sender, EventArgs e)
        {
            project.ModificationDate = DateTime.Now;
            project.LastEditedBy = ShibUser.GetEmail();
            Fillproject(project);

            db.SubmitChanges();

            project.OverOnePage = new PdfCreator().CalcNumberOfPages(project) > 1;
            project.Publish(db);
            db.SubmitChanges();

#if !DEBUG // Notification e-mail
            var mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if(project.Advisor1?.Mail!=null && project.Advisor1.Mail.IsValidEmail() && project.Advisor1.Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor1.Mail);
            if(project.Advisor2?.Mail!=null && project.Advisor2.Mail.IsValidEmail() && project.Advisor2.Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor2.Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = $"Projekt '{project.Name}' veröffentlicht";
            mailMessage.Body = $"Dein Projekt '{project.Name}' wurde von {ShibUser.GetFirstName()} veröffentlicht.\n"
                + "\n"
                + "----------------------\n"
                + "Automatische Nachricht von ProStudCreator\n"
                + "https://www.cs.technik.fhnw.ch/prostud/";
            
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
#endif

            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        protected void refuseProject_Click(object sender, EventArgs e)
        {
            refusedReason.Visible = true;
            refuseProject.Visible = false;
            publishProject.Visible = false;
            saveProject.Visible = false;
            refusedReasonText.Text = $"Dein Projekt '{project.Name}' wurde von {ShibUser.GetFirstName()} abgelehnt.\n"
                                     + "\n"
                                     + "Dies sind die Gründe dafür:\n"
                                     + "\n"
                                     + "\n"
                                     + "\n"
                                     + "Freundliche Grüsse\n"
                                     + ShibUser.GetFirstName();
        }

        protected void refuseDefinitiveNewProject_Click(object sender, EventArgs e)
        {
            project.Reject();
            db.SubmitChanges();

#if !DEBUG
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if (project.Advisor1?.Mail!=null && project.Advisor1.Mail.IsValidEmail() && project.Advisor1.Mail!=project.Creator)
                mailMessage.To.Add(project.Advisor1.Mail);
            if (project.Advisor2?.Mail!=null && project.Advisor2.Mail.IsValidEmail() && project.Creator!=project.Advisor2.Mail)
                mailMessage.To.Add(project.Advisor2.Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.CC.Add(ShibUser.GetEmail());
            mailMessage.Subject = $"Projekt '{project.Name}' abgelehnt";
            mailMessage.Body =
refusedReasonText.Text + "\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttps://www.cs.technik.fhnw.ch/prostud/";
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
#endif
            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        protected void cancelRefusion_Click(object sender, EventArgs e)
        {
            saveProject.Visible = true;
            refusedReason.Visible = false;
            refuseProject.Visible = true;
            publishProject.Visible = true;
        }

        protected void rollbackProject_Click(object sender, EventArgs e)
        {
            if (project.UserCanUnpublish())
                project.Unpublish();
            else if (project.UserCanUnsubmit())
                project.Unsubmit();
            db.SubmitChanges();
            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        #endregion

        #region Other view event handlers

        protected void deleteImage_Click(object sender, EventArgs e)
        {
            project.Picture = null;
            db.SubmitChanges();
            Response.Redirect("AddNewProject?id=" + project.Id);
        }

        protected void TeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleReservationTwoVisible();
        }

        protected void dropPreviousProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropPreviousProject.SelectedValue == "dropPreviousProjectImpossibleValue")
            {
                DisplayClient(null);
                DisplayPriority(null);
                DisplayReservations(null);
            }
            else
            {
                var previousProject = db.Projects.Single(p => p.Id == int.Parse(dropPreviousProject.SelectedValue));
                DisplayClient(previousProject);
                DisplayReservations(previousProject);
                DisplayPriority(previousProject);
            }
            updateReservation.Update();
            updateClient.Update();
        }

        protected void radioClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(radioClientType.SelectedIndex);


            if (radioClientType.SelectedValue == "Company")
            {
                divClientForm.Visible = true;
                divClientCompany.Visible = true;
            }
            else if (radioClientType.SelectedValue == "Intern")
            {
                divClientForm.Visible = false;
                divClientCompany.Visible = false;
            }
            else
            {
                divClientForm.Visible = true;
                divClientCompany.Visible = false;
            }
        }

        #endregion

        #region private methods

        private void Fillproject(Project project)
        {
            project.Name = ProjectName.Text.FixupParagraph();

            if (dropAdvisor1.SelectedValue == "ImpossibleValue")
                project.Advisor1Id = null;
            else
                project.Advisor1Id = int.Parse(dropAdvisor1.SelectedValue);

            if (dropAdvisor2.SelectedValue == "ImpossibleValue")
                project.Advisor2Id = null;
            else
                project.Advisor2Id = int.Parse(dropAdvisor2.SelectedValue);


            if (radioClientType.SelectedValue != "Intern")
            {
                if (radioClientType.SelectedValue == "Company")
                    project.ClientCompany = txtClientCompany.Text.FixupParagraph();
                project.ClientAddressTitle = drpClientTitle.SelectedItem.Text;
                project.ClientPerson = txtClientName.Text.FixupParagraph();
                project.ClientAddressDepartment = txtClientDepartment.Text.FixupParagraph();
                project.ClientAddressStreet = txtClientStreet.Text.FixupParagraph();
                project.ClientAddressPostcode = txtClientPLZ.Text.FixupParagraph();
                project.ClientAddressCity = txtClientCity.Text.FixupParagraph();
                project.ClientReferenceNumber = txtClientReference.Text.FixupParagraph();
                project.ClientMail = txtClientEmail.Text.Trim().ToLowerInvariant();
            }
            else
            {
                project.ClientAddressTitle = "Herr";

                project.ClientCompany =
                    project.ClientPerson =
                        project.ClientAddressDepartment =
                            project.ClientAddressStreet =
                                project.ClientAddressPostcode =
                                    project.ClientAddressCity =
                                        project.ClientReferenceNumber =
                                            project.ClientMail = "";
            }

            // Project categories
            project.TypeDesignUX = projectType[0];
            project.TypeHW = projectType[1];
            project.TypeCGIP = projectType[2];
            project.TypeMathAlg = projectType[3];
            project.TypeAppWeb = projectType[4];
            project.TypeDBBigData = projectType[5];
            project.TypeSysSec = projectType[6];
            project.TypeSE = projectType[7];

            // Languages
            if (Language.SelectedIndex == 0)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = true;
            }
            else if (Language.SelectedIndex == 1)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = false;
            }
            else if (Language.SelectedIndex == 2)
            {
                project.LanguageGerman = false;
                project.LanguageEnglish = true;
            }
            else
            {
                throw new ArgumentException("Es muss eine Sprache ausgewählt werden.", "original");
            }

            //project.LanguageGerman = LanguageGerman.Checked;
            //project.LanguageEnglish = LanguageEnglish.Checked;

            // Duration
            project.DurationOneSemester = DurationOneSemester.Checked;

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
                project.P2TypeId = int.Parse(PTwoType.SelectedValue);
                project.P2TeamSizeId = int.Parse(PTwoTeamSize.SelectedValue);
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

            if (Reservation2Name.Visible
            ) // TODO Check team size instead of visibility (just because it makes more sense)
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


            var oldDepartmentId = project.DepartmentId;
            project.DepartmentId = int.Parse(Department.SelectedValue);

            // If project changed departments & already has a ProjectNr, generate a new one
            if (project.DepartmentId != oldDepartmentId && project.ProjectNr > 0)
            {
                project.ProjectNr = 0; // 'Remove' project number to allow finding a new one.
                project.GenerateProjectNr();
            }

            if (project.Semester == null)
                project.Semester = Semester.NextSemester(db);

            //picture description
            project.ImgDescription = imgdescription.Text.FixupParagraph();

            if (AddPicture.HasFile)
                using (var input = AddPicture.PostedFile.InputStream)
                {
                    var data = new byte[AddPicture.PostedFile.ContentLength];
                    var offset = 0;
                    for (; ; )
                    {
                        var read = input.Read(data, offset, data.Length - offset);
                        if (read == 0)
                            break;

                        offset += read;
                    }
                    project.Picture = new Binary(data);
                }


            //Previous Project

            if (dropPreviousProject.SelectedValue == "dropPreviousProjectImpossibleValue")
            {
                project.PreviousProjectID = null;
            }
            else
            {
                var previousProjectId = int.Parse(dropPreviousProject.SelectedValue);
                project.Project1 = db.Projects.Single(p => p.Id == previousProjectId);
            }
        }

        private void DisplayReservations(Project previousProject)
        {
            if (previousProject == null)
            {
                Reservation1Name.Enabled = Reservation1Mail.Enabled =
                    Reservation2Mail.Enabled = Reservation2Name.Enabled = true;
                Reservation1Mail.Text = Reservation1Name.Text = Reservation2Mail.Text = Reservation2Name.Text = "";
            }
            else
            {
                Reservation1Name.Enabled = Reservation1Mail.Enabled =
                    Reservation2Mail.Enabled = Reservation2Name.Enabled = false;
                Reservation1Mail.Text = previousProject.LogStudent1Mail;
                Reservation2Mail.Text = previousProject.LogStudent2Mail;
                Reservation1Name.Text = previousProject.LogStudent1Name;
                Reservation2Name.Text = previousProject.LogStudent2Name;
            }
        }

        private void DisplayClient(Project project)
        {
            if (project == null)
            {
                drpClientTitle.SelectedValue = "1"; //Default

                txtClientCompany.Text =
                    txtClientName.Text =
                        txtClientDepartment.Text =
                            txtClientStreet.Text =
                                txtClientPLZ.Text =
                                    txtClientCity.Text =
                                        txtClientReference.Text =
                                            txtClientEmail.Text = "";

                divClientForm.Visible = false;
                radioClientType.SelectedValue = "Intern";
            }
            else
            {
                txtClientCompany.Text = project?.ClientCompany;
                drpClientTitle.SelectedValue = project?.ClientAddressTitle == "Herr" ? "1" : "2";
                txtClientName.Text = project?.ClientPerson;
                txtClientDepartment.Text = project?.ClientAddressDepartment;
                txtClientStreet.Text = project?.ClientAddressStreet;
                txtClientPLZ.Text = project?.ClientAddressPostcode;
                txtClientCity.Text = project?.ClientAddressCity;
                txtClientReference.Text = project?.ClientReferenceNumber;
                txtClientEmail.Text = project?.ClientMail;

                prepareClientForm(project);
            }
        }

        private void DisplayPriority(Project project)
        {
            if (project == null)
            {
                POneTeamSize.Enabled = POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = true;
                POneType.SelectedValue = db.ProjectTypes.Single(p => p.P5 && p.P6).Id.ToString();
                POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => p.Size1 && p.Size2).Id.ToString();
                divPriorityTwo.Visible = true;
            }
            else
            {
                divPriorityTwo.Visible = false;
                POneTeamSize.Enabled = POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = false;
                POneType.SelectedValue = db.ProjectTypes.Single(p => !p.P5 && p.P6).Id.ToString();
                if (string.IsNullOrEmpty(project.LogStudent2Mail))
                    POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => p.Size1 && !p.Size2).Id.ToString();
                else
                    POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => !p.Size1 && p.Size2).Id.ToString();
            }
        }


        private void prepareForm(bool hasPreviousProj)
        {
            //Priority
            divPriorityTwo.Visible = POneTeamSize.Enabled =
                POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = !hasPreviousProj;
            //Reservations
            Reservation1Name.Enabled = Reservation1Mail.Enabled =
                Reservation2Mail.Enabled = Reservation2Name.Enabled = !hasPreviousProj;
        }

        private void prepareClientForm(Project project)
        {
            var hasClient = false;

            string[] clientInformationStrings =
            {
                project.ClientAddressPostcode, project.ClientAddressCity, project.ClientAddressDepartment,
                project.ClientAddressStreet, project.ClientAddressTitle, project.ClientCompany, project.ClientMail,
                project.ClientPerson, project.ClientReferenceNumber
            };

            foreach (var clientInformationstirng in clientInformationStrings)
                if (!string.IsNullOrEmpty(clientInformationstirng))
                    hasClient = true;

            if (hasClient)
            {
                if (!string.IsNullOrEmpty(project?.ClientCompany))
                {
                    divClientCompany.Visible = divClientForm.Visible = true;
                    radioClientType.SelectedValue = "Company";
                }
                else
                {
                    divClientCompany.Visible = false;
                    divClientForm.Visible = true;
                    radioClientType.SelectedValue = "PrivatePerson";
                }
            }
            else
            {
                divClientCompany.Visible = divClientForm.Visible = false;
                radioClientType.SelectedValue = "Intern";
            }
        }
    }
}

#endregion