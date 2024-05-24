using System;

namespace IngenieriaProyecto.Models;

public class InventarioModel
{
    public int Id { get; set; }
    public string NombreProducto { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    
    public decimal PrecioUnitario { get; set; }
}