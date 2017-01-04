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
            //var semestersToDisplay = db.Semester.Where(item => item == Semester.CurrentSemester || item == Semester.LastSemester || item == Semester.NextSemester || item == Semester.AfterNextSemester);
            var semestersToDisplay = new Semester[] {Semester.LastSemester, Semester.CurrentSemester, Semester.NextSemester, Semester.AfterNextSemester };


            DataTable dt = new DataTable();
            string[] headers = { "Semester", "Semesterstart", "Semesterende", "Projekteinreichung", "Projektzuteilung", "Abgabe IP5 Voll-/Teilzeit", "Abgabe IP5 Berufsbegleitend", "Abgabe IP6 (normal)", "Abgabe IP6 (Variante 2 sem)", "Verteidigung IP6 Start", "Verteidigung IP6 Ende", "Ausstellung Bachelorthesen" };

            for (int i = 0; i < 12; i++)
            {
                dt.Columns.Add(headers[i]);
            }
            foreach (Semester semester in semestersToDisplay)
            {
                dt.Rows.Add(AttributesToDisplay(semester));
            }


            headers[0] = "Semester";
            Semester[] semestersArray = semestersToDisplay.ToArray();
            String[] semesterHeaders = new String[semestersArray.Length];
            for (int i = 0; i < semestersArray.Length; i++)
            {
                semesterHeaders[i] = semestersArray[i].Name;
            }

            for (int i = 1; i < 5; i++)
            {
                headers[i] = semesterHeaders[i - 1];
            }


            DataTable newTable = FlipDataTable(dt, headers);
            newTable.Rows[0].Delete();
            AllEvents.DataSource = newTable;
            AllEvents.DataBind();
        }

        public string[] AttributesToDisplay(Semester semester)
        {
            string[] attributes = new string[12];
            attributes[0] = semester.Name;
            attributes[1] = semester.StartDate.ToShortDateString();
            attributes[2] = semester.EndDate.ToShortDateString();
            attributes[3] = semester.ProjectSubmissionUntil.ToShortDateString();
            attributes[4] = semester.ProjectAllocation;
            attributes[5] = semester.SubmissionIP5FullPartTime;
            attributes[6] = semester.SubmissionIP5Accompanying;
            attributes[7] = semester.SubmissionIP6Normal;
            attributes[8] = semester.SubmissionIP6Variant2;
            attributes[9] = semester.DefenseIP6Start;
            attributes[10] = semester.DefenseIP6End;
            attributes[11] = semester.ExhibitionBachelorThesis;
            return attributes;
        }

        public static DataTable FlipDataTable(DataTable dt, String[] semesters)
        {
            DataTable table = new DataTable();
            //Get all the rows and change into columns
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                table.Columns.Add(semesters[i]);
            }
            DataRow dr;

            //get all the columns and make it as rows
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                dr = table.NewRow();
                dr[0] = dt.Columns[j].ToString();
                for (int k = 1; k <= dt.Rows.Count; k++)
                    dr[k] = dt.Rows[k - 1][j];
                table.Rows.Add(dr);
            }
            return table;
        }
    }
}