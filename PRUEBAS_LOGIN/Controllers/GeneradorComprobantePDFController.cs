using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PRUEBAS_LOGIN.Controllers
{
    public class GeneradorComprobantePDF
    {
        public void GenerarComprobantePDF(int Cantidad_Pedido, string Nombre_Empresa_Comprador, string Nombre_Producto)
        {
            Document doc = new Document(PageSize.A4);

            // Ruta a la carpeta de "Descargas" del usuario actual
            string rutaDescargas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string rutaArchivo = Path.Combine(rutaDescargas, $"Comprobante_Pedido_{Cantidad_Pedido}.pdf");

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
                doc.Open();

                // Título del documento
                Paragraph titulo = new Paragraph($"Comprobante de Pedido: {Cantidad_Pedido}", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                doc.Add(new Paragraph(" "));

                // Información del cliente
                doc.Add(new Paragraph($"Cliente: {Nombre_Empresa_Comprador}"));
                doc.Add(new Paragraph($"Número de Pedido: {Cantidad_Pedido}"));
                doc.Add(new Paragraph($"Fecha: {DateTime.Now.ToString("dd/MM/yyyy")}"));

                doc.Add(new Paragraph(" "));

                // Detalles del pedido
                doc.Add(new Paragraph("Detalles del Pedido:"));
                doc.Add(new Paragraph(Nombre_Producto));

                doc.Add(new Paragraph(" "));

                // Mensaje de éxito
                Console.WriteLine($"Comprobante generado exitosamente: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar comprobante: {ex.Message}");
            }
            finally
            {
                doc.Close();
            }
        }
    }
}
