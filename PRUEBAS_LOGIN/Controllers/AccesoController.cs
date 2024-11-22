using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

using PRUEBAS_LOGIN;
using PRUEBAS_LOGIN.Models;
using System.Web.Services.Description;
using System.Configuration;

namespace PRUEBAS_LOGIN.Controllers
{
    public class AccesoController : Controller
    {
        // Cadena de conexión
        static string cadena = ConfigurationManager.ConnectionStrings["cadena"].ToString();

        // Método para acceder a la vista de loggeo
        public ActionResult Login()
        {
            return View();
        }

        // Método para acceder a la vista de registro
        public ActionResult Registrar()
        {
            return View();
        }

         /*Método para usar procedimiento almacenado de registro
         * 
         *Como se va a trabajar con las propiedades de la clase Usuario
         se cre un objeto llamado oUsuario y se ingresa en el método 
         como un parámetro*/
        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            /*Este condicional compara la clave ingresada en un registro de usuario con su
            confirmación, en caso de coincidir la clave dentro del objeto oUsuario se 
            encripta en el formato SHA256*/
            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                //Se encripta la contraseña
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            /*En caso de no coicidir ViewData["mensaje"] obtiene de valor el mensaje*/
            else
            {
                ViewData["mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            //Se abre una conexión a la base de datos con un objeto SqlConnection
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                //Se crea un objeto SqlCommand llamdo cmd para ejecutar el procedimiento almacenado de la base de datos
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);

                /*Al objeto cmd de tipo SqlCommand se le agregan un parámetro y un nombre
                 en este caso Correo, Clave y Nombre son los nombres de los parámetros
                 y las propiedades del objeto oUsuario son los valores del mismo*/
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);

                /*En esta parte se agregan dos parámetros de salida que irán a la base de datos
                 para indicar el registro exitoso del usuario*/
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                //Abre la conexion con la BD
                cn.Open();
                //Executa el procedimiento almacenado sp_Registrar 
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
                return RedirectToAction("Login", "Acceso");
            }

            //En caso de un false se retornará a la vista actual
            else 
            {
                return View();
            }
        }

        [HttpPost]

        //Se crea un método para loggearse y se agrega de parametro el objeto oUsuario
        public ActionResult Login(Usuario oUsuario)
        {
            //Se encripta la contraseña recibida
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            //Se crea un objeto SqlConnection 
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                //Se crea un objeto SqlCommand que ejecutarpa el procedimiento almacenado sp_ValidarUsuario
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario ", cn);
                //Se asocian las propiedades del objeto con los parámetros del procedimiento
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                //Se abre la conexión
                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                try
                {
                    // Obtener los roles del usuario
                    cmd = new SqlCommand("sp_ObtenerRolesPorUsuario", cn);
                    cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                    cmd.CommandType = CommandType.StoredProcedure;
                    int rolId = (int)cmd.ExecuteScalar();

                    // Asigna el rol obtenido
                    oUsuario.Id_rol = (Rol)rolId;
                }
                catch (Exception)
                {
                    TempData["mensaje2"] = "Revise sus credenciales ususario no encontrado.";
                }
            }

            /*En caso de que el Id del usuario sea diferente a 0,
              es decir que si exista entonces se redirigirá a la
              vista Index del controlador Home*/
            if (oUsuario.IdUsuario != 0)
            {
                Session["usuario"] = oUsuario;
                return RedirectToAction("Index", "Home");
            }

            //Caso contrario ViewData tomará el valor del mensaje y se retornará a la vista actual
            else 
            {
                ViewData["mensaje"] = "Usuario no encontrado";
                return View();
            }
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