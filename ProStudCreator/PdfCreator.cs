using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public class PdfCreator
    {
        enum Layout
        {
            BigPictureInTheMiddle,
            MediumPictureInTheMiddle,
            BigPictureRight,
            SmallPictureInTheMiddle,
            MediumPictureRight,
            SmallPictureRight,
        }

        enum ImageSize
        {
            Big,
            Medium,
            Small,
        }

        public const float LINE_HEIGHT = 1.1f;
        public const float SPACING_BEFORE_TITLE = 16f;
        public float SPACING_AFTER_TITLE = 2f;
        public float SPACING_BEFORE_IMAGE = 16f;

        public Font fontHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        public Font fontRegular = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        public Font fontsmall = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        HyphenationAuto hyph = new HyphenationAuto("de", "none", 2, 2);



        public void AppendToPDF(Document document, MemoryStream output, IEnumerable<Project> projects)
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

            foreach (Project project in projects)
            {
                ef.CurrentProject = project;
                WritePDF(project, document, writer);
            }
        }

        private void AppendToPDF(MemoryStream output, IEnumerable<Project> projects, Layout layout, Document document)
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

            foreach (Project project in projects)
            {
                ef.CurrentProject = project;
                WritePDF(project, document, layout, writer);

            }

        }

        private void WritePDF(Project currentProject, Document document, PdfWriter writer)
        {
            foreach (Layout l in Enum.GetValues(typeof(Layout)))
            {
                if (CalcNumberOfPages(currentProject, l) == 1)
                {
                    WritePDF(currentProject, document, l, writer);
                    return;
                }
            }

            WritePDF(currentProject, document, Layout.SmallPictureRight, writer);
        }

        private void WritePDF(Project currentProject, Document document, Layout layout, PdfWriter writer)
        {
            var fontRegularLink = new Font(fontRegular);
            fontRegularLink.Color = BaseColor.BLUE;
            fontRegularLink.SetStyle("underline");

            //
            // Header contents
            //

            var proj = currentProject;
            var currentProjectType = GetCurrentProjectTypeOne(proj);

            iTextSharp.text.Image projectTypeImage = iTextSharp.text.Image.GetInstance(HttpContext.Current.Request.MapPath("~/pictures/" + currentProjectType));
            projectTypeImage.SetAbsolutePosition(388, PageSize.A4.Height - document.TopMargin + 10);

            projectTypeImage.ScaleToFit(50f, 150f);
            document.Add(projectTypeImage);

            currentProjectType = GetCurrentProjectTypeTwo(proj);
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

            //  Row 1
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

            // Row 2
            if (proj.Advisor2Name != "")
            {
                projectTable.AddCell("");
                projectTable.AddCell(new Anchor(proj.Advisor2Name, fontRegularLink)
                {
                    Reference = "mailto:" + proj.Advisor2Mail
                });
            }
            else if (proj.ClientCompany != "")
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

            // Row 3
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

            // Row 4
            var strLang = "Deutsch oder Englisch";
            if (proj.LanguageEnglish && !proj.LanguageGerman)
                strLang = "Englisch";
            if (proj.LanguageGerman && !proj.LanguageEnglish)
                strLang = "Deutsch";

            projectTable.AddCell(new Paragraph("Sprachen:", fontHeading));
            projectTable.AddCell(new Paragraph(strLang.ToString(), fontRegular));
            projectTable.AddCell("");
            projectTable.AddCell("");
            projectTable.AddCell("");

            // End header
            document.Add(projectTable);

            //
            // Body
            //

            //pdf with image
            if (proj.Picture != null)
            {
                try
                {
                    var img = iTextSharp.text.Image.GetInstance(proj.Picture.ToArray());
                    float height = img.ScaledHeight;
                    float width = img.ScaledWidth;

                    //checks witch layout should be used.
                    switch (layout)
                    {
                        case Layout.BigPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Big);
                            break;
                        case Layout.BigPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Big, writer);
                            break;
                        case Layout.MediumPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Medium);
                            break;
                        case Layout.MediumPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Medium, writer);
                            break;
                        case Layout.SmallPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Small);
                            break;
                        case Layout.SmallPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Small, writer);
                            break;
                    }
                }
                catch (IOException)
                {
                    //could not parse the image...

                    //pdf without image
                    AddParagraph(proj.InitialPosition, document, "Ausgangslage", proj.InitialPosition);
                    AddParagraph(proj.Objective, document, "Ziel der Arbeit", proj.Objective);
                    AddParagraph(proj.ProblemStatement, document, "Problemstellung", proj.ProblemStatement);
                    AddParagraph(proj.References, document, "Technologien/Fachliche Schwerpunkte/Referenzen", proj.References);
                    AddParagraph(proj.Remarks, document, "Bemerkungen", proj.Remarks);
                }
            }
            else
            {
                //pdf without image
                AddParagraph(proj.InitialPosition, document, "Ausgangslage", proj.InitialPosition);
                AddParagraph(proj.Objective, document, "Ziel der Arbeit", proj.Objective);
                AddParagraph(proj.ProblemStatement, document, "Problemstellung", proj.ProblemStatement);
                AddParagraph(proj.References, document, "Technologien/Fachliche Schwerpunkte/Referenzen", proj.References);
                AddParagraph(proj.Remarks, document, "Bemerkungen", proj.Remarks);
            }


            //
            // Footer
            //
            var strComments = "";
            if (proj.Reservation1Name != "")
            {
                strComments += "Dieses Projekt ist für " + proj.Reservation1Name;
                if (proj.Reservation2Name != "") strComments += " und " + proj.Reservation2Name;
                strComments += " reserviert.\n";
            }

            if (proj.DurationOneSemester && (proj.POneType.P6 || proj.PTwoType?.P6 == true))
            {
                strComments += "Dieses Projekt muss in einem einzigen Semester durchgeführt werden.\n";
            }

            if (strComments.Length > 0)
            {
                document.Add(new Paragraph("Bemerkungen", fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                var fontRegularRed = new Font(fontRegular);
                fontRegularRed.Color = BaseColor.RED;

                var text = new Paragraph(strComments.ToString(), fontRegularRed);
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
            public Project CurrentProject { get; set; }

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
                PdfPCell cell = new PdfPCell(new Phrase("Studiengang Informatik/" + CurrentProject.Department.DepartmentName + "/Studierendenprojekte " + CurrentProject.GetSemester().ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
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

        private string GetCurrentProjectTypeOne(Project proj)
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
            else if (proj.TypeSysSec)
                return "projectTypSysSec.png";
            else if (proj.TypeSE)
                return "projectTypSE.png";
            else
                return "projectTypDBBigData.png";
        }

        private string GetCurrentProjectTypeTwo(Project proj)
        {
            // Note: Complicated conditional statements relate to the order of returns in getCurrentProjectTypeOne
            // TODO Consider extracting logic to a method that returns two project types. e.g. put in a list and pull out first two relevant types.

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
            else if (proj.TypeSysSec && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg || proj.TypeAppWeb || proj.TypeDBBigData))
                return "projectTypSysSec.png";
            else if (proj.TypeSE && (proj.TypeDesignUX || proj.TypeHW || proj.TypeCGIP || proj.TypeMathAlg || proj.TypeAppWeb || proj.TypeDBBigData || proj.TypeSysSec))
                return "projectTypSE.png";
            else
                return "projectTypTransparent.png";
        }

        public int CalcNumberOfPages(Project PDF)
        {
            var minimumNumberOfPages = int.MaxValue;
            foreach (Layout l in Enum.GetValues(typeof(Layout)))
            {
                minimumNumberOfPages = Math.Min(minimumNumberOfPages, CalcNumberOfPages(PDF, l));
            }
            return minimumNumberOfPages;
        }

        private int CalcNumberOfPages(Project PDF, Layout layout)
        {
            using (var output = new MemoryStream())
            {
                using (var document = CreateDocument())
                {
                    AppendToPDF(output, Enumerable.Repeat(PDF, 1), layout, document);
                }
                using (var pdfReader = new PdfReader(output.ToArray()))
                {
                    return pdfReader.NumberOfPages;
                }
            }
        }

        private void PictureRightLayout(Project proj, iTextSharp.text.Image img, Document document, ImageSize imgsize, PdfWriter writer)
        {

            // http://stackoverflow.com/questions/9272777/auto-scaling-of-images
            // image.ScaleAbsoluteWidth(160f);

            switch (imgsize)
            {
                case ImageSize.Big:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 300, 300));
                    break;
                case ImageSize.Medium:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 250, 250));
                    break;
                case ImageSize.Small:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 200, 200));
                    break;
            }

            AddParagraph(proj.InitialPosition, document, "Ausgangslage", proj.InitialPosition);
            AddParagraph(proj.Objective, document, "Ziel der Arbeit", proj.Objective);
            AddParagraph(proj.ProblemStatement, document, "Problemstellung", proj.ProblemStatement);
            AddParagraph(proj.References, document, "Technologien/Fachliche Schwerpunkte/Referenzen", proj.References);
            AddParagraph(proj.Remarks, document, "Bemerkungen", proj.Remarks);
        }

        public iTextSharp.text.Image DescribedImage(PdfWriter writer, iTextSharp.text.Image img, String description, float heighttoscale, float widthtoscale)
        {
            img.ScaleToFit(heighttoscale, widthtoscale);
            float width = img.ScaledWidth;
            float height = img.ScaledHeight;

            //height used for one line of imgdescription
            float descriptionheight = 12f;

            //set up template
            PdfContentByte cb = writer.DirectContent;
            PdfTemplate template = cb.CreateTemplate(width + 10f, height + (descriptionheight * 5));


            Paragraph linkedPara = new Paragraph();
            linkedPara.AddRange(description.ToLinkedParagraph(fontsmall, hyph));


            //set up ct for description
            ColumnText ct = new ColumnText(template);
            Phrase imgDescription = new Phrase(linkedPara);
            ct.SetSimpleColumn(imgDescription, 10f, 0, template.Width, descriptionheight * 5, 10, Element.ALIGN_JUSTIFIED);
            ct.Go(true);

            //get the lines used to write the comment
            int lineswriten = ct.LinesWritten;

            template.Height = height + lineswriten * descriptionheight;
            
            //add the comment to the template
            ct.SetSimpleColumn(imgDescription, 10f, 0, template.Width, descriptionheight * lineswriten, 10, Element.ALIGN_JUSTIFIED);
            ct.Go(false);

            //add img to template
            template.AddImage(img, width, 0, 0, height, template.Width - width, template.Height - height);

            iTextSharp.text.Image i = iTextSharp.text.Image.GetInstance(template);
            i.Alignment = iTextSharp.text.Image.ALIGN_RIGHT | iTextSharp.text.Image.TEXTWRAP;
            return i;
        }


        private void AddParagraph(String test, Document document, String title, String content)
        {
            if (test != "")
            {
                document.Add(new Paragraph(title, fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });


                foreach (var text in content.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(text);
                }
            }
        }

        private void PictureInTheMiddle(Project proj, iTextSharp.text.Image img, Document document, ImageSize imgsize)
        {
            switch (imgsize)
            {
                case ImageSize.Big:
                    img.ScaleToFit(480f, 480f);
                    break;
                case ImageSize.Medium:
                    img.ScaleToFit(380f, 380f);
                    break;
                case ImageSize.Small:
                    img.ScaleToFit(290f, 290f);
                    break;
            }

            img.Alignment = Element.ALIGN_MIDDLE;
            img.SpacingBefore = SPACING_BEFORE_IMAGE;

            img.SpacingBefore = SPACING_BEFORE_TITLE;
            document.Add(img);

            if (proj.ImgDescription != "")
            {
                fontsmall.SetColor(100, 100, 100);

                Paragraph p = new Paragraph(proj.ImgDescription, fontsmall);
                p.PaddingTop = -1f;
                p.Alignment = Element.ALIGN_CENTER;

                document.Add(p);
            }
            AddParagraph(proj.InitialPosition, document, "Ausgangslage", proj.InitialPosition);
            AddParagraph(proj.Objective, document, "Ziel der Arbeit", proj.Objective);
            AddParagraph(proj.ProblemStatement, document, "Problemstellung", proj.ProblemStatement);
            AddParagraph(proj.References, document, "Technologien/Fachliche Schwerpunkte/Referenzen", proj.References);
            AddParagraph(proj.Remarks, document, "Bemerkungen", proj.Remarks);
        }

        public static Document CreateDocument()
        {
            var margin = Utilities.MillimetersToPoints(20f);
            var document = new Document(PageSize.A4, margin, margin, margin, margin);

            return document;
        }

    }
}