using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public class PdfCreator
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        public void CreatePDF(Document document, MemoryStream output, bool multiPDF, int idPDF, HttpRequest currentRequest, List<int> gridViewProjectsId)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // the image we're using for the page header      
            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(currentRequest.MapPath("~/pictures/Logo.png"));

            // instantiate the custom PdfPageEventHelper
            MyPageEventHandler ef = new MyPageEventHandler()
            {
                ImageHeader = imageHeader
            };

            // and add it to the PdfWriter
            writer.PageEvent = ef;
            document.Open();
            int projectCounter = 0;
            var currentProjectType = "";
            Rectangle defaultPageSize = PageSize.A4;
            if (multiPDF)
            {
                Project currentProject;
                foreach (int projectId in gridViewProjectsId)
                {
                    currentProject = db.Projects.Single(item => item.Id == projectId);
                    projectCounter += 1;
                    WritePDF(currentProject, currentProjectType, document, defaultPageSize, projectCounter, currentRequest);
                }
            }
            else
            {
                projectCounter += 1;
                var singleProject = db.Projects.Single(item => item.Id == idPDF);
                WritePDF(singleProject, currentProjectType, document, defaultPageSize, projectCounter, currentRequest);
            }
        }

        private void WritePDF(Project currentProject, String currentProjectType, Document document, Rectangle defaultPageSize, int projectCounter, HttpRequest currentRequest)
        {
            var proj = currentProject;
            currentProjectType = getCurrentProjectTypeOne(proj);

            iTextSharp.text.Image projectTypeImage = iTextSharp.text.Image.GetInstance(currentRequest.MapPath("~/pictures/" + currentProjectType));
            projectTypeImage.SetAbsolutePosition(388, defaultPageSize.Height - document.TopMargin + 10);

            projectTypeImage.ScaleToFit(50f, 150f);
            document.Add(projectTypeImage);

            currentProjectType = getCurrentProjectTypeTwo(proj);
            projectTypeImage = iTextSharp.text.Image.GetInstance(currentRequest.MapPath("~/pictures/" + currentProjectType));
            projectTypeImage.SetAbsolutePosition(443, defaultPageSize.Height - document.TopMargin + 10);
            projectTypeImage.ScaleToFit(50f, 150f);
            document.Add(projectTypeImage);

            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.SpacingAfter = 8f;

            Paragraph title;

            if (projectCounter < 10)
            {
                title = new Paragraph(proj.Department + "0" + projectCounter + ": " + proj.Name, FontFactory.GetFont("Arial", 16, Font.BOLD));
            }
            else
            {

                title = new Paragraph(proj.Department + projectCounter + ": " + proj.Name, FontFactory.GetFont("Arial", 16, Font.BOLD));
            }

            title.SpacingBefore = 8f;
            tableTitle.DefaultCell.Border = Rectangle.NO_BORDER;
            title.Alignment = Element.ALIGN_JUSTIFIED;
            tableTitle.HorizontalAlignment = Element.ALIGN_LEFT;
            tableTitle.WidthPercentage = 100f;
            tableTitle.AddCell(title);
            document.Add(tableTitle);

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(currentRequest.MapPath("~/pictures/projectTypTransparent.png"));
            if (proj.Picture != null)
            {
                byte[] imageBytes = proj.Picture.ToArray();
                image = iTextSharp.text.Image.GetInstance(imageBytes);
                // http://stackoverflow.com/questions/9272777/auto-scaling-of-images
                // image.ScaleAbsoluteWidth(160f);
                float h = image.ScaledHeight;
                float w = image.ScaledWidth;
                image.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_RIGHT;
                //float scalePercent;

                float width = defaultPageSize.Width - document.RightMargin - document.LeftMargin;
                float height = defaultPageSize.Height - document.TopMargin - document.BottomMargin;

                if (w > h)
                {
                    image.ScaleToFit(250f, 150f);
                }
                else if (h >= 300 || w >= 200)
                {
                    image.ScaleToFit(150f, 250f);
                }

                else
                {
                    image.ScaleToFit(100f, 200f);
                }
                /*
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
            */
                // image.SetAbsolutePosition(388, defaultPageSize.Height - document.TopMargin - image.ScaledHeight);
            }

            PdfPTable projectTable = new PdfPTable(5);
            projectTable.SpacingAfter = 8f;
            projectTable.DefaultCell.Border = Rectangle.NO_BORDER;
            projectTable.HorizontalAlignment = Element.ALIGN_RIGHT;

            projectTable.WidthPercentage = 100f;
            float[] widthProject = new float[] { 22, 50, 25, 25, 25 };
            projectTable.SetWidths(widthProject);



            Paragraph text = new Paragraph();
            text.Alignment = Element.ALIGN_JUSTIFIED | Element.ALIGN_LEFT;
            text.SetLeading(1.0f, 2.0f);


            PdfPCell advisorCell;
            text = new Paragraph("BetreuerIn:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            advisorCell = new PdfPCell(text);
            advisorCell.Border = Rectangle.NO_BORDER;
            projectTable.AddCell(advisorCell);

            text = new Paragraph(proj.Advisor + ", " + proj.AdvisorMail, FontFactory.GetFont(FontFactory.HELVETICA, 10));
            projectTable.AddCell(text);

            projectTable.AddCell(" ");
            text = new Paragraph("Priorität 1", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            projectTable.AddCell(text);
            text = new Paragraph("Priorität 2", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            projectTable.AddCell(text);

            if (proj.Advisor2 != "")
            {
                projectTable.AddCell(" ");
                text = new Paragraph(proj.Advisor2 + ", " + proj.AdvisorMail2, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                projectTable.AddCell(text);
            }
            else
            {
                text = new Paragraph("Auftraggeber:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                projectTable.AddCell(text);

                text = new Paragraph(proj.Employer, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                projectTable.AddCell(text);
            }

            text = new Paragraph("Arbeitsumfang:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            projectTable.AddCell(text);
            text = new Paragraph(checkPriorityOne(proj), FontFactory.GetFont(FontFactory.HELVETICA, 10));
            projectTable.AddCell(text);
            text = new Paragraph(checkPriorityTwo(proj), FontFactory.GetFont(FontFactory.HELVETICA, 10));
            projectTable.AddCell(text);


            if (proj.Employer != "" && proj.Advisor2 != "")
            {
                text = new Paragraph("Auftraggeber:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                projectTable.AddCell(text);

                text = new Paragraph(proj.Employer, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                projectTable.AddCell(text);

            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    projectTable.AddCell(" ");
                }
            }

            text = new Paragraph("Teamgrösse:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
            projectTable.AddCell(text);
            text = new Paragraph(proj.POneTeamSize, FontFactory.GetFont(FontFactory.HELVETICA, 10));
            projectTable.AddCell(text);
            text = new Paragraph(proj.PTwoTeamSize, FontFactory.GetFont(FontFactory.HELVETICA, 10));
            projectTable.AddCell(text);

            document.Add(projectTable);

            if (proj.Picture != null)
            {
                document.Add(image);
            }

            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        if (proj.InitialPosition != "")
                        {
                            text = new Paragraph("Ausgangslage:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                            text.SetLeading(0.0f, 1.0f);
                            document.Add(text);

                            text = new Paragraph(proj.InitialPosition, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                            text.SetLeading(0, 1.0f);
                            text.Alignment = Element.ALIGN_JUSTIFIED;
                            text.IndentationRight = 10f;
                            document.Add(text);
                        }
                        break;
                    case 1:
                        if (proj.Objective != "")
                        {
                            text = new Paragraph("Ziel der Arbeit:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                            text.SetLeading(0.0f, 2.0f);
                            text.Add("");
                            document.Add(text);

                            text = new Paragraph(proj.Objective, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                            text.SetLeading(0, 1.0f);
                            text.Alignment = Element.ALIGN_JUSTIFIED;
                            text.IndentationRight = 10f;
                            document.Add(text);
                        }
                        break;
                    case 2:
                        if (proj.ProblemStatement != "")
                        {
                            text = new Paragraph("Problemstellung:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                            text.SetLeading(0.0f, 2.0f);
                            text.Add("");
                            document.Add(text);

                            text = new Paragraph(proj.ProblemStatement, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                            text.SetLeading(0, 1.0f);
                            text.Alignment = Element.ALIGN_JUSTIFIED;
                            text.IndentationRight = 10f;
                            document.Add(text);
                        }
                        break;
                    case 3:
                        if (proj.References != "")
                        {
                            text = new Paragraph("Technologien / Fachliche Schwerpunkte / Referenzen:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                            text.SetLeading(0.0f, 2.0f);
                            text.Add("");
                            document.Add(text);

                            text = new Paragraph(proj.References, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                            text.SetLeading(0, 1.0f);
                            text.Alignment = Element.ALIGN_JUSTIFIED;
                            text.IndentationRight = 10f;
                            document.Add(text);
                        }
                        break;
                    case 4:
                        if (proj.Remarks != "")
                        {
                            text = new Paragraph("Bemerkungen:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                            text.SetLeading(0.0f, 2.0f);
                            text.Add("");
                            document.Add(text);

                            text = new Paragraph(proj.Remarks, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                            text.SetLeading(0, 1.0f);
                            text.Alignment = Element.ALIGN_JUSTIFIED;
                            text.IndentationRight = 10f;
                            document.Add(text);
                        }
                        break;

                    /* CANCELLED PART!
                    case 5:
                    text = new Paragraph("Wichtigkeit:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                    document.Add(text);
                    if (proj.Importance)
                    {
                        text = new Paragraph("wichtig aus Sicht Institut oder FHNW", FontFactory.GetFont(FontFactory.HELVETICA, 10));
                    }
                    else
                    {
                        text = new Paragraph("Normal", FontFactory.GetFont(FontFactory.HELVETICA, 10));
                    }
                    document.Add(text);
                    break;
                     */
                    default:
                        break;
                }
            }
            if (proj.ReservationNameOne != "")
            {
                text = new Paragraph("Reservation:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10));
                text.SetLeading(0.0f, 2.0f);
                text.Add("");
                document.Add(text);

                if (proj.ReservationNameTwo != "")
                {
                    text = new Paragraph("Dieses Projekt ist für " + proj.ReservationNameOne + " und " + proj.ReservationNameTwo + " reserviert.", FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.RED));
                    text.SetLeading(0, 1.0f);
                }
                else
                {
                    text = new Paragraph("Dieses Projekt ist für " + proj.ReservationNameOne + " reserviert.", FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.RED));
                    text.SetLeading(0, 1.0f);
                }
                document.Add(text);
            }
            document.NewPage();
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
                head.DefaultCell.Border = Rectangle.NO_BORDER;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell c = new PdfPCell(ImageHeader, true);
                c.HorizontalAlignment = Element.ALIGN_MIDDLE;
                c.FixedHeight = cellHeight - 15;
                c.PaddingLeft = 35;
                c.PaddingTop = 5;
                c.PaddingBottom = 5;
                c.Border = PdfPCell.NO_BORDER;
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
                PdfPTable foot = new PdfPTable(1);
                foot.TotalWidth = page2.Width + 2;
                foot.DefaultCell.Border = Rectangle.NO_BORDER;

                var today = DateTime.Now;
                var projectSaison = "";
                if (today.Month >= 1 && today.Month <= 5)
                {
                    projectSaison = "HS" + today.ToString("yy");
                }
                else
                {
                    projectSaison = "FS" + today.AddYears(+1).ToString("yy");
                }

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell cell = new PdfPCell(new Phrase("Studiengang Informatik / i4DS / Studierendenprojekte " + projectSaison, new Font(Font.FontFamily.HELVETICA, 8)));
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = cellHeightFooter - 15;
                cell.PaddingLeft = 58;
                cell.PaddingTop = 8;
                cell.PaddingBottom = 0;
                cell.Border = PdfPCell.NO_BORDER;
                foot.AddCell(cell);
                foot.WriteSelectedRows(0, -1, -1, 40, writer.DirectContent);
            }
        }

        private String checkPriorityOne(Project proj)
        {
            var priorityOne = "";
            if (proj.POneP5 && proj.POneP6)
            {
                priorityOne = "P5 oder P6";

            }
            else if (proj.POneP5)
            {
                priorityOne = "P5";
            }
            else
            {
                priorityOne = "P6";
            }

            return priorityOne;
        }

        private String checkPriorityTwo(Project proj)
        {
            var priorityTwo = "";
            if (proj.PTwoP5 && proj.PTwoP6)
            {
                priorityTwo = "P5 oder P6";
            }
            else if (proj.PTwoP5)
            {
                priorityTwo = "P5";
            }
            else if (proj.PTwoP6)
            {
                priorityTwo = "P6";
            }
            else
            {
                priorityTwo = "------";

            }
            return priorityTwo;
        }

        private String getCurrentProjectTypeOne(Project proj)
        {
            var projectType = "";

            if (proj.TypeDesignUX)
            {
                projectType = "projectTypDesignUX.png";
            }
            else if (proj.TypeHW)
            {
                projectType = "projectTypHW.png";
            }
            else if (proj.TypeCGIP)
            {
                projectType = "projectTypCGIP.png";
            }
            else if (proj.TypeMathAlg)
            {
                projectType = "projectTypMathAlg.png";
            }
            else if (proj.TypeAppWeb)
            {
                projectType = "projectTypAppWeb.png";
            }
            else
            {
                projectType = "projectTypDBBigData.png";
            }

            return projectType;
        }

        private String getCurrentProjectTypeTwo(Project proj)
        {
            var projectType = "";


            if (proj.TypeHW && proj.TypeDesignUX)
            {
                projectType = "projectTypHW.png";
            }
            else if (proj.TypeCGIP && (proj.TypeDesignUX || proj.TypeHW))
            {
                projectType = "projectTypCGIP.png";
            }
            else if (proj.TypeMathAlg && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP))
            {
                projectType = "projectTypMathAlg.png";
            }
            else if (proj.TypeAppWeb && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg))
            {
                projectType = "projectTypAppWeb.png";
            }
            else if (proj.TypeDBBigData && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg || proj.TypeAppWeb))
            {
                projectType = "projectTypDBBigData.png";
            }
            else
            {
                projectType = "projectTypTransparent.png";
            }

            return projectType;
        }
        public int getNumberOfPDFPages(int idPDF, HttpRequest currentRequest)
        {
            float margin = Utilities.MillimetersToPoints(Convert.ToSingle(20));
            int numberOfPages;
            using (var output = new MemoryStream())
            {
                using (var document = new Document(iTextSharp.text.PageSize.A4, margin, margin, margin, margin))
                {
                    CreatePDF(document, output, false, idPDF, currentRequest, null);
                }
                PdfReader pdfReader = new PdfReader(output.ToArray());
                numberOfPages = pdfReader.NumberOfPages;
            }
            return numberOfPages;
        }
    }
}