using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ProStudCreator
{
    public partial class Version
    {
        public String dateString;

        public String state
        {
            
            get
            {
                var s = Projects.Single(p => p.Id == p_id).State;
                switch (s)
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

        public void InitNew(int pid)
        {
            date = DateTime.Now;
            p_id = pid;
            dateString = date.ToString();
        }


    }
}