using System;
using System.Configuration;
using System.Linq;
using System.Web;
namespace ProStudCreator
{
    public static class ShibUser
    {
        static bool BYPASS_AUTH = true;

        public static bool IsAuthenticated()
        {
            if (BYPASS_AUTH)
                return true;

            return ShibUser.IsStaff() && ShibUser.GetDepartmentId().HasValue;
        }
        public static bool IsAdmin()
        {
            if (BYPASS_AUTH)
                return true;

            if (HttpContext.Current.Items["IsAdmin"] == null)
            {
                HttpContext.Current.Items["IsAdmin"] = ConfigurationManager.AppSettings["admins"].Split(new char[]
                {
                    ';'
                }).Contains(ShibUser.GetEmail());
            }
            return (bool)HttpContext.Current.Items["IsAdmin"];
        }
        public static bool IsStaff()
        {
            if (BYPASS_AUTH)
                return true;

            string aff = HttpContext.Current.Request.Headers["affiliation"];
            return aff != null && aff.Split(new char[]
            {
                ';'
            }).Contains("staff");
        }
        public static string GetEmail()
        {
            if (BYPASS_AUTH)
                return "stephen.randles@fhnw.ch";

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
        }
        public static string GetFirstName()
        {
            if (BYPASS_AUTH)
                return "Stephen";

            return HttpContext.Current.Request.Headers["givenName"];
        }
        public static string GetLastName()
        {
            if (BYPASS_AUTH)
                return "Randles";

            return HttpContext.Current.Request.Headers["surname"];
        }
        public static string GetPhoneNumber()
        {
            return HttpContext.Current.Request.Headers["telephoneNumber"];
        }
        public static int? GetDepartmentId()
        {
            if (BYPASS_AUTH)
                return 0;

            int? result;
            using (ProStudentCreatorDBDataContext dbx = new ProStudentCreatorDBDataContext())
            {
                Department dep = dbx.Departments.ToList<Department>().SingleOrDefault((Department d) => HttpContext.Current.Request.Headers["orgunit-dn"].Contains(d.OUCode));
                if (dep == null)
                {
                    result = null;
                }
                else
                {
                    result = new int?(dep.Id);
                }
            }
            return result;
        }
        public static string GetDepartmentName()
        {
            if (BYPASS_AUTH)
                return null;

            string result;
            using (ProStudentCreatorDBDataContext dbx = new ProStudentCreatorDBDataContext())
            {
                Department dep = dbx.Departments.ToList<Department>().SingleOrDefault((Department d) => HttpContext.Current.Request.Headers["orgunit-dn"].Contains(d.OUCode));
                if (dep == null)
                {
                    result = null;
                }
                else
                {
                    result = dep.DepartmentName;
                }
            }
            return result;
        }

        public static string GetDebugInfo()
        {
            return HttpContext.Current.Request.Headers["affiliation"] + ", "+ HttpContext.Current.Request.Headers["orgunit-dn"];
        }

        internal static string GetFullName()
        {
            return ShibUser.GetFirstName() + " " + ShibUser.GetLastName();
        }
    }
}
