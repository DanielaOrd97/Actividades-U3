using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using U3Cliente.Models;

namespace U3Cliente.Views
{
    /// <summary>
    /// Lógica de interacción para DepartamentosView.xaml
    /// </summary>
    public partial class DepartamentosView : UserControl
    {
        public DepartamentosView()
        {
            InitializeComponent();
            this.DataContext = App.ActividadesViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var depActual = ((Button)sender).DataContext as DepartamentosDTO;
            if (depActual != null)
            {
                App.ActividadesViewModel.VerDetallesDepartamentoCommand.Execute(depActual);
            }
        }
    }
}
