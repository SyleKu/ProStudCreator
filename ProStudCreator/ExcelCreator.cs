using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;

namespace ProStudCreator
{
    public class ExcelCreator
    {
        private static string SHEET_NAME = "Projects";
        private static readonly string[] HEADERS = new string[] {
                "Abbreviation",
                "Name",
                "Display Name",
                "1P_Type",
                "2P_Type",
                "1P_Teamsize",
                "2P_Teamsize",
                "Major",
                "Wichtigkeit",
                "Betreuer",
                "Betreuer2",
                "Modules",
                "Fixe Zuteilung",
                "Fixe Zuteilung 2",
                "Kommentar"
            };

        // References
        // - http://poi.apache.org/spreadsheet/quick-guide.html#NewWorkbook

        public static void generateProjectList(Stream outStream)
        { 
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (int i=0; i < HEADERS.Length; i++)
            {
                worksheet.GetRow(0).CreateCell(i).SetCellValue(HEADERS[i]);
            }

            // Project entries
            ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
            var projects = db.Projects.Where(p =>
                p.PublishedDate >= Semester.CurrentSemester.StartDate
                && p.PublishedDate < Semester.CurrentSemester.EndDate
                && p.State == ProjectState.Published
            ).OrderBy(p => p.ProjectNr).ToArray();

            for (var i=0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                projectToExcelRow(projects[i], row);
            }

            for (int i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }

        private static void projectToExcelRow(Project p, IRow row)
        {
            string abbreviation = Semester.CurrentSemester.ToString() + "_" + p.Department.DepartmentName + p.ProjectNr;
            string dispName = abbreviation + "_" + p.Name;

            row.CreateCell(0).SetCellValue(abbreviation);
            row.CreateCell(1).SetCellValue(p.Name);
            row.CreateCell(2).SetCellValue(dispName);
            row.CreateCell(3).SetCellValue(p.POneType.Export());
            row.CreateCell(4).SetCellValue(p.PTwoType != null ? p.PTwoType.Export() : p.POneType.Export());
            row.CreateCell(5).SetCellValue(p.POneTeamSize.Export());
            row.CreateCell(6).SetCellValue( p.PTwoTeamSize != null ? p.PTwoTeamSize.Export() : p.POneTeamSize.Export() );
            row.CreateCell(7).SetCellValue("-");    // Major undefined
            row.CreateCell(8).SetCellValue(0);      // Importance undefined
            row.CreateCell(9).SetCellValue(p.Advisor1Mail);
            row.CreateCell(10).SetCellValue(p.Advisor2Mail);
            row.CreateCell(11).SetCellValue("");    // Modules undefined
            row.CreateCell(12).SetCellValue(p.Reservation1Mail);
            row.CreateCell(13).SetCellValue(p.Reservation2Mail);
            row.CreateCell(14).SetCellValue("");  // Comment undefined
            
        }

    }
}