using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf.hyphenation;

namespace ProStudCreator
{
    public class PdfCreator
    {
        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        HyphenationAuto hyph = new HyphenationAuto("de", "none", 2, 2);

        public void AppendToPDF(Document document, MemoryStream output, IEnumerable<int> projectIds)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // the image we're using for the page header      
            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(HttpContext.Current.Request.MapPath("~/pictures/Logo.png"));

            // instantiate the custom PdfPageEventHelper
            var ef = new MyPageEventHandler()
            {
                ImageHeader = imageHeader
            };

            // and add it to the PdfWriter
            writer.PageEvent = ef;
            document.Open();
            foreach (int projectId in projectIds)
            {
                var proj = db.Projects.Single(item => item.Id == projectId);
                ef.CurrentProject = proj;
                WritePDF(proj, document);
            }
        }

        private void WritePDF(Project currentProject, Document document)
        {
            var fontHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var fontRegular = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            var fontRegularLink = new Font(fontRegular);
            fontRegularLink.Color = BaseColor.BLUE;
            fontRegularLink.SetStyle("underline");

            const float LINE_HEIGHT = 1.1f;
            const float SPACING_BEFORE_TITLE = 16f;
            const float SPACING_AFTER_TITLE = 2f;

            var proj = currentProject;
            var currentProjectType = getCurrentProjectTypeOne(proj);

            iTextSharp.text.Image projectTypeImage = iTextSharp.text.Image.GetInstance(HttpContext.Current.Request.MapPath("~/pictures/" + currentProjectType));
            projectTypeImage.SetAbsolutePosition(388, PageSize.A4.Height - document.TopMargin + 10);

            projectTypeImage.ScaleToFit(50f, 150f);
            document.Add(projectTypeImage);

            currentProjectType = getCurrentProjectTypeTwo(proj);
            projectTypeImage = iTextSharp.text.Image.GetInstance(HttpContext.Current.Request.MapPath("~/pictures/" + currentProjectType));
            projectTypeImage.SetAbsolutePosition(443, PageSize.A4.Height - document.TopMargin + 10);
            projectTypeImage.ScaleToFit(50f, 150f);
            document.Add(projectTypeImage);

            var title = new Paragraph(proj.Department.DepartmentName + currentProject.ProjectNr.ToString("00") + ": " + proj.Name, FontFactory.GetFont("Arial", 16, Font.BOLD)).Hyphenate(hyph);
            title.SpacingBefore = 8f;
            title.SpacingAfter = 16f;
            title.SetLeading(0.0f, LINE_HEIGHT);
            document.Add(title);

            var projectTable = new PdfPTable(5);
            projectTable.SpacingAfter = 6f;
            projectTable.DefaultCell.Border = Rectangle.NO_BORDER;
            projectTable.HorizontalAlignment = Element.ALIGN_RIGHT;
            projectTable.WidthPercentage = 100f;
            projectTable.SetWidths(new float[] { 22, 50, 25, 25, 25 });



            projectTable.AddCell(new Paragraph("Betreuer:", fontHeading));
            if (proj.Advisor1Name != "")
                projectTable.AddCell(new Anchor(proj.Advisor1Name, fontRegularLink)
                {
                    Reference = "mailto:" + proj.Advisor1Mail
                });
            else
                projectTable.AddCell(new Paragraph("?", fontRegular));

            projectTable.AddCell("");
            projectTable.AddCell(new Paragraph("Priorität 1", fontHeading));
            projectTable.AddCell(new Paragraph("Priorität 2", fontHeading));

            if (proj.Advisor2Name != "")
            {
                projectTable.AddCell("");
                projectTable.AddCell(new Anchor(proj.Advisor2Name, fontRegularLink)
                {
                    Reference = "mailto:" + proj.Advisor2Mail
                });
            }
            else if(proj.ClientCompany!= "")
            {
                projectTable.AddCell(new Paragraph("Auftraggeber:", fontHeading));
                projectTable.AddCell(new Paragraph(proj.ClientCompany, fontRegular));
            }
            else
            {
                projectTable.AddCell("");
                projectTable.AddCell("");
            }

            projectTable.AddCell(new Paragraph("Arbeitsumfang:", fontHeading));
            projectTable.AddCell(new Paragraph(proj.POneType.Description, fontRegular));
            projectTable.AddCell(new Paragraph(proj.PTwoType == null ? "---" : proj.PTwoType.Description, fontRegular));


            if (proj.ClientCompany != "" && proj.Advisor2Name != "")
            {
                projectTable.AddCell(new Paragraph("Auftraggeber:", fontHeading));
                projectTable.AddCell(new Paragraph(proj.ClientCompany, fontRegular));
            }
            else
            {
                projectTable.AddCell("");
                projectTable.AddCell("");
            }

            projectTable.AddCell(new Paragraph("Teamgrösse:", fontHeading));
            projectTable.AddCell(new Paragraph(proj.POneTeamSize.Description, fontRegular));
            projectTable.AddCell(new Paragraph(proj.PTwoTeamSize == null ? "---" : proj.PTwoTeamSize.Description, fontRegular));

            document.Add(projectTable);

            if (proj.Picture != null)
            {
                var image = iTextSharp.text.Image.GetInstance(proj.Picture.ToArray());
                // http://stackoverflow.com/questions/9272777/auto-scaling-of-images
                // image.ScaleAbsoluteWidth(160f);
                float h = image.ScaledHeight;
                float w = image.ScaledWidth;
                image.Alignment = iTextSharp.text.Image.TEXTWRAP | Element.ALIGN_RIGHT;

                if (w > h)
                    image.ScaleToFit(250f, 150f);
                else if (h >= 300 || w >= 200)
                    image.ScaleToFit(150f, 250f);
                else
                    image.ScaleToFit(100f, 200f);
                document.Add(image);
            }

            if (proj.InitialPosition != "")
            {
                document.Add(new Paragraph("Ausgangslage", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                
                foreach(var text in proj.InitialPosition.ToLinkedParagraph(fontRegular,hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    text.IndentationRight = 10f;
                    document.Add(text);

                }
            }
            if (proj.Objective != "")
            {
                document.Add(new Paragraph("Ziel der Arbeit", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });


                foreach (var text in proj.Objective.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    text.IndentationRight = 10f;
                    document.Add(text);
                }
            }
            if (proj.ProblemStatement != "")
            {
                document.Add(new Paragraph("Problemstellung", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                foreach (var text in proj.ProblemStatement.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, 1.0f);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    text.IndentationRight = 10f;
                    document.Add(text);
                }
                
            }
            if (proj.References != "")
            {
                document.Add(new Paragraph("Technologien/Fachliche Schwerpunkte/Referenzen", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                foreach (var text in proj.References.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    text.IndentationRight = 10f;
                    document.Add(text);
                }
            }
            if (proj.Remarks != "")
            {
                document.Add(new Paragraph("Bemerkungen", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                foreach (var text in proj.Remarks.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    text.IndentationRight = 10f;
                    document.Add(text);
                }
            }

            if (proj.Reservation1Name != "")
            {
                document.Add(new Paragraph("Reservation", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                var text = new Paragraph("Dieses Projekt ist für " + proj.Reservation1Name + (proj.Reservation2Name!="" ? " und " + proj.Reservation2Name : "") + " reserviert.", FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.RED));
                text.SpacingAfter = 1f;
                text.SetLeading(0.0f, LINE_HEIGHT);
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
            public Project CurrentProject { get; set;  }

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

                // create two column table
                var foot = new PdfPTable(1);
                foot.TotalWidth = document.PageSize.Width + 2;
                foot.DefaultCell.Border = Rectangle.NO_BORDER;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell cell = new PdfPCell(new Phrase("Studiengang Informatik/"+ CurrentProject.Department.DepartmentName +"/Studierendenprojekte " + CurrentProject.GetSemester().ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = document.TopMargin - 15;
                cell.PaddingLeft = 58;
                cell.PaddingTop = 8;
                cell.PaddingBottom = 0;
                cell.Border = PdfPCell.NO_BORDER;
                foot.AddCell(cell);
                foot.WriteSelectedRows(0, -1, -1, 40, writer.DirectContent);
            }
        }

        private string getCurrentProjectTypeOne(Project proj)
        {
            if (proj.TypeDesignUX)
                return "projectTypDesignUX.png";
            else if (proj.TypeHW)
                return "projectTypHW.png";
            else if (proj.TypeCGIP)
                return "projectTypCGIP.png";
            else if (proj.TypeMathAlg)
                return "projectTypMathAlg.png";
            else if (proj.TypeAppWeb)
                return "projectTypAppWeb.png";
            else
                return "projectTypDBBigData.png";
        }

        private string getCurrentProjectTypeTwo(Project proj)
        {
            if (proj.TypeHW && proj.TypeDesignUX)
                return "projectTypHW.png";
            else if (proj.TypeCGIP && (proj.TypeDesignUX || proj.TypeHW))
                return "projectTypCGIP.png";
            else if (proj.TypeMathAlg && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP))
                return "projectTypMathAlg.png";
            else if (proj.TypeAppWeb && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg))
                return "projectTypAppWeb.png";
            else if (proj.TypeDBBigData && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg || proj.TypeAppWeb))
                return "projectTypDBBigData.png";
            else
                return "projectTypTransparent.png";
        }
        public int CalcNumberOfPages(int idPDF, HttpRequest currentRequest)
        {
            var margin = Utilities.MillimetersToPoints(Convert.ToSingle(20));
            using (var output = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, margin, margin, margin, margin))
                {
                    AppendToPDF(document, output, Enumerable.Repeat(idPDF, 1));
                }
                using (var pdfReader = new PdfReader(output.ToArray()))
                {
                    return pdfReader.NumberOfPages;
                }
            }
        }
    }
}