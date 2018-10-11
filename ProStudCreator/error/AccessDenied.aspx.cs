using System;
using System.Web.UI;

namespace ProStudCreator.error
{
    public partial class AccessDenied : Page
    {
        protected string errorMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            errorMsg += "Login:\t" + (ShibUser.GetEmail() ?? "(Nicht verfübar)") + "\n";
            errorMsg += "Abteilung: " + (ShibUser.GetDepartment(new ProStudentCreatorDBDataContext())?.DepartmentName ?? "(Unbekannt)") + "\n";
        }
    }
}