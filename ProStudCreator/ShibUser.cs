using System;
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
            int? result;
            string userOU = HttpContext.Current.Request.Headers["orgunit-dn"];

            using (ProStudentCreatorDBDataContext dbx = new ProStudentCreatorDBDataContext())
            {
                Department dep = dbx.Departments.ToList<Department>().SingleOrDefault((Department d) => userOU.Contains(d.OUCode));
                if (dep == null)
                {
                    // TODO Replace makeshift fix for "Studiengang Informatik" users
                    if (userOU.Contains(",OU=62_I,"))
                    {
                        result = 0; // Default to i4Ds. Could be defined by user somehow.
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = new int?(dep.Id);
                }
            }
            return result;
            #endif
        }
        public static string GetDepartmentName()
        {
            #if DEBUG
            return null;
            #else
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
    }
}
