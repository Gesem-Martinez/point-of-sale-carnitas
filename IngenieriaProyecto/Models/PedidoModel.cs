using System;
using System.Collections.Generic;

namespace IngenieriaProyecto.Models;
public class PedidoModel
{
    public int NumeroPedido { get; set; }
    public List<ProductoModel> Productos { get; set; }

    public PedidoModel()
    {
        Productos = new List<ProductoModel>();
    }
}

public class ProductoModel
{
    public int CantidadProducto { get; set; }

    public string NombreProducto { get; set;  }
    public int IdProducto { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioTotal { get; set; }
}

public class ProductoDisponibleModel
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; }
    
}