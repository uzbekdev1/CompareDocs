using System;
using System.Collections.Generic;
using System.IO;
using CompareDocs.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;

namespace CompareDocs.Exporter
{
    public static class PdfExporter
    {
        public static string Save(IEnumerable<CompareResult> items, string fileName)
        {
            var filePath = Path.ChangeExtension(fileName, "pdf");

            using (var document = new Document(PageSize.A4, 20f, 20f, 20f, 20f))
            {
                var sylfaenpath = Environment.CurrentDirectory + "\\SYLFAEN.TTF";
                var emptyLine = "________________________________________";

                FontFactory.Register(sylfaenpath);
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                var sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var head = new Font(sylfaen, 14f, Font.NORMAL, BaseColor.BLUE);
                var normal = new Font(sylfaen, 10f, Font.NORMAL, BaseColor.BLACK);

                document.Open();
                document.Add(new Paragraph("ЎЗБЕКИСТОН РЕСПУБЛИКАСИ БАНК-МОЛИЯ АКАДЕМИЯСИ", head)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("ТИНГЛОВЧИЛАРИНИНГ МАГИСТРЛИК ДИССЕРТАЦИЯСИНИ", head)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("“АНТИПЛАГИАТ” ДАСТУРИДА ТЕКШИРИШ", head)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("НАТИЖАЛАРИ", head)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Тингловчи Ф.И.Ш.:" + Path.GetFileNameWithoutExtension(fileName).Replace("_", " "), normal));
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Диссертация мавзуси:" + emptyLine, normal));
                document.Add(new Paragraph(Environment.NewLine, normal));

                var table = new PdfPTable(3);

                //actual width of table in points
                table.TotalWidth = 500f;

                //fix the absolute width of the table
                table.LockedWidth = true;

                //relative col widths in proportions - 1/3 and 2/3 and 3/3
                var widths = new float[] { 1f, 3f, 2f };
                table.SetWidths(widths);
                table.HorizontalAlignment = Element.ALIGN_CENTER;

                //leave a gap before and after the table
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;

                table.AddCell(new Phrase(new Chunk("№", normal)));
                table.AddCell(new Phrase(new Chunk("Диссертация ишлари", normal)));
                table.AddCell(new Phrase(new Chunk("Ўхшашлик фоизи", normal)));

                var index = 0;
                foreach (var item in items)
                {
                    index++;

                    table.AddCell(new Phrase(new Chunk(index.ToString(), normal)));
                    table.AddCell(new Phrase(new Chunk(item.FileName, normal)));
                    table.AddCell(new Phrase(new Chunk(item.Percent, normal)));
                }
                document.Add(table);
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Сана:" + DateTime.Now.ToLongDateString(), normal));
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Магистратура факультетлари декани:" + emptyLine, normal));
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Ахборот-ресурс маркази бошлиги:" + emptyLine, normal));
                document.Add(new Paragraph(Environment.NewLine, normal));

                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("Магистратура факультетлари деканатининг", normal));
                document.Add(new Paragraph(Environment.NewLine, normal));
                document.Add(new Paragraph("маркетинг бўйича катта инспектори:" + emptyLine, normal));


                document.Close();
            }

            return filePath;
        }
    }
}
