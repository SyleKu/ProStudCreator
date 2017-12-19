using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ProStudCreator
{
    public static class ProjectExtensions
    {
        public static readonly int EXPECTEDPROPCOUNT = 88;

        #region Actions

        public static void InitNew(this Project _p)
        {
            _p.Creator = ShibUser.GetEmail();
            _p.CreateDate = DateTime.Now;
            _p.PublishedDate = DateTime.Now;
            _p.State = ProjectState.InProgress;
            _p.IsMainVersion = true;
        }
        public static void InitNewVersion(this Project _p, Project previousProject)
        {
            _p.Creator = previousProject.Creator;
            _p.CreateDate = previousProject.CreateDate;
            _p.PublishedDate = previousProject.PublishedDate;
            _p.State = previousProject.State;
            _p.ProjectNr = previousProject.ProjectNr;
            _p.BaseVersionId = previousProject.BaseVersionId;
            _p.ModificationDate = DateTime.Now;
            _p.LastEditedBy = ShibUser.GetEmail();
            _p.IsMainVersion = true;
        }
        public static void IsUpdated()
        {
            var actualPropCount = typeof(Project).GetProperties().Count();

            if (actualPropCount != EXPECTEDPROPCOUNT)
            {
                throw new OutdatedSaveMethod("The Save-Method is outdated. You have mostlikely edited the DBML. Please Update ProjectExtension.cs AND update the constant 'EXPECTEDPROPCOUNT'. PropertyCount: " + actualPropCount);
            }

        }
        public static void SaveAsNewVersion(this Project _p, ProStudentCreatorDBDataContext db)
        {
            IsUpdated();
            Project project = new Project();
            project.ModificationDate = DateTime.Now;
            project.LastEditedBy = ShibUser.GetEmail();
            project.IsMainVersion = true;
            _p.IsMainVersion = false;

            _p.MapProject(project);
            
            db.Projects.InsertOnSubmit(project);
            db.SubmitChanges();
            
        }

        public static void MapProject(this Project _p, Project target)
        {
            target.Ablehnungsgrund = _p.Ablehnungsgrund;
            target.Advisor1 = _p.Advisor1;
            target.Advisor2 = _p.Advisor2;
            target.Attachements = _p.Attachements;
            target.BaseVersionId = _p.BaseVersionId;
            target.BillingStatus = _p.BillingStatus;
            target.ClientAddressCity = _p.ClientAddressCity;
            target.ClientAddressDepartment = _p.ClientAddressDepartment;
            target.ClientAddressPostcode = _p.ClientAddressPostcode;
            target.ClientAddressStreet = _p.ClientAddressStreet;
            target.ClientAddressTitle = _p.ClientAddressTitle;
            target.ClientCompany = _p.ClientCompany;
            target.ClientMail = _p.ClientMail;
            target.ClientPerson = _p.ClientPerson;
            target.ClientReferenceNumber = _p.ClientReferenceNumber;
            target.ClientType = _p.ClientType;
            target.CreateDate = _p.CreateDate;
            target.Creator = _p.Creator;
            target.Department = _p.Department;
            target.DurationOneSemester = target.DurationOneSemester;
            target.Expert = _p.Expert;
            target.ImgDescription = _p.ImgDescription;
            target.Important = _p.Important;
            target.InitialPosition = _p.InitialPosition;
            target.IsContinuation = _p.IsContinuation;
            target.LanguageEnglish = _p.LanguageEnglish;
            target.LanguageGerman = _p.LanguageGerman;
            target.LogDefenceDate = _p.LogDefenceDate;
            target.LogDefenceRoom = _p.LogDefenceRoom;
            target.LogExpertID = _p.LogExpertID;
            target.LogExpertPaid = _p.LogExpertPaid;
            target.LogGradeStudent1 = _p.LogGradeStudent1;
            target.LogGradeStudent2 = _p.LogGradeStudent2;
            target.LogLanguageEnglish = _p.LogLanguageEnglish;
            target.LogLanguageGerman = _p.LogLanguageGerman;
            target.LogProjectDuration = _p.LogProjectDuration;
            target.LogProjectType = _p.LogProjectType;
            target.LogProjectTypeID = _p.LogProjectTypeID;
            target.LogStudent1Mail = _p.LogStudent1Mail;
            target.LogStudent1Name = _p.LogStudent1Name;
            target.LogStudent2Mail = _p.LogStudent2Mail;
            target.LogStudent2Name = _p.LogStudent2Name;
            target.Name = _p.Name;
            target.Objective = _p.Objective;
            target.OverOnePage = _p.OverOnePage;
            target.Picture = _p.Picture;
            target.POneTeamSize = _p.POneTeamSize;
            target.POneType = _p.POneType;
            target.PreviousProjectID = _p.PreviousProjectID;
            target.ProblemStatement = _p.ProblemStatement;
            target.Project1 = _p.Project1;
            target.ProjectNr = _p.ProjectNr;
            target.Projects = target.Projects;
            target.PTwoTeamSize = _p.PTwoTeamSize;
            target.PTwoType = _p.PTwoType;
            target.PublishedDate = _p.PublishedDate;
            target.References = _p.References;
            target.Remarks = _p.Remarks;
            target.Reservation1Mail = _p.Reservation1Mail;
            target.Reservation1Name = _p.Reservation1Name;
            target.Reservation2Mail = _p.Reservation2Mail;
            target.Reservation2Name = _p.Reservation2Name;
            target.Semester = _p.Semester;
            target.State = _p.State;
            target.Tasks = _p.Tasks;
            target.TypeAppWeb = _p.TypeAppWeb;
            target.TypeCGIP = _p.TypeCGIP;
            target.TypeDBBigData = _p.TypeDBBigData;
            target.TypeDesignUX = _p.TypeDesignUX;
            target.TypeHW = _p.TypeHW;
            target.TypeMlAlg = _p.TypeMlAlg;
            target.TypeSE = _p.TypeSE;
            target.TypeSysSec = _p.TypeSysSec;
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
            _dbx.SubmitChanges();
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

        public static void GenerateProjectNr(this Project _p, ProStudentCreatorDBDataContext dbx)
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
            var date = Semester.ActiveSemester(new DateTime(2017, 4, 1), db).EndDate;
            if (p.Semester.EndDate >= date)
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

        public static string stateColor(this Project p)
        {
            switch (p.State)
            {
                case ProjectState.InProgress:
                    return "#EFF3FB";

                case ProjectState.Published:
                    return "#A9F5A9";

                case ProjectState.Rejected:
                    return "#F5A9A9";

                case ProjectState.Submitted:
                    return "#ffcc99";

                default:
                    return "";

            }
        
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