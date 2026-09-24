using Evaluacion_SanchezCori.Models;
using Evaluacion_SanchezCori.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Evaluacion_SanchezCori.Services
{
    public class PdfReportService
    {
        public byte[] GenerarCitas(
            List<Cita> citas,
            string titulo)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.DefaultTextStyle(
                        x => x.FontSize(10)
                    );

                    page.Header()
                        .PaddingBottom(20)
                        .Column(column =>
                        {
                            column.Item()
                                .Text("Clínica Verde")
                                .FontSize(22)
                                .Bold()
                                .FontColor("#4F5B3A");

                            column.Item()
                                .Text(titulo)
                                .FontSize(14)
                                .FontColor("#66724D");

                            column.Item()
                                .Text(
                                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}"
                                )
                                .FontSize(9);
                        });

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(
                                columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                }
                            );

                            table.Header(header =>
                            {
                                Header(
                                    header.Cell(),
                                    "Cliente"
                                );

                                Header(
                                    header.Cell(),
                                    "Mascota"
                                );

                                Header(
                                    header.Cell(),
                                    "Servicio"
                                );

                                Header(
                                    header.Cell(),
                                    "Fecha"
                                );

                                Header(
                                    header.Cell(),
                                    "Estado"
                                );
                            });

                            foreach (var cita in citas)
                            {
                                Celda(
                                    table.Cell(),
                                    cita.Mascota?
                                        .Usuario?
                                        .NombreCompleto ?? "-"
                                );

                                Celda(
                                    table.Cell(),
                                    cita.Mascota?.Nombre ?? "-"
                                );

                                Celda(
                                    table.Cell(),
                                    cita.ServicioVeterinario?
                                        .Nombre ?? "-"
                                );

                                Celda(
                                    table.Cell(),
                                    cita.FechaCita
                                        .ToString("dd/MM/yyyy")
                                );

                                Celda(
                                    table.Cell(),
                                    cita.Estado.ToString()
                                );
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                        });
                });
            }).GeneratePdf();
        }

        public byte[] GenerarServicios(
            List<ServicioReporteItem> servicios)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .PaddingBottom(20)
                        .Column(column =>
                        {
                            column.Item()
                                .Text("Clínica Verde")
                                .FontSize(22)
                                .Bold()
                                .FontColor("#4F5B3A");

                            column.Item()
                                .Text("Servicios más solicitados")
                                .FontSize(14)
                                .FontColor("#66724D");
                        });

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(
                                columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                }
                            );

                            table.Header(header =>
                            {
                                Header(
                                    header.Cell(),
                                    "Servicio"
                                );

                                Header(
                                    header.Cell(),
                                    "Cantidad de citas"
                                );
                            });

                            foreach (
                                var servicio
                                in servicios)
                            {
                                Celda(
                                    table.Cell(),
                                    servicio.Nombre
                                );

                                Celda(
                                    table.Cell(),
                                    servicio.Cantidad.ToString()
                                );
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                        });
                });
            }).GeneratePdf();
        }

        private static void Header(
            IContainer container,
            string texto)
        {
            container
                .Background("#4F5B3A")
                .Padding(7)
                .Text(texto)
                .FontColor(Colors.White)
                .Bold();
        }

        private static void Celda(
            IContainer container,
            string texto)
        {
            container
                .BorderBottom(1)
                .BorderColor("#DCE8D4")
                .Padding(7)
                .Text(texto);
        }
    }
}