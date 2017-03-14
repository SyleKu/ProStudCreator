﻿using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace ProStudCreator
{
    public class ExcelCreator
    {
        private static string SHEET_NAME = "Projects";
        private static string MARKETING_SHEET_NAME = "Projekte";
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
                "Kommentar",
                "ID",
                "SingleSemester",
                "Continuation",
                "German",
                "English"
            };

        private static readonly string[] MARKETING_HEADERS = new string[]
        {
            "Projektnummer",
            "Insitut",
            "Projekttitel",
            "Studierende",
            "Betreuende/r",
            "Projekttyp",
            "Unternehmen",
            "Anrede",
            "Name",
            "Abteilung",
            "Strasse und Nummer",
            "PLZ",
            "Ort",
            "Referenznummer des Kunden"
        };

        // References
        // - http://poi.apache.org/spreadsheet/quick-guide.html#NewWorkbook

        public static void GenerateProjectList(Stream outStream, IEnumerable<Project> _projects)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (int i = 0; i < HEADERS.Length; i++)
            {
                worksheet.GetRow(0).CreateCell(i).SetCellValue(HEADERS[i]);
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
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
            ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
            p.Semester = p.Semester == null ? Semester.NextSemester(db) : p.Semester = p.Semester;

            string abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester.ToString() + "_" + p.Department.DepartmentName + p.ProjectNr.ToString("D2");
            string dispName = abbreviation + "_" + p.Name;

            row.CreateCell(0).SetCellValue(abbreviation);
            row.CreateCell(1).SetCellValue(p.Name);
            row.CreateCell(2).SetCellValue(dispName);
            row.CreateCell(3).SetCellValue(p.POneType.Export());
            row.CreateCell(4).SetCellValue(p.PTwoType != null ? p.PTwoType.Export() : p.POneType.Export());
            row.CreateCell(5).SetCellValue(p.POneTeamSize.Export());
            row.CreateCell(6).SetCellValue(p.PTwoTeamSize != null ? p.PTwoTeamSize.Export() : p.POneTeamSize.Export());
            row.CreateCell(7).SetCellValue("-");    // Major undefined
            row.CreateCell(8).SetCellValue(0);      // Importance undefined
            row.CreateCell(9).SetCellValue(p.Advisor1Mail);
            row.CreateCell(10).SetCellValue(p.Advisor2Mail);
            row.CreateCell(11).SetCellValue("");    // Modules undefined
            row.CreateCell(12).SetCellValue(p.Reservation1Mail);
            row.CreateCell(13).SetCellValue(p.Reservation2Mail);
            row.CreateCell(14).SetCellValue("");  // Comment undefined
            row.CreateCell(15).SetCellValue(p.Id);

            row.CreateCell(16).SetCellValue(p.DurationOneSemester ? 1 : 0);
            row.CreateCell(17).SetCellValue(p.IsContinuation ? 1 : 0);
            row.CreateCell(18).SetCellValue(p.LanguageGerman ? 1 : 0);
            row.CreateCell(19).SetCellValue(p.LanguageEnglish ? 1 : 0);
        }


        public static void GenerationMarketingList(Stream outStream, IEnumerable<Project> _projects)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(MARKETING_SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (int i = 0; i < MARKETING_HEADERS.Length; i++)
            {
                worksheet.GetRow(0).CreateCell(i).SetCellValue(MARKETING_HEADERS[i]);
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                ProjectToExcelMarketingRow(projects[i], row);
            }

            for (int i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }


        private static void ProjectToExcelMarketingRow(Project p, IRow row)
        {
            var db = new ProStudentCreatorDBDataContext();

            p.Semester = p.Semester == null ? Semester.NextSemester(db) : p.Semester = p.Semester;
            var abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester.ToString() + "_" + p.Department.DepartmentName + p.ProjectNr.ToString("D2");

            row.CreateCell(0).SetCellValue(abbreviation);
            row.CreateCell(1).SetCellValue(p.Department.DepartmentName);
            row.CreateCell(2).SetCellValue(p.Name);
            row.CreateCell(3).SetCellValue(GetStudentEmails(p));
            row.CreateCell(4).SetCellValue(p.Advisor1Mail);
            row.CreateCell(5).SetCellValue(p.LogProjectType?.ExportValue ?? "-");
            row.CreateCell(6).SetCellValue(p.ClientCompany ?? "");
            row.CreateCell(7).SetCellValue(p.ClientAddressTitle ?? "");
            row.CreateCell(8).SetCellValue(p.ClientPerson ?? "");
            row.CreateCell(9).SetCellValue(p.ClientAddressDepartment ?? "");
            row.CreateCell(10).SetCellValue(p.ClientAddressStreet ?? "");
            row.CreateCell(11).SetCellValue(p.ClientAddressPostcode ?? "");
            row.CreateCell(12).SetCellValue(p.ClientAddressCity ?? "");
            row.CreateCell(13).SetCellValue(p.ClientReferenceNumber ?? "");
        }

        private static string GetStudentEmails(Project p)
        {
            if (string.IsNullOrEmpty(p.LogStudent2Mail))
            {
                return p.LogStudent1Mail;
            }
            else
            {
                return p.LogStudent1Mail + ", " + p.LogStudent2Mail;
            }
        }
    }
}