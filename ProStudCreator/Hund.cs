using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProStudCreator
{
    public class Hund:Tier
    {
        void Bellen()
        {
            Console.WriteLine("Wuff");
            Name = "fifi";

            Sterbe();
        }
    }
}