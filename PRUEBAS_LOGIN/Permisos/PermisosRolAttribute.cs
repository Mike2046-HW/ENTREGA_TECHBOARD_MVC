using PRUEBAS_LOGIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PRUEBAS_LOGIN.Permisos
{
    public class PermisosRolAttribute : ActionFilterAttribute
    {
        /*private Rol Id_rol;

        public PermisosRolAttribute(Rol _idrol)
        {
            Id_rol = _idrol;
        }*/

        private readonly Rol[] _rolesPermitidos;

        public PermisosRolAttribute(params Rol[] roles)
        {
            _rolesPermitidos = roles;
        }

        /*Redirecciona al usuario en caso de que no tenga el rol necesario para
        acceder a ciertas paginas*/
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["usuario"] != null)
            {
                Usuario usuario = HttpContext.Current.Session["usuario"] as Usuario;

                if (!_rolesPermitidos.Contains(usuario.Id_rol))
                {
                    filterContext.Result = new RedirectResult("~/Home/RolInvalido");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}