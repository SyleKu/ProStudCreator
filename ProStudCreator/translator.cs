using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public class Translator
    {
        public readonly static string ENGLISCH = "Englisch";
        public readonly static string DEUTSCH = "Deutsch";
        public readonly static string DEUTSCHENGLISCH = "Englisch";

        public string language { get; set; }

        public Translator()
        {
            language = ENGLISCH;
        }

        public Translator(string language)
        {
            this.language = language;
        }

          public string getHeadingInitialPosition()
        {
            if(language == ENGLISCH)
                return "Initial position";
            return "Ausgangslage";
        }
        public string getHeadingObjective()
        {
            if (language == ENGLISCH)
                return "Objective";
            return "Ziel der Arbeit";
        }
        public string getHeadingProblemStatement()
        {
            if(language == ENGLISCH)
                return "Problem statement";
            return "Problemstellung";
        }

        public string getHeadingAnnotation()
        {
            if (language == ENGLISCH)
                return "Note";
            return "Bemerkung";
        }
        public string getHeadingTechnology()
        {
            if (language == ENGLISCH)
                return "Technologies/Technical emphasis/References";
            return "Technologien/Fachliche Schwerpunkte/Referenzen";
        }
        public  string getHeadingOneSemester()
        {
            if(language == ENGLISCH)
                return "This project must be realised in a single semester.\n";
            return "Dieses Projekt muss in einem einzigen Semester durchgeführt werden.\n";
        }

        public string getReservedString(string Reservation1Name, string Reservation2Name)
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
        public string getHeadingAdvisor()
        {
            if(language == ENGLISCH)
                return "Advisor";
            return "Betreuer";
        }
        public string getHeadingPriority()
        {
            if(language == ENGLISCH)
                return "Priority";
            return "Priorität";
        }
        public string getHeadingClient()
        {
            if(language == ENGLISCH)
                return "Client";
            return "Auftragsgeber";
        }
        public string getHeadingWorkScope()
        {
            if (language == ENGLISCH)
                return "Work scope";
            return "Arbeitsumfang";
        }
        public string getHeadingTeamSize()
        {
            if (language == ENGLISCH)
                return "Team size";
            return "Teamgrösse";
        }
        public string getHeadingLangugages()
        {
            if (language == ENGLISCH)
                return "Languages";
            return "Sprachen";
        }

        public string getHeadingFooter(Project CurrentProject, ProStudentCreatorDBDataContext db)
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