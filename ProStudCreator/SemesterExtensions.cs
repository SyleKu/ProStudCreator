using System;
using System.Linq;

namespace ProStudCreator
{
    public partial class Semester
    {

        private static ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        public static Semester CurrentSemester
        {
            get
            {
                return ActiveSemester(DateTime.Now);
            }
        }

        public static Semester NextSemester
        {
            get
            {
                Semester CurrentSemester = ActiveSemester(DateTime.Now);
                DateTime SafeSemesterActiveUntil = CurrentSemester.SemesterActiveUntil ?? default(DateTime);
                DateTime SafeDate = SafeSemesterActiveUntil.AddDays(7);
                return ActiveSemester(SafeDate);
            }
        }

        public static Semester AfterNextSemester
        {
            get
            {
                Semester NextSemester = Semester.NextSemester;
                DateTime SafeSemesterActiveUntil = NextSemester.SemesterActiveUntil ?? default(DateTime);
                DateTime SafeDate = SafeSemesterActiveUntil.AddDays(7);
                return ActiveSemester(SafeDate);
            }
        }

        public static Semester LastSemester
        {
            get
            {
                Semester CurrentSemester = ActiveSemester(DateTime.Now);
                DateTime SafeDate = CurrentSemester.StartDate.AddDays(-7);
                return ActiveSemester(SafeDate);
            }
        }

        public static Semester BevorLastSemester
        {
            get
            {
                DateTime SafeDate = LastSemester.StartDate.AddDays(-7);
                return ActiveSemester(SafeDate);
            }
        }

        public static explicit operator Semester(DateTime _dt)
        {
            return ActiveSemester(_dt);
        }

        private static Semester ActiveSemester(DateTime date)
        {
            IQueryable<Semester> semesters = AllSemesters();
            foreach (Semester semester in semesters)
            {
                if (semester.StartDate < date && semester.SemesterActiveUntil > date)
                {
                    return semester;
                }
            }
            throw new Exception("No active Semester!");
        }

        private static IQueryable<Semester> AllSemesters() => db.Semester.Select(i => i);

        public override string ToString() => Name;

    }
}