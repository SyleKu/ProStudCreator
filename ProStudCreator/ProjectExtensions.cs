using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public static class ProjectExtensions
    {
        public static void Submit(this Project _p)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            GenerateProjectNr(_p);
            _p.State = ProjectState.Submitted;
        }

        public static bool UserHasEditPermission(this Project _p)
        {
            if (ShibUser.IsAdmin())
                return true;

            return _p.Creator == ShibUser.GetEmail() || _p.ClientMail == ShibUser.GetEmail() || _p.Advisor1Mail == ShibUser.GetEmail() || _p.Advisor2Mail == ShibUser.GetEmail();
        }

        public static void InitNew(this Project _p)
        {
            _p.Creator = ShibUser.GetEmail();
            _p.CreateDate = DateTime.Now;
            _p.PublishedDate = DateTime.Now;
            _p.State = ProjectState.InProgress;
        }

        public static void Publish(this Project _p)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Published;
        }

        public static void GenerateProjectNr(this Project _p)
        {
            DateTime semesterStart = ((Semester)_p.PublishedDate).StartDate;
            DateTime semesterEnd = ((Semester)_p.PublishedDate).EndDate;

            using (ProStudentCreatorDBDataContext dbx = new ProStudentCreatorDBDataContext())
            {
                // Get project numbers from this semester & same department
                int[] nrs = (
                    from p in dbx.Projects
                    where p.PublishedDate >= semesterStart && p.PublishedDate <= semesterEnd
                        && p.Id != _p.Id
                        && (p.State == ProjectState.Published || p.State == ProjectState.Submitted)
                        && p.Department == _p.Department
                    select p.ProjectNr).ToArray<int>();
                if (_p.ProjectNr >= 100 || nrs.Contains(_p.ProjectNr) || _p.ProjectNr < 1)
                {
                    _p.ProjectNr = 1;
                    while (nrs.Contains(_p.ProjectNr))
                    {
                        _p.ProjectNr++;
                    }
                }
            }
        }
        public static void Delete(this Project _p)
        {
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Deleted;
        }

        public static void Reject(this Project _p)
        {
            _p.State = ProjectState.Rejected;
        }

        public static void Unsubmit(this Project _p)
        {
            _p.State = ProjectState.InProgress;
        }

        public static void Unpublish(this Project _p)
        {
            _p.State = ProjectState.Submitted;
        }

        public static int getProjectTeamSize(this Project _p)
        {
            if (_p.POneTeamSize.Size2 ||
               (_p.PTwoTeamSize != null && _p.PTwoTeamSize.Size2))
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public static Semester GetSemester(this Project _p)
        {
            switch (_p.State)
            {
                case ProjectState.Published:
                    return _p.PublishedDate;
                case ProjectState.Deleted:
                case ProjectState.InProgress:
                case ProjectState.Rejected:
                case ProjectState.Submitted:
                    return _p.ModificationDate;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}