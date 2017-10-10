using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ProStudCreator
{
    public partial class Project { 

        public String StateAsString
        {

            get
            {
                switch (State)
                {
                    case ProjectState.InProgress:
                        return "In Bearbeitung";
                    case ProjectState.Published:
                        return "Veröffentlicht";
                    case ProjectState.Rejected:
                        return "Abgelehnt";
                    case ProjectState.Submitted:
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