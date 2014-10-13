using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProStudCreator
{

    public class Peter
    {
        public int id { get; set; }
        public string advisorName { get; set; }
        public string advisorEmail { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
    }


    public partial class projectlist : System.Web.UI.Page
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.Name == "test@testEmail.ch")
            {
                AdminView.Visible = true;
                AdminViewPDF.Visible = true;

                /*
                 * SELECT * FROM Projects p INNER JOIN AspNetUsers u ON u.EMail==p.AdvisorMail WHERE u.EmailConfirmed = 1;
                 * db.Projects.Join(db.AspNetUsers, p => p.AdvisorMail, u => u.Email, (p, u) => Tuple.Create(p, u)).Where(both => both.Item2.EmailConfirmed);
                 * projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png"
                 * i.POne + " " + i.PTwo,
                 */

                CheckProjects.DataSource = db.Projects.Where(item => !item.Published).Select(i => new Peter()
                {
                    id = i.Id,
                    advisorName = i.Advisor + " " + i.Advisor2,
                    advisorEmail = i.AdvisorMail + " " + i.AdvisorMail2,
                    projectName = i.Name,
                    p5 = (i.POneP5 ? true : false || i.PTwoP5 ? true : false),
                    p6 = (i.POneP6 ? true : false || i.PTwoP6 ? true : false),
                    projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png",
                    projectType2 = "pictures/projectTyp" +
                    ((i.TypeHW && i.TypeDesignUX) ? "HW" :
                    (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" :
                    (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" :
                    (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" :
                    (i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb) ? "DBBigData" : "Transparent")) + ".png"
                });

                AllProjects.DataSource = db.Projects.Select(i => new Peter()
                {
                    id = i.Id,
                    advisorName = i.Advisor + " " + i.Advisor2,
                    advisorEmail = i.AdvisorMail + " " + i.AdvisorMail2,
                    projectName = i.Name,
                    p5 = (i.POneP5 ? true : false || i.PTwoP5 ? true : false),
                    p6 = (i.POneP6 ? true : false || i.PTwoP6 ? true : false),
                    projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png",
                    projectType2 = "pictures/projectTyp" +
                    ((i.TypeHW && i.TypeDesignUX) ? "HW" :
                    (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" :
                    (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" :
                    (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" :
                    (i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb) ? "DBBigData" : "Transparent")) + ".png"
                });
            }
            else
            {
                /* var projects = db.Projects.Select(i => new Peter() */
                // p = i,
                AllProjects.DataSource = db.Projects.Where(item => item.Creator == User.Identity.Name).Select(i => new Peter()
                {
                    id = i.Id,
                    advisorName = i.Advisor + " " + i.Advisor2,
                    advisorEmail = i.AdvisorMail + " " + i.AdvisorMail2,
                    projectName = i.Name,
                    p5 = (i.POneP5 ? true : false || i.PTwoP5 ? true : false),
                    p6 = (i.POneP6 ? true : false || i.PTwoP6 ? true : false),
                    projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : i.TypeMathAlg ? "MathAlg" : i.TypeAppWeb ? "AppWeb" : "DBBigData"))) + ".png",
                    projectType2 = "pictures/projectTyp" +
                    ((i.TypeHW && i.TypeDesignUX) ? "HW" :
                    (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" :
                    (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" :
                    (i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" :
                    (i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb) ? "DBBigData" : "Transparent")) + ".png"
                });
                //.ToList();

                /*
                projects.ForEach(updateProject);
            
                AllProjects.DataSource = projects;
                */
            }

            CheckProjects.DataBind();
            AllProjects.DataBind();
        }
        /*
        private void updateProject(Peter p)
        {
            if(p.p.)
            {
                p.Image1 = sadpaosdipaosdi

            }
            else
            {
                p.Image2 = asdalsduaosdiu
            }
        }
        */

        protected void newProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("/AddNewProject");
        }


        protected void CheckProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "showProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument + "&show=true");
                    break;
                case "editProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument);
                    break;
                case "deleteProject":
                    var id = Convert.ToInt32(e.CommandArgument);
                    var details = db.Projects.Where(item => item.Id == id);
                    db.Projects.DeleteAllOnSubmit(details);
                    db.SubmitChanges();
                    CheckProjects.DataBind();
                    Response.Redirect(Request.RawUrl);
                    break;
                default:
                    break;
            }
        }
        protected void AllProjectsEvent(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "showProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument + "&show=true");
                    break;
                case "editProject":
                    Response.Redirect("/AddNewProject?id=" + e.CommandArgument);
                    break;
                case "deleteProject":
                    var id = Convert.ToInt32(e.CommandArgument);
                    var details = db.Projects.Where(item => item.Id == id);
                    db.Projects.DeleteAllOnSubmit(details);
                    db.SubmitChanges();
                    AllProjects.DataBind();
                    Response.Redirect(Request.RawUrl);
                    break;
                default:
                    break;
            }
        }

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            float margin = Utilities.MillimetersToPoints(Convert.ToSingle(20));

            using (var output = new FileStream("C:/Test.pdf", FileMode.Create))
            {
                using (var document = new Document(iTextSharp.text.PageSize.A4, margin, margin, margin, margin))
                {
                    //var output = new MemoryStream();

                    PdfWriter writer = PdfWriter.GetInstance(document, output);

                    // the image we're using for the page header      
                    iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(Request.MapPath("~/pictures/Logo.png"));

                    // instantiate the custom PdfPageEventHelper
                    MyPageEventHandler ef = new MyPageEventHandler()
                    {
                        ImageHeader = imageHeader
                    };
                    // and add it to the PdfWriter
                    writer.PageEvent = ef;
                    document.Open();

                    // finally we add some pages with text to show the headers are working
                    string text = "Test";
                    for (int k = 0; k < 10; ++k)
                    {
                        text += " " + text;
                        Paragraph p = new Paragraph(text);
                        p.SpacingBefore = 8f;
                        document.Add(p);
                    }
                }
            }

            

            //var pdfDokument=output.ToArray();

            /*
             
            ///////////////////////////////////////////////////////
            // CREATE PDF (OTHER WAY) 
            //////////////////////////////////////////////////////
            
            // Create a Document object
            // var doc = new Document(iTextSharp.text.PageSize.A4, margin, margin, margin, margin);
            var doc = new Document(iTextSharp.text.PageSize.A4, 50, 50, 25, 25);

            // Create a new PdfWriter object, specifying the output stream
            // var output = new FileStream(Server.MapPath("MyFirstPDF.pdf"), FileMode.Create);
            // var output = new MemoryStream();
            var output = new FileStream("C:/Test.pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(doc, output);

            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);


            // Open the Document for writing
            doc.Open();

            doc.Add(new Paragraph("This is a custom size"));
            // var logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/pictures/Logo.png"));
            // logo.ScaleAbsolute(159f, 159f);
            // logo.SetAbsolutePosition(20, 790);
            // doc.Add(logo);
            doc.AddHeader("Header", "HeaderText");

            // Close the Document - this saves the document contents to the output stream
            doc.Close();

            writer.SetFullCompression();
            writer.CloseStream = true;
            /*
             
          
            ///////////////////////////////////////////////////////
            // SAVE AS PROMPT 
            //////////////////////////////////////////////////////
            /*
            string FilePath = "C:/";
            string FileName = "i4DsFS14Projekte.pdf";

            // Creates the file on server
            File.WriteAllText(FilePath + FileName, "hello");

            // Prompts user to save file
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/pdf";
            response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
            response.TransmitFile(FilePath + FileName);
            response.Flush();

            // Deletes the file on server
            File.Delete(FilePath + FileName);

            response.End();
            */
        }


        private class MyPageEventHandler : PdfPageEventHelper
        {
            /*
             * We use a __single__ Image instance that's assigned __once__;
             * the image bytes added **ONCE** to the PDF file. If you create 
             * separate Image instances in OnEndPage()/OnEndPage(), for example,
             * you'll end up with a much bigger file size.
             */
            public iTextSharp.text.Image ImageHeader { get; set; }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                ///////////////////////////////////////////////////////
                // HEADER 
                //////////////////////////////////////////////////////

                // cell height 
                float cellHeight = document.TopMargin;
                // PDF document size      
                Rectangle page = document.PageSize;

                // create two column table
                PdfPTable head = new PdfPTable(1);
                head.TotalWidth = page.Width;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell c = new PdfPCell(ImageHeader, true);
                c.HorizontalAlignment = Element.ALIGN_MIDDLE;
                c.FixedHeight = cellHeight - 15;
                c.PaddingLeft = 35;
                c.PaddingTop = 8;
                c.PaddingBottom = 5;
                c.Border = PdfPCell.ALIGN_BOTTOM;
                head.AddCell(c);

                /*
                // add the header text
                c = new PdfPCell(new Phrase(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " GMT", new Font(Font.FontFamily.COURIER, 8)));
                c.Border = PdfPCell.ALIGN_BOTTOM;
                c.VerticalAlignment = Element.ALIGN_BOTTOM;
                c.FixedHeight = cellHeight;
                head.AddCell(c);
                */
                // since the table header is implemented using a PdfPTable, we call
                // WriteSelectedRows(), which requires absolute positions!
                head.WriteSelectedRows(
                  0, -1,  // first/last row; -1 flags all write all rows
                  0,      // left offset
                    // ** bottom** yPos of the table
                  page.Height - cellHeight + head.TotalHeight,
                  writer.DirectContent
                );

                ///////////////////////////////////////////////////////
                // FOOTER 
                //////////////////////////////////////////////////////

                DateTime today = DateTime.Now;
                String footerText = "Studiengang Informatik / i4Ds / Studierendenprojekte HS" + today.ToString("yy");
                Paragraph footer = new Paragraph(footerText, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL));
                // footer.Alignment = Element.ALIGN_RIGHT;

                PdfPTable footerTbl = new PdfPTable(2);
                footerTbl.TotalWidth = page.Width;

                PdfPCell cell = new PdfPCell(footer);
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = cellHeight - 15;
                cell.PaddingLeft = 35;
                cell.PaddingTop = 8;
                cell.PaddingBottom = 5;
                cell.Border = PdfPCell.ALIGN_TOP;
                footerTbl.AddCell(cell);

                String footerText2 = today.ToString("MMMM yyyy");
                Paragraph footer2 = new Paragraph(footerText2, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL));
                cell = new PdfPCell(footer2);
                cell.Border = PdfPCell.ALIGN_TOP;
                cell.VerticalAlignment = Element.ALIGN_BOTTOM;

                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 30, writer.DirectContent);
            }
        }
    }
}