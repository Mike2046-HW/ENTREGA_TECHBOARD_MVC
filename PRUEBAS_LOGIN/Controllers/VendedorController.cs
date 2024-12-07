using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class VendedorController : Controller
    {

        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        private static List<Vendedor> ObjLista = new List<Vendedor>();

        public ActionResult Vendedores()
        {
            ObjLista = new List<Vendedor>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM v_Vendedor", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Vendedor nuevoVendedor = new Vendedor();

                        nuevoVendedor.Vendedor_Id = Convert.ToInt32(Lector["ID"]);
                        nuevoVendedor.Nombre_Empresa = Lector["Empresa"].ToString();
                        nuevoVendedor.Correo_Electronico = Lector["Email"].ToString();
                        nuevoVendedor.Telefono = Lector["Telefono"].ToString();
                        nuevoVendedor.Direccion_Vendedor_Id = Convert.ToInt32(Lector["IDdir"]);
                        nuevoVendedor.Calle = Lector["Calle"].ToString();
                        nuevoVendedor.Ciudad = Lector["Ciudad"].ToString();
                        nuevoVendedor.Estado = Lector["Estado"].ToString();
                        nuevoVendedor.Codigo_Postal = Lector["CP"].ToString();

                        ObjLista.Add(nuevoVendedor);
                    }
                }
            }

            return View(ObjLista);
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult RegistrarVendedor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarVendedor(Vendedor ObjVendedor)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Vendedor", ObjConexion);
                cmd.Parameters.AddWithValue("Calle", ObjVendedor.Calle);
                cmd.Parameters.AddWithValue("Ciudad", ObjVendedor.Ciudad);
                cmd.Parameters.AddWithValue("Estado", ObjVendedor.Estado);
                cmd.Parameters.AddWithValue("Codigo_Postal", ObjVendedor.Codigo_Postal);
                cmd.Parameters.AddWithValue("Nombre_Empresa", ObjVendedor.Nombre_Empresa);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjVendedor.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjVendedor.Telefono);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Vendedores", "Vendedor");
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult EditarVendedor(int? idvendedor)
        {
            if (idvendedor == null)
                return RedirectToAction("Vendedores", "Vendedor");

            Vendedor ObjVendedor = ObjLista.Where(v => v.Vendedor_Id == idvendedor).FirstOrDefault();

            return View(ObjVendedor);
        }

        [PermisosRol(Rol.Admin)]
        [HttpPost]
        public ActionResult EditarVendedor(Vendedor ObjVendedor)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Editar_Vendedor", ObjConexion);
                cmd.Parameters.AddWithValue("Vendedor_Id", ObjVendedor.Vendedor_Id);
                cmd.Parameters.AddWithValue("Nombre_Empresa", ObjVendedor.Nombre_Empresa);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjVendedor.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjVendedor.Telefono);
                cmd.Parameters.AddWithValue("Direccion_Vendedor_Id", ObjVendedor.Direccion_Vendedor_Id);
                cmd.Parameters.AddWithValue("Calle", ObjVendedor.Calle);
                cmd.Parameters.AddWithValue("Ciudad", ObjVendedor.Ciudad);
                cmd.Parameters.AddWithValue("Estado", ObjVendedor.Estado);
                cmd.Parameters.AddWithValue("Codigo_Postal", ObjVendedor.Codigo_Postal);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Vendedores", "Vendedor");
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult EliminarVendedor(int? idvendedor)
        {
            if (idvendedor == null)
                return RedirectToAction("Vendedores", "Vendedor");

            Vendedor ObjVendedor = ObjLista.Where(v => v.Vendedor_Id == idvendedor).FirstOrDefault();

            return View(ObjVendedor);
        }

        [PermisosRol(Rol.Admin)]
        [HttpPost]
        public ActionResult EliminarVendedor(string Vendedor_Id)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarVendedor", ObjConexion);
                cmd.Parameters.AddWithValue("Vendedor_Id", Vendedor_Id);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    TempData["mensaje"] = "No es posible eliminar al vendedor en este momento.";
                }
            }
            return RedirectToAction("Vendedores", "Vendedor");

        }

    }
}