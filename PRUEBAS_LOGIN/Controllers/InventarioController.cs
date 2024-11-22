using PRUEBAS_LOGIN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PRUEBAS_LOGIN.Permisos;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class InventarioController : Controller
    {
        // Cadena de conexión
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        // Lista
        private static List<Productos> ObjLista = new List<Productos>();

        public ActionResult Inventario()
        {
            ObjLista = new List<Productos>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM v_Productos", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Productos nuevoProducto = new Productos();

                        nuevoProducto.Producto_Id = Convert.ToInt32(Lector["ID"]);
                        nuevoProducto.Nombre = Lector["Nombre"].ToString();
                        nuevoProducto.Nombre_Tipo = Lector["Categoria"].ToString();
                        nuevoProducto.Cantidad = Convert.ToInt32(Lector["Stock"]);

                        ObjLista.Add(nuevoProducto);
                    }
                }
            }

            var productosData = ObjLista.Select(p => new
            {
                p.Nombre,
                p.Cantidad // Utilizamos directamente la cantidad de stock de cada producto
            }).ToList();

            ViewBag.ProductosData = productosData;

            return View(ObjLista);
        }

        [HttpGet]
        public ActionResult AgregarProducto()
        {
            // Crear la lista de tipos de productos
            List<SelectListItem> tiposProductoList = new List<SelectListItem>();

            // Lista de vendedores
            List<SelectListItem> vendedoresList = new List<SelectListItem>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                // Obtener tipos de productos
                SqlCommand cmdTipo = new SqlCommand("SELECT Tipo_Producto_Id, Nombre_Tipo FROM Tipo_Producto", ObjConexion);
                cmdTipo.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmdTipo.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        tiposProductoList.Add(new SelectListItem
                        {
                            Value = Lector["Tipo_Producto_Id"].ToString(),
                            Text = Lector["Nombre_Tipo"].ToString()
                        });
                    }
                }
                ObjConexion.Close();

                // Obtener lista de vendedores
                SqlCommand cmdVendedores = new SqlCommand("SELECT Vendedor_Id, Nombre_Empresa FROM Vendedor", ObjConexion);
                cmdVendedores.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmdVendedores.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        vendedoresList.Add(new SelectListItem
                        {
                            Value = Lector["Vendedor_Id"].ToString(),
                            Text = Lector["Nombre_Empresa"].ToString()
                        });
                    }
                }
                ObjConexion.Close();
            }

            ViewBag.TipoProductoList = tiposProductoList;
            ViewBag.VendedoresList = vendedoresList;

            return View();
        }

        [HttpPost]
        public ActionResult AgregarProducto(Productos ObjProducto, int Vendedor_Id)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                // Obtener el nombre del tipo de producto usando el ID
                string nombreTipoProducto = "";

                SqlCommand cmdTipo = new SqlCommand("SELECT Nombre_Tipo FROM Tipo_Producto WHERE Tipo_Producto_Id = @Tipo_Producto_Id", ObjConexion);
                cmdTipo.Parameters.AddWithValue("@Tipo_Producto_Id", ObjProducto.Nombre_Tipo);  // Aquí, ObjProducto.Nombre_Tipo es el ID

                cmdTipo.CommandType = CommandType.Text;
                ObjConexion.Open();

                // Ejecutar la consulta para obtener el nombre del tipo de producto
                nombreTipoProducto = cmdTipo.ExecuteScalar()?.ToString();  // Recupera el nombre del tipo
                ObjConexion.Close();

                // Verifica si se recuperó el nombre del tipo
                if (string.IsNullOrEmpty(nombreTipoProducto))
                {
                    ModelState.AddModelError("", "El tipo de producto seleccionado no es válido.");
                    return View(ObjProducto);
                }

                // Llamada al procedimiento almacenado con el nombre del tipo de producto
                SqlCommand cmd = new SqlCommand("sp_Insertar_Producto", ObjConexion);
                cmd.Parameters.AddWithValue("@Tipo", nombreTipoProducto);  // Enviar el nombre del tipo
                cmd.Parameters.AddWithValue("@Nombre", ObjProducto.Nombre);
                cmd.Parameters.AddWithValue("@Stock", ObjProducto.Cantidad);
                cmd.Parameters.AddWithValue("@Vendedor_Id", Vendedor_Id); // Se añade el Vendedor_Id

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Inventario", "Inventario");
        }


        [HttpGet]
        public ActionResult EditarProducto(int? Producto_Id)
        {
            if (Producto_Id == null)
                return RedirectToAction("Inventario", "Inventario");

            Productos ObjProducto = ObjLista.Where(p => p.Producto_Id == Producto_Id).FirstOrDefault();

            // Crear la lista de tipos de productos
            List<SelectListItem> tiposProductoList = new List<SelectListItem>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                // Obtener tipos de productos
                SqlCommand cmdTipo = new SqlCommand("SELECT Tipo_Producto_Id,Nombre_Tipo FROM Tipo_Producto", ObjConexion);
                cmdTipo.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmdTipo.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        tiposProductoList.Add(new SelectListItem
                        {
                            Value = Lector["Tipo_Producto_Id"].ToString(),
                            Text = Lector["Nombre_Tipo"].ToString(),
                            Selected = Lector["Nombre_Tipo"].ToString() == ObjProducto.Nombre_Tipo
                        });
                    }
                }
                ObjConexion.Close();
            }

            ViewBag.TipoProductoList = tiposProductoList;

            return View(ObjProducto);
        }

        [HttpPost]
        public ActionResult EditarProducto(Productos ObjProducto)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Editar_Producto", ObjConexion);
                cmd.Parameters.AddWithValue("Producto_Id", ObjProducto.Producto_Id);
                cmd.Parameters.AddWithValue("Nombre_Producto", ObjProducto.Nombre);
                cmd.Parameters.AddWithValue("Stock", ObjProducto.Cantidad);
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Inventario", "Inventario");
        }

        [HttpGet]
        public ActionResult EliminarProducto(int? Producto_Id)
        {
            if (Producto_Id == null)
                return RedirectToAction("Inventario", "Inventario");

            Productos ObjProducto = ObjLista.Where(p => p.Producto_Id == Producto_Id).FirstOrDefault();

            return View(ObjProducto);
        }

        [HttpPost]
        public ActionResult EliminarProducto(string Id_Producto)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Eliminar_Producto", ObjConexion);
                cmd.Parameters.AddWithValue("Id_Producto", Id_Producto);
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    TempData["mensaje"] = "No es posible eliminar el producto en este momento.";
                }
            }

            return RedirectToAction("Inventario", "Inventario");
        }
    }
}
