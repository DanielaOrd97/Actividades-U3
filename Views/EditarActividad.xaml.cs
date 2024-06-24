using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace U3Cliente.Views
{
    /// <summary>
    /// Lógica de interacción para EditarActividad.xaml
    /// </summary>
    public partial class EditarActividad : UserControl
    {
        public EditarActividad()
        {
            InitializeComponent();
            this.DataContext = App.ActividadesViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                if (IsImage(fileName))
                {
                    byte[] imageBytes = File.ReadAllBytes(fileName);
                    string base64String = Convert.ToBase64String(imageBytes);
                    App.ActividadesViewModel.NuevaEvidencia = base64String;


                    App.ActividadesViewModel.NombreNuevoArchivo = fileName;
                    App.ActividadesViewModel.Actualizar();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un archivo de imagen válido.");

                }
            }
        }
        private bool IsImage(string filePath)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string extension = System.IO.Path.GetExtension(filePath);
            return imageExtensions.Contains(extension.ToLower());
        }
    }
}
