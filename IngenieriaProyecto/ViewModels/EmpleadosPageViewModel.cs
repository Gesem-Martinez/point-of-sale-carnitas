using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngenieriaProyecto.Models;
using IngenieriaProyecto.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IngenieriaProyecto.ViewModels;

public partial class EmpleadosPageViewModel : ViewModelBase
{
    private ObservableCollection<EmpleadoModel> _empleados;
    private DatabaseManagerModel dbMan;
    [ObservableProperty]private EmpleadoModel? _empleadoSeleccionado;
    [ObservableProperty]private bool _isEmpleadoSeleccionado = false;
    [ObservableProperty]private bool _isAgregarEmpleadoVisible = false;
    
    //Propiedades para agregar empleado
    [ObservableProperty]private string _nombreEmpleado;
    [ObservableProperty]private string _usuarioEmpleado;
    [ObservableProperty]private string _passwordEmpleado;
    [ObservableProperty]private string _rolEmpleado;
    [ObservableProperty]private decimal _salarioEmpleado;
    [ObservableProperty] private string _mensajeConfirmacion;
    
    //Propiedades para editar empleado
    [ObservableProperty]private bool _isEditarEmpleadoVisible = false;
    [ObservableProperty] private string _mensajeConfirmacionEditar;
    [ObservableProperty]private string _nombreEdit;
    [ObservableProperty]private string _usuarioEdit;
    [ObservableProperty]private string _passwordEdit;
    [ObservableProperty]private string _rolEdit;
    [ObservableProperty]private decimal _salarioEdit;
    [ObservableProperty]private int _usuarioIdEdit;
    [ObservableProperty] private IBrush _colorMensajeConfirmacion;
    [ObservableProperty] private IBrush _colorMensajeConfirmacionEditar;
    private int _idEmpleado = 0;

    public int IdEmpleado => _idEmpleado;

    public ObservableCollection<EmpleadoModel> Empleados
    {
        get => _empleados;
        set => SetProperty(ref _empleados, value);
    }
    
    //Constructor
    public EmpleadosPageViewModel()
    {
        dbMan = DatabaseManagerModel.Instance;
        Empleados = new ObservableCollection<EmpleadoModel>();
        
        CargarUsuarios();
    }

    //Metodos
    partial void OnEmpleadoSeleccionadoChanged(EmpleadoModel? valor)
    {
        IsEmpleadoSeleccionado = true;

        EmpleadoSeleccionado.Username = dbMan.ConsultaUsername(EmpleadoSeleccionado.Id);
        EmpleadoSeleccionado.Password = dbMan.ConsultaPassword(EmpleadoSeleccionado.Id);

        NombreEdit = EmpleadoSeleccionado.Nombre;
        UsuarioEdit = EmpleadoSeleccionado.Username;
        PasswordEdit = EmpleadoSeleccionado.Password;
        RolEdit = EmpleadoSeleccionado.Rol;
        SalarioEdit = EmpleadoSeleccionado.Salario;
        UsuarioIdEdit = EmpleadoSeleccionado.Id;
        Console.WriteLine(EmpleadoSeleccionado.Username);
    }


    public void CargarUsuarios()
    {
        var meserosList = dbMan.ConsultaMeseros();
        Empleados = new ObservableCollection<EmpleadoModel>(meserosList);
    }

    [RelayCommand]
    public void AgregarUsuarioOpen()
    {
        if (IsEditarEmpleadoVisible)
        {
            return;
        }
        IsAgregarEmpleadoVisible = true;
    }

    [RelayCommand]
    public void AgregarUsuarioClose()
    {
        IsAgregarEmpleadoVisible = false;
        NombreEmpleado = "";
        UsuarioEmpleado = "";
        PasswordEmpleado = "";
        RolEmpleado = "";
        SalarioEmpleado = 0;
    }

    [RelayCommand]
    public void EditarEmpleadoOpen()
    {
        if (IsAgregarEmpleadoVisible)
        {
            return;
        }
        IsEditarEmpleadoVisible = true;
    }

    [RelayCommand]
    public void EditarEmpleadoClose()
    {
        IsEditarEmpleadoVisible = false;
    }
    
    [RelayCommand]
    public void AgregarUsuariosHandler()
    {
        try
        {
            if (NombreEmpleado == "" || UsuarioEmpleado == "" || PasswordEmpleado == "" || RolEmpleado == "" ||
                SalarioEmpleado == 0)
            {
                ColorMensajeConfirmacion = Brush.Parse("#DD5746");
                MensajeConfirmacion = "Todos los campos son obligatorios";
                return;
            }
            ColorMensajeConfirmacion = Brush.Parse("#90D26D");
            MensajeConfirmacion = dbMan.ConsultaAgregarUsuario(NombreEmpleado, UsuarioEmpleado, PasswordEmpleado, RolEmpleado, SalarioEmpleado);
            CargarUsuarios();

            NombreEmpleado = "";
            UsuarioEmpleado = "";
            PasswordEmpleado = "";
            RolEmpleado = "";
            SalarioEmpleado = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    [RelayCommand]
    public void EditarEmpleadoHandler()
    {
        try
        {
            Console.WriteLine("DATOS A MANDARA A MANAGER");
            Console.WriteLine(UsuarioEdit);
            if (EmpleadoSeleccionado != null)
            {
                if (NombreEdit == "" || UsuarioEdit == "" || PasswordEdit == "" || RolEdit == "" ||
                    SalarioEdit == 0)
                {
                    ColorMensajeConfirmacionEditar = Brush.Parse("#DD5746");
                    MensajeConfirmacionEditar = "Todos los campos son obligatorios";
                    return;
                }
                ColorMensajeConfirmacionEditar = Brush.Parse("#90D26D");
                MensajeConfirmacionEditar = dbMan.ConsultaEditarUsuario(NombreEdit,
                    UsuarioEdit, PasswordEdit, RolEdit,
                    SalarioEdit, EmpleadoSeleccionado.Id);
                CargarUsuarios();
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    [RelayCommand]
    public async Task EliminarEmpleadoAsync()
    {
        if (EmpleadoSeleccionado == null)
        {
            Console.WriteLine("No hay empleado seleccionado");
            return;
        }

        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
            "Confirmación",
            "¿Está seguro de que desea eliminar este empleado?",
            ButtonEnum.YesNo,
            Icon.Question);

        var result = await messageBoxStandardWindow.ShowWindowAsync();

        if (result == ButtonResult.Yes)
        {
            Console.WriteLine("EmpleadoID: " + UsuarioIdEdit);
            dbMan.ConsultaEliminarEmpleado(UsuarioIdEdit);
            CargarUsuarios();
            IsEmpleadoSeleccionado = false;
        }
    }
    
}