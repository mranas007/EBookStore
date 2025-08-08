using System.Globalization;
using BookShopingCartMVC.Services.IServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace BookShopingCartMVC.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] GeneratePdfForOrderList(Order orderDetails)
        {
            try
            {
                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(25);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Clean header
                        page.Header().Height(70).Background(Colors.Blue.Medium).Padding(15).Column(col =>
                        {
                            col.Item().Text("ORDER INVOICE")
                                .FontSize(20).Bold().FontColor(Colors.White);
                            col.Item().Text($"Order #{orderDetails.Id} - {orderDetails.CreateDate:MMM dd, yyyy}")
                                .FontSize(11).FontColor(Colors.White);
                        });

                        page.Content().PaddingVertical(15).Column(x =>
                        {
                            x.Spacing(15);

                            AddSimpleOrderSummary(x, orderDetails);
                            AddSimpleCustomerInfo(x, orderDetails);
                            AddSimpleOrderItems(x, orderDetails);
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                    });
                }).GeneratePdf();

                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF generation failed: {ex.Message}", ex);
            }
        }

        private void AddSimpleOrderSummary(ColumnDescriptor column, Order order)
        {
            column.Item().Background(Colors.Grey.Lighten3).Padding(15).Column(col =>
            {
                col.Item().Text("ORDER SUMMARY").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                col.Item().PaddingTop(10);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Status Information").Bold().FontSize(11);
                        c.Item().Text($"Order Status: {order.OrderStatus?.StatusName ?? "Processing"}");
                        c.Item().Text($"Payment Status: {(order.IsPaid ? "✓ PAID" : "⚠ PENDING")}").Bold();
                        c.Item().Text($"Payment Method: {order.PaymentMethod ?? "Not specified"}");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Order Details").Bold().FontSize(11);
                        c.Item().Text($"Total Items: {order.OrderDetail?.Sum(item => item.Quantity) ?? 0}");
                        c.Item().Text($"Order Date: {order.CreateDate:MMM dd, yyyy 'at' HH:mm}");
                    });
                });
            });
        }

        private void AddSimpleCustomerInfo(ColumnDescriptor column, Order order)
        {
            column.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(col =>
            {
                col.Item().Text("CUSTOMER INFORMATION").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                col.Item().PaddingTop(10);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Contact Details").Bold().FontSize(11).FontColor(Colors.Grey.Darken1);
                        c.Item().PaddingTop(5);
                        c.Item().Text($"Name: {order.Name ?? "N/A"}");
                        c.Item().Text($"Email: {order.Email ?? "N/A"}");
                        c.Item().Text($"Phone: {order.MobileNumber ?? "N/A"}");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Shipping Address").Bold().FontSize(11).FontColor(Colors.Grey.Darken1);
                        c.Item().PaddingTop(5);
                        c.Item().Text(order.Address ?? "N/A").LineHeight(1.2f);
                    });
                });
            });
        }

        private void AddSimpleOrderItems(ColumnDescriptor column, Order order)
        {
            column.Item().Column(col =>
            {
                col.Item().Text("ORDER ITEMS").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                col.Item().PaddingTop(10);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Simple table header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Blue.Medium).Padding(8)
                            .Text("Book Details").Bold().FontSize(9).FontColor(Colors.White);
                        header.Cell().Background(Colors.Blue.Medium).Padding(8)
                            .Text("Genre").Bold().FontSize(9).FontColor(Colors.White);
                        header.Cell().Background(Colors.Blue.Medium).Padding(8).AlignRight()
                            .Text("Price").Bold().FontSize(9).FontColor(Colors.White);
                        header.Cell().Background(Colors.Blue.Medium).Padding(8).AlignCenter()
                            .Text("Qty").Bold().FontSize(9).FontColor(Colors.White);
                        header.Cell().Background(Colors.Blue.Medium).Padding(8).AlignRight()
                            .Text("Total").Bold().FontSize(9).FontColor(Colors.White);
                    });

                    if (order.OrderDetail != null)
                    {
                        foreach (var item in order.OrderDetail)
                        {
                            table.Cell().Padding(8).Column(c =>
                            {
                                c.Item().Text(item.Book?.BookName ?? "N/A").FontSize(9).Bold();
                                if (!string.IsNullOrWhiteSpace(item.Book?.AuthorName))
                                {
                                    c.Item().Text($"by {item.Book.AuthorName}")
                                        .FontSize(8).FontColor(Colors.Grey.Medium);
                                }
                            });

                            table.Cell().Padding(8).Text(item.Book?.Genre?.GenreName ?? "N/A").FontSize(9);
                            table.Cell().Padding(8).AlignRight().Text(item.UnitPrice.ToString("C")).FontSize(9);
                            table.Cell().Padding(8).AlignCenter().Text(item.Quantity.ToString()).FontSize(9).Bold();
                            table.Cell().Padding(8).AlignRight().Text((item.Quantity * item.UnitPrice).ToString("C")).FontSize(9).Bold();
                        }

                        // Total row
                        table.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten3).Padding(8).AlignRight()
                            .Text("TOTAL AMOUNT:").Bold().FontSize(10);
                        table.Cell().Background(Colors.Grey.Lighten3).Padding(8).AlignRight()
                            .Text(order.OrderDetail.Sum(x => x.Quantity * x.UnitPrice).ToString("C"))
                            .Bold().FontSize(12).FontColor(Colors.Blue.Medium);
                    }
                });
            });
        }
    }
}