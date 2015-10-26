using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public static class ProjectExtensions
    {        

        #region Actions

        public static void InitNew(this Project _p)
        {
            _p.Creator = ShibUser.GetEmail();
            _p.CreateDate = DateTime.Now;
            _p.PublishedDate = DateTime.Now;
            _p.State = ProjectState.InProgress;
        }

        /// <summary>
        /// Submits user's project for approval by an admin.
        /// </summary>
        /// <param name="_p"></param>
        public static void Submit(this Project _p)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            GenerateProjectNr(_p);
            _p.State = ProjectState.Submitted;
        }

        /// <summary>
        /// Publishes a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Publish(this Project _p)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Published;
        }

        /// <summary>
        /// Rejects a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Reject(this Project _p)
        {
            _p.State = ProjectState.Rejected;
        }

        /// <summary>
        /// Generates a new project number that is unique per semester and institute.
        /// Applies to projects after submission.
        /// </summary>
        /// <param name="_p"></param>
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

        /// <summary>
        /// Sets project state to deleted so it's no longer listed. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Delete(this Project _p)
        {
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Deleted;
        }

        /// <summary>
        /// Rolls back project into editable state.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unsubmit(this Project _p)
        {
            _p.State = ProjectState.InProgress;
        }

        /// <summary>
        /// Revokes project state from 'published' to 'submitted'. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unpublish(this Project _p)
        {
            _p.State = ProjectState.Submitted;
        }

        public static void MoveToNextSemester(this Project _p)
        {
            if (!_p.UserCanMoveToNextSemester())
                throw new UnauthorizedAccessException();

            _p.State = ProjectState.InProgress;
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
        }

        #endregion

        #region Getters

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

        public static int GetProjectTeamSize(this Project _p)
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

        #endregion

        #region Permissions

        public static bool UserCanEdit(this Project _p)
        {
            return ShibUser.IsAdmin()
                || (_p.Creator == ShibUser.GetEmail() || _p.ClientMail == ShibUser.GetEmail() || _p.Advisor1Mail == ShibUser.GetEmail() || _p.Advisor2Mail == ShibUser.GetEmail());
        }

        public static bool UserCanMoveToNextSemester(this Project _p)
        {
            return ShibUser.IsAdmin()
                || (_p.UserCanEdit() && _p.State != ProjectState.Submitted && _p.State != ProjectState.Published);
        }

        public static bool UserCanPublish(this Project _p)
        {
            return _p.State == ProjectState.Submitted && ShibUser.IsAdmin();
        }

        public static bool UserCanUnpublish(this Project _p)
        {
            return _p.State == ProjectState.Published && ShibUser.IsAdmin();
        }

        public static bool UserCanRefuse(this Project _p)
        {
            return (_p.State == ProjectState.Submitted || _p.State == ProjectState.Published)
                && ShibUser.IsAdmin();
        }

        public static bool UserCanSubmit(this Project _p)
        {
            return _p.UserCanEdit() && _p.State == ProjectState.InProgress;
        }

        public static bool UserCanUnsubmit(this Project _p)
        {
            return _p.UserCanEdit() && _p.State == ProjectState.Submitted;
        }

        #endregion
    }
}