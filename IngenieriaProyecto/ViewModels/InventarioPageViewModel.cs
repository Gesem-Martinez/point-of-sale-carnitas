using System.Collections.ObjectModel;
using IngenieriaProyecto.Models;

namespace IngenieriaProyecto.ViewModels;

public class InventarioPageViewModel : ViewModelBase
{
    private ObservableCollection<InventarioModel> _inventario;
    private DatabaseManagerModel dbMan;

    public ObservableCollection<InventarioModel> Inventario
    {
        get => _inventario;
        set => SetProperty(ref _inventario, value);
    }

    public InventarioPageViewModel()
    {
        dbMan = DatabaseManagerModel.Instance;
        Inventario = new ObservableCollection<InventarioModel>();
        CargarInventario();
    }

    public void CargarInventario()
    {
        var inventarioList = dbMan.ConsultaInventario();
        Inventario = new ObservableCollection<InventarioModel>(inventarioList);
    }
}