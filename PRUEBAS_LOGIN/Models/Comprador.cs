using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Comprador
    {
        //TABLA DIRECCIÓN DE COMPRADOR
        public int Direccion_Comprador_Id { get; set; }
        public string Calle {  get; set; }
        public string Ciudad {  get; set; }
        public string Estado { get; set; }
        public string Codigo_Postal { get; set; }

        //TABLA DE COMPRADOR
        public int Comprador_Id { get; set; }
        public string Nombre_Empresa { get; set; }
        public string RFC { get; set; }
        public string Correo_Electronico { get; set; }
        public string Telefono {  get; set; }

    }
}