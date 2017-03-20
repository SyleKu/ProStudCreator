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
            errorMsg += "Login:\t" + (ShibUser.GetEmail() ?? "(Nicht verfübar)") + "\n";
            errorMsg += "Abteilung: " + (ShibUser.GetDepartmentName(new ProStudentCreatorDBDataContext()) ?? "(Nicht verfübar)") + "\n";
        }
    }
}