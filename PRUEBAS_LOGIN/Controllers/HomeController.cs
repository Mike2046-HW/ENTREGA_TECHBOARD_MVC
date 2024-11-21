using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PRUEBAS_LOGIN.Controllers
{

    [ValidarSesion]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }

        public ActionResult Pedidos()
        {
            return View();
        }

        public ActionResult Paqueteria()
        {
            return View();
        }

        [PermisosRol(Rol.Admin)]
        public ActionResult Usuarios()
        {
            return View();
        }

        //Error en caso de no tener el rol necesario
        public ActionResult Rolinvalido()
        {
            return View();
        }

    }
}