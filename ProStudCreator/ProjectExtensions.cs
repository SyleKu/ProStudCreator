using System;
using System.Globalization;
using System.Linq;

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
            _p.IsMainVersion = true;
        }

        /// <summary>
        ///     Submits user's project for approval by an admin.
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
        ///     Publishes a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Publish(this Project _p, ProStudentCreatorDBDataContext _dbx)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Published;
            _p.Semester = _dbx.Semester.OrderBy(x => x.StartDate).First(x => x.StartDate > DateTime.Now);
        }

        /// <summary>
        ///     Rejects a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Reject(this Project _p)
        {
            _p.State = ProjectState.Rejected;
        }

        /// <summary>
        ///     Generates a new project number that is unique per semester and institute.
        ///     Applies to projects after submission.
        /// </summary>
        /// <param name="_p"></param>
        public static void GenerateProjectNr(this Project _p)
        {
            using (var dbx = new ProStudentCreatorDBDataContext())
            {
                var semesterStart = Semester.ActiveSemester(_p.PublishedDate, dbx).StartDate;
                var semesterEnd = Semester.ActiveSemester(_p.PublishedDate, dbx).EndDate;

                // Get project numbers from this semester & same department
                var nrs = (
                    from p in dbx.Projects
                    where p.PublishedDate >= semesterStart && p.PublishedDate <= semesterEnd
                          && p.Id != _p.Id
                          && (p.State == ProjectState.Published || p.State == ProjectState.Submitted)
                          && p.Department == _p.Department
                    select p.ProjectNr).ToArray();
                if (_p.ProjectNr >= 100 || nrs.Contains(_p.ProjectNr) || _p.ProjectNr < 1)
                {
                    _p.ProjectNr = 1;
                    while (nrs.Contains(_p.ProjectNr))
                        _p.ProjectNr++;
                }
            }
        }

        /// <summary>
        ///     Sets project state to deleted so it's no longer listed. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Delete(this Project _p)
        {
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Deleted;
        }

        /// <summary>
        ///     Rolls back project into editable state.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unsubmit(this Project _p)
        {
            _p.State = ProjectState.InProgress;
        }

        /// <summary>
        ///     Revokes project state from 'published' to 'submitted'. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unpublish(this Project _p)
        {
            _p.State = ProjectState.Submitted;
        }

        #endregion

        #region Getters

        public static int GetProjectTeamSize(this Project _p)
        {
            if (_p.POneTeamSize.Size2 ||
                _p.PTwoTeamSize != null && _p.PTwoTeamSize.Size2)
                return 2;
            return 1;
        }

        public static DateTime GetDeliveryDate(this Project _p)
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                DateTime dbDate;

                if (_p?.LogProjectDuration == 1 && (_p?.LogProjectType?.P5 ?? false)) //IP5 Projekt Voll/TeilZeit
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP5FullPartTime, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate
                        : Semester.NextSemester(db).EndDate;
                if (_p?.LogProjectDuration == 2 && (_p?.LogProjectType?.P5 ?? false)) //IP5 Berufsbegleitend
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP5Accompanying, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate
                        : Semester.NextSemester(db).EndDate;
                if (_p?.LogProjectDuration == 1 && (_p?.LogProjectType?.P6 ?? false)) //IP6 Variante 1 Semester
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP6Normal, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate
                        : Semester.NextSemester(db).EndDate;
                if (_p?.LogProjectDuration == 2 && (_p?.LogProjectType?.P6 ?? false)) //IP6 Variante 2 Semester
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP6Variant2, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate
                        : Semester.NextSemester(db).EndDate;
                return Semester.NextSemester(db).EndDate;
            }
        }


        public static bool CanEditTitle(this Project _p)
        {
            return DateTime.Now < _p.GetDeliveryDate().AddDays(
                       -11 * 7 /* 11 weeks before delivery date */
                       + 2 /* give some leeway */);
        }


        public static Semester GetEndSemester(this Project _p, ProStudentCreatorDBDataContext db)
        {
            return _p.LogProjectDuration == 2 ? Semester.NextSemester(_p.Semester, db) : _p.Semester;
        }

        public static string ExhibitionBachelorThesis(this Project _p, ProStudentCreatorDBDataContext db)
        {
            if (_p.LogProjectType?.P5 == true
                || (_p.LogProjectType?.P6 == true && _p.LogProjectDuration == 1 && !_p.Semester.IsSpringSemester())
                || (_p.LogProjectType?.P6 == true && _p.LogProjectDuration == 2 && _p.Semester.IsSpringSemester()))
            {
                return "";
            }
            return _p.GetEndSemester(db).ExhibitionBachelorThesis;
        }


        public static bool GetProjectRelevantForGradeTasks(this Project p, ProStudentCreatorDBDataContext db)
        {

            if (p.Semester.EndDate >= Semester.ActiveSemester(new DateTime(2017, 4, 1), db).EndDate)
            {
                if (p.LogProjectType?.P5 == true)
                {
                    if (DateTime.Now > p.GetDeliveryDate().AddDays(21))
                    {
                        return true;
                    }
                }
                if (p.LogProjectType?.P6 == true)
                {
                    if (p.LogProjectDuration == 2)
                    {
                        if (DateTime.ParseExact(p.Semester.DefenseIP6BEnd, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture) < DateTime.Now)
                        {
                            return true;
                        }
                    }
                    if (p.LogProjectDuration == 1)
                    {
                        if (DateTime.ParseExact(p.Semester.DefenseIP6End, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture) < DateTime.Now)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region Permissions

        public static bool UserCanEdit(this Project _p)
        {
            return ShibUser.CanEditAllProjects() || _p.UserIsOwner() &&
                   (_p.State == ProjectState.InProgress || _p.State == ProjectState.Rejected ||
                    _p.State == ProjectState.Submitted);
        }

        public static bool UserIsOwner(this Project _p)
        {
            return _p.Creator == ShibUser.GetEmail() || _p.ClientMail == ShibUser.GetEmail() ||
                   _p.Advisor1?.Mail == ShibUser.GetEmail() || _p.Advisor2?.Mail == ShibUser.GetEmail();
        }

        public static bool UserCanEditAfterStart(this Project _p)
        {
            return ShibUser.CanEditAllProjects() || _p.UserIsOwner();
        }

        public static bool UserCanPublish(this Project _p)
        {
            return _p.State == ProjectState.Submitted && ShibUser.CanPublishProject();
        }

        public static bool UserCanUnpublish(this Project _p)
        {
            return _p.State == ProjectState.Published && ShibUser.CanPublishProject();
        }

        public static bool UserCanReject(this Project _p)
        {
            return (_p.State == ProjectState.Submitted || _p.State == ProjectState.Published)
                   && ShibUser.CanPublishProject();
        }

        public static bool UserCanSubmit(this Project _p)
        {
            return _p.UserCanEdit() && (_p.State == ProjectState.InProgress || _p.State == ProjectState.Rejected);
        }

        public static bool UserCanUnsubmit(this Project _p)
        {
            return _p.UserCanEdit() && _p.State == ProjectState.Submitted;
        }

        #endregion

        #region LINQ-derived type extensions

        public static string Export(this ProjectType pt)
        {
            if (pt.P5 && !pt.P6) return "P5";
            if (!pt.P5 && pt.P6) return "P6";
            return "both";
        }

        public static string Export(this ProjectTeamSize pts)
        {
            if (pts.Size1 && !pts.Size2) return "1";
            if (!pts.Size1 && pts.Size2) return "2";
            return "1;2";
        }

        #endregion
    }
}