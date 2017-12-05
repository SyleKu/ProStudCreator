using System;
using System.Linq;
using System.Web.UI;

namespace ProStudCreator
{
    public partial class FAQ : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new ProStudentCreatorDBDataContext();
            var departmentId = ShibUser.GetDepartmentId(db);
            var department = db.Departments.Single(i => i.Id == departmentId);

            if (department.IMVS)
            {
                imvs1.Visible = true;
                imvs2.Visible = true;
            }
            else if (department.i4DS)
            {
                i4ds1.Visible = true;
                i4ds2.Visible = true;
            }else if (department.IIT)
            {
                iit1.Visible = true;
                iit2.Visible = true;
            }
            else
            {
                throw new UnauthorizedAccessException(
                    "Sie sind nicht mit einem der drei Informatikinstitute angemeldet!");
            }
        }
    }
}