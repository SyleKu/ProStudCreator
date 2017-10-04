using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public static class TaskExtensions
    {
        private static bool addedToList = false;

        public static void AddToList(this Task _t)
        {
            addedToList = true;
        }

        public static void RemoveFromList(this Task _t)
        {
            addedToList = false;
        }

        public static bool isAddedToList(this Task _t)
        {
            return addedToList;
        }
    }
}