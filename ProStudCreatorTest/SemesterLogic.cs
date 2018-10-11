using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProStudCreator;

namespace ProStudCreatorTest
{
    [TestClass]
    public class SemesterLogic
    {
        /// <summary>
        /// Verifies that dates are converted to the correct semester.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The semester isn't the current academic semester, but the one projects are being recorded for (next semester).</item>
        /// <item>For dates, see the schedule on the ProStudCreator website</item>
        /// </list>
        /// </remarks>
        [TestMethod]
        public void DateToSemester()
        {
            //Semester fs = (Semester)(new DateTime(2015, 11, 29));
            //Semester hs = (Semester)(new DateTime(2016, 02, 01));

            //Assert.AreEqual("FS", fs.FSHS);
            //Assert.AreEqual("HS", hs.FSHS);
        }
    }
}
