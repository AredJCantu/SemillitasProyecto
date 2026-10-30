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

// Todo lo de inscripciones: inscribir a un alumno en talleres y la cobranza (marcar pagos)
public class InscripcionesViewModel : ViewModelBase
{
    TalleresService talleresService = new();
    AlumnosService alumnosService = new();
    InscripcionesService inscripcionesService = new();
    PeriodosService periodosService = new();

    private readonly MainViewModel _main;
    private List<TallerDTO> _talleres = new();
    private List<AlumnoDTO> _alumnos = new();
    private List<InscripcionesDTO> _inscripciones = new();
    private readonly List<TallerDTO> _agregados = new();   // talleres elegidos en esta inscripción
    private AlumnoDTO? _alumnoExistente;                   // si se eligió con el buscador
    private TallerDTO? _tallerInicial;                     // el que se pulsó en Disponibilidad

    // Período para inscribir y para cobranza
    public ObservableCollection<PeriodoDTO> Periodos { get; } = new();

    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado
    {
        get => _periodoSeleccionado;
        set
        {
            if (!SetField(ref _periodoSeleccionado, value)) return;
            _agregados.RemoveAll(t => t.IdPeriodo != value?.Id);   // no se mezclan períodos
            Reconstruir();
            AplicarCobranza();
        }
    }

    // ---------- Inscribir ----------
    private NombreAlumnoDTO _alumnoSeleccionado;
    public NombreAlumnoDTO AlumnoSeleccionado { get=>_alumnoSeleccionado; set { SetField(ref _alumnoSeleccionado, value); Aplicar(); } }



    public List<NombreAlumnoDTO> NombresdeAlumnos { get; set; } = new();
    private string _nombreAlumno = "";
    public string NombreAlumno { get => _nombreAlumno; set => SetField(ref _nombreAlumno, value); }
    private string _apellidoAlumno;
    public string ApellidoAlumno { get => _apellidoAlumno; set => SetField(ref _apellidoAlumno, value); }

    private string _nombreTutor = "";
    public string NombreTutor { get => _nombreTutor; set => SetField(ref _nombreTutor, value); }

    private string _telefonoAlumno = "";
    public string TelefonoAlumno { get => _telefonoAlumno; set => SetField(ref _telefonoAlumno, value); }

    public ObservableCollection<TallerFila> TalleresPeriodo { get; } = new();
    public ObservableCollection<TallerFila> Resumen { get; } = new();

    private string _total = "$0";
    public string Total { get => _total; set => SetField(ref _total, value); }

    // ---------- Cobranza ----------

    private string _cobrado = "$0";
    public string Cobrado { get => _cobrado; set => SetField(ref _cobrado, value); }

    private string _porCobrar = "$0";
    public string PorCobrar { get => _porCobrar; set => SetField(ref _porCobrar, value); }

    private int _alumnosConAdeudo;
    public int AlumnosConAdeudo { get => _alumnosConAdeudo; set => SetField(ref _alumnosConAdeudo, value); }

    public List<string> Filtros { get; } = new() { "Todos", "Deben", "Al corriente" };

    private string _filtroSeleccionado = "Todos";
    public string FiltroSeleccionado
    {
        get => _filtroSeleccionado;
        set { if (SetField(ref _filtroSeleccionado, value)) AplicarCobranza(); }
    }

    private string _textoBusquedaCobro = "";
    public string TextoBusquedaCobro
    {
        get => _textoBusquedaCobro;
        set { if (SetField(ref _textoBusquedaCobro, value)) AplicarCobranza(); }
    }

    public ObservableCollection<InscripcionFila> Cobros { get; } = new();

    // ---------- Commands ----------

    public ICommand AlternarTallerCommand { get; }
    public ICommand QuitarCommand { get; }
    public ICommand ConfirmarCommand { get; }
    public ICommand LimpiarCommand { get; }
    public ICommand MarcarPagadoCommand { get; }

    public InscripcionesViewModel(MainViewModel main)
    {
        _main = main;

        AlternarTallerCommand = new RelayCommand(AlternarTaller);
        QuitarCommand = new RelayCommand(Quitar);
        ConfirmarCommand = new RelayCommand(_ => Confirmar());
        LimpiarCommand = new RelayCommand(_ => Limpiar());
        MarcarPagadoCommand = new RelayCommand(MarcarPagado);
    }

    // Disponibilidad llama esto antes de abrir Inscribir
    public void Preseleccionar(TallerDTO taller) => _tallerInicial = taller;

    public async void Cargar()
    {
        try
        {
            _talleres = await talleresService.GetAll();
            _alumnos = await alumnosService.GetAll();
            NombresdeAlumnos.Clear();
            foreach (var a in _alumnos) {
                NombreAlumnoDTO dto = new()
                {
                    Id = a.Id,
                    Nombre = a.Nombre
                };
                NombresdeAlumnos.Add(dto); 
            }
            _inscripciones = await inscripcionesService.GetAll();
            var lista = await periodosService.GetAll();

            // Período: el del taller elegido en Disponibilidad, el que ya estaba o el más reciente
            var idPeriodo = _tallerInicial?.IdPeriodo ?? PeriodoSeleccionado?.Id;
            Periodos.Clear();
            lista.ForEach(Periodos.Add);
            PeriodoSeleccionado = Periodos.FirstOrDefault(p => p.Id == idPeriodo) ?? Periodos.OrderBy(p => p.Id).LastOrDefault();

            // Se usan los datos recién cargados (el cupo pudo cambiar)
            _agregados.RemoveAll(a => !_talleres.Any(t => t.Id == a.Id));
            if (_tallerInicial is not null)
            {
                var actual = _talleres.FirstOrDefault(t => t.Id == _tallerInicial.Id);
                if (actual is not null && actual.Cupo > 0 && !_agregados.Any(a => a.Id == actual.Id))
                    _agregados.Add(actual);
                _tallerInicial = null;
            }

            Reconstruir();
            AplicarCobranza();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private static bool Traslapa(int inicioA, int finA, int inicioB, int finB) => inicioA < finB && inicioB < finA;

    // Choque si se traslapan entre semana o el sábado
    private static bool Chocan(TallerDTO a, TallerDTO b) =>
        Traslapa(a.HoraInicio, a.HoraFin, b.HoraInicio, b.HoraFin)
        || Traslapa(a.HoraInicioSabado, a.HoraFinSabado, b.HoraInicioSabado, b.HoraFinSabado);

    // Recalcula la acción de cada taller (Disponible/Agregado/Choque/Lleno), el resumen y el total
    private void Reconstruir()
    {
        TalleresPeriodo.Clear();
        foreach (var t in _talleres.Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id))
        {
            var fila = new TallerFila(t)
            {
                Accion = _agregados.Any(a => a.Id == t.Id) ? "Agregado"
                       : t.Cupo <= 0 ? "Lleno"
                       : _agregados.Any(a => Chocan(a, t)) ? "Choque"
                       : "Disponible"
            };
            TalleresPeriodo.Add(fila);
        }

        Resumen.Clear();
        foreach (var t in _agregados) Resumen.Add(new TallerFila(t));

        Total = Formato.Dinero(_agregados.Sum(t => t.Costo));
    }
    private void AlternarTaller(object? parametro)
    {
        if (parametro is not TallerFila fila) return;

        if (fila.Accion == "Disponible") _agregados.Add(fila.Taller);
        else if (fila.Accion == "Agregado") _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        else return;   // Choque y Lleno no hacen nada

        Reconstruir();
    }

    private void Quitar(object? parametro)
    {
        if (parametro is not TallerFila fila) return;
        _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        Reconstruir();
    }

    private async void Confirmar()
    {
        // Los datos del alumno se validan con el mismo validator que usa el API
        var datos = new CreateAlumnoDTO
        {
            Nombre = NombreAlumno.Trim(),
            Apellido = ApellidoAlumno.Trim(),
            NombreTutor = NombreTutor.Trim(),
            Telefono = TelefonoAlumno.Trim()
        };
        var validacion = new CreateAlumnoValidator().IsValid(datos);
        if (_agregados.Count == 0) validacion.Errores.Add("Agrega al menos un taller.");
        if (!validacion.EsValido)
        {
            MostrarErrores(validacion.Errores);
            return;
        }

        try
        {
            // Alumno nuevo: se crea primero para tener su Id. Alumno existente: se usa tal cual.
            var errores = new ValidatorClass();
            var alumno = _alumnoExistente ?? await alumnosService.Create(datos, errores);
            if (alumno is null)
            {
                MostrarErrores(errores.Errores);
                return;
            }
            alumno.Id = _alumnoSeleccionado.Id;
            // Una inscripción por taller; Debe = true hasta que se marque el pago en Cobranza
            foreach (var taller in _agregados.ToList())
            {
                var nueva = new CreateInscripcionesDTO { IdAlumno = alumno.Id, IdTaller = taller.Id, Debe = true };
                if (await inscripcionesService.Create(nueva, errores) is null)
                {
                    MostrarErrores(errores.Errores);   // cupo lleno, choque de horario...
                    return;
                }
                _agregados.Remove(taller);   // si falla uno, los ya inscritos no se repiten al reintentar
            }

            _main.ShowToast("Inscripción registrada", $"{alumno.Nombre} quedó inscrito.");
            Limpiar();
            Cargar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private void Limpiar()
    {
        _agregados.Clear();
        _alumnoExistente = null;
        NombreAlumno = ApellidoAlumno = NombreTutor = TelefonoAlumno = "";
        Errores.Clear();
        Reconstruir();
    }

    // Una fila por inscripción de un taller del período elegido; el costo sale del taller
    private void AplicarCobranza()
    {
        var filas = (from i in _inscripciones
                     join t in _talleres on i.IdTaller equals t.Id
                     join a in _alumnos on i.IdAlumno equals a.Id
                     where t.IdPeriodo == PeriodoSeleccionado?.Id
                     select new { i, t, a }).ToList();

        // Los totales no dependen del filtro ni de la búsqueda
        Cobrado = Formato.Dinero(filas.Where(x => x.i.Debe != true).Sum(x => x.t.Costo));
        PorCobrar = Formato.Dinero(filas.Where(x => x.i.Debe == true).Sum(x => x.t.Costo));
        AlumnosConAdeudo = filas.Where(x => x.i.Debe == true).Select(x => x.a.Id).Distinct().Count();

        Cobros.Clear();
        foreach (var x in filas)
        {
            var pasaFiltro = FiltroSeleccionado switch
            {
                "Deben" => x.i.Debe == true,
                "Al corriente" => x.i.Debe != true,
                _ => true
            };
            var pasaBusqueda = string.IsNullOrWhiteSpace(TextoBusquedaCobro)
                               || Formato.Contiene(Formato.NombreCompleto(x.a), TextoBusquedaCobro)
                               || Formato.Contiene(x.a.NombreTutor, TextoBusquedaCobro)
                               || Formato.Contiene(x.t.Nombre, TextoBusquedaCobro);

            if (pasaFiltro && pasaBusqueda) Cobros.Add(InscripcionFila.Crear(x.i, x.a, x.t));
        }
    }

    private async void MarcarPagado(object? parametro)
    {
        if (parametro is not InscripcionFila fila) return;

        var respuesta = MessageBox.Show($"¿{fila.Alumno} pagó {fila.Taller}?", "Marcar pagado",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (respuesta != MessageBoxResult.Yes) return;

        try
        {
            var errores = new ValidatorClass();
            if (await inscripcionesService.MarcarPagada(fila.Inscripcion, errores))
            {
                _main.ShowToast("Pago registrado", $"{fila.Alumno} pagó {fila.Taller}.");
                AplicarCobranza();   // MarcarPagada ya cambió Debe en la misma inscripción
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
    private void Aplicar()
    {
        var alumno = _alumnos.Where(x => x.Id == _alumnoSeleccionado.Id).FirstOrDefault();
        
        NombreAlumno = alumno.Nombre;
        ApellidoAlumno = alumno.Apellido;
        NombreTutor = alumno.NombreTutor;
        TelefonoAlumno = alumno.Telefono;
        _alumnoExistente = new();
    }
}
