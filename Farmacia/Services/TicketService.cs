using Farmacia.Interfaces;
using Farmacia.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Farmacia.Services
{
    public class TicketService : ITicketService
    {
        private byte[] GetQrCodeImage(string data)
        {
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCoder.PngByteQRCode(qrData);
            return qrCode.GetGraphic(20);
        }

        public byte[] GenerarTicketVentaPdf(Venta venta, Cliente? cliente)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A6);
                    page.Margin(10);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Content().Column(col =>
                    {
                        // Encabezado
                        col.Item().Text("Farmacia SmartFarma")
                            .Bold().FontSize(18).FontColor(Colors.Blue.Medium).AlignCenter();

                        col.Item().Text("CUIT: 30-12345678-9").FontSize(9).AlignCenter();
                        col.Item().Text("Punto de Venta: 0001").FontSize(9).AlignCenter();
                        col.Item().Text("Comprobante: Ticket B Nº 0001-00000042").FontSize(9).AlignCenter();
                        col.Item().Text($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}").FontSize(9).AlignCenter();

                        col.Item().PaddingVertical(4).Text($"Cliente: {cliente?.Nombre} {cliente?.Apellido}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Tabla de productos
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Producto
                                columns.RelativeColumn(3); // Presentación
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(2); // Precio Unitario
                                columns.RelativeColumn(2); // Subtotal
                            });

                            // Encabezado
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Producto").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Presentación").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(1).Text("Cant").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(1).Text("P.Unit").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(1).Text("Subtotal").Bold();
                            });

                            // Detalles
                            foreach (var d in venta.DetallesVenta)
                            {
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).Text(d.Producto?.NombreComercial ?? "");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).Text(d.Producto?.PresentacionCompleta ?? "");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text($"{d.Cantidad}");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).AlignRight().Text($"${d.PrecioUnitario:F2}");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).AlignRight().Text($"${d.Subtotal:F2}");
                            }
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Totales
                        col.Item().AlignRight().Text($"Total: ${venta.Total:F2}")
                            .Bold().FontSize(12).FontColor(Colors.Green.Darken2);

                        if (venta.DescuentoAplicado.HasValue && venta.DescuentoAplicado.Value > 0)
                        {
                            col.Item().AlignRight().Text($"Descuento: -${venta.DescuentoAplicado.Value:F2}")
                                .Italic().FontColor(Colors.Red.Medium);
                        }

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // CAE y vencimiento
                        col.Item().Text("CAE: 61234567891234").FontSize(9).AlignCenter();
                        col.Item().Text("Vto. CAE: 20/06/2025").FontSize(9).AlignCenter();

                        // QR
                        var qrImage = Image.FromBinaryData(GetQrCodeImage("CO-WORKERS FARMACIA SMART FARMA"));
                        col.Item()
                           .AlignCenter()
                           .PaddingTop(5)
                           .Height(80)
                           .Width(80)
                           .Image(qrImage);
                    });
                });
            }).GeneratePdf();
        }
    }
}