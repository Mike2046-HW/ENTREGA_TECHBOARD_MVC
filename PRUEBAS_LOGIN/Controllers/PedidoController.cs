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

            using (SqlConnection ObjConexion = new SqlConnection (conexion))
            {
                SqlCommand cmd = new SqlCommand ("Select * From v_Detalles_Pedido", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open ();

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

                        ObjLista.Add (nuevoPedido);

                    }
                }
            }
            return View(ObjLista);
        }

        //Llamado Comprobante PDF
        private readonly GeneradorComprobantePDF comprobanteService;

        public PedidoController()
        {
            comprobanteService = new GeneradorComprobantePDF();
        }

        [HttpGet]
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
        public ActionResult AgregarPedido(Pedido ObjPedido,int Comprador_Id, int Producto_Id, int Cantidad, int Paqueteria_Id, int Estado_Envio_Id, int Vendedor_Id)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {

                    SqlCommand cmd = new SqlCommand("sp_Insertar_Pedido", ObjConexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    cmd.Parameters.AddWithValue("Comprador_Id", Comprador_Id);
                    cmd.Parameters.AddWithValue("Producto_Id", Producto_Id);
                    cmd.Parameters.AddWithValue("Cantidad", Cantidad);
                    cmd.Parameters.AddWithValue("Paqueteria_Id", Paqueteria_Id);
                    cmd.Parameters.AddWithValue("Estado_Envio_Id", Estado_Envio_Id);
                    cmd.Parameters.AddWithValue("Vendedor_Id", Vendedor_Id);


                // Ejecutar el procedimiento almacenado
                ObjConexion.Open();
                cmd.ExecuteNonQuery();

                ObjPedido.Cantidad_Pedido = Cantidad;
                ObjPedido.Nombre_Empresa_Comprador = Comprador_Id.ToString();
                ObjPedido.NombreProducto = Producto_Id.ToString();


                // Llamar al método para generar el comprobante en PDF
                comprobanteService.GenerarComprobantePDF(
                    ObjPedido.Cantidad_Pedido,
                    ObjPedido.Nombre_Empresa_Comprador,
                    ObjPedido.Nombre_Producto
                    );

            }

            // Redirigir o mostrar mensaje de éxito
            return RedirectToAction("Pedido", "Pedido");
        }
    }
}