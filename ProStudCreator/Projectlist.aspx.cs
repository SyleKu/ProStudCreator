using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    
    class Peter
    {
        public int id { get; set; }
        public string advisorName {get;set;}
        public string advisorEmail { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public string p5 { get; set; }
        public string p6 { get; set; }
    }
    

    public partial class projectlist : System.Web.UI.Page
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {

            /*
             * SELECT * FROM Projects p INNER JOIN AspNetUsers u ON u.EMail==p.AdvisorMail WHERE u.EmailConfirmed = 1;
             * db.Projects.Join(db.AspNetUsers, p => p.AdvisorMail, u => u.Email, (p, u) => Tuple.Create(p, u)).Where(both => both.Item2.EmailConfirmed);
             * projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png"
             * i.POne + " " + i.PTwo,
             */

            CheckProjects.DataSource = db.Projects.Where(item => item.Published == false).Select(i => new Peter()
            {
                id = i.Id,
                advisorName = i.Advisor + " " + i.Advisor2,
                advisorEmail = i.AdvisorMail + " " + i.AdvisorMail2,
                projectName = i.Name,
                /*
                p5 = (i.POne == "P5 (180h pro Student)" ? true : (i.POne == "P6 (360h pro Student)" ? true : false)),
                p6 = (i.POne == "P5 (180h pro Student)" ? true : (i.POne == "P6 (360h pro Student)" ? true : false)),
                 */
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png",
                projectType2 = "pictures/projectTyp" + ((i.TypeHW && i.TypeDesignUX) ? "HW" : ((i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" : (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" : (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" : "DBBigData")) + ".png"
            });

            /* var projects = db.Projects.Select(i => new Peter() */ 
            // p = i,
            AllProjects.DataSource = db.Projects.Select(i => new Peter()
            {
                id = i.Id,
                advisorName = i.Advisor + " " + i.Advisor2,
                advisorEmail = i.AdvisorMail + " " + i.AdvisorMail2,
                projectName = i.Name,
                p5 = i.POne,
                p6 = i.PTwo,
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png",
                projectType2 = "pictures/projectTyp" + ((i.TypeHW && i.TypeDesignUX) ? "HW" : ((i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" : (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" : (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" : "DBBigData")) + ".png"
            }); 
            //.ToList();

            /*
            projects.ForEach(updateProject);
            
            AllProjects.DataSource = projects;
            */
            
            
            CheckProjects.DataBind();
            AllProjects.DataBind();
        }
        /*
        private void updateProject(Peter p)
        {
            if(p.p.)
            {
                p.Image1 = sadpaosdipaosdi

            }
            else
            {
                p.Image2 = asdalsduaosdiu
            }
        }
        */

        protected void newProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("/AddNewProject");
        }

        protected void AllProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "showProject")
            {
                var rowId = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = AllProjects.Rows[rowId];
                var id = Convert.ToInt32(Server.HtmlDecode(row.Cells[0].Text));
                Response.Redirect("/AddNewProject?id=" + id + "&show=true");
            }

            if (e.CommandName == "editProject")
            {
                var rowId = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = AllProjects.Rows[rowId];
                var id = Convert.ToInt32(Server.HtmlDecode(row.Cells[0].Text));
                Response.Redirect("/AddNewProject?id=" + id);
            }

            if (e.CommandName == "deleteProject")
            {
                var rowId = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = AllProjects.Rows[rowId];
                var id = Convert.ToInt32(Server.HtmlDecode(row.Cells[0].Text));
                var details = db.Projects.Where(item => item.Id == id);
                db.Projects.DeleteAllOnSubmit(details);
                db.SubmitChanges();
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void CheckProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            var rowId = Convert.ToInt32(e.CommandArgument);
            var proj = (Peter)CheckProjects.Rows[rowId].DataItem;

            if (e.CommandName == "showProject")
            {
                Response.Redirect("/AddNewProject?id=" + proj.id + "&show=true");
            }

            if (e.CommandName == "editProject")
            {
                Response.Redirect("/AddNewProject?id=" + proj.id);
            }

            if (e.CommandName == "deleteProject")
            {
                var details = db.Projects.Where(item => item.Id == proj.id);
                db.Projects.DeleteAllOnSubmit(details);
                db.SubmitChanges();

                CheckProjects.DataBind();
            }
        }
    }
}