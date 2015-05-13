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
        private System.DateTime today = System.DateTime.Now;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (ShibUser.IsAdmin())
            {
                this.AdminView.Visible = true;
            }
            if (base.Request.QueryString["id"] != null)
            {
                this.id = new int?(int.Parse(base.Request.QueryString["id"]));
                this.project = this.db.Projects.Single((Project p) => (int?)p.Id == this.id);
            }
            if (base.IsPostBack)
            {
                this.projectType = (bool[])this.ViewState["Types"];
            }
            else
            {
                this.InitialPositionContent.Attributes.Add("placeholder", "Beispiel: Die Open-Source WebGL-Library three.js stellt Benutzern einen einfachen Editor zum Erstellen von 3D-Szenen zur Verfügung. Eine Grundversion dieses Editors ist unter http://threejs.org/editor abrufbar. Dieser Editor wird als Basis für die hochschulübergreifende strategische Initiative „Playful Media Practices“ verwendet, wo er zum Design audiovisueller Szenen verwendet wird. Diesem Editor fehlt jedoch eine Undo-/Redo-Funktion, welche in diesem Projekt hinzuzufügen ist.");
                this.ObjectivContent.Attributes.Add("placeholder", "Beispiel: Das Ziel dieser Arbeit ist die Erarbeitung eines Undo-/Redo-Konzepts für den three.js-Editor sowie dessen Implementation. Da three.js eine Library für die cutting-edge-Technologie WebGL ist, nutzt auch der three.js-Editor modernste Browsermittel wie LocalStorage oder FileAPI. Deshalb gilt es nicht, die Implementation kompatibel zu alten Browsern zu halten, sondern das Maximum aus aktuellen Browsern zu holen.");
                this.ProblemStatementContent.Attributes.Add("placeholder", "Beispiel: Der three.js-Editor hat mittlerweile eine beachtliche Komplexität erreicht, entsprechend muss für verschiedene Bereiche anders mit Undo&Redo umgegangen werden. Wenn beispielsweise jemand neue Texturen hochlädt, müssen die vorherigen Texturen im Speicher behalten werden.");
                this.ReferencesContent.Attributes.Add("placeholder", "Beispiel:\n- JavaScript\n- Komplexe Datenstrukturen\n- Three.js/WebGL");
                this.RemarksContent.Attributes.Add("placeholder", "Beispiel: Ein Pullrequest der Implementation wird diese Erweiterung einem weltweiten Publikum öffentlich zugänglich machen. Sie leisten damit einen entscheidenden Beitrag für die Open-Source Community von three.js!");
                this.POneType.DataSource = this.db.ProjectTypes;
                this.POneTeamSize.DataSource = this.db.ProjectTeamSizes;
                this.PTwoType.DataSource = Enumerable.Repeat<ProjectType>(new ProjectType
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(this.db.ProjectTypes);
                this.PTwoTeamSize.DataSource = Enumerable.Repeat<ProjectTeamSize>(new ProjectTeamSize
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(this.db.ProjectTeamSizes);
                this.Department.DataSource = this.db.Departments;
                int? dep = ShibUser.GetDepartmentId();
                if (dep.HasValue)
                {
                    this.Department.SelectedValue = dep.Value.ToString();
                }
                this.NameBetreuer2.Text = ShibUser.GetFullName();
                this.EMail2.Text = ShibUser.GetEmail();
                this.DataBind();
                this.ViewState["Types"] = this.projectType;
                this.AddPictureLabel.Text = "Bild hinzufügen:";
                this.SiteTitle.Text = "Neues Projekt anlegen";
                this.saveProject.Text = "Speichern";
                if (this.id.HasValue)
                {
                    this.RetrieveProjectToEdit();
                }
                if (base.Request.QueryString["show"] != null)
                {
                    this.RetrieveProjectToView();
                }
            }
        }
        private void RetrieveProjectToEdit()
        {
            this.Page.Title = "Projekt bearbeiten";
            this.SiteTitle.Text = "Projekt bearbeiten";
            this.CreatorID.Text = this.project.Creator + "/" + this.project.CreateDate.ToString("yyyy-MM-dd");
            this.saveProject.Visible = true;
            this.saveProject.Text = "Speichern";
            this.AddPictureLabel.Text = "Bild ändern:";
            this.saveProject.Width = 120;
            if (ShibUser.IsAdmin() && this.project.State == ProjectState.Submitted)
            {
                this.publishProject.Visible = true;
                this.refuseProject.Visible = true;
            }
            this.ProjectName.Text = this.project.Name;
            this.Employer.Text = this.project.ClientName;
            this.EmployerMail.Text = this.project.ClientMail;
            this.NameBetreuer1.Text = this.project.Advisor1Name;
            this.EMail1.Text = this.project.Advisor1Mail;
            this.NameBetreuer2.Text = this.project.Advisor2Name;
            this.EMail2.Text = this.project.Advisor2Mail;
            if (this.project.TypeDesignUX)
            {
                this.DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                this.projectType[0] = true;
            }
            if (this.project.TypeHW)
            {
                this.HW.ImageUrl = "pictures/projectTypHW.png";
                this.projectType[1] = true;
            }
            if (this.project.TypeCGIP)
            {
                this.CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                this.projectType[2] = true;
            }
            if (this.project.TypeMathAlg)
            {
                this.MathAlg.ImageUrl = "pictures/projectTypMathAlg.png";
                this.projectType[3] = true;
            }
            if (this.project.TypeAppWeb)
            {
                this.AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                this.projectType[4] = true;
            }
            if (this.project.TypeDBBigData)
            {
                this.DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                this.projectType[5] = true;
            }
            this.POneType.Text = this.project.POneType.Description;
            this.PTwoType.Text = ((this.project.PTwoType == null) ? null : this.project.PTwoType.Description);
            this.POneTeamSize.Text = this.project.POneTeamSize.Description;
            this.PTwoTeamSize.Text = ((this.project.PTwoTeamSize == null) ? null : this.project.PTwoTeamSize.Description);
            this.InitialPositionContent.Text = this.project.InitialPosition;
            this.Image1.Visible = true;
            if (this.project.Picture != null)
            {
                this.Image1.ImageUrl = "data:image/png;base64," + System.Convert.ToBase64String(this.project.Picture.ToArray());
                this.DeleteImageButton.Visible = true;
            }
            else
            {
                this.ImageLabel.Visible = false;
                this.Image1.Visible = false;
            }
            this.ObjectivContent.Text = this.project.Objective;
            this.ProblemStatementContent.Text = this.project.ProblemStatement;
            this.ReferencesContent.Text = this.project.References;
            this.RemarksContent.Text = this.project.Remarks;
            this.submitProject.Visible = (this.id.HasValue && this.project.State == ProjectState.InProgress);
            this.ReservationNameOne.Text = this.project.Reservation1Name;
            if (this.project.Reservation2Name != "")
            {
                this.ReservationNameTwo.Text = this.project.Reservation2Name;
                this.ReservationNameTwo.Visible = true;
            }
            this.Department.Text = this.project.Department.DepartmentName;
            if (this.project.State == ProjectState.Published && ShibUser.IsAdmin())
            {
                if (this.project.PublishedDate == Semester.CurrentSemester - 1)
                {
                    this.moveProjectToTheNextSemester.Visible = true;
                }
                this.rollbackProject.Visible = true;
            }
        }
        private void RetrieveProjectToView()
        {
            this.Page.Title = "Projekt betrachten";
            this.SiteTitle.Text = "Projekt betrachten";
            this.saveProject.Visible = false;
            this.editProject.Visible = true;
            if (ShibUser.IsAdmin() && this.project.State == ProjectState.Submitted)
            {
                this.publishProject.Visible = true;
                this.refuseProject.Visible = true;
                this.submitProject.Visible = false;
            }
            else if (ShibUser.IsAdmin() && this.project.State == ProjectState.InProgress)
            {
                this.publishProject.Visible = false;
                this.refuseProject.Visible = false;
                this.submitProject.Visible = true;
            }
            else if (this.project.State == ProjectState.InProgress)
            {
                this.submitProject.Visible = true;
            }
            else
            {
                this.saveProject.Visible = false;
                this.publishProject.Visible = false;
                this.refuseProject.Visible = false;
                this.submitProject.Visible = false;
            }
            this.ProjectName.ReadOnly = true;
            this.Employer.ReadOnly = true;
            this.EmployerMail.ReadOnly = true;
            this.NameBetreuer1.ReadOnly = true;
            this.EMail1.ReadOnly = true;
            this.NameBetreuer2.ReadOnly = true;
            this.EMail2.ReadOnly = true;
            this.DesignUX.Enabled = false;
            this.HW.Enabled = false;
            this.CGIP.Enabled = false;
            this.MathAlg.Enabled = false;
            this.AppWeb.Enabled = false;
            this.DBBigData.Enabled = false;
            this.POneType.Enabled = false;
            this.POneTeamSize.Enabled = false;
            this.PTwoType.Enabled = false;
            this.PTwoTeamSize.Enabled = false;
            this.InitialPositionContent.ReadOnly = true;
            this.ObjectivContent.ReadOnly = true;
            this.AddPictureLabel.Visible = false;
            this.ImageLabel.Text = "Image:";
            this.AddPicture.Visible = false;
            this.DeleteImageButton.Visible = false;
            this.ObjectivContent.ReadOnly = true;
            this.ProblemStatementContent.ReadOnly = true;
            this.ReferencesContent.ReadOnly = true;
            this.RemarksContent.ReadOnly = true;
            this.ReservationNameOne.Enabled = false;
            this.ReservationNameTwo.Enabled = false;
            this.Department.Enabled = false;
            if (this.project.State == ProjectState.Published)
            {
                this.newProjectDiv.Attributes.Add("class", "publishedProjectBackground well newProjectSettings non-selectable");
                this.moveProjectToTheNextSemester.Visible = (this.rollbackProject.Visible = (this.saveProject.Visible = ShibUser.IsAdmin()));
            }
            else if (this.project.State == ProjectState.Rejected)
            {
                this.newProjectDiv.Attributes.Add("class", "refusedProjectBackground well newProjectSettings non-selectable");
            }
        }
        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (this.DesignUX.ImageUrl == "pictures/projectTypDesignUXUnchecked.png")
            {
                this.DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                this.projectType[0] = true;
            }
            else
            {
                this.DesignUX.ImageUrl = "pictures/projectTypDesignUXUnchecked.png";
                this.projectType[0] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (this.HW.ImageUrl == "pictures/projectTypHWUnchecked.png")
            {
                this.HW.ImageUrl = "pictures/projectTypHW.png";
                this.projectType[1] = true;
            }
            else
            {
                this.HW.ImageUrl = "pictures/projectTypHWUnchecked.png";
                this.projectType[1] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (this.CGIP.ImageUrl == "pictures/projectTypCGIPUnchecked.png")
            {
                this.CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                this.projectType[2] = true;
            }
            else
            {
                this.CGIP.ImageUrl = "pictures/projectTypCGIPUnchecked.png";
                this.projectType[2] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void MathAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (this.MathAlg.ImageUrl == "pictures/projectTypMathAlgUnchecked.png")
            {
                this.MathAlg.ImageUrl = "pictures/projectTypMathAlg.png";
                this.projectType[3] = true;
            }
            else
            {
                this.MathAlg.ImageUrl = "pictures/projectTypMathAlgUnchecked.png";
                this.projectType[3] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (this.AppWeb.ImageUrl == "pictures/projectTypAppWebUnchecked.png")
            {
                this.AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                this.projectType[4] = true;
            }
            else
            {
                this.AppWeb.ImageUrl = "pictures/projectTypAppWebUnchecked.png";
                this.projectType[4] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (this.DBBigData.ImageUrl == "pictures/projectTypDBBigDataUnchecked.png")
            {
                this.DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                this.projectType[5] = true;
            }
            else
            {
                this.DBBigData.ImageUrl = "pictures/projectTypDBBigDataUnchecked.png";
                this.projectType[5] = false;
            }
            this.ViewState["Types"] = this.projectType;
        }
        protected void editProject_Click(object sender, System.EventArgs e)
        {
            base.Response.Redirect("AddNewProject?id=" + this.id);
        }
        protected void saveProjectButton(object sender, System.EventArgs e)
        {
            string fileExt = System.IO.Path.GetExtension(this.AddPicture.FileName.ToUpper());
            if (fileExt == ".JPEG" || fileExt == ".JPG" || fileExt == ".PNG" || fileExt == "")
            {
                this.SaveProject();
                base.Response.Redirect("projectlist");
            }
            else
            {
                string message = "Es werden nur JPEGs und PNGs als Bildformat unterstützt.";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                base.ClientScript.RegisterClientScriptBlock(base.GetType(), "alert", sb.ToString());
            }
        }
        private void SaveProject()
        {
            if (this.project == null)
            {
                this.project = new Project();
                this.project.Creator = ShibUser.GetEmail();
                this.project.State = ProjectState.InProgress;
                this.project.CreateDate = (this.project.PublishedDate = System.DateTime.Now);
                this.db.Projects.InsertOnSubmit(this.project);
            }
            this.project.ModificationDate = System.DateTime.Now;
            this.project.LastEditedBy = ShibUser.GetEmail();
            this.project.Name = this.ProjectName.Text.Trim();
            this.project.ClientName = this.Employer.Text.Trim();
            this.project.ClientMail = this.EmployerMail.Text.Trim().ToLowerInvariant();
            this.project.Advisor1Name = this.NameBetreuer1.Text.Trim();
            this.project.Advisor1Mail = this.EMail1.Text.Trim().ToLowerInvariant();
            if (this.NameBetreuer2.Text != "" && this.EMail2.Text != "")
            {
                this.project.Advisor2Name = this.NameBetreuer2.Text.Trim();
                this.project.Advisor2Mail = this.EMail2.Text.Trim().ToLowerInvariant();
            }
            else
            {
                this.project.Advisor2Name = "";
                this.project.Advisor2Mail = "";
            }
            if (this.project.Advisor1Name == "" && this.project.Advisor1Mail == "")
            {
                this.project.Advisor1Name = this.project.Advisor2Name;
                this.project.Advisor1Mail = this.project.Advisor2Mail;
                this.project.Advisor2Name = "";
                this.project.Advisor2Mail = "";
            }
            this.project.TypeDesignUX = this.projectType[0];
            this.project.TypeHW = this.projectType[1];
            this.project.TypeCGIP = this.projectType[2];
            this.project.TypeMathAlg = this.projectType[3];
            this.project.TypeAppWeb = this.projectType[4];
            this.project.TypeDBBigData = this.projectType[5];
            this.project.P1TypeId = int.Parse(this.POneType.SelectedValue);
            this.project.P1TeamSizeId = int.Parse(this.POneTeamSize.SelectedValue);
            if (this.PTwoType.SelectedIndex == 0 || this.PTwoTeamSize.SelectedIndex == 0)
            {
                this.project.P2TypeId = null;
                this.project.P2TeamSizeId = null;
            }
            else
            {
                this.project.P2TypeId = new int?(int.Parse(this.PTwoType.SelectedValue));
                this.project.P2TeamSizeId = new int?(int.Parse(this.PTwoTeamSize.SelectedValue));
            }
            if (this.project.P1TeamSizeId == this.project.P2TeamSizeId && this.project.P1TypeId == this.project.P2TypeId)
            {
                this.project.P2TeamSizeId = null;
                this.project.P2TypeId = null;
            }
            this.project.InitialPosition = this.InitialPositionContent.Text.Trim();
            this.project.Objective = this.ObjectivContent.Text.Trim();
            this.project.ProblemStatement = this.ProblemStatementContent.Text.Trim();
            this.project.References = this.ReferencesContent.Text.Trim();
            this.project.Remarks = this.RemarksContent.Text.Trim();
            this.project.Reservation1Name = this.ReservationNameOne.Text.Trim();
            if (this.ReservationNameTwo.Visible)
            {
                this.project.Reservation2Name = this.ReservationNameTwo.Text.Trim();
            }
            else
            {
                this.project.Reservation2Name = "";
            }
            if (this.project.Reservation1Name == "" && this.project.Reservation2Name != "")
            {
                this.project.Reservation1Name = this.project.Reservation2Name;
                this.project.Reservation2Name = "";
            }
            this.project.DepartmentId = int.Parse(this.Department.SelectedValue);
            if (this.AddPicture.HasFile)
            {
                using (System.IO.Stream input = this.AddPicture.PostedFile.InputStream)
                {
                    byte[] data = new byte[this.AddPicture.PostedFile.ContentLength];
                    int offset = 0;
                    while (true)
                    {
                        int read = input.Read(data, offset, data.Length - offset);
                        if (read == 0)
                        {
                            break;
                        }
                        offset += read;
                    }
                    this.project.Picture = new Binary(data);
                }
            }
            this.db.SubmitChanges();
            this.project.OverOnePage = (new PdfCreator().CalcNumberOfPages(this.project.Id, base.Request) > 1);
            this.db.SubmitChanges();
        }
        protected void cancelNewProject_Click(object sender, System.EventArgs e)
        {
            base.Response.Redirect("projectlist");
        }
        protected void publishProject_Click(object sender, System.EventArgs e)
        {
            this.project.State = ProjectState.Published;
            this.project.PublishedDate = System.DateTime.Now;
            this.db.SubmitChanges();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(this.project.Creator);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = "Projekt '" + this.project.Name + "' veröffentlicht";
            mailMessage.Body = string.Concat(new string[]
            {
                "Ihr Projekt '",
                this.project.Name,
                "' wurde von ",
                ShibUser.GetFullName(),
                " veröffentlicht.\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttp://prostudcreator.cs.technik.fhnw.ch/"
            });
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
            base.Response.Redirect("projectlist");
        }
        protected void refuseProject_Click(object sender, System.EventArgs e)
        {
            this.refusedReason.Visible = true;
            this.refuseProject.Visible = false;
            this.publishProject.Visible = false;
            this.saveProject.Visible = false;
            this.refusedReasonText.Text = string.Concat(new string[]
            {
                "Ihr Projekt '",
                this.project.Name,
                "' wurde von ",
                ShibUser.GetFullName(),
                " abgelehnt.\n\nDies sind die Gründe dafür:\n\n\n\nFreundliche Grüsse\n",
                ShibUser.GetFullName()
            });
        }
        protected void refuseDefinitiveNewProject_Click(object sender, System.EventArgs e)
        {
            this.project.Reject();
            this.db.SubmitChanges();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(this.project.Creator);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = "Projekt '" + this.project.Name + "' abgelehnt";
            mailMessage.Body = this.refusedReasonText.Text + "\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttp://prostudcreator.cs.technik.fhnw.ch/";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
            base.Response.Redirect("projectlist");
        }
        protected void submitProject_Click(object sender, System.EventArgs e)
        {
            this.SaveProject();
            string message = null;
            if (message == null && !this.project.Advisor1Name.IsValidName())
            {
                message = "Bitte geben Sie den Namen des Hauptbetreuers an (Vorname Nachname).";
            }
            if (message == null && !this.project.Advisor1Mail.IsValidEmail())
            {
                message = "Bitte geben Sie die E-Mail-Adresse des Hauptbetreuers an.";
            }
            if (message == null && this.project.Advisor2Name != "" && !this.project.Advisor2Name.IsValidName())
            {
                message = "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";
            }
            if (message == null && this.project.Advisor2Name != "" && !this.project.Advisor2Mail.IsValidEmail())
            {
                message = "Bitte geben Sie die E-Mail-Adresse des Zweitbetreuers an.";
            }
            if (message == null && this.project.Advisor2Name == "" && this.project.Advisor2Mail != "")
            {
                message = "Bitte geben Sie den Namen des Zweitbetreuers an (Vorname Nachname).";
            }
            bool arg_164_0;
            if (message == null)
            {
                if (this.projectType.Count((bool a) => a) >= 1)
                {
                    arg_164_0 = (this.projectType.Count((bool a) => a) <= 2);
                }
                else
                {
                    arg_164_0 = false;
                }
            }
            else
            {
                arg_164_0 = true;
            }
            if (!arg_164_0)
            {
                message = "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";
            }
            if (message == null && this.project.OverOnePage)
            {
                message = "Der Projektbeschrieb passt nicht auf eine A4-Seite. Bitte kürzen Sie die Beschreibung.";
            }
            if (message != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                base.ClientScript.RegisterClientScriptBlock(base.GetType(), "alert", sb.ToString());
            }
            else
            {
                this.project.Submit();
                this.db.SubmitChanges();
                base.Response.Redirect("projectlist");
            }
        }
        protected void deleteImage_Click(object sender, System.EventArgs e)
        {
            this.project.Picture = null;
            this.db.SubmitChanges();
            base.Response.Redirect(base.Request.RawUrl);
        }
        protected void POneTeamSize_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((this.POneTeamSize.SelectedIndex == 0 && this.PTwoTeamSize.SelectedIndex == 1) || (this.POneTeamSize.SelectedIndex == 0 && this.PTwoTeamSize.SelectedIndex == 0))
            {
                this.ReservationNameTwo.Visible = false;
            }
            else
            {
                this.ReservationNameTwo.Visible = true;
            }
        }
        protected void PTwoTeamSize_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((this.POneTeamSize.SelectedIndex == 0 && this.PTwoTeamSize.SelectedIndex == 1) || (this.POneTeamSize.SelectedIndex == 0 && this.PTwoTeamSize.SelectedIndex == 0))
            {
                this.ReservationNameTwo.Visible = false;
            }
            else
            {
                this.ReservationNameTwo.Visible = true;
            }
        }
        protected void rollbackProject_Click(object sender, System.EventArgs e)
        {
            this.project.Unpublish();
            this.db.SubmitChanges();
            base.Response.Redirect("projectlist");
        }
        protected void cancelRefusion_Click(object sender, System.EventArgs e)
        {
            this.saveProject.Visible = true;
            this.refusedReason.Visible = false;
            this.refuseProject.Visible = true;
            this.publishProject.Visible = true;
        }
        protected void moveProjectToTheNextSemester_Click(object sender, System.EventArgs e)
        {
            this.project.State = ProjectState.InProgress;
            this.project.PublishedDate = (this.project.ModificationDate = System.DateTime.Now);
            this.db.SubmitChanges();
            base.Response.Redirect("projectlist");
        }
    }
}
