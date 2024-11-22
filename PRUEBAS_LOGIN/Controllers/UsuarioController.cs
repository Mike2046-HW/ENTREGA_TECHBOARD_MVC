using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class UsuarioController : Controller
    {
        //Cadena de conexión
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        //Lista de usuarios
        private static List<Usuario> ObjLista = new List<Usuario>();

        [PermisosRol(Rol.Admin)]
        public ActionResult Usuarios()
        {
            ObjLista = new List<Usuario>();

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM v_Usuarios", ObjConexion);
                cmd.CommandType = CommandType.Text;
                ObjConexion.Open();

                using (SqlDataReader Lector = cmd.ExecuteReader())
                {
                    while (Lector.Read())
                    {
                        Usuario nuevoUsuario = new Usuario();

                        nuevoUsuario.IdUsuario = Convert.ToInt32(Lector["ID"]);
                        nuevoUsuario.Nombre = Lector["Nombre"].ToString();
                        nuevoUsuario.Correo = Lector["Correo"].ToString();
                        //Convertir valor de Rol de (enum) a un (int) para la BD
                        nuevoUsuario.Id_rol = (Rol)Enum.Parse(typeof(Rol), Lector["id_rol"].ToString());

                        ObjLista.Add(nuevoUsuario);
                    }
                }
            }

            // Retornar la vista con la lista de usuarios
            return View(ObjLista);
        }

        /*Agregar usuarios como Administrador*/
        [HttpGet]
        [PermisosRol(Rol.Admin)]
        public ActionResult AgregarUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AgregarUsuario(Usuario ObjUsuario)
        {
            bool registrado;
            string mensaje;

            if (ObjUsuario.Clave == ObjUsuario.ConfirmarClave)
            {
                //Se encripta la contraseña
                ObjUsuario.Clave = ConvertirSha256(ObjUsuario.Clave);
            }
            /*En caso de no coicidir ViewData["mensaje"] obtiene de valor el mensaje*/
            else
            {
                ViewData["mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Usuario", ObjConexion);
                cmd.Parameters.AddWithValue("Nombre", ObjUsuario.Nombre);
                cmd.Parameters.AddWithValue("Correo", ObjUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", ObjUsuario.Clave);
                cmd.Parameters.AddWithValue("Id_rol", (int)ObjUsuario.Id_rol);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();

                //Obtienen los valores de los parámetos de salida
                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();

            }
            //Asigna el ViewData el valor de la variable mensaje
            ViewData["mensaje"] = mensaje;

            /*Si el valor de la variable registrado es true entonces se re dirige
              a la vista Login del controlador Acceso*/
            if (registrado == true)
            {
                return RedirectToAction("Usuarios", "Usuario");
            }

            //En caso de un false se retornará a la vista actual
            else
            {
                return View();
            }

        }


        /*Editar Usuarios*/
        [HttpGet]
        [PermisosRol(Rol.Admin)]
        public ActionResult EditarUsuario(int? IdUsuario)
        {
            if (IdUsuario == null)
                return RedirectToAction("Usuarios", "Usuario");

            Usuario ObjUsuario = ObjLista.Where(p => p.IdUsuario == IdUsuario).FirstOrDefault();

            return View(ObjUsuario);
        }

        [HttpPost]
        public ActionResult EditarUsuario(Usuario ObjUsuario)
        {

            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Editar_Usuario", ObjConexion);
                cmd.Parameters.AddWithValue("IdUsuario", ObjUsuario.IdUsuario);
                cmd.Parameters.AddWithValue("Nombre", ObjUsuario.Nombre);
                cmd.Parameters.AddWithValue("Correo", ObjUsuario.Correo);
                cmd.Parameters.AddWithValue("Rol", (int)ObjUsuario.Id_rol);
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Usuarios", "Usuario");
        }

        /*Eliminar Usuario*/
        [HttpGet]
        [PermisosRol(Rol.Admin)]
        public ActionResult EliminarUsuario(int? IdUsuario)
        {
            if (IdUsuario == null)
                return RedirectToAction("Usuarios", "Usuario");

            Usuario ObjUsuario = ObjLista.Where(p => p.IdUsuario == IdUsuario).FirstOrDefault();

            return View(ObjUsuario);
        }

        [HttpPost]
        public ActionResult EliminarUsuario(string IdUsuario)
        {
            using (SqlConnection ObjConexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Eliminar_Usuario", ObjConexion);
                cmd.Parameters.AddWithValue("IdUsuario", IdUsuario);
                cmd.CommandType = CommandType.StoredProcedure;
                ObjConexion.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }

                catch (Exception)
                {
                    TempData["mensaje"] = "No es posible eliminar el usuario en este momento.";
                }

            }

            return RedirectToAction("Usuarios", "Usuario");
        }

        //Método de encriptación SHA256
        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

    }
}