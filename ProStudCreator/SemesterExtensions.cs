using System;
using System.Linq;

namespace ProStudCreator
{
    public partial class Semester
    {
        public static Semester CurrentSemester(ProStudentCreatorDBDataContext db) => ActiveSemester(DateTime.Now, db);

        public static Semester NextSemester(ProStudentCreatorDBDataContext db)
        {
            var safeSemesterActiveUntil = Semester.CurrentSemester(db).SemesterActiveUntil ?? default(DateTime);
            var safeDate = safeSemesterActiveUntil.AddDays(7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester AfterNextSemester(ProStudentCreatorDBDataContext db)
        {

            var nextSemester = Semester.NextSemester(db);
            var safeSemesterActiveUntil = nextSemester.SemesterActiveUntil ?? default(DateTime);
            var safeDate = safeSemesterActiveUntil.AddDays(7);
            return ActiveSemester(safeDate,db);

        }

        public static Semester LastSemester(ProStudentCreatorDBDataContext db)
        {

            var currentSemester = ActiveSemester(DateTime.Now,db);
            var safeDate = currentSemester.StartDate.AddDays(-7);
            return ActiveSemester(safeDate,db);

        }

        public static Semester ActiveSemester(DateTime date, ProStudentCreatorDBDataContext db) => db.Semester.Single(s => s.StartDate < date && s.SemesterActiveUntil > date);
        
        public override string ToString() => Name;

    }
}