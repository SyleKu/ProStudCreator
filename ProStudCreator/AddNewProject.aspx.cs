using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace ProStudCreator
{
    public partial class AddNewProject : System.Web.UI.Page
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        bool[] projectType = new bool[6];
        private static int id;
        private Project projects;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.Name == "test@fhnw.ch")
            {
                AdminView.Visible = true;
            }
            if (IsPostBack)
            {
                projectType = (bool[])ViewState["Types"];
            }
            else
            {
                ViewState["Types"] = projectType;
                AddPictureLabel.Text = "Bild hinzufügen";
                SiteTitle.Text = "Neues Projekt erstellen:";
                saveNewProject.Text = "Speichern";

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

            /*
            if (proj.Creator != User.Identity.Name)
            {
                Response.Redirect("/projectlist");
            }
            */
            SiteTitle.Text = "Projekt bearbeiten:";
            CreatorID.Text = "Ersteller: " + proj.Creator;
            saveNewProject.Text = "Änderungen speichern";
            AddPictureLabel.Text = "Bild ändern";
            saveNewProject.Width = 175;

            if (User.Identity.Name == "test@fhnw.ch" && !proj.Published && Request.QueryString["show"] != null)
            {
                publishProject.Visible = true;
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


            if (proj.POneP5 && proj.POneP6)
            {
                POneContent.Text = "P5 oder P6";

            }
            else if (proj.POneP5)
            {
                POneContent.Text = "P5 (180h pro Student)";
            }
            else
            {
                POneContent.Text = "P6 (360h pro Student)";
            }


            if (proj.PTwoP5 && proj.PTwoP6)
            {
                PTwoContent.Text = "P5 oder P6";
            }
            else if (proj.PTwoP5)
            {
                PTwoContent.Text = "P5 (180h pro Student)";
            }
            else if (proj.PTwoP6)
            {
                PTwoContent.Text = "P6 (360h pro Student)";
            }
            else
            {
                PTwoContent.Text = "------";
            }

            if (proj.POneTeamSize == "Einzelarbeit")
            {
                POneTeamSize.Text = "Einzelarbeit";
            }
            else if (proj.POneTeamSize == "2er Team")
            {
                POneTeamSize.Text = "2er Team";
            }
            else
            {
                POneTeamSize.Text = "1er oder 2er Team";
            }

            if (proj.PTwoTeamSize == "Einzelarbeit")
            {
                PTwoTeamSize.Text = "Einzelarbeit";
            }
            else if (proj.PTwoTeamSize == "2er Team")
            {
                PTwoTeamSize.Text = "2er Team";
            }
            else
            {
                PTwoTeamSize.Text = "1er oder 2er Team";
            }

            InitialPositionContent.Text = proj.InitialPosition;

            Image1.Visible = true;

            if (proj.Picture != null)
            {
                Image1.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(proj.Picture.ToArray());
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

            if (proj.Importance)
            {
                ImportanceContent.Text = "wichtig aus Sicht Institut oder FHNW";
            }
            else
            {
                ImportanceContent.Text = "Normal";
            }

            Department.Text = proj.Department;

            if (proj.InProgress)
            {
                InProgressCheckBox.Checked = true;
            }
        }

        private void showOnlyContent()
        {
            var proj = db.Projects.Single(i => i.Id == id);

            SiteTitle.Text = "Projekt Ansicht:";
            saveNewProject.Text = "Bearbeiten";
            saveNewProject.Width = 100;
            if (User.Identity.Name == "test@fhnw.ch" && !proj.Published)
            {
                publishProject.Visible = true;
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

            POneContent.Enabled = false;
            POneTeamSize.Enabled = false;
            PTwoContent.Enabled = false;
            PTwoTeamSize.Enabled = false;
            InitialPositionContent.ReadOnly = true;

            ObjectivContent.ReadOnly = true;
            AddPictureLabel.Visible = false;
            ImageLabel.Text = "Bild:";
            AddPicture.Visible = false;

            ObjectivContent.ReadOnly = true;
            ProblemStatementContent.ReadOnly = true;
            ReferencesContent.ReadOnly = true;
            RemarksContent.ReadOnly = true;
            ImportanceContent.Enabled = false;
            Department.Enabled = false;
            InProgressCheckBox.Enabled = false;
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

        protected void saveProject(object sender, EventArgs e)
        {
            if (Request.QueryString["show"] != null)
            {
                var id = Request.QueryString["id"];
                Response.Redirect("/AddNewProject?id=" + id);
            }

            //else if (ProjectNameAvailable() && projectType.Any() && NameBetreuer1.Text != "" && EmailAvailable() || Request.QueryString["id"] != null)
            else if (ProjectNameAvailable() && projectType.Any() && NameBetreuer1.Text != "" || Request.QueryString["id"] != null)
            {
                if (Request.QueryString["id"] != null)
                {
                    projects = db.Projects.Single(i => i.Id == id);
                }
                else
                {
                    projects = new Project();
                }

                projects.Name = ProjectName.Text;
                projects.Employer = Employer.Text;
                projects.EmployerEmail = EmployerMail.Text;
                projects.Creator = User.Identity.Name;
                projects.Advisor = NameBetreuer1.Text;
                projects.AdvisorMail = EMail1.Text;


                // TO DO
                if (NameBetreuer1.Text != "" && EMail1.Text != "")
                {

                }

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

                if (POneContent.Text == "P5 (180h pro Student)")
                {
                    projects.POneP5 = true;
                    projects.POneP6 = false;
                }
                else if (POneContent.Text == "P6 (360h pro Student)")
                {

                    projects.POneP5 = false;
                    projects.POneP6 = true;
                }
                else
                {
                    projects.POneP5 = true;
                    projects.POneP6 = true;
                }


                if (PTwoContent.Text == "P5 (180h pro Student)")
                {
                    projects.PTwoP5 = true;
                    projects.PTwoP6 = false;
                }
                else if (PTwoContent.Text == "P6 (360h pro Student)")
                {

                    projects.PTwoP5 = false;
                    projects.PTwoP6 = true;
                }
                else if (PTwoContent.Text == "P5 oder P6")
                {

                    projects.PTwoP5 = true;
                    projects.PTwoP6 = true;
                }
                else
                {
                    projects.PTwoP5 = false;
                    projects.PTwoP6 = false;
                }

                projects.POneTeamSize = POneTeamSize.Text;
                projects.PTwoTeamSize = PTwoTeamSize.Text;

                projects.InitialPosition = InitialPositionContent.Text;
                projects.Objective = ObjectivContent.Text;
                projects.ProblemStatement = ProblemStatementContent.Text;
                projects.References = ReferencesContent.Text;
                projects.Remarks = RemarksContent.Text;
                projects.Published = false;
                applyImportance(projects);
                projects.Department = Department.Text;
                projects.InProgress = InProgressCheckBox.Checked;
                projects.CreateDate = DateTime.Today;

                if (AddPicture.FileName != "")
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
                Response.Redirect("/Projectlist");
            }
            // TO DO
            else if (ProjectNameAvailable() && !projectType.Any())
            {

            }
            else if (!ProjectNameAvailable())
            {

            }

            else if (!projectType.Any())
            {

            }

        }

        private bool ProjectNameAvailable()
        {
            return !db.Projects.Any(item => item.Name == ProjectName.Text);
        }

        private bool EmailAvailable()
        {
            return db.AspNetUsers.Any(item => item.Email == EMail1.Text);
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

        private void applyImportance(Project _is)
        {
            if (ImportanceContent.Text == "Normal")
            {
                _is.Importance = false;
            }
            else
                _is.Importance = true;
        }

        protected void cancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("/projectlist.aspx");
        }

        protected void publishProject_Click(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var proj = db.Projects.Single(i => i.Id == id);
            proj.Published = true;
            db.SubmitChanges();
            Response.Redirect("/Projectlist");
        }
    }
}