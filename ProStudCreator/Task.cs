using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{

    public enum TaskType
    {
        RemindOnce, RemindUntilDone
    }

    public partial class Task
    {

        private static readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        public static void AddTasks(Task[] tasks)
        {
            foreach (var task in tasks)
            {
                db.Tasks.InsertOnSubmit(task);
            }
        }
    }
}