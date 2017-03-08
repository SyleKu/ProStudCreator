using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ProStudCreator
{
    public partial class ProjectFilesDownload : System.Web.UI.Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var fileName = Request.QueryString["fname"];

            var attach = db.Attachements.Single(i => i.ProjectId == id && i.FileName == fileName && !i.Deleted);

            if (!ShibUser.IsAuthenticated())
            {
                //throw new HttpException(403, "Nicht berechtigt");
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
                return;
            }
            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=\"" + attach.FileName + "\"");

            using (SqlConnection connection = new SqlConnection("Data Source=FLAVIOLAPTOP;Initial Catalog=aspnet-ProStudCreator-20140818043155;Integrated Security=True"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ROWGUID = @ROWGUID;", connection);
                command.Parameters.AddWithValue("@ROWGUID", attach.ROWGUID.ToString());
                using (SqlTransaction tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    command.Transaction = tran;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the pointer for the file  
                            string path = reader.GetString(0);
                            byte[] transactionContext = reader.GetSqlBytes(1).Buffer;

                            // Create the SqlFileStream  
                            using (
                                Stream fileStream = new SqlFileStream(path, transactionContext, FileAccess.Read,
                                    FileOptions.SequentialScan, allocationSize: 0))
                            {
                                fileStream.CopyTo(Response.OutputStream);
                                Response.Flush();
                            }
                        }
                    }

                    tran.Commit();
                }
            }
            Response.End();
        }
    }
}