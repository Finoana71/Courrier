using Courrier.Models.Courrier;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Collections.Generic;
using System.IO;

namespace Courrier.Services
{
    public class ExportPdfService
    {
        public void ExportCourriersToPdf(List<CourrierDestinataire> list, Stream outputStream)
        {
            var document = CreatePdfDocument();
            var writer = PdfWriter.GetInstance(document, outputStream);

            document.Open();
            AddTitle(document, "Liste des courriers");
            AddCourriersTable(document, list);
            document.Close();
        }


        private Document CreatePdfDocument()
        {
            return new Document();
        }

        private void AddTitle(Document document, string titleText)
        {
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, iTextSharp.text.Color.BLACK);
            var title = new Paragraph(titleText, titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;

            document.Add(title);
        }

        private void AddCourriersTable(Document document, List<CourrierDestinataire> list)
        {
            var table = new PdfPTable(9);
            table.WidthPercentage = 100;

            AddTableHeaders(table);
            AddTableData(table, list);

            document.Add(table);
        }

        private void AddTableHeaders(PdfPTable table)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, iTextSharp.text.Color.WHITE);
            var cellBackgroundColor = new iTextSharp.text.Color(51, 102, 153);

            var headerCell = new PdfPCell();
            headerCell.BackgroundColor = cellBackgroundColor;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            headerCell.Padding = 5;

            headerCell.Phrase = new Phrase("Objet", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Expediteur", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Référence", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Département", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Fichier joint", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Date de création", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Flag", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Externe", headerFont);
            table.AddCell(headerCell);

            headerCell.Phrase = new Phrase("Statut", headerFont);
            table.AddCell(headerCell);
        }

        private void AddTableData(PdfPTable table, IEnumerable<CourrierDestinataire> courriers)
        {
            var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Color.BLACK);

            foreach (var courrier in courriers)
            {
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Objet, dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Expediteur, dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Reference, dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Departement.Nom, dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Fichier != null ? "Oui" : "Non", dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.DateCreation.ToShortDateString(), dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Flag.Libelle, dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Courrier.Interne ? "Non" : "Oui", dataFont)));
                table.AddCell(new PdfPCell(new Phrase(courrier.Statut.Libelle, dataFont)));
            }
        }
    }
}
