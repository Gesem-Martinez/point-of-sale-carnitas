using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using MySql.Data.MySqlClient;

namespace IngenieriaProyecto.Models;

public class DatabaseManagerModel
{
    private static DatabaseManagerModel _instance;
    private MySqlConnection _conexion;
    private bool _isOpen;

    public DatabaseManagerModel()
    {
        string conexionString = "server=localhost;user=gesem;database=IngenieriaFinal;port=3306;password=daka08082016@";
        _conexion = new MySqlConnection(conexionString);
        _isOpen = false;
    }

    public static DatabaseManagerModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DatabaseManagerModel();
            }

            return _instance;
        }
    }

    public bool IsOpen => _isOpen;

    public void Conectar()
    {
        try
        {
            _conexion.Open();
            _isOpen = true;
            Console.WriteLine("CONECTADO con la base de datos.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _isOpen = false;
            throw;
        }
    }

    public void Desconectar()
    {
        try
        {
            Console.WriteLine("Cerrando conexion con DB...");
            _conexion.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public MySqlDataReader Consulta(string query)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, _conexion);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<EmpleadoModel> ConsultaMeseros()
    {
        List<EmpleadoModel> meseros = new List<EmpleadoModel>();
        try
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Empleados ORDER BY rol", _conexion);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    meseros.Add(new EmpleadoModel((int)reader["ID"], (string)reader["nombre"], reader["rol"].ToString(), (decimal)reader["salario"], (int)reader["usuario_id"]));
                }
            }
            return meseros;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string ConsultaUsername(int usuarioId)
    {
        string query = $"SELECT usuario FROM Usuarios WHERE ID = {usuarioId}";
        string username = "";
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, _conexion);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    username = (string)reader["usuario"];
                }
            }
            return username;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public string ConsultaPassword(int usuarioId)
    {
        string query = $"SELECT contrasena FROM Usuarios WHERE ID = {usuarioId}";
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, _conexion);
            MySqlDataReader reader = cmd.ExecuteReader();
            string password = "";

            using (reader)
            {
                while (reader.Read())
                {
                    password = reader["contrasena"].ToString();
                }
            }
            return password;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public string ConsultaAgregarUsuario(string nombre, string usuario, string password, string rol, decimal salario)
    {
        int usuarioId = 0;
        string mensajeConfirmacion = "";

        try
        {
            // Verificar si el usuario ya existe en la tabla Usuarios
            string checkUserQuery = "SELECT COUNT(*) FROM Usuarios WHERE usuario = @Usuario";
            using (MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, _conexion))
            {
                checkUserCmd.Parameters.AddWithValue("@Usuario", usuario);
                int userCount = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                if (userCount > 0)
                {
                    // El usuario ya existe, manejar adecuadamente
                    Console.WriteLine("El usuario ya existe.");
                    mensajeConfirmacion = "El usuario ya existe.";
                    return mensajeConfirmacion;
                }
            }
        
            string checkNameQuery = "SELECT COUNT(*) FROM Empleados WHERE nombre = @Nombre";
            using (MySqlCommand checkUserCmd = new MySqlCommand(checkNameQuery, _conexion))
            {
                checkUserCmd.Parameters.AddWithValue("@Nombre", nombre);
                int userCount = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                if (userCount > 0)
                {
                    // El usuario ya existe, manejar adecuadamente
                    Console.WriteLine("El nombre ya existe.");
                    mensajeConfirmacion = "El usuario ya existe.";
                    return mensajeConfirmacion;
                }
            }
        
            using (MySqlTransaction transaction = _conexion.BeginTransaction())
            {
                try
                {
                    // Insertar en la tabla Usuarios
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Usuarios(usuario, contrasena) VALUES(@Usuario, @Password)", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Usuario", usuario);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.ExecuteNonQuery();
                    }

                    // Obtener el ID del usuario recién insertado
                    string getUsuarioId = "SELECT LAST_INSERT_ID()";
                    using (MySqlCommand cmdDos = new MySqlCommand(getUsuarioId, _conexion, transaction))
                    {
                        usuarioId = Convert.ToInt32(cmdDos.ExecuteScalar());
                    }

                    // Insertar en la tabla Empleados
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Empleados(nombre, rol, salario, usuario_id) VALUES(@Nombre, @Rol, @Salario, @UsuarioId)", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Rol", rol);
                        cmd.Parameters.AddWithValue("@Salario", salario);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmd.ExecuteNonQuery();
                    }

                    mensajeConfirmacion = "Empleado agregado exitosamente";
                    // Confirmar la transacción
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    // Revertir la transacción en caso de error
                    transaction.Rollback();
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return mensajeConfirmacion;
    }
    
    public string ConsultaEditarUsuario(string nombre, string usuario, string password, string rol, decimal salario, int empleadoId)
    {
        int usuarioId = 0;
        string mensajeConfirmacion = "";

        try
        {
            using (MySqlTransaction transaction = _conexion.BeginTransaction())
            {
                try
                {
                    using (MySqlCommand cmdDos = new MySqlCommand("SELECT usuario_id FROM Empleados WHERE ID = @EmpleadoID", _conexion, transaction))
                    {
                        cmdDos.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        usuarioId = Convert.ToInt32(cmdDos.ExecuteScalar());
                    }
                    
                    // Insertar en la tabla Usuarios
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE Usuarios SET usuario = @Usuario, contrasena = @Password WHERE ID = @UsuarioID", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Usuario", usuario);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);
                        cmd.ExecuteNonQuery();
                    }


                    // Insertar en la tabla Empleados
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE Empleados SET nombre = @Nombre, rol = @Rol, salario = @Salario WHERE ID = @EmpleadoID", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Rol", rol);
                        cmd.Parameters.AddWithValue("@Salario", salario);
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        
                        Console.WriteLine(cmd.CommandText);
                        foreach (MySqlParameter param in cmd.Parameters)
                        {
                            Console.WriteLine($"{param.ParameterName}: {param.Value}");
                        }
                        cmd.ExecuteNonQuery();
                    }

                    mensajeConfirmacion = "Empleado editado exitosamente";
                    // Confirmar la transacción
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    // Revertir la transacción en caso de error
                    transaction.Rollback();
                    throw;
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return mensajeConfirmacion;
    }

    public void ConsultaEliminarEmpleado(int empleadoId)
    {
        try
        {
            using (MySqlTransaction transaction = _conexion.BeginTransaction())
            {
                try
                {
                    // Obtener el usuario_id del empleado
                    int usuarioId;
                    using (MySqlCommand cmd = new MySqlCommand("SELECT usuario_id FROM Empleados WHERE ID = @EmpleadoID", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        usuarioId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Eliminar el empleado
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Empleados WHERE ID = @EmpleadoID", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        cmd.ExecuteNonQuery();
                    }

                    // Eliminar el usuario
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Usuarios WHERE ID = @UsuarioID", _conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);
                        cmd.ExecuteNonQuery();
                    }

                    // Confirmar la transacción
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    // Revertir la transacción en caso de error
                    transaction.Rollback();
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public List<PedidoModel> ConsultaPedidos()
    {
        List<PedidoModel> pedidos = new List<PedidoModel>();

        try
        {
            string query = @"
                SELECT 
                    p.NumeroPedido, 
                    p.cantidadProducto, 
                    p.IDProducto, 
                    p.precioUnitario, 
                    p.precioTotal, 
                    pr.NombreProducto
                FROM 
                    Pedidos p
                INNER JOIN 
                    Productos pr ON p.IDProducto = pr.ID
                ORDER BY 
                    p.NumeroPedido";
                    
            using (MySqlCommand cmd = new MySqlCommand(query, _conexion))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int numeroPedido = reader.GetInt32("NumeroPedido");
                        var pedido = pedidos.FirstOrDefault(p => p.NumeroPedido == numeroPedido);
                        if (pedido == null)
                        {
                            pedido = new PedidoModel
                            {
                                NumeroPedido = numeroPedido
                            };
                            pedidos.Add(pedido);
                        }

                        var producto = new ProductoModel
                        {
                            CantidadProducto = reader.GetInt32("cantidadProducto"),
                            IdProducto = reader.GetInt32("IDProducto"),
                            PrecioUnitario = reader.GetDecimal("precioUnitario"),
                            PrecioTotal = reader.GetDecimal("precioTotal"),
                            NombreProducto = reader.GetString("NombreProducto")
                        };

                        pedido.Productos.Add(producto);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return pedidos;
    }
    
    public List<ProductoDisponibleModel> ConsultaProductos()
    {
        List<ProductoDisponibleModel> productos = new List<ProductoDisponibleModel>();
        try
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Productos", _conexion);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    productos.Add(new ProductoDisponibleModel
                    {
                        IdProducto = (int)reader["ID"],
                        NombreProducto = (string)reader["nombreProducto"]
                    });
                }
            }
            return productos;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void ConsultaAgregarPedido(PedidoModel pedido)
    {
        try
        {
            using (MySqlTransaction transaction = _conexion.BeginTransaction())
            {
                try
                {
                    foreach (var producto in pedido.Productos)
                    {
                        string query = @"
                            INSERT INTO Pedidos (NumeroPedido, cantidadProducto, IDProducto, precioUnitario, precioTotal)
                            VALUES (@NumeroPedido, @CantidadProducto, @IDProducto, @PrecioUnitario, @PrecioTotal)";
                        
                        using (MySqlCommand cmd = new MySqlCommand(query, _conexion, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NumeroPedido", pedido.NumeroPedido);
                            cmd.Parameters.AddWithValue("@CantidadProducto", producto.CantidadProducto);
                            cmd.Parameters.AddWithValue("@IDProducto", producto.IdProducto);
                            cmd.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);
                            cmd.Parameters.AddWithValue("@PrecioTotal", producto.PrecioTotal);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<InventarioModel> ConsultaInventario()
    {
        List<InventarioModel> inventario = new List<InventarioModel>();

        try
        {
            string query = "SELECT ID, nombreProducto, cantidad, precioUnitario, fecha FROM Inventario ORDER BY ID";
            using (MySqlCommand cmd = new MySqlCommand(query, _conexion))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new InventarioModel
                        {
                            Id = reader.GetInt32("ID"),
                            NombreProducto = reader.GetString("nombreProducto"),
                            Cantidad = reader.GetInt32("cantidad"),
                            PrecioUnitario = reader.GetDecimal("precioUnitario"),
                            Fecha = reader.GetDateTime("fecha")
                        };
                        inventario.Add(item);
                    }
                }
            }
        } catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return inventario;
    }
    
    public UsuarioModel VerificarCredenciales(string usuario, string contrasena)
    {
        try
        {
            string query = "SELECT ID, usuario, contrasena FROM Usuarios WHERE usuario = @Usuario AND contrasena = @Contrasena";
            using (MySqlCommand cmd = new MySqlCommand(query, _conexion))
            {
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UsuarioModel
                        {
                            Id = reader.GetInt32("ID"),
                            Username = reader.GetString(reader.GetOrdinal("usuario")),
                            Password = reader.GetInt32("contrasena").ToString()
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }
}