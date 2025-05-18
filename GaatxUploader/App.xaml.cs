using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Wpf.Ui.Appearance;

namespace GaatxUploader;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public new static App Current => (App) Application.Current;
    public IServiceProvider Services { get; }
    
    public App()
    {
        Services = BuildServiceProvider();
        InitializeComponent();
        ApplicationThemeManager.ApplySystemTheme();
        ExcelPackage.License.SetNonCommercialOrganization("Instituto Tecnologico Superior de Xalapa");
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        return services.BuildServiceProvider();
    }
}