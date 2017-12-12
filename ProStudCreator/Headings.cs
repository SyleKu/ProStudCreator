using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public class Headings
    {
        public readonly static string ENGLISCH = "Englisch";
        public readonly static string DEUTSCH = "Deutsch";
        public readonly static string DEUTSCHENGLISCH = "Deutsch oder Englisch";

          public static string getHeadingInitialPosition(string language)
        {
            if(language == ENGLISCH)
                return "Initial position";
            return "Ausgangslage";
        }
        public static string getHeadingObjective(string language)
        {
            if (language == ENGLISCH)
                return "Objective";
            return "Ziel der Arbeit";
        }
        public static string getHeadingProblemStatement(string language)
        {
            if(language == ENGLISCH)
                return "Problem statement";
            return "Problemstellung";
        }

        public static string getHeadingAnnotation(string language)
        {
            if (language == ENGLISCH)
                return "Note";
            return "Bemerkung";
        }
        public static string getHeadingTechnology(string language)
        {
            if (language == ENGLISCH)
                return "Technologies/Technical emphasis/References";
            return "Technologien/Fachliche Schwerpunkte/Referenzen";
        }
        public static string getHeadingOneSemester(string language)
        {
            if(language == ENGLISCH)
                return "This project must be realised in a single semester.\n";
            return "Dieses Projekt muss in einem einzigen Semester durchgeführt werden.\n";
        }

        public static string getReservedString(string language, string Reservation1Name, string Reservation2Name)
        {
            var strReservations = "";
            if(language == ENGLISCH)
            {
                strReservations = "This Project is reserved for " + Reservation1Name;
                if (Reservation2Name != "") strReservations += " and " + Reservation2Name;
                return strReservations;
            }

            strReservations = "Dieses Projekt ist für " + Reservation1Name;
            if (Reservation2Name != "") strReservations += " und " + Reservation2Name;
            strReservations += " reserviert.\n";

            return strReservations;
        }
        public static string getHeadingAdvisor(string Language)
        {
            if(Language == ENGLISCH)
                return "Advisor";
            return "Betreuer";
        }
        public static string getHeadingPriority(string language)
        {
            if(language == ENGLISCH)
                return "Priority";
            return "Priorität";
        }
        public static string getHeadingClient(string language)
        {
            if(language == ENGLISCH)
                return "Client";
            return "Auftragsgeber";
        }
        public static string getHeadingWorkScope(string language)
        {
            if (language == ENGLISCH)
                return "Work scope";
            return "Arbeitsumfang";
        }
        public static string getHeadingTeamSize(string language)
        {
            if (language == ENGLISCH)
                return "Team size";
            return "Teamgrösse";
        }
        public static string getHeadingLangugages(string language)
        {
            if (language == ENGLISCH)
                return "Languages";
            return "Sprachen";
        }

        public static string getHeadingFooter(Project CurrentProject, ProStudentCreatorDBDataContext db)
        {
            var foot = "";
            if (CurrentProject.LanguageEnglish)
            {
                foot += " Computer Science/" + CurrentProject.Department.DepartmentName + "/Student projects " +
                    CurrentProject?.Semester?.Name ?? Semester.NextSemester(db).Name;
                return foot;
            }
            foot += "Studiengang Informatik/" + CurrentProject.Department.DepartmentName + "/Studierendenprojekte " +
                    CurrentProject?.Semester?.Name ?? Semester.NextSemester(db).Name;
            return foot;
        }



    }
}