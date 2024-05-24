using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngenieriaProyecto.Models;

namespace IngenieriaProyecto.ViewModels;

public partial class PedidosPageViewModel : ViewModelBase
{
     private ObservableCollection<PedidoModel> _pedidos;
        private DatabaseManagerModel dbMan;
        [ObservableProperty] private bool _isAgregarPedidoVisible = false;
        [ObservableProperty] private int _nuevaCantidadProducto;
        [ObservableProperty] private ProductoDisponibleModel _productoSeleccionado;
        [ObservableProperty] private decimal _nuevoPrecioUnitario;
        [ObservableProperty] private decimal _nuevoPrecioTotal;
        private ObservableCollection<ProductoDisponibleModel> _productosDisponibles;
        [ObservableProperty] private string _mensajeConfirmacion;
        [ObservableProperty] private IBrush _colorMensajeConfirmacion;

        partial void OnNuevoPrecioUnitarioChanged(decimal unitario)
        {
            if (NuevaCantidadProducto >= 0)
            {
                NuevoPrecioTotal = unitario * NuevaCantidadProducto;
            }
        }

        public ObservableCollection<ProductoDisponibleModel> ProductosDisponibles
        {
            get => _productosDisponibles;
            set => SetProperty(ref _productosDisponibles, value);
        }
        
        public ObservableCollection<PedidoModel> Pedidos
        {
            get => _pedidos;
            set => SetProperty(ref _pedidos, value);
        }

        public PedidosPageViewModel()
        {
            dbMan = DatabaseManagerModel.Instance;
            ProductosDisponibles = new ObservableCollection<ProductoDisponibleModel>();
            CargarPedidos();
        }

        public void CargarPedidos()
        {
            var pedidosList = dbMan.ConsultaPedidos();
            var productosDisponiblesList = dbMan.ConsultaProductos();
            Pedidos = new ObservableCollection<PedidoModel>(pedidosList);
            ProductosDisponibles = new ObservableCollection<ProductoDisponibleModel>(productosDisponiblesList);
        }

        [RelayCommand]
        public void AgregarPedido()
        {
            IsAgregarPedidoVisible = true;
        }
        
        [RelayCommand]
        public void CancelarPedido()
        {
            IsAgregarPedidoVisible = false;
            LimpiarCampos();
        }

        [RelayCommand]
        public void AceptarPedido()
        {
            if (NuevaCantidadProducto == 0 || NuevoPrecioUnitario == 0 || NuevoPrecioTotal == 0 || ProductoSeleccionado == null)
            {
                MensajeConfirmacion = "Todos los campos son obligatorios";
                ColorMensajeConfirmacion = Brush.Parse("#DD5746");
                return;
            }
            var nuevoProducto = new ProductoModel
            {
                CantidadProducto = NuevaCantidadProducto,
                IdProducto = ProductoSeleccionado.IdProducto,
                NombreProducto = ProductoSeleccionado.NombreProducto,
                PrecioUnitario = NuevoPrecioUnitario,
                PrecioTotal = NuevoPrecioTotal
            };

            var nuevoPedido = new PedidoModel
            {
                NumeroPedido = Pedidos.Count + 1,
                Productos = new List<ProductoModel> { nuevoProducto }
            };

            dbMan.ConsultaAgregarPedido(nuevoPedido);
            Pedidos.Add(nuevoPedido);
            MensajeConfirmacion = "Pedido realizado con exito";
            ColorMensajeConfirmacion = Brush.Parse("#90D26D");
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            NuevaCantidadProducto = 0;
            ProductoSeleccionado = null;
            NuevoPrecioUnitario = 0;
            NuevoPrecioTotal = 0;
        }
}