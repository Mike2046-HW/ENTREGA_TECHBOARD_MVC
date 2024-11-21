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
    public class PaqueteriaController : Controller
    {
        //Cadena de conexión
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();

        private static List<Paqueteria> ObjLista = new List<Paqueteria>();


        public ActionResult Paqueteria()
        {

            ObjLista = new List<Paqueteria>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("Select * From Paqueteria", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Paqueteria nuevaPaqueteria = new Paqueteria();
                        nuevaPaqueteria.Paqueteria_Id = Convert.ToInt32(Lector["Paqueteria_Id"]);
                        nuevaPaqueteria.Nombre = Lector["Nombre"].ToString();
                        nuevaPaqueteria.Correo_Electronico = Lector["Correo_Electronico"].ToString();
                        nuevaPaqueteria.Telefono = Lector["Telefono"].ToString();

                        ObjLista.Add(nuevaPaqueteria);

                    }
                }

            }
            return View(ObjLista);
        }

        [HttpGet]
        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(Paqueteria ObjPaqueteria)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("Registrar_Paqueteria", ObjConexion);
                cmd.Parameters.AddWithValue("Nombre", ObjPaqueteria.Nombre);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjPaqueteria.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjPaqueteria.Telefono);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();

            }
            //return RedirectToAction("MÉTODO","CONTROLADOR");
            return RedirectToAction("Paqueteria", "Paqueteria");
        }

        [HttpGet]
        public ActionResult Editar(int? Paqueteria_Id)
        {

            if (Paqueteria_Id == null)
                return RedirectToAction("Paqueteria", "Paqueteria");

            Paqueteria ObjPaqueteria = ObjLista.Where(p => p.Paqueteria_Id == Paqueteria_Id).FirstOrDefault();

            return View(ObjPaqueteria);
        }

        [HttpPost]
        public ActionResult Editar(Paqueteria ObjPaqueteria)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("Editar_Paqueteria", ObjConexion);
                cmd.Parameters.AddWithValue("Paqueteria_Id", ObjPaqueteria.Paqueteria_Id);
                cmd.Parameters.AddWithValue("Nombre", ObjPaqueteria.Nombre);
                cmd.Parameters.AddWithValue("Correo_Electronico", ObjPaqueteria.Correo_Electronico);
                cmd.Parameters.AddWithValue("Telefono", ObjPaqueteria.Telefono);

                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();

            }

            return RedirectToAction("Paqueteria", "Paqueteria");
        }

        [HttpGet]
        public ActionResult Eliminar(int? Paqueteria_Id)
        {

            if (Paqueteria_Id == null)
                return RedirectToAction("Paqueteria", "Paqueteria");

            Paqueteria ObjPaqueteria = ObjLista.Where(p => p.Paqueteria_Id == Paqueteria_Id).FirstOrDefault();

            return View(ObjPaqueteria);
        }

        //AGREGAR UN INDICADOR QUE DIGA "NO PUEDES ELIMINAR ESTA PAQUETERIA EN ESTE MOMENTO"
        [HttpPost]
        public ActionResult Eliminar(string Paqueteria_Id)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("Eliminar_Paqueteria", ObjConexion);
                cmd.Parameters.AddWithValue("Paqueteria_Id", Paqueteria_Id);
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();
                try 
                    {
                        cmd.ExecuteNonQuery();
                    }

                catch (Exception)
                    {
                        TempData["mensaje"] = "No es posible eliminar la paqueteria en este momento.";
                    }

            }

            return RedirectToAction("Paqueteria", "Paqueteria");
        }

    }
}