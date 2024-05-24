using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngenieriaProyecto.Models;
using IngenieriaProyecto.Services;
using IngenieriaProyecto.Views;

namespace IngenieriaProyecto.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Window _ventanaActual;
    private readonly DatabaseManagerModel _dbMan;
    private readonly UserSessionService _userSessionService;

    public MainWindowViewModel(Window ventanaActual)
    {
        this._ventanaActual = ventanaActual;
        AppConfigModel.Instance.LoginWindow = ventanaActual;
        _dbMan = DatabaseManagerModel.Instance;
        _userSessionService = new UserSessionService();
        _dbMan.Conectar();
    }

    // Propiedades
    private string _nombreUsuario;
    public string NombreUsuario
    {
        get => _nombreUsuario;
        set => SetProperty(ref _nombreUsuario, value);
    }

    private string _passUsuario;
    public string PassUsuario
    {
        get => _passUsuario;
        set => SetProperty(ref _passUsuario, value);
    }

    [ObservableProperty]
    private string _errorMensaje = "";

    // Métodos
    public void Salir_OnClick()
    {
        this._ventanaActual.Close();
    }

    [RelayCommand]
    public void Login_OnClick()
    {
        if (string.IsNullOrEmpty(NombreUsuario) || string.IsNullOrEmpty(PassUsuario))
        {
            ErrorMensaje = "Todos los campos son obligatorios.";
            return;
        }

        var usuario = _dbMan.VerificarCredenciales(NombreUsuario, PassUsuario);
        if (usuario != null)
        {
            _userSessionService.UserId = usuario.Id;
            _userSessionService.Username = usuario.Username;
            _userSessionService.Role = "UserRole"; 
            
            DashboardWindowView dash = new DashboardWindowView
            {
                DataContext = new DashboardWindowViewModel(),
            };

            AppConfigModel.Instance.DashWindow = dash;

            dash.Show();
            this._ventanaActual.Hide();
            NombreUsuario = "";
            PassUsuario = "";
        }
        else
        {
            ErrorMensaje = "Nombre de usuario o contraseña incorrectos.";
        }
    }
}