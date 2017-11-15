using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ProStudCreator
{
    public partial class Project {
        public String StateColor { get; set; }
        public String StateAsString
        {

            get
            {
                switch (State)
                {
                    case ProjectState.InProgress:
                        StateColor = "#EFF3FB";
                        return "In Bearbeitung";

                    case ProjectState.Published:
                        StateColor = "#A9F5A9";
                        return "Veröffentlicht";

                    case ProjectState.Rejected:
                        StateColor = "#F5A9A9";
                        return "Abgelehnt";

                    case ProjectState.Submitted:
                        StateColor = "#ffcc99";
                        return "Eingereicht";

                    case ProjectState.Deleted:
                        return "Gelöscht";

                    default:
                        return "Error!";

                }


            }
        }

    }
}