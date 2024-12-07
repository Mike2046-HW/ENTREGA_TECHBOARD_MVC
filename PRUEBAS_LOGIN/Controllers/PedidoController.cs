using PRUEBAS_LOGIN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PRUEBAS_LOGIN.Permisos;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Dapper;
using iTextSharp.text.pdf.draw;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class PedidoController : Controller
    {
        //Cadena de conexión
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();

        private static List<Pedido> ObjLista = new List<Pedido>();

        // GET: Pedido
        public ActionResult Pedido()
        {
            ObjLista = new List<Pedido>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("Select * From v_Detalles_Pedido", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Pedido nuevoPedido = new Pedido();
                        nuevoPedido.ID = Convert.ToInt32(Lector["ID"]);
                        nuevoPedido.FechaPedido = Lector.GetDateTime(Lector.GetOrdinal("Fecha Pedido"));
                        nuevoPedido.Comprador = Lector["Comprador"].ToString();
                        nuevoPedido.Estado = Lector["Estado"].ToString();
                        nuevoPedido.TipoDeProducto = Lector["Tipo de producto"].ToString();
                        nuevoPedido.NombreProducto = Lector["Nombre Producto"].ToString();
                        nuevoPedido.Cantidad = Convert.ToInt32(Lector["Cantidad"]);
                        nuevoPedido.Vendedor = Lector["Vendedor"].ToString();
                        nuevoPedido.Paqueteria = Lector["Paquetería"].ToString();

                        ObjLista.Add(nuevoPedido);
                    }
                }
            }
            return View(ObjLista);
        }

        [HttpGet]
        [PermisosRol(Rol.Admin)]
        public ActionResult AgregarPedido()
        {
            List<SelectListItem> Vendedores = new List<SelectListItem>();
            List<SelectListItem> Compradores = new List<SelectListItem>();
            List<SelectListItem> Productos = new List<SelectListItem>();
            List<SelectListItem> Paqueterias = new List<SelectListItem>();
            List<SelectListItem> Estados = new List<SelectListItem>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd1 = new SqlCommand("SELECT Vendedor_Id, Nombre_Empresa FROM Vendedor", ObjConexion);
                cmd1.CommandType = CommandType.Text;
                ObjConexion.Open();

                // Cargar Vendedores
                using (SqlDataReader Lector = cmd1.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Vendedores.Add(new SelectListItem
                        {
                            Text = Lector["Nombre_Empresa"].ToString(),
                            Value = Lector["Vendedor_Id"].ToString() // Asignando el ID al Value
                        });
                    }
                }

                // Cargar Compradores
                SqlCommand cmd2 = new SqlCommand("SELECT Comprador_Id, Nombre_Empresa FROM Comprador", ObjConexion);
                cmd2.CommandType = CommandType.Text;

                using (SqlDataReader Lector = cmd2.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Compradores.Add(new SelectListItem
                        {
                            Text = Lector["Nombre_Empresa"].ToString(),
                            Value = Lector["Comprador_Id"].ToString() // Asignando el ID al Value
                        });
                    }
                }

                // Cargar Productos
                SqlCommand cmd3 = new SqlCommand("SELECT Producto_Id, Nombre_Producto FROM Producto", ObjConexion);
                cmd3.CommandType = CommandType.Text;

                using (SqlDataReader Lector = cmd3.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Productos.Add(new SelectListItem
                        {
                            Text = Lector["Nombre_Producto"].ToString(),
                            Value = Lector["Producto_Id"].ToString() // Asignando el ID al Value
                        });
                    }
                }

                // Cargar Paqueterías
                SqlCommand cmd4 = new SqlCommand("SELECT Paqueteria_Id, Nombre FROM Paqueteria", ObjConexion);
                cmd4.CommandType = CommandType.Text;

                using (SqlDataReader Lector = cmd4.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Paqueterias.Add(new SelectListItem
                        {
                            Text = Lector["Nombre"].ToString(),
                            Value = Lector["Paqueteria_Id"].ToString() // Asignando el ID al Value
                        });
                    }
                }

                // Cargar Estados
                SqlCommand cmd5 = new SqlCommand("SELECT Estado_Envio_Id, Estado FROM Estado_Envio", ObjConexion);
                cmd5.CommandType = CommandType.Text;

                using (SqlDataReader Lector = cmd5.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Estados.Add(new SelectListItem
                        {
                            Text = Lector["Estado"].ToString(),
                            Value = Lector["Estado_Envio_Id"].ToString() // Asignando el ID al Value
                        });
                    }
                }
            }

            ViewBag.VendedoresList = Vendedores;
            ViewBag.CompradoresList = Compradores;
            ViewBag.ProductosList = Productos;
            ViewBag.PaqueteriasList = Paqueterias;
            ViewBag.EstadosList = Estados;

            return View();
        }

        [HttpPost]
        [PermisosRol(Rol.Admin)]
        public ActionResult AgregarPedido(Pedido ObjPedido, int Comprador_Id, int Producto_Id, int Cantidad, int Paqueteria_Id, int Estado_Envio_Id)
        {
            // Obtener el Vendedor_Id automáticamente para el Producto_Id seleccionado
            int vendedorId = 0;
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                // Consultar el vendedor asociado al producto
                SqlCommand cmdVendedor = new SqlCommand("SELECT TOP 1 Vendedor_Id FROM Vendedor_Producto WHERE Producto_Id = @Producto_Id", ObjConexion);
                cmdVendedor.Parameters.AddWithValue("@Producto_Id", Producto_Id);
                ObjConexion.Open();
                var result = cmdVendedor.ExecuteScalar();

                // Si no se encuentra un vendedor para el producto, mostrar mensaje de error
                if (result != null)
                {
                    vendedorId = Convert.ToInt32(result);
                }
                else
                {
                    // Si no se encontró un vendedor, puedes manejar el error aquí (ej. redirigir o mostrar un mensaje)
                    ViewBag.ErrorMessage = "No se encontró un vendedor para el producto seleccionado.";
                    return View();
                }
            }

            // Ahora que tenemos el Vendedor_Id, pasamos todos los parámetros al procedimiento almacenado
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Pedido", ObjConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Agregar parámetros
                cmd.Parameters.AddWithValue("@Comprador_Id", Comprador_Id);
                cmd.Parameters.AddWithValue("@Producto_Id", Producto_Id);
                cmd.Parameters.AddWithValue("@Cantidad", Cantidad);
                cmd.Parameters.AddWithValue("@Paqueteria_Id", Paqueteria_Id);
                cmd.Parameters.AddWithValue("@Estado_Envio_Id", Estado_Envio_Id);
                cmd.Parameters.AddWithValue("@Vendedor_Id", vendedorId);  // Asignamos el Vendedor_Id automáticamente

                // Ejecutar el procedimiento almacenado
                ObjConexion.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    TempData["mensaje"] = "Por favor asegurese de haber ingresado una cantidad disponible del producto asociado al pedido.";
                }

            }

            // Redirigir o mostrar mensaje de éxito
            return RedirectToAction("Pedido", "Pedido");
        }

        // Método para obtener los datos del comprobante según el ID del pedido
        private dynamic ObtenerDatosComprobante(int idPedido)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                string query = "SELECT * FROM v_DatosComprobante WHERE ID = @ID";
                return connection.QueryFirstOrDefault(query, new { ID = idPedido });
            }
        }

        [HttpGet]
        public ActionResult TerminarPedido(int? idpedido)
        {
            if (idpedido == null)
                return RedirectToAction("Pedido", "Pedido");

            Pedido ObjPedido = ObjLista.Where(p => p.ID == idpedido).FirstOrDefault();
            return View(ObjPedido);
        }

        [PermisosRol(Rol.Admin)]
        [HttpPost]
        public ActionResult TerminarPedido(int ID)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Finalizar_Pedido", ObjConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Asegúrate de que el parámetro @Id_Pedido esté presente en el procedimiento almacenado
                cmd.Parameters.AddWithValue("@Id_Pedido", ID);

                ObjConexion.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Pedido", "Pedido");
        }


        // Método para generar el PDF
        [HttpGet]
        public ActionResult Recibo(int? idpedido)
        {
            if (idpedido == null)
                return RedirectToAction("Pedido", "Pedido");

            // Obtener datos del comprobante desde la vista 'v_DatosComprobante'
            var datosComprobante = ObtenerDatosComprobante(idpedido.Value);

            if (datosComprobante == null)
                return RedirectToAction("Pedido", "Pedido");

            // Generar el PDF usando los datos obtenidos
            GenerarComprobantePDF(datosComprobante);

            return RedirectToAction("Pedido", "Pedido"); // Redirige a la acción que prefieras
        }

        private void GenerarComprobantePDF(dynamic datosComprobante)
        {
            Document doc = new Document(PageSize.A4, 50, 50, 25, 25);

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter.GetInstance(doc, memoryStream);
                    doc.Open();

                    // Convertir la ruta relativa a una absoluta
                    string logoPath = Server.MapPath("~/Images/TECHBOARD.png");

                    // Verificar si el archivo existe
                    if (System.IO.File.Exists(logoPath))
                    {
                        // Cargar y agregar la imagen
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(150, 75); // Ajustar el tamaño de la imagen
                        logo.Alignment = Element.ALIGN_LEFT; // Alinear la imagen a la izquierda
                        doc.Add(logo);
                    }
                    else
                    {
                        throw new Exception($"No se encontró la imagen en la ruta especificada: {logoPath}");
                    }

                    // Título del comprobante
                    Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    Paragraph titulo = new Paragraph("COMPROBANTE", tituloFont);
                    titulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(titulo);
                    doc.Add(new Paragraph(" "));

                    // Fecha del pedido
                    doc.Add(new Paragraph($"Fecha del Pedido: {datosComprobante.Fecha_del_pedido}"));
                    doc.Add(new Paragraph("\n"));

                    // Separador
                    LineSeparator separator = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
                    doc.Add(new Chunk(separator));
                    doc.Add(new Paragraph(" "));

                    // Información del comprador
                    doc.Add(new Paragraph("Información del Comprador:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(new Paragraph($"Empresa: {datosComprobante.Empresa_Comprador}"));
                    doc.Add(new Paragraph($"Correo: {datosComprobante.Correo_Comprador}"));
                    doc.Add(new Paragraph($"Teléfono: {datosComprobante.Telefono_Comprador}"));
                    doc.Add(new Paragraph($"Dirección: {datosComprobante.Calle_Comprador}, {datosComprobante.Ciudad_Comprador}, {datosComprobante.Estado_Comprador}, {datosComprobante.Codigo_Postal_Comprador}"));
                    doc.Add(new Paragraph(" "));

                    // Tabla de productos
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;

                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new PdfPCell(new Phrase("Producto", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Categoría", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Cantidad", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    Font rowFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    table.AddCell(new PdfPCell(new Phrase(datosComprobante.Producto, rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(datosComprobante.Categoria_Producto, rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(datosComprobante.Cantidad.ToString(), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    doc.Add(table);
                    doc.Add(new Paragraph(" "));

                    // Información de la paquetería
                    doc.Add(new Chunk(separator));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Información de la Paquetería:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));

                    PdfPTable paqueteriaTable = new PdfPTable(2);
                    paqueteriaTable.WidthPercentage = 100;

                    paqueteriaTable.AddCell(new PdfPCell(new Phrase("Nombre de la Paquetería", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    paqueteriaTable.AddCell(new PdfPCell(new Phrase(datosComprobante.Paqueteria, rowFont)) { HorizontalAlignment = Element.ALIGN_LEFT });

                    paqueteriaTable.AddCell(new PdfPCell(new Phrase("Correo Paquetería", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    paqueteriaTable.AddCell(new PdfPCell(new Phrase(datosComprobante.Correo_Paqueteria, rowFont)) { HorizontalAlignment = Element.ALIGN_LEFT });

                    paqueteriaTable.AddCell(new PdfPCell(new Phrase("Teléfono Paquetería", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    paqueteriaTable.AddCell(new PdfPCell(new Phrase(datosComprobante.Telefono_Paqueteria, rowFont)) { HorizontalAlignment = Element.ALIGN_LEFT });

                    doc.Add(paqueteriaTable);
                    doc.Add(new Paragraph(" "));

                    // Información del vendedor
                    doc.Add(new Paragraph("Información del Vendedor:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(new Paragraph($"Empresa: {datosComprobante.Empresa_Vendedor}"));
                    doc.Add(new Paragraph($"Correo: {datosComprobante.Correo_Vendedor}"));
                    doc.Add(new Paragraph($"Teléfono: {datosComprobante.Telefono_Vendedor}"));
                    doc.Add(new Paragraph($"Dirección: {datosComprobante.Calle_Vendedor}, {datosComprobante.Ciudad_Vendedor}, {datosComprobante.Estado_Vendedor}, {datosComprobante.Codigo_Postal_Vendedor}"));

                    doc.Close();

                    // Enviar el PDF al cliente como descarga
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=Comprobante_Pedido_{datosComprobante.ID}.pdf");
                    Response.Buffer = true;
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar comprobante: {ex.Message}");
            }
        }


        /*
        [HttpGet]
        public ActionResult Recibo (int? idpedido)
        {

            if (idpedido == null)
                return RedirectToAction("Pedido", "Pedido");

            Pedido ObPedido = ObjLista.Where(p => p.ID == idpedido).FirstOrDefault();

            return View(idpedido);
        }*/

    }
}