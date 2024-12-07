using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Vendedor
    {
        //TABLA DIRECCIÓN DE VENDEDOR
        public int Direccion_Vendedor_Id { get; set; }
        public string Calle { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Codigo_Postal { get; set; }

        //TABLA DE VENDEDOR 
        public int Vendedor_Id { get; set; }
        public string Nombre_Empresa { get; set; }
        public string Correo_Electronico { get; set; }
        public string Telefono { get; set; }
    }
}