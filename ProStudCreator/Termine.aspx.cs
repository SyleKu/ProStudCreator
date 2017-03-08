using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{

    public class SemesterSingleElement
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ProjectSubmissionUntil { get; set; }
        public string ProjectAllocation { get; set; }
        public string SubmissionIP5FullPartTime { get; set; }
        public string SubmissionIP5Accompanying { get; set; }
        public string SubmissionIP6Normal { get; set; }
        public string SubmissionIP6Variant2 { get; set; }
        public string DefenseIP6Start { get; set; }
        public string DefenseIP6End { get; set; }
        public string ExhibitionBachelorThesis { get; set; }
        public string Name { get; set; }
    }

    public partial class Termine : System.Web.UI.Page
    {

        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected GridView AllEvents;

        protected void Page_Load(object sender, EventArgs e)
        {
            var semestersToDisplay = new Semester[] { Semester.LastSemester(db), Semester.CurrentSemester(db), Semester.NextSemester(db), Semester.AfterNextSemester(db) };


            var dt = new DataTable();
            foreach (var header in new string[]
                {
                    "Semester",
                    "",
                    "Projekteinreichung (extern)",
                    "Projekteinreichung (intern)",
                    "Projektzuteilung",
                    "Abgabe IP5 (normal)",
                    "Abgabe IP5 (Variante 2 Sem.)",
                    "Abgabe IP6 (normal)",
                    "Abgabe IP6 (Variante 2 Sem.)",
                    "Verteidigung IP6",
                    "Ausstellung Bachelorthesen"
                })
                dt.Columns.Add(header);

            foreach (Semester semester in semestersToDisplay)
            {
                dt.Rows.Add(new string[]
                    {
                        semester.Name,
                        $"{semester.StartDate.ToShortDateString()} bis {semester.EndDate.ToShortDateString()}",
                        semester.ProjectSubmissionUntil.AddDays(-7*6).ToShortDateString(),
                        semester.ProjectSubmissionUntil.ToShortDateString(),
                        semester.ProjectAllocation,
                        semester.SubmissionIP5FullPartTime,
                        semester.SubmissionIP5Accompanying,
                        semester.SubmissionIP6Normal,
                        semester.SubmissionIP6Variant2,
                        $"{semester.DefenseIP6Start} bis {semester.DefenseIP6End}",
                        semester.ExhibitionBachelorThesis
                    });
            }


            var flipHeaders = new string[5];
            flipHeaders[0] = "Semester";
            var semesterHeaders = semestersToDisplay.Select(s => s.Name).ToArray();
            for (int i = 1; i < 5; i++)
                flipHeaders[i] = semesterHeaders[i - 1];


            var newTable = FlipDataTable(dt, flipHeaders);
            newTable.Rows[0].Delete();
            AllEvents.DataSource = newTable;
            AllEvents.DataBind();
        }

        public static DataTable FlipDataTable(DataTable dt, string[] semesters)
        {
            var table = new DataTable();
            //Get all the rows and change into columns
            for (int i = 0; i <= dt.Rows.Count; i++)
                table.Columns.Add(semesters[i]);

            //get all the columns and make it as rows
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                var dr = table.NewRow();
                dr[0] = dt.Columns[j].ToString();
                for (int k = 1; k <= dt.Rows.Count; k++)
                    dr[k] = dt.Rows[k - 1][j];
                table.Rows.Add(dr);
            }
            return table;
        }
    }
}