using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class FAQ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new ProStudentCreatorDBDataContext();
            var departmentId = ShibUser.GetDepartmentId(db);
            var department = db.Departments.Single(i => i.Id == departmentId);

            if (department.IMVS)
            {
                pDefenseOrganisation.InnerText = "Du bist für die Organisation des Experten und der Verteidigung verantwortlich.";
            }
            else if (department.i4DS)
            {
            }
            else
            {
                throw new UnauthorizedAccessException("Sie sind nicht mit einem der beiden Informatikinstitute angemeldet!");
            }
        }
    }
}