using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace ProStudCreator
{
    public partial class ProjectFilesDownload : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var fileName = Request.QueryString["fname"];

            var attach = db.Attachements.Single(i => i.ProjectId == id && i.FileName == fileName && !i.Deleted);

            if (!ShibUser.IsAuthenticated(db))
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
            Response.AddHeader("Content-Lenght", attach.UploadSize.ToString());

            using (var connection = new SqlConnection(db.Connection.ConnectionString))
            {
                connection.Open();
                var command =
                    new SqlCommand(
                        $"SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ROWGUID = @ROWGUID;",
                        connection);
                command.Parameters.AddWithValue("@ROWGUID", attach.ROWGUID.ToString());
                using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    command.Transaction = tran;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the pointer for the file  
                            var path = reader.GetString(0);
                            var transactionContext = reader.GetSqlBytes(1).Buffer;

                            // Create the SqlFileStream  
                            using (
                                Stream fileStream = new SqlFileStream(path, transactionContext, FileAccess.Read,
                                    FileOptions.SequentialScan, 0))
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