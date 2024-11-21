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
        //Cadena de conexión
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        //Lista 
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

            // Convertir la lista de productos a un formato adecuado para Google Charts (JSON)
            var productosData = ObjLista.Select(p => new
            {
                p.Nombre,
                p.Cantidad // Utilizamos directamente la cantidad de stock de cada producto
            }).ToList();

            // Pasar los datos a la vista en formato JSON
            ViewBag.ProductosData = productosData;

            // Retornar la vista con la lista de productos
            return View(ObjLista);
        }


        [HttpGet]
        public ActionResult AgregarProducto()
        {
            // Crear la lista de tipos de productos
            List<SelectListItem> tiposProductoList = new List<SelectListItem>();

            // Establecer conexión con la base de datos
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                // Supongamos que tienes una tabla 'Tipo_Producto' con 'Tipo_Producto_Id' y 'Nombre_Tipo'
                SqlCommand cmd = new SqlCommand("SELECT Tipo_Producto_Id,Nombre_Tipo FROM Tipo_Producto", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        // Crear elementos para la lista desplegable
                        tiposProductoList.Add(new SelectListItem
                        {
                           // Value = Lector["Tipo_Producto_Id"].ToString(),
                            Text = Lector["Nombre_Tipo"].ToString()
                        });
                    }
                }
            }

            // Asignar la lista al ViewBag para que esté disponible en la vista
            ViewBag.TipoProductoList = tiposProductoList;

            return View();
        }

        
        [HttpPost]
        public ActionResult AgregarProducto(Productos ObjProducto)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Producto", ObjConexion);
                cmd.Parameters.AddWithValue("Tipo", ObjProducto.Nombre_Tipo);
                cmd.Parameters.AddWithValue("Nombre", ObjProducto.Nombre);
                cmd.Parameters.AddWithValue("Stock", ObjProducto.Cantidad);

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
                cmd.Parameters.AddWithValue("Stock",ObjProducto.Cantidad);
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