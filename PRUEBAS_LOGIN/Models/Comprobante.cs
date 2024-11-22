using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Comprobante
    {
        // Campos de la tabla Pedido
        public int ID { get; set; }
        public DateTime FechaPedido { get; set; }

        // Campos de la tabla Paqueteria
        public string Paqueteria { get; set; }
        public string CorreoPaqueteria { get; set; }
        public string TelefonoPaqueteria { get; set; }

        // Campos de la tabla Comprador
        public string EmpresaComprador { get; set; }
        public string CorreoComprador { get; set; }
        public string TelefonoComprador { get; set; }

        // Campos de la tabla Direccion_Comprador
        public string CalleComprador { get; set; }
        public string CiudadComprador { get; set; }
        public string EstadoComprador { get; set; }
        public string CodigoPostalComprador { get; set; }

        // Campos de la tabla Pedido_Producto
        public int Cantidad { get; set; }

        // Campos de la tabla Producto
        public string Producto { get; set; }

        // Campos de la tabla Tipo_Producto
        public string CategoriaProducto { get; set; }

        // Campos de la tabla Vendedor
        public string EmpresaVendedor { get; set; }
        public string CorreoVendedor { get; set; }
        public string TelefonoVendedor { get; set; }

        // Campos de la tabla Direccion_Vendedor
        public string CalleVendedor { get; set; }
        public string CiudadVendedor { get; set; }
        public string EstadoVendedor { get; set; }
        public string CodigoPostalVendedor { get; set; }
    }
}