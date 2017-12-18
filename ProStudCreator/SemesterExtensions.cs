using System;
using System.Linq;
using NPOI.Util;

namespace ProStudCreator
{
    public partial class Semester
    {
        public static Semester CurrentSemester(ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(DateTime.Now, db);
        }

        public static Semester NextSemester(ProStudentCreatorDBDataContext db)
        {
            var safeDayBeforeNextSemester = CurrentSemester(db).DayBeforeNextSemester ?? default(DateTime);
            var safeDate = safeDayBeforeNextSemester.AddDays(7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester AfterNextSemester(ProStudentCreatorDBDataContext db)
        {
            var nextSemester = NextSemester(db);
            var safeDayBeforeNextSemester = nextSemester.DayBeforeNextSemester ?? default(DateTime);
            var safeDate = safeDayBeforeNextSemester.AddDays(7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester LastSemester(ProStudentCreatorDBDataContext db)
        {
            var currentSemester = ActiveSemester(DateTime.Now, db);
            var safeDate = currentSemester.StartDate.AddDays(-7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester NextSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(semester.DayBeforeNextSemester.Value.AddDays(7), db);
        }

        public static Semester AfterNextSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return NextSemester(NextSemester(semester, db), db);
        }

        public static Semester LastSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(semester.StartDate.AddDays(-7), db);
        }

        public static Semester ActiveSemester(DateTime date, ProStudentCreatorDBDataContext db)
        {
            return db.Semester.Single(s => s.StartDate < date && s.DayBeforeNextSemester > date);
        }

        public bool IsSpringSemester()
        {
            return this.Name.Contains("FS");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}