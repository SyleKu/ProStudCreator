using System.Configuration;
using System.Linq;
using System.Web;
namespace ProStudCreator
{
    public static class ShibUser
    {
        public static bool IsAuthenticated()
        {
            return ShibUser.IsStaff() && ShibUser.GetDepartmentId().HasValue;
        }
        public static bool IsAdmin()
        {
#if DEBUG
            return false;
#else
            if (HttpContext.Current.Items["IsAdmin"] == null)
            {
                HttpContext.Current.Items["IsAdmin"] = ConfigurationManager.AppSettings["admins"].Split(new char[]
                {
                        ';'
                }).Contains(ShibUser.GetEmail());
            }
            return (bool)HttpContext.Current.Items["IsAdmin"];
#endif
        }
        public static bool IsStaff()
        {
#if DEBUG
            return true;
#else
            string aff = HttpContext.Current.Request.Headers["affiliation"];
            return aff != null && aff.Split(new char[]
            {
                    ';'
            }).Contains("staff");
#endif
        }
        public static string GetEmail()
        {
#if DEBUG
            return "stephen.randles@fhnw.ch";
#else
            string mail = HttpContext.Current.Request.Headers["mail"];
            string result;
            if (mail == null)
            {
                result = null;
            }
            else
            {
                result = mail.Trim().ToLowerInvariant();
            }
            return result;
#endif
        }
        public static string GetFirstName()
        {
#if DEBUG
            return "Stephen";
#else
            return HttpContext.Current.Request.Headers["givenName"];
#endif
        }
        public static string GetLastName()
        {
#if DEBUG
            return "Randles";
#else
            return HttpContext.Current.Request.Headers["surname"];
#endif
        }
        public static string GetPhoneNumber()
        {
            return HttpContext.Current.Request.Headers["telephoneNumber"];
        }
        public static int? GetDepartmentId()
        {
#if DEBUG
            return 0; // Department 0 = i4Ds

#else
            string orgUnitDn = HttpContext.Current.Request.Headers["orgunit-dn"];
            Department dep = GetDepartment(orgUnitDn);

            if (dep == null) return null;
            else return dep.Id;
#endif
        }
        public static string GetDepartmentName()
        {
#if DEBUG
            return "i4Ds";
#else

            string orgUnitDn = HttpContext.Current.Request.Headers["orgunit-dn"];
            Department dep = GetDepartment(orgUnitDn);

            return (dep == null) ? null : dep.DepartmentName;

#endif
        }

        public static string GetDebugInfo()
        {
            return HttpContext.Current.Request.Headers["affiliation"] + ", " + HttpContext.Current.Request.Headers["orgunit-dn"];
        }

        internal static string GetFullName()
        {
            return ShibUser.GetFirstName() + " " + ShibUser.GetLastName();
        }

        private static Department GetDepartment(string orgUnitDn)
        {
            if (orgUnitDn == null) orgUnitDn = "";

            using (ProStudentCreatorDBDataContext dbx = new ProStudentCreatorDBDataContext())
            {
                Department dept;
                dept = dbx.Departments.ToList<Department>().SingleOrDefault((Department d) => orgUnitDn.Contains(d.OUCode));

                if (dept == null)
                {
                    // Check if user is specifically mapped to a department. If so, return that dept.
                    string userEmail = ShibUser.GetEmail();
                    var userDeptMap = dbx.UserDepartmentMap.SingleOrDefault(m => m.email == userEmail);

                    if (userDeptMap == null) return null;
                    else dept = dbx.Departments.SingleOrDefault(d => d.Id == userDeptMap.departmentId);
                }

                return dept;
            }
        }

        public static bool CanExportExcel()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanExportExcel"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanExportExcel"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.email == ShibUser.GetEmail())?.canExportExcel ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanExportExcel"];
#endif
        }

        public static bool CanPublishProject()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanPublishProject"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanPublishProject"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.email == ShibUser.GetEmail())?.canPublishProject ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanPublishProject"];
#endif
        }

        public static bool CanVisitAdminPage()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanVisitAdminPage"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanVisitAdminPage"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.email == ShibUser.GetEmail())?.canVisitAdminPage ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanVisitAdminPage"];
#endif
        }

        public static bool CanSeeAllProjectsInProgress()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanSeeAllProjectsInProgress"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanSeeAllProjectsInProgress"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.email == ShibUser.GetEmail())?.canSeeAllProjectsInProgress ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanSeeAllProjectsInProgress"];
#endif
        }

        public static bool CanEditAllProjects()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanEditAllProjects"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanEditAllProjects"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.email == ShibUser.GetEmail())?.canSeeAllProjectsInProgress ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanEditAllProjects"];
#endif
        }

    }
}
