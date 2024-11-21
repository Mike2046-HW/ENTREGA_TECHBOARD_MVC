using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Pedido
    {

        public int Comprador_Id { get; set; }
        public int Producto_Id { get; set; }
        public int Cantidad { get; set; }
        public int Paqueteria_Id { get; set; }
        public string Estado { get; set; }
        public int Vendedor_Id { get; set; }


        //View v_Detalles_Pedido
        public int ID { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Comprador { get; set; }

        //Parámetros de Estado_Envio
        //public string Estado { get; set; }

        public string Paqueteria { get; set; }
        public string NombreProducto { get; set; }
        public string TipoDeProducto { get; set; }
        //public int Cantidad { get; set; }
        public string Vendedor { get; set; }
        //---------------------------------------//
        //---------------------------------------//
        //---------------------------------------//


        //Parámetros de dirección de Comprador
        public string Calle_Comprador { get; set; }
        public string Ciudad_Comprador { get; set; }
        public string Estado_Comprador { get; set; }
        public string Codigo_Postal_Comprador { get; set; }

        //Parámetros de Comprador
        public string Nombre_Empresa_Comprador { get; set; }
        public string RFC_Comprador { get; set; }
        public string Correo_Comprador { get; set; }
        public string Telefono_Comprador { get; set; }

        //Parámetros de dirección del Vendedor
        public string Calle_Vendedor { get; set; }
        public string Ciudad_Vendedor { get; set; }
        public string Estado_Vendedor { get; set; }
        public string Codigo_Postal_Vendedor { get; set; }

        //Parámetros de vendedor
        public string Nombre_Empresa_Vendedor { get; set; }
        public string Correo_Vendedor { get; set; }
        public string Telefono_Vendedor { get; set; }

        //Parámetro tipo de producto
        public string Nombre_Tipo_Producto { get; set; }

        //Parámetros de producto
        public string Nombre_Producto { get; set; }
        public int Cantidad_Producto { get; set; }

        //Parámetros de paqueteria
        public string Nombre_Paqueteria { get; set; }
        public string Correo_Paqueteria { get; set; }
        public string Telefono_Paqueteria { get; set; }

        //Parámetros de Pedido_Producto
        public int Cantidad_Pedido { get; set; }

    }

}
