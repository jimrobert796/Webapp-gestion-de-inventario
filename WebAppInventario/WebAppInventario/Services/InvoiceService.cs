using FacturacionElectronica.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FacturacionElectronica.Services
{
    public interface IInvoiceService
    {
        Task<bool> SendInvoiceEmail(Invoice invoiceData);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly SmtpSettings _smtpSettings;

        public InvoiceService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;

            // Configurar la licencia gratuita de QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // Genera XML y PDF real
        // Genera XML y PDF real CON IMAGEN
        // Genera XML y PDF real
        private List<(string fileName, byte[] content, string mimeType)> GenerateInvoiceFiles(Invoice invoiceData)
        {
            // XML
            var xmlContent = $"<Invoice><Number>{invoiceData.InvoiceNumber}</Number><Total>{invoiceData.GrandTotal}</Total></Invoice>";
            var xmlBytes = Encoding.UTF8.GetBytes(xmlContent);

            // PDF MEJORADO
            byte[] pdfBytes;
            using (var stream = new MemoryStream())
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // ENCABEZADO PROFESIONAL
                        page.Header().Column(header =>
                        {
                            header.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("FACTURA ELECTRÓNICA")
                                        .SemiBold()
                                        .FontSize(16)
                                        .FontColor(Colors.Blue.Darken3);

                                    col.Item().Text($"No. {invoiceData.InvoiceNumber}")
                                        .SemiBold()
                                        .FontSize(12)
                                        .FontColor(Colors.Grey.Darken2);

                                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                                        .FontSize(9)
                                        .FontColor(Colors.Grey.Darken1);
                                });

                                row.ConstantItem(150).Height(60).Background(Colors.Blue.Lighten5)
                                    .Padding(10).AlignCenter().AlignMiddle()
                                    .Text("Ferreteria El Maestro")
                                    .SemiBold()
                                    .FontSize(10)
                                    .FontColor(Colors.Blue.Darken3);
                            });

                            header.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        });

                        // CONTENIDO PRINCIPAL
                        page.Content().PaddingTop(15).Column(content =>
                        {
                            // INFORMACIÓN DEL EMISOR Y RECEPTOR
                            content.Item().Row(row =>
                            {
                                row.RelativeItem().PaddingRight(10).Column(col =>
                                {
                                    col.Item().Text("EMISOR").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken3);
                                    col.Item().Text("Ferreteria El Maestro").FontSize(9);
                                    col.Item().Text("NIT: 900.123.456-7").FontSize(9);
                                    col.Item().Text("Dirección: Cra 45 #26-85").FontSize(9);
                                    col.Item().Text("Teléfono: +57 601 123 4567").FontSize(9);
                                    col.Item().Text("Email: info@empresaficticia.com").FontSize(9);
                                });

                                row.RelativeItem().PaddingLeft(10).Column(col =>
                                {
                                    col.Item().Text("CLIENTE").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken3);
                                    col.Item().Text(invoiceData.Customer.Name).FontSize(9);
                                    col.Item().Text($"Email: {invoiceData.Customer.Email}").FontSize(9);
                                    col.Item().Text($"Dirección: {invoiceData.Customer.Address}").FontSize(9);
                                });
                            });

                            content.Item().PaddingVertical(15);

                            // TABLA DE PRODUCTOS/SERVICIOS
                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Descripción
                                    columns.ConstantColumn(60); // Cantidad
                                    columns.ConstantColumn(70); // Precio Unitario
                                    columns.ConstantColumn(80); // Total
                                });

                                // ENCABEZADO DE LA TABLA
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten5).Padding(8).Text("DESCRIPCIÓN").SemiBold().FontSize(9);
                                    header.Cell().Background(Colors.Blue.Lighten5).Padding(8).AlignRight().Text("CANT.").SemiBold().FontSize(9);
                                    header.Cell().Background(Colors.Blue.Lighten5).Padding(8).AlignRight().Text("PRECIO UNIT.").SemiBold().FontSize(9);
                                    header.Cell().Background(Colors.Blue.Lighten5).Padding(8).AlignRight().Text("SUBTOTAL.").SemiBold().FontSize(9);
                                });

                                // ITEMS DE LA FACTURA
                                foreach (var item in invoiceData.Items)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).Text(item.Description).FontSize(8);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).AlignRight().Text(item.Quantity.ToString("N0")).FontSize(8);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).AlignRight().Text(item.UnitPrice.ToString("C")).FontSize(8);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).AlignRight().Text(item.LineTotal.ToString("C")).FontSize(8);
                                }
                            });

                            content.Item().PaddingTop(20);

                            // RESUMEN DE TOTALES
                            content.Item().AlignRight().Width(200).Column(totals =>
                            {
                                // Si se necesita de deja comentado para mas despues 
                                /*
                                totals.Item().PaddingBottom(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Subtotal:").FontSize(9);
                                    row.ConstantItem(80).AlignRight().Text(invoiceData.Subtotal.ToString("C")).FontSize(9);
                                });

                                totals.Item().PaddingBottom(5).Row(row =>
                                {
                                    row.RelativeItem().Text("IVA (13%):").FontSize(9);
                                    row.ConstantItem(80).AlignRight().Text(invoiceData.TaxAmount.ToString("C")).FontSize(9);
                                }); */

                                totals.Item().BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAL:").SemiBold().FontSize(11);
                                    row.ConstantItem(80).AlignRight().Text(invoiceData.GrandTotal.ToString("C")).SemiBold().FontSize(11);
                                });
                            });

                            // INFORMACIÓN ADICIONAL
                            content.Item().PaddingTop(20).Background(Colors.Grey.Lighten5).Padding(10).Column(info =>
                            {
                                info.Item().Text("INFORMACIÓN ADICIONAL").SemiBold().FontSize(9).FontColor(Colors.Blue.Darken3);
                                info.Item().Text("Esta factura electrónica cumple con los requisitos legales").FontSize(8).Italic();
                                info.Item().Text("Gracias por su preferencia").FontSize(8);
                            });
                        });

                        // PIE DE PÁGINA
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Factura electrónica generada el ").FontSize(7);
                            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).FontSize(7).SemiBold();
                            text.Span(" - Documento válido tributariamente").FontSize(7);
                        });
                    });
                });

                document.GeneratePdf(stream);
                pdfBytes = stream.ToArray();
            }

            return new List<(string, byte[], string)>
    {
        ($"Factura_{invoiceData.InvoiceNumber}.xml", xmlBytes, "application/xml"),
        ($"Factura_{invoiceData.InvoiceNumber}.pdf", pdfBytes, "application/pdf")
    };
        }

        // Envía la factura por correo
        public async Task<bool> SendInvoiceEmail(Invoice invoiceData)
        {
            try
            {
                var files = GenerateInvoiceFiles(invoiceData);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_smtpSettings.SenderEmail, "Tu Empresa Ficticia");
                    mail.To.Add(invoiceData.Customer.Email);
                    mail.Subject = $"Tu Factura Electrónica No. {invoiceData.InvoiceNumber}";
                    mail.Body = $"Hola {invoiceData.Customer.Name},\n\nAdjuntamos tu factura electrónica. ¡Gracias por tu compra!";
                    mail.IsBodyHtml = false;

                    foreach (var file in files)
                    {
                        var stream = new MemoryStream(file.content);
                        mail.Attachments.Add(new Attachment(stream, file.fileName, file.mimeType));
                    }

                    using (SmtpClient smtp = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword);
                        smtp.EnableSsl = _smtpSettings.UseSsl;

                        await smtp.SendMailAsync(mail);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR FATAL - SMTP] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // Para ver el error completo en Swagger
            }
        }
    }
}