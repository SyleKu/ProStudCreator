using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace ProStudCreator
{
    public partial class addNewProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (DesignUX.ImageUrl=="/pictures/projectTypDesignUXUnchecked.png")
            {
                DesignUX.ImageUrl = "/pictures/projectTypDesignUX.png";
            }
            else
            {
                DesignUX.ImageUrl = "/pictures/projectTypDesignUXUnchecked.png";
            }
        }

        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (HW.ImageUrl == "/pictures/projectTypHWUnchecked.png")
            {
                HW.ImageUrl = "/pictures/projectTypHW.png";
            }
            else
            {
                HW.ImageUrl = "/pictures/projectTypHWUnchecked.png";
            }
        }

        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (CGIP.ImageUrl == "/pictures/projectTypCGIPUnchecked.png")
            {
                CGIP.ImageUrl = "/pictures/projectTypCGIP.png";
            }
            else
            {
                CGIP.ImageUrl = "/pictures/projectTypCGIPUnchecked.png";
            }
        }

        protected void MathAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (MathAlg.ImageUrl == "/pictures/projectTypMathAlgUnchecked.png")
            {
                MathAlg.ImageUrl = "/pictures/projectTypMathAlg.png";
            }
            else
            {
                MathAlg.ImageUrl = "/pictures/projectTypMathAlgUnchecked.png";
            }
        }

        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (AppWeb.ImageUrl == "/pictures/projectTypAppWebUnchecked.png")
            {
                AppWeb.ImageUrl = "/pictures/projectTypAppWeb.png";
            }
            else
            {
                AppWeb.ImageUrl = "/pictures/projectTypAppWebUnchecked.png";
            }
        }

        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (DBBigData.ImageUrl == "/pictures/projectTypDBBigDataUnchecked.png")
            {
                DBBigData.ImageUrl = "/pictures/projectTypDBBigData.png";
            }
            else
            {
                DBBigData.ImageUrl = "/pictures/projectTypDBBigDataUnchecked.png";
            }
        }
        
        protected void AddPicture_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton img = new ImageButton();
            img.ImageUrl = "/pictures/addPicture.png";
            img.ID = "AddPicture";
            img.Height = 100;
            img.OnClientClick = "AddPicture_Click";
            PlaceHolder1.Controls.Add(img);
        }

        protected void saveNewProject_Click(object sender, EventArgs e)
        {
            
            ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
            InputStore i = db.InputStores.Where(item => item.Id == 1 && item.Importance == .ToArray();
        }

    }
}