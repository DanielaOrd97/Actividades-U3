using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using U3Cliente.Helpers;
using U3Cliente.Models;
using U3Cliente.Services;

namespace U3Cliente.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        LoginService service;
        LoginDTO loginDTO;
        Cifrado cifrado;
        public ICommand LoginCommand { get; set; }
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string Error { get; set; } = "";
        public bool IsLoading = false;
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            service = new LoginService();
            loginDTO = new();
            cifrado = new();

            Login();
        }

        private async void Login()
        {

            try
            {
                IsLoading = true;
                Error = "";
                Actualizar();

                if (string.IsNullOrWhiteSpace(User))
                    Error = "El nombre de usuario no debe ir vacio" + Environment.NewLine;
                if (string.IsNullOrWhiteSpace(Password))
                    Error = "La contraseña no debe ir vacia";
                if (Error == "")
                {
                    loginDTO.User = User.Trim();
                    loginDTO.Password = CifradoHash.StringToSHA512(Password);

                    var login = await service.Login(loginDTO);


                    if (login.Contains("id"))
                    {
                        User = "";
                        Password = "";
                        DepartamentosDTO? departamento = JsonConvert.DeserializeObject<DepartamentosDTO>(login);
                        if (departamento != null)
                        {
                            App.ActividadesViewModel.DefineDepto(departamento);
                            App.ActividadesViewModel.LoadData();

                            App.ActividadesViewModel.DepartamentoActual = departamento;
                            App.MainViewModel.CambiarVista(Vista.Act);
                            App.ActividadesViewModel.Actualizar();
                        }

                    }
                    else
                        Error = login;
                }
                IsLoading = false;
                Actualizar();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                App.MostrarError(Error);
            }
        }

        public void Logout()
        {
            App.ActividadesViewModel.DefineDepto(new DepartamentosDTO());
            App.ActividadesViewModel.Deptos.Clear();

            App.ActividadesViewModel.DepartamentoActual = new();
            App.MainViewModel.CambiarVista(Vista.Login);
        }

        public void Actualizar(string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
