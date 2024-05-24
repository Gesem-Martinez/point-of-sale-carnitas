using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IngenieriaProyecto.Models;

public class EmpleadoModel
{
    public EmpleadoModel(int id, string nombre, string rol, decimal salario, int usuarioId)
    {
        Id = id;
        Nombre = nombre;
        Rol = rol;
        Salario = salario;
        UsuarioId = usuarioId;

    }

    public string Username
    {
        get;
        set;
    }

    public string Password
    {
        get;
        set;
    }
    
    public int Id
    {
        get;
    }

    public string Nombre
    {
        get;
    }

    public string Rol
    {
        get;
    }

    public decimal Salario
    {
        get;
    }

    public int UsuarioId
    {
        get;
    }
}
