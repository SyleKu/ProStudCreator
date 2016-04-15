using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using ProStudCreator;
using Moq;

namespace ProStudCreatorTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class StateTransitions
    {

        public StateTransitions()
        {
            // Init test project
            //project = new Project();

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        //[DataSource(@"Provider=System.Data.SqlClient; Data Source=(LocalDb)\v11.0;AttachDbFilename=App_Data\aspnet-ProStudCreator-20140818043155.mdf;Initial Catalog=aspnet-ProStudCreator-20140818043155;Integrated Security=True", "projects")]
        [TestMethod]
        public void Submitted_valid_transitions()
        {
            // NOTE: Cannot mock ShibUser, as it's static. Hmm!
            //Mock user = new Mock<ShibUser>();

            Project p;
            
            p = new Project();
            p.State = ProjectState.Submitted;
            p.Publish();    // Should be admin
            Assert.AreEqual(ProjectState.Published, p.State);

            p = new Project();
            p.State = ProjectState.Submitted;
            p.Reject();     // Should be admin
            Assert.AreEqual(ProjectState.Rejected, p.State);

            p = new Project();
            p.State = ProjectState.Submitted;
            p.Unsubmit();
            Assert.AreEqual(ProjectState.InProgress, p.State);

            p = new Project();
            p.State = ProjectState.Submitted;
            p.Delete();
            Assert.AreEqual(ProjectState.Deleted, p.State);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Submitted_unauthorized_publish()
        {
            // Need to mock ShibUser -> return "not admin"

            Project p;
            p = new Project();
            p.State = ProjectState.Submitted;
            p.Publish();
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Submitted_unauthorized_reject()
        {
            // Need to mock ShibUser -> return "not admin"

            Project p;
            p = new Project();
            p.State = ProjectState.Submitted;
            p.Reject();
        }

        [TestMethod]
        public void Published_valid_transitions()
        {
            // NOTE: Cannot mock ShibUser, as it's static. Hmm!
            //Mock user = new Mock<ShibUser>();

            Project p;

            p = new Project();
            p.State = ProjectState.Published;
            p.Unpublish();    // Should be admin
            Assert.AreEqual(ProjectState.Submitted, p.State);

            p = new Project();
            p.State = ProjectState.Published;
            p.Reject();     // Should be admin
            Assert.AreEqual(ProjectState.Rejected, p.State);
        }

    }
}
