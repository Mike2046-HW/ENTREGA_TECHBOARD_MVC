using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Productos
    {
        public int Producto_Id { get; set; }
        public string Nombre { get; set; }
        public string Nombre_Tipo { get; set; }

        public int Cantidad { get; set; }
    }
}