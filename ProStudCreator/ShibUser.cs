using System.Configuration;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public static class ShibUser
    {
        public static bool IsAuthenticated(ProStudentCreatorDBDataContext db)
        {
#if DEBUG
            return true;
#else
            return ShibUser.IsStaff() && ShibUser.GetDepartmentId(db).HasValue && db.UserDepartmentMap.Any(i => i.Mail == ShibUser.GetEmail());
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

        public static int? GetDepartmentId(ProStudentCreatorDBDataContext db)
        {
#if DEBUG
            return 2; // Department 2 = IIT

#else
            string orgUnitDn = HttpContext.Current.Request.Headers["orgunit-dn"];
            Department dep = GetDepartment(orgUnitDn, db);

            if (dep == null) return null;
            else return dep.Id;
#endif
        }

        public static string GetDepartmentName(ProStudentCreatorDBDataContext db)
        {
#if DEBUG
            return "IIT";
#else

            string orgUnitDn = HttpContext.Current.Request.Headers["orgunit-dn"];
            Department dep = GetDepartment(orgUnitDn, db);

            return (dep == null) ? null : dep.DepartmentName;

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
        public static string GetGravatar(string email)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            md5.Initialize();
            md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(email));
            return System.BitConverter.ToString(md5.Hash).Replace("-", "") + "&d=identicon";
        }

        public static string GetDebugInfo()
        {
            return HttpContext.Current.Request.Headers["affiliation"] + ", " +
                   HttpContext.Current.Request.Headers["orgunit-dn"];
        }

        internal static string GetFullName()
        {
            return GetFirstName() + " " + GetLastName();
        }

        private static Department GetDepartment(string orgUnitDn, ProStudentCreatorDBDataContext dbx)
        {
            if (orgUnitDn == null) orgUnitDn = "";

            Department dept;
            dept = dbx.Departments.ToList().SingleOrDefault(d => orgUnitDn.Contains(d.OUCode));

            if (dept == null)
            {
                // Check if user is specifically mapped to a department. If so, return that dept.
                var userEmail = GetEmail();
                var userDeptMap = dbx.UserDepartmentMap.SingleOrDefault(m => m.Mail == userEmail);

                if (userDeptMap == null) return null;
                dept = dbx.Departments.SingleOrDefault(d => d.Id == userDeptMap.DepartmentId);
            }

            return dept;
        }

        private static Department GetDepartment(string orgUnitDn)
        {
            if (orgUnitDn == null) orgUnitDn = "";

            Department dept;
            using (var dbx = new ProStudentCreatorDBDataContext())
            {
                dept =
                    dbx.Departments.ToList().SingleOrDefault(d => orgUnitDn.Contains(d.OUCode));

                if (dept == null)
                {
                    // Check if user is specifically mapped to a department. If so, return that dept.
                    var userEmail = GetEmail();
                    var userDeptMap = dbx.UserDepartmentMap.SingleOrDefault(m => m.Mail == userEmail);

                    if (userDeptMap == null) return null;
                    dept = dbx.Departments.SingleOrDefault(d => d.Id == userDeptMap.DepartmentId);
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
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanExportExcel ==
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
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanPublishProject ==
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
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanVisitAdminPage ==
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
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanSeeAllProjectsInProgress ==
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
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanEditAllProjects ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanEditAllProjects"];
#endif
        }

        public static bool CanSubmitAllProjects()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanSubmitAllProjects"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanSubmitAllProjects"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanSubmitAllProjects ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanSubmitAllProjects"];
#endif
        }

        public static bool CanSeeCreationDetails()
        {
#if DEBUG
            return true;
#else
            if (HttpContext.Current.Items["CanSeeCreationDetails"] == null)
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    HttpContext.Current.Items["CanSeeCreationDetails"] =
                        db.UserDepartmentMap.SingleOrDefault(u => u.Mail == ShibUser.GetEmail())?.CanSeeCreationDetails ==
                        true;
                }
            }
            return (bool)HttpContext.Current.Items["CanSeeCreationDetails"];
#endif
        }
    }
}