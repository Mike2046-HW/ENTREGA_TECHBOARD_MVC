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
    public class CompradorController : Controller
    {

        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        private static List<Comprador> ObjLista = new List<Comprador>();

        public ActionResult Compradores()
        {
            ObjLista = new List<Comprador>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM v_Comprador", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Comprador nuevoComprador = new Comprador();

                        nuevoComprador.Comprador_Id = Convert.ToInt32(Lector["ID"]);
                        nuevoComprador.Nombre_Empresa = Lector["Empresa"].ToString();
                        nuevoComprador.RFC = Lector["RFC"].ToString();
                        nuevoComprador.Correo_Electronico = Lector["Correo"].ToString();
                        nuevoComprador.Telefono = Lector["Telefono"].ToString();
                        nuevoComprador.Calle = Lector["Calle"].ToString();
                        nuevoComprador.Direccion_Comprador_Id = Convert.ToInt32(Lector["IDR"]);
                        nuevoComprador.Ciudad = Lector["Ciudad"].ToString();
                        nuevoComprador.Estado = Lector["Estado"].ToString();
                        nuevoComprador.Codigo_Postal = Lector["CP"].ToString();

                        ObjLista.Add(nuevoComprador);
                    }
                }
            }

            return View(ObjLista);
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult RegistrarComprador()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarComprador(Comprador ObjComprador)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Comprador", ObjConexion);
                cmd.Parameters.AddWithValue("Calle", ObjComprador.Calle);
                cmd.Parameters.AddWithValue("Ciudad", ObjComprador.Ciudad);
                cmd.Parameters.AddWithValue("Estado", ObjComprador.Estado);
                cmd.Parameters.AddWithValue("Codigo_Postal", ObjComprador.Codigo_Postal);
                cmd.Parameters.AddWithValue("Nombre_Empresa", ObjComprador.Nombre_Empresa);
                cmd.Parameters.AddWithValue("RFC", ObjComprador.RFC);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjComprador.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjComprador.Telefono);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Compradores","Comprador");
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult EditarComprador(int? idcomprador)
        {
            if (idcomprador == null)
                return RedirectToAction("Compradores", "Comprador");

            Comprador ObjComprador = ObjLista.Where(c=> c.Comprador_Id == idcomprador).FirstOrDefault();

            return View(ObjComprador);
        }

        [PermisosRol(Rol.Admin)]
        [HttpPost]
        public ActionResult EditarComprador(Comprador ObjComprador)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Editar_Comprador", ObjConexion);
                cmd.Parameters.AddWithValue("Comprador_Id", ObjComprador.Comprador_Id);
                cmd.Parameters.AddWithValue("Nombre_Empresa", ObjComprador.Nombre_Empresa);
                cmd.Parameters.AddWithValue("RFC", ObjComprador.RFC);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjComprador.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjComprador.Telefono);
                cmd.Parameters.AddWithValue("Direccion_Comprador_Id", ObjComprador.Direccion_Comprador_Id);
                cmd.Parameters.AddWithValue("Calle", ObjComprador.Calle);
                cmd.Parameters.AddWithValue("Ciudad", ObjComprador.Ciudad);
                cmd.Parameters.AddWithValue("Estado", ObjComprador.Estado);
                cmd.Parameters.AddWithValue("Codigo_Postal", ObjComprador.Codigo_Postal);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Compradores", "Comprador");
        }

        [PermisosRol(Rol.Admin)]
        [HttpGet]
        public ActionResult EliminarComprador(int? idcomprador)
        {
            if (idcomprador == null)
                return RedirectToAction("Compradores", "Comprador");

            Comprador ObjComprador = ObjLista.Where(c => c.Comprador_Id == idcomprador).FirstOrDefault();

            return View(ObjComprador);
        }

        [PermisosRol(Rol.Admin)]
        [HttpPost]
        public ActionResult EliminarComprador(string Comprador_Id)
        {

                using (SqlConnection ObjConexion = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarComprador", ObjConexion);
                    cmd.Parameters.AddWithValue("Comprador_Id", Comprador_Id);

                    cmd.CommandType = CommandType.StoredProcedure;
                    ObjConexion.Open();

                try { 
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                    {
                        TempData["mensaje"] = "No es posible eliminar al comprador en este momento.";
                    }              
                }
                return RedirectToAction("Compradores", "Comprador");

        }

    }
}