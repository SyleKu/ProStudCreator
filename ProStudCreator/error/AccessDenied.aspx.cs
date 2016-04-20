using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator.error
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        protected string errorMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            string deptName = deptName = ShibUser.GetDepartmentName();

            errorMsg += "Login:\t" + ShibUser.GetEmail() + "\n";

            if (deptName==null)
                errorMsg += "Abteilung: Konnte nicht abgerufen werden.\n";
            else
                errorMsg += "Abteilung: " + ShibUser.GetDepartmentName() + "\n";
            
        }
    }
}