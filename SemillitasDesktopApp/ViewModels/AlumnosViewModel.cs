using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasDesktopApp.Helpers;
using SemillitasDesktopApp.Infrastructure;
using SemillitasDesktopApp.Models;
using SemillitasDesktopApp.Services;

namespace SemillitasDesktopApp.ViewModels;

// Todo lo de alumnos: lista, formulario (nuevo/editar), detalle con sus inscripciones y eliminar
public class AlumnosViewModel : ViewModelBase
{
    AlumnosService alumnosService = new();
    InscripcionesService inscripcionesService = new();
    TalleresService talleresService = new();
    PeriodosService periodosService = new();

    private readonly MainViewModel _main;
    private List<AlumnoDTO> _todos = new();
    private List<InscripcionesDTO> _inscripciones = new();
    private AlumnoDTO? _editando;

    // ---------- Lista ----------

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Filtrar(); }
    }

    public ObservableCollection<AlumnoFila> Alumnos { get; } = new();

    private string _resumenListado = "";
    public string ResumenListado { get => _resumenListado; set => SetField(ref _resumenListado, value); }

    // ---------- Formulario ----------

    private bool _isFormularioAbierto;
    public bool IsFormularioAbierto { get => _isFormularioAbierto; set => SetField(ref _isFormularioAbierto, value); }

    private string _tituloFormulario = "";
    public string TituloFormulario { get => _tituloFormulario; set => SetField(ref _tituloFormulario, value); }

    private string _nombreAlumno = "";
    public string NombreAlumno { get => _nombreAlumno; set => SetField(ref _nombreAlumno, value); }

    private string _apellidoAlumno = "";
    public string ApellidoAlumno { get => _apellidoAlumno; set => SetField(ref _apellidoAlumno, value); }

    private string _nombreTutor = "";
    public string NombreTutor { get => _nombreTutor; set => SetField(ref _nombreTutor, value); }

    private string _telefono = "";
    public string Telefono { get => _telefono; set => SetField(ref _telefono, value); }

    // ---------- Detalle ----------

    private bool _isDetalleAbierto;
    public bool IsDetalleAbierto { get => _isDetalleAbierto; set => SetField(ref _isDetalleAbierto, value); }

    private AlumnoFila? _detalle;
    public AlumnoFila? Detalle { get => _detalle; set => SetField(ref _detalle, value); }

    public ObservableCollection<InscripcionFila> InscripcionesAlumno { get; } = new();

    private string _totalAdeudo = "$0";
    public string TotalAdeudo { get => _totalAdeudo; set => SetField(ref _totalAdeudo, value); }

    // ---------- Commands ----------

    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand VerDetalleCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand CerrarCommand { get; }
    public ICommand MarcarPagadoCommand { get; }

    public AlumnosViewModel(MainViewModel main)
    {
        _main = main;

        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(Editar);
        VerDetalleCommand = new RelayCommand(VerDetalle);
        EliminarCommand = new RelayCommand(Eliminar);
        GuardarCommand = new RelayCommand(_ => Guardar());
        CerrarCommand = new RelayCommand(_ => Cerrar());
        MarcarPagadoCommand = new RelayCommand(MarcarPagado);
    }

    public async void Cargar()
    {
        try
        {
            _todos = await alumnosService.GetAll();
            _inscripciones = await inscripcionesService.GetAll();
            Filtrar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private void Filtrar()
    {
        Alumnos.Clear();
        foreach (var a in _todos)
        {
            if (!string.IsNullOrWhiteSpace(TextoBusqueda)
                && !Formato.Contiene(Formato.NombreCompleto(a), TextoBusqueda)
                && !Formato.Contiene(a.NombreTutor, TextoBusqueda)
                && !Formato.Contiene(a.Telefono, TextoBusqueda))
                continue;

            var suyas = _inscripciones.Where(i => i.IdAlumno == a.Id).ToList();
            var debe = suyas.Any(i => i.Debe == true);
            Alumnos.Add(new AlumnoFila
            {
                Alumno = a,
                NombreCompleto = Formato.NombreCompleto(a),
                Tutor = a.NombreTutor,
                Telefono = a.Telefono,
                CantidadTalleres = suyas.Count,
                Estado = debe ? "Debe" : "Al corriente",
                Tono = debe ? "Rojo" : "Verde"
            });
        }
        ResumenListado = $"{Alumnos.Count} de {_todos.Count} alumnos";
    }

    private void Nuevo()
    {
        _editando = null;
        TituloFormulario = "Nuevo alumno";
        NombreAlumno = ApellidoAlumno = NombreTutor = Telefono = "";
        Errores.Clear();
        IsFormularioAbierto = true;
    }

    private void Editar(object? parametro)
    {
        if (parametro is not AlumnoFila fila) return;

        _editando = fila.Alumno;
        TituloFormulario = "Editar alumno";
        NombreAlumno = fila.Alumno.Nombre;
        ApellidoAlumno = fila.Alumno.Apellido;
        NombreTutor = fila.Alumno.NombreTutor;
        Telefono = fila.Alumno.Telefono;
        Errores.Clear();
        IsDetalleAbierto = false;
        IsFormularioAbierto = true;
    }

    private async void Guardar()
    {
        var dto = new EditAlumnoDTO
        {
            Id = _editando?.Id ?? 0,
            Nombre = NombreAlumno.Trim(),
            Apellido = ApellidoAlumno.Trim(),
            NombreTutor = NombreTutor.Trim(),
            Telefono = Telefono.Trim()
        };

        try
        {
            var errores = new ValidatorClass();
            bool listo;

            if (_editando is null)
            {
                var validacion = new CreateAlumnoValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await alumnosService.Create(dto, errores) != null;
            }
            else
            {
                var validacion = new EditAlumnoValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await alumnosService.Update(dto, errores);
            }

            if (!listo)
            {
                MostrarErrores(errores.Errores);
                return;
            }

            IsFormularioAbierto = false;
            _main.ShowToast("Alumno guardado", $"Se guardó a {dto.Nombre} {dto.Apellido}.");
            Cargar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private async void Eliminar(object? parametro)
    {
        if (parametro is not AlumnoFila fila) return;

        var respuesta = MessageBox.Show($"¿Eliminar a \"{fila.NombreCompleto}\"?", "Eliminar alumno",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (respuesta != MessageBoxResult.Yes) return;

        try
        {
            var errores = new ValidatorClass();
            if (await alumnosService.Delete(fila.Alumno.Id, errores))
            {
                IsDetalleAbierto = false;
                _main.ShowToast("Alumno eliminado", $"Se eliminó a {fila.NombreCompleto}.");
                Cargar();
            }
            else
            {
                _main.ShowToast("No se pudo eliminar", string.Join("\n", errores.Errores));
            }
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private async void VerDetalle(object? parametro)
    {
        if (parametro is not AlumnoFila fila) return;

        Detalle = fila;
        IsDetalleAbierto = true;
        await CargarDetalle();
    }

    // Inscripciones del alumno con taller, período, costo y adeudo
    private async Task CargarDetalle()
    {
        if (Detalle is null) return;
        InscripcionesAlumno.Clear();

        try
        {
            var talleres = await talleresService.GetAll();
            var periodos = await periodosService.GetAll();
            var inscripciones = await inscripcionesService.GetAll();

            var filas = (from i in inscripciones
                         where i.IdAlumno == Detalle.Alumno.Id
                         join t in talleres on i.IdTaller equals t.Id
                         select new { i, t }).ToList();

            foreach (var x in filas)
            {
                var periodo = periodos.FirstOrDefault(p => p.Id == x.t.IdPeriodo)?.ToString() ?? "";
                InscripcionesAlumno.Add(InscripcionFila.Crear(x.i, Detalle.Alumno, x.t, periodo));
            }

            TotalAdeudo = Formato.Dinero(filas.Where(x => x.i.Debe == true).Sum(x => x.t.Costo));
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private async void MarcarPagado(object? parametro)
    {
        if (parametro is not InscripcionFila fila) return;

        var respuesta = MessageBox.Show($"¿Marcar \"{fila.Taller}\" como pagado?", "Marcar pagado",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (respuesta != MessageBoxResult.Yes) return;

        try
        {
            var errores = new ValidatorClass();
            if (await inscripcionesService.MarcarPagada(fila.Inscripcion, errores))
            {
                _main.ShowToast("Pago registrado", $"{fila.Taller} quedó pagado.");
                await CargarDetalle();
                Cargar();
            }
            else
            {
                _main.ShowToast("No se pudo marcar el pago", string.Join("\n", errores.Errores));
            }
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private void Cerrar()
    {
        IsFormularioAbierto = false;
        IsDetalleAbierto = false;
    }
}
