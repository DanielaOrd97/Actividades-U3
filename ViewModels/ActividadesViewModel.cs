using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using U3Cliente.Models;
using GalaSoft.MvvmLight.Command;
using U3Cliente.Services;
using U3Cliente.Helpers;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace U3Cliente.ViewModels
{
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        public ICommand VerActividadCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand VerNuevoCommand { get; set; }
        public ICommand GuardarNuevoCommand { get; set; }
        public ICommand EditarActividadCommand { get; set; }
        public ICommand EliminarActividadCommand { get; set; }
        public ICommand VerActividadesCommand { get; set; }
        public ICommand VerDepartamentosCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand VerDetallesDepartamentoCommand { get; set; }
        public ICommand EditarDeptoCommand { get; set; }
        public ICommand EliminarDeptoCommand { get; set; }
        public ICommand VerNuevoDeptoCommand { get; set; }
        public ICommand RegistraDeptoCommand { get; set; }
        public ICommand FiltrarCommand { get; set; }

        public List<Estatus> Estados { get; set; }
        public ObservableCollection<DepartamentosDTO> Deptos { get; set; }
        public ObservableCollection<DepartamentosDTO> OtrosDeptos { get; set; }
        public MisActividadesYSubordinados ActividadesEntrantes { get; set; }
        public ObservableCollection<ActividadesDTO> ActividadesFiltradas { get; set; }
        public DepartamentosDTO DepartamentoActual { get; set; }
        public DepartamentosDTO DepartamentoUser { get; set; }
        public ActividadesDTO ActividadActual { get; set; }
        public DepartamentosDTO departamentoActualDetalles { get; set; }
        public DateTime FechaRealizacionActActual { get; set; }
        public Estatus? EstadoActualAct { get; set; }
        public string NuevaEvidencia { get; set; } = "";
        public string NombreNuevoArchivo { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public bool IsReadOnly { get; set; }

        ActividadService actividadService;
        DepartamentoService departamentoService;

        public ActividadesViewModel()
        {
            VerActividadesCommand = new RelayCommand(VerActividades);
            VerDepartamentosCommand = new RelayCommand(VerDepartamentos);
            VerNuevoDeptoCommand = new RelayCommand(VerNuevoDepto);
            RegistraDeptoCommand = new RelayCommand(RegistraDepto);
            GuardarNuevoCommand = new RelayCommand(GuardarNuevo);
            EditarActividadCommand = new RelayCommand(EditarCommand);
            EditarDeptoCommand = new RelayCommand(EditarDepto);
            VerDetallesDepartamentoCommand = new RelayCommand<DepartamentosDTO>(VerDetallesDepartamento);
            EliminarActividadCommand = new RelayCommand(EliminarActividad);
            EliminarDeptoCommand = new RelayCommand(EliminarDepto);
            VerActividadCommand = new RelayCommand<ActividadesDTO>(SetActividad);
            CancelarCommand = new RelayCommand(Cancelar);
            VerNuevoCommand = new RelayCommand(VerNuevo);
            FiltrarCommand = new RelayCommand<int>(Filtrar);
            LogoutCommand = new RelayCommand(Logout);

            Deptos = new ObservableCollection<DepartamentosDTO>();
            Estados = new()
            {
                new Estatus(){Value = 0 , Text = "Borrador"},
                new Estatus(){Value = 1 , Text = "Publicado"},
                new Estatus(){Value = 2 , Text = "Eliminado"}
            };

            actividadService = App.actividadService;
            departamentoService = App.departamentoService;
            DepartamentoActual = new DepartamentosDTO();
            DepartamentoUser = new();
            ActividadesFiltradas = new();
            ActividadesEntrantes = new();
            ActividadActual = new();
            EstadoActualAct = new();
            OtrosDeptos = new();
            departamentoActualDetalles = new();
        }

        private void VerDepartamentos()
        {
            App.MainViewModel.CambiarVista(Vista.Dept);
        }
        private void VerActividades()
        {
            App.MainViewModel.CambiarVista(Vista.Act);
        }
        private void Logout()
        {
            try
            {
                DefineDepto(new DepartamentosDTO());
                Deptos.Clear();
                DepartamentoActual = new();
                App.MainViewModel.CambiarVista(Vista.Login);
            }
            catch (Exception ex)
            {
                App.MostrarError(ex.Message);
            }
        }
        private async void RegistraDepto()
        {
            try
            {
                Mensaje = "";
                ValidarDepto(departamentoActualDetalles);

                if (Mensaje == "") {
                    departamentoActualDetalles.IdSuperior = DepartamentoUser.Id;
                    var result = await departamentoService.Post(departamentoActualDetalles);

                    if (result == "") {
                        CargarDeptos();
                        LoadData();

                        App.MainViewModel.CambiarVista(Vista.Dept);
                    } else {
                        Mensaje = result;
                    }
                }
                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
            }
        }


        private void VerNuevoDepto()
        {
            try {
                departamentoActualDetalles = new();
                Mensaje = "";
                App.MainViewModel.CambiarVista(Vista.RegDept);
                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
            }
        }

        private async void EliminarDepto()
        {
            try{
                MessageBoxResult res = MessageBox.Show("¿Desea eliminar este departamento?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.No)
                    return;

                Mensaje = "";
                if (departamentoActualDetalles != null) {
                    var result = await departamentoService.Delete(departamentoActualDetalles.Id);
                    
                    if (result == "") {
                        CargarDeptos();
                        LoadData();
                        App.MainViewModel.CambiarVista(Vista.Dept);
                    } else
                        Mensaje = result;
                } else {
                    Mensaje = "Debes elegir un departamento";
                }
                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
            }
        }

        private async void EditarDepto()
        {
            try {
                Mensaje = "";
                ValidarDepto(departamentoActualDetalles);

                if (Mensaje == "")
                {
                    var result = await departamentoService.Put(departamentoActualDetalles);

                    if (result == "") {
                        CargarDeptos();
                        LoadData();
                        App.MainViewModel.CambiarVista(Vista.Dept);
                    }
                }
                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
            }
        }

        private string ValidarDepto(DepartamentosDTO DPTO)
        {
            if (string.IsNullOrWhiteSpace(departamentoActualDetalles.Nombre))
                Mensaje += "Especifique el nombre del departamento";
            if (string.IsNullOrWhiteSpace(departamentoActualDetalles.Username))
                Mensaje += "Especifique el usuario del departamento";
            if (string.IsNullOrWhiteSpace(departamentoActualDetalles.Password))
                Mensaje += "Especifique la contraseña";
            if (!string.IsNullOrWhiteSpace(departamentoActualDetalles.Password))
                departamentoActualDetalles.Password = CifradoHash.StringToSHA512(departamentoActualDetalles.Password);

            return Mensaje;
        }

        private void VerDetallesDepartamento(DepartamentosDTO departamentoEntrante)
        {
            Mensaje = "";
            departamentoActualDetalles = new()
            {
                Id = departamentoEntrante.Id,
                IdSuperior = departamentoEntrante.IdSuperior,
                Nombre = departamentoEntrante.Nombre,
                Username = departamentoEntrante.Username,
                Password = ""
            };

            App.MainViewModel.CambiarVista(Vista.DetDept);
            Actualizar();
        }

        private async void EliminarActividad()
        {
            try {

                MessageBoxResult result = MessageBox.Show("¿Desea eliminar esta actividad?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;

                if (ActividadActual != null && ActividadActual.Id > 0) {
                    Mensaje = await actividadService.Delete(ActividadActual.Id);
                    
                    if (Mensaje == "") {
                        ClearProperties();
                        LoadData();
                        App.MainViewModel.CambiarVista(Vista.Act);
                    }
                }
                else
                    Mensaje = "Debe escoger una actividad a eliminar";

                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
                Actualizar();
            }
        }

        private async void EditarCommand()
        {
            try {
                Mensaje = "";
                ValidarPropiedades(ActividadActual);

                if (Mensaje == "") {

                    ActividadActual.Estado = EstadoActualAct.Value;
                    ActividadActual.FechaRealizacion = DateOnly.FromDateTime(FechaRealizacionActActual);

                    ActividadActual.IdDepartamento = DepartamentoUser.Id;

                    if (NuevaEvidencia != "")
                        ActividadActual.Evidencia = NuevaEvidencia;
                    else
                        ActividadActual.Evidencia = "";

                    Mensaje = await actividadService.Put(ActividadActual);

                    if (Mensaje == "") {
                        ClearProperties();
                        LoadData();

                        App.MainViewModel.CambiarVista(Vista.Act);
                    }
                }

                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
                Actualizar();
            }
        }

        private async void GuardarNuevo()
        {
            try
            {
                Mensaje = "";

                ValidarPropiedades(ActividadActual);

                if (Mensaje == "") {

                    ActividadActual.Estado = EstadoActualAct.Value;
                    ActividadActual.FechaRealizacion = DateOnly.FromDateTime(FechaRealizacionActActual);
                    ActividadActual.IdDepartamento = DepartamentoUser.Id;
                    ActividadActual.Evidencia = NuevaEvidencia;

                    Mensaje = await actividadService.Post(ActividadActual);
                    if (Mensaje == "") {
                        ClearProperties();
                        LoadData();

                        App.MainViewModel.CambiarVista(Vista.Act);
                    }
                }

                Actualizar();
            } catch (Exception ex) {
                App.MostrarError(ex.Message);
                Actualizar();
            }
        }



        private string ValidarPropiedades(ActividadesDTO Act)
        {
            if (string.IsNullOrWhiteSpace(Act.Titulo))
                Mensaje = "Asigne un titulo a la actividad";
            if (string.IsNullOrWhiteSpace(Act.Descripcion))
                Mensaje = "Asigne una descripcion a la Actividad";
            if (FechaRealizacionActActual > DateTime.Now.ToMexicoTime())
                Mensaje = "La fecha de realizacion no puede ser en el futuro";
            if (DepartamentoActual == null)
                Mensaje = "Especifique el departamento";
            if (EstadoActualAct == null || EstadoActualAct.Text == "")
                Mensaje = "Especifique el estado de la actividad";
            if (string.IsNullOrWhiteSpace(NuevaEvidencia))
                Mensaje = "Debe contener evidencia";

            return Mensaje;
        }

        private void VerNuevo()
        {
            try
            {
                if (DepartamentoActual != null) {
                    ActividadActual = new();
                    NuevaEvidencia = "";
                    NombreNuevoArchivo = "";
                    FechaRealizacionActActual = DateTime.Now.AddDays(-1);
                    App.MainViewModel.CambiarVista(Vista.RegAct);
                    Actualizar();
                } else {
                    App.MostrarError("Debe escoger un departamento");
                }

            } catch (Exception ex)  {
                App.MostrarError(ex.Message);
            }
        }

        private void SetActividad(ActividadesDTO dTO)
        {

            ClearProperties();
            IsReadOnly = true;
            if (dTO != null)  {
                if (dTO.IdDepartamento == DepartamentoUser.Id)
                    IsReadOnly = false;
                else
                    IsReadOnly = true;

                ActividadActual = new()
                {
                    Id = dTO.Id,
                    Descripcion = dTO.Descripcion,
                    IdDepartamento = dTO.IdDepartamento,
                    Estado = dTO.Estado,
                    Evidencia = dTO.Evidencia,
                    FechaActualizacion = dTO.FechaActualizacion,
                    FechaCreacion = dTO.FechaCreacion,
                    FechaRealizacion = dTO.FechaRealizacion,
                    Titulo = dTO.Titulo
                };

                FechaRealizacionActActual = dTO.FechaRealizacion.Value.ToDateTime(TimeOnly.MinValue);

                EstadoActualAct = Estados.FirstOrDefault(x => x.Value == dTO.Estado);
                NuevaEvidencia = "";
                NombreNuevoArchivo = "";

                string uniqueUrl = $"{dTO.Evidencia}?timestamp={DateTime.Now.Ticks}";
                ImageSource = new BitmapImage(new Uri(uniqueUrl));

                App.MainViewModel.CambiarVista(Vista.DetAct);
                Actualizar();
            }
        }

        private BitmapImage _imageSource;

        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                Actualizar(nameof(ImageSource));
            }
        }

        private void Filtrar(int idDep)
        {
            if (ActividadesEntrantes != null && ActividadesEntrantes.ActividadesSubordinadas.Count > 0) {
                var actSeleccionada = ActividadesEntrantes.ActividadesSubordinadas.FirstOrDefault(x => x.IdDepartamento == idDep);

                if (actSeleccionada != null) {
                    ActividadesFiltradas.Clear();

                    foreach (var a in actSeleccionada.Actividades) {
                        ActividadesFiltradas.Add(a);
                    }
                }
            }
        }

        public void DefineDepto(DepartamentosDTO departamentoEntrante)
        {
            DepartamentoUser = departamentoEntrante;
        }
        async void CargarDeptos()
        {
            Deptos = new((await departamentoService.GetDeptos(DepartamentoUser.Id)));
            OtrosDeptos.Clear();

            foreach (var d in Deptos) {
                if (d.Id != DepartamentoUser.Id)
                    OtrosDeptos.Add(d);
            }
        }
        public async void LoadData()
        {
            if (Deptos == null || Deptos.Count == 0)
                CargarDeptos();

            ActividadesEntrantes = await actividadService.GetActividadesYSubordinadas(DepartamentoUser.Id);

            if (DepartamentoActual == null)
                Filtrar(DepartamentoUser.Id);
            else
                Filtrar(DepartamentoActual.Id);

            Actualizar();
        }
        private void ClearProperties()
        {
            ActividadActual = new();
            NuevaEvidencia = "";
            NombreNuevoArchivo = "";
        }
        private void Cancelar()
        {
            App.MainViewModel.CambiarVista(Vista.Act);
        }
        public void Actualizar(string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
