using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using IngenieriaProyecto.ViewModels;

namespace IngenieriaProyecto.Models;

public class AppConfigModel
{
    private static AppConfigModel _instance;
    private ViewModelBase _paginaActual;
    private Window _loginWindow;
    private Window _dashWindow;

    public static AppConfigModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AppConfigModel();
            }

            return _instance;
        }
    }

    public Window DashWindow
    {
        get => _dashWindow;
        set => _dashWindow = value ?? throw new ArgumentNullException(nameof(value));
    }
    public Window LoginWindow
    {
        get => _loginWindow;
        set => _loginWindow = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ViewModelBase PaginaActual
    {
        get => _paginaActual;
        set => _paginaActual = value ?? throw new ArgumentNullException(nameof(value));
    }
}