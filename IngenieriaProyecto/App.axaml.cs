using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using IngenieriaProyecto.Services;
using IngenieriaProyecto.ViewModels;
using IngenieriaProyecto.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IngenieriaProyecto;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(mainWindow);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}