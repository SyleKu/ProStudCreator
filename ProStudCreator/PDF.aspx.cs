﻿using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace ProStudCreator
{
    public partial class PDF : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var forceDl = bool.Parse(Request.QueryString["dl"]);

            var idPDF = db.Projects.Single(i => i.Id == id);

            if (!(ShibUser.IsAuthenticated(db) || idPDF.State == ProjectState.Published))
            {
                //throw new HttpException(403, "Nicht berechtigt");
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
                return;
            }
            if (!idPDF.IsMainVersion)
            {
                var mainProject = db.Projects.Single(p => p.ProjectId == idPDF.ProjectId && p.IsMainVersion);
                Response.Redirect(@"~/PDF?dl=" + forceDl.ToString() + "&id=" + mainProject.Id.ToString());
            }
            

            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                var document = PdfCreator.CreateDocument();
                try
                {
                    var pdfCreator = new PdfCreator();
                    pdfCreator.AppendToPDF(document, output, Enumerable.Repeat(idPDF, 1));
                    document.Dispose();
                }
                catch
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch
                    {
                    }
                    throw;
                }

                bytesInStream = output.ToArray();
            }
            Response.Clear();

            if (forceDl)
            {
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition",
                    "attachment; filename=" + idPDF.Department.DepartmentName + idPDF.ProjectNr.ToString("00") +
                    ".pdf");
            }
            else
            {
                Response.ContentType = "application/pdf";
            }
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }
    }
}