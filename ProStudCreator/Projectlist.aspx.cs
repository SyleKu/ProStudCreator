using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;

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
                    // var output = new MemoryStream();

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


                    int id = 11;
                    var proj = db.Projects.Single(i => i.Id == id);
                    Paragraph text;

                    Paragraph title = new Paragraph("i4Ds: " + proj.Name, FontFactory.GetFont("Arial", 18, Font.BOLD));
                    title.SpacingBefore = 8f;
                    title.SpacingAfter = 8f;
                    document.Add(title);

                    byte[] imageBytes = proj.Picture.ToArray();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                    // http://stackoverflow.com/questions/9272777/auto-scaling-of-images
                    // image.ScaleAbsoluteWidth(160f);
                    float h = image.ScaledHeight;
                    float w = image.ScaledWidth;
                    image.Alignment = 6;
                    float scalePercent;

                    Rectangle defaultPageSize = PageSize.A4;
                    float width = defaultPageSize.Width - document.RightMargin - document.LeftMargin;
                    float height = defaultPageSize.Height - document.TopMargin - document.BottomMargin;

                    if (h >= 300 || w >= 200)
                    {
                        image.ScaleToFit(200f, 300f);
                    }

                    else if (h > w)
                    {
                        // only scale image if it's height is __greater__ than
                        // the document's height, accounting for margins
                        if (h > height)
                        {
                            scalePercent = height / h;
                            image.ScaleAbsolute(w * scalePercent, h * scalePercent);
                        }
                    }
                    else
                    {
                        // same for image width        
                        if (w > width)
                        {
                            scalePercent = width / w;
                            image.ScaleAbsolute(w * scalePercent, h * scalePercent);
                        }
                    }

                    //image.SetAbsolutePosition(380, defaultPageSize.Height - document.TopMargin - image.ScaledHeight - 35);

                    //document.Add(image);
                    PdfPTable cellProject = new PdfPTable(3);
                    cellProject.DefaultCell.Border = Rectangle.NO_BORDER;
                    cellProject.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellProject.WidthPercentage = 100f;
                    float[] widthProject = new float[] { 20, 40, 30 };
                    cellProject.SetWidths(widthProject);

                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;
                    text = new Paragraph("Betreuer:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested.AddCell(text);
                    text = new Paragraph("Betreuer 2:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested.AddCell(text);
                    text = new Paragraph("Auftraggeber:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested.AddCell(text);
                    nested.AddCell(" ");
                    nested.AddCell(" ");
                    text = new Paragraph("Arbeitsumfang:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested.AddCell(text);
                    text = new Paragraph("Teamgrösse:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested.AddCell(text);
                    PdfPCell nesthousing = new PdfPCell(nested);
                    nesthousing.Border = Rectangle.NO_BORDER;
                    nesthousing.Padding = 0f;
                    cellProject.AddCell(nesthousing);

                    PdfPTable nested2 = new PdfPTable(1);
                    nested2.DefaultCell.Border = Rectangle.NO_BORDER;
                    text = new Paragraph("Name des Betreuers", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2.AddCell(text);
                    text = new Paragraph("Name des Betreuers2", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2.AddCell(text);
                    text = new Paragraph("Auftraggeber XYZ", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2.AddCell(text);
                    nested2.AddCell(" "); 

                    PdfPTable nested2A = new PdfPTable(2);
                    nested2A.DefaultCell.Border = Rectangle.NO_BORDER;                    
                    text = new Paragraph("Priorität 1", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested2A.AddCell(text);
                    text = new Paragraph("Priorität 2", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    nested2A.AddCell(text);

                    text = new Paragraph("P5 & P6", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2A.AddCell(text);
                    text = new Paragraph("P5 & P6 2", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2A.AddCell(text);

                    text = new Paragraph("Einzelarbeit", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2A.AddCell(text);
                    text = new Paragraph("1er oder 2er Team", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                    nested2A.AddCell(text);
                    nested2.AddCell(nested2A);

                    PdfPCell nesthousing2 = new PdfPCell(nested2);
                    nesthousing2.Border = Rectangle.NO_BORDER;
                    nesthousing2.Padding = 0f;
                    cellProject.AddCell(nesthousing2);

                    PdfPTable nested3 = new PdfPTable(1);
                    nested3.DefaultCell.Border = Rectangle.NO_BORDER;
                    PdfPCell cell = new PdfPCell(image);
                    cell.Border = Rectangle.NO_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    nested3.AddCell(cell);
                    PdfPCell nesthousing3 = new PdfPCell(nested3);
                    nesthousing3.Border = Rectangle.NO_BORDER;
                    nesthousing3.Padding = 0f;
                    cellProject.AddCell(nesthousing3);

                    document.Add(cellProject);
                    cellProject.SpacingAfter = 8f;

                    for (int i = 0; i < 6; i++)
                    {
                        PdfPTable table = new PdfPTable(2);
                        table.DefaultCell.Border = Rectangle.NO_BORDER;
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.WidthPercentage = 100f;
                        float[] widthsContent = new float[] { 26, 94 };
                        table.SetWidths(widthsContent);
                        PdfPCell cellContent = new PdfPCell();
                        cellContent.Border = Rectangle.NO_BORDER;
                        cellContent.Colspan = 2;
                        cellContent.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                        table.AddCell(cellContent);
                        table.SpacingBefore = 8f;
                        switch (i)
                        {
                            case 0:
                                text = new Paragraph("Ausgangslage:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                text = new Paragraph(proj.InitialPosition, FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                text.SetLeading(0.0f, 2.0f);
                                table.AddCell(text);
                                break;
                            case 1:
                                text = new Paragraph("Ziel der Arbeit:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                text = new Paragraph(proj.Objective, FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                text.SetLeading(0.0f, 2.0f);
                                table.AddCell(text);
                                break;
                            case 2:
                                text = new Paragraph("Problemstellung:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                text = new Paragraph(proj.ProblemStatement, FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                text.SetLeading(0.0f, 2.0f);
                                table.AddCell(text);
                                break;
                            case 3:
                                text = new Paragraph("Technologien / Fachliche Schwerpunkte / Referenzen:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                text = new Paragraph(proj.References, FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                text.SetLeading(0.0f, 2.0f);
                                table.AddCell(text);
                                break;
                            case 4:
                                text = new Paragraph("Bemerkungen:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                text = new Paragraph(proj.Remarks, FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                text.SetLeading(0.0f, 2.0f);
                                table.AddCell(text);
                                break;
                            case 5:
                                text = new Paragraph("Wichtigkeit:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                                table.AddCell(text);
                                if (proj.Importance)
                                {
                                    text = new Paragraph("wichtig aus Sicht Institut oder FHNW", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                }
                                else
                                {
                                    text = new Paragraph("Normal", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                                }
                                table.AddCell(text);
                                break;
                            default:
                                break;
                        }
                        document.Add(table);
                    }


                    /*
                    // finally we add some pages with text to show the headers are working
                    string text = "Test";
                    for (int k = 0; k < 10; ++k)
                    {
                        text += " " + text;
                        Paragraph p = new Paragraph(text);
                        p.SpacingBefore = 8f;
                        document.Add(p);
                    }
                    */
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
                head.TotalWidth = page.Width + 2;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell c = new PdfPCell(ImageHeader, true);
                c.HorizontalAlignment = Element.ALIGN_MIDDLE;
                c.FixedHeight = cellHeight - 15;
                c.PaddingLeft = 35;
                c.PaddingTop = 5;
                c.PaddingBottom = 5;
                c.Border = PdfPCell.ALIGN_BOTTOM;
                head.AddCell(c);

                // since the table header is implemented using a PdfPTable, we call
                // WriteSelectedRows(), which requires absolute positions!
                head.WriteSelectedRows(
                  0, -1,  // first/last row; -1 flags all write all rows
                  -1,      // left offset
                    // ** bottom** yPos of the table
                  page.Height - cellHeight + head.TotalHeight,
                  writer.DirectContent
                );

                ///////////////////////////////////////////////////////
                // FOOTER 
                //////////////////////////////////////////////////////

                // cell height 
                float cellHeightFooter = document.TopMargin;
                // PDF document size      
                Rectangle page2 = document.PageSize;

                // create two column table
                PdfPTable head2 = new PdfPTable(1);
                head2.TotalWidth = page2.Width + 2;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell cell = new PdfPCell(new Phrase("Studiengang Informatik / i4DS / Studierendenprojekte HS14", new Font(Font.FontFamily.HELVETICA, 8)));
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = cellHeightFooter - 15;
                cell.PaddingLeft = 58;
                cell.PaddingTop = 8;
                cell.PaddingBottom = 0;
                head2.AddCell(cell);
                head2.WriteSelectedRows(0, -1, -1, 40, writer.DirectContent);
            }
        }
    }
}