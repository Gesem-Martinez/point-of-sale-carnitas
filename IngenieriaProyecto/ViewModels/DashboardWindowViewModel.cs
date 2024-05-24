using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngenieriaProyecto.Models;
using IngenieriaProyecto.Services;
using IngenieriaProyecto.Views;

namespace IngenieriaProyecto.ViewModels;

public partial class DashboardWindowViewModel : ViewModelBase
{
    private AppConfigModel _appConfigSingle = AppConfigModel.Instance;
    [ObservableProperty] private bool _panelAbierto = true;
    [ObservableProperty] private ViewModelBase _paginaActual = AppConfigModel.Instance.PaginaActual;
    
    private readonly UserSessionService _userSessionService;

    public string Username => _userSessionService.Username;
    public string Role => _userSessionService.Role;
    
    public ObservableCollection<ListItemTemplate> ListItems { get; } = new()
    {
        new ListItemTemplate(typeof(InicioPageViewModel), "InicioRegular"),
        new ListItemTemplate(typeof(EmpleadosPageViewModel), "EmpleadosRegular"),
        new ListItemTemplate(typeof(PedidosPageViewModel), "PedidosRegular"),
        new ListItemTemplate(typeof(InventarioPageViewModel), "InventarioRegular"),
    };

    [ObservableProperty] private ListItemTemplate? _listItemSelecc;
    partial void OnListItemSeleccChanged(ListItemTemplate? valor)
    {
        if (valor is null) return;
        var instancia = Activator.CreateInstance(valor.TipoPagina);
        if (instancia is null) return;
        _appConfigSingle.PaginaActual = (ViewModelBase)instancia;
        PaginaActual = _appConfigSingle.PaginaActual;
    }

    [RelayCommand]
    private void CambiarPanel()
    {
        PanelAbierto = !PanelAbierto;
    }

    [RelayCommand]
    private void CerrarAplicacion()
    {
        AppConfigModel.Instance.LoginWindow.Show();
        AppConfigModel.Instance.DashWindow.Close();
        
    }

}

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey)
    {
        TipoPagina = type;
        Etiqueta = type.Name.Replace("PageViewModel", "");

        Application.Current!.TryFindResource(iconKey, out var result);
        ListItemIcono = (StreamGeometry)result!;
    }
    
    public string Etiqueta { get; }
    public Type TipoPagina { get; }
    public StreamGeometry ListItemIcono { get; }
}
