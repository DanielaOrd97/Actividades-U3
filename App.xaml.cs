using System.Configuration;
using System.Data;
using System.Windows;
using U3Cliente.Services;
using U3Cliente.ViewModels;

namespace U3Cliente
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Url = "https://apiactnuevateam.websitos256.com/api/";
        ///public static string Url = "https://localhost:44380/api/";
        public static DepartamentoService departamentoService = new();
        public static ActividadService actividadService = new();
        public static VistaViewModel MainViewModel { get; set; } = new VistaViewModel();
        public static ActividadesViewModel ActividadesViewModel { get; set; } = new();

        public static void MostrarError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
