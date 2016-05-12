using System.Configuration;
using System.Linq;
using System.Web;
namespace ProStudCreator
{
    public static class ShibUser
    {
        public static bool IsAuthenticated()
        {
            #if DEBUG
                return true;
            #else
                return ShibUser.IsStaff() && ShibUser.GetDepartmentId().HasValue;
            #endif
        }
        public static bool IsAdmin()
        {
            #if DEBUG
                return true;
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

            if (dep != null) return dep.DepartmentName;
            else return null;

            #endif
        }

        public static string GetDebugInfo()
        {
            return HttpContext.Current.Request.Headers["affiliation"] + ", "+ HttpContext.Current.Request.Headers["orgunit-dn"];
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
                    var userDeptMap = dbx.UserDepartmentMaps.SingleOrDefault(m => m.email == userEmail);
                    dept = dbx.Departments.SingleOrDefault(d => d.Id == userDeptMap.departmentId);
                }

                return dept;
            }
        }

    }
}
