using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasDesktopApp.Helpers;
using SemillitasDesktopApp.Infrastructure;
using SemillitasDesktopApp.Models;
using SemillitasDesktopApp.Services;
using SemillitasProyecto.Commons;

namespace SemillitasDesktopApp.ViewModels;

// Todo lo de talleres: Disponibilidad, Agenda, catálogo (CRUD) y detalle con inscritos
public class TalleresViewModel : ViewModelBase
{
    // Rangos de horas que aceptan los validators del Core
    private const int SemanaPrimera = 12, SemanaUltima = 22;
    private const int SabadoInicioPrimera = 5, SabadoInicioUltima = 12;
    private const int SabadoFinPrimera = 6, SabadoFinUltima = 13;
    private const string InstructorPendiente = "Por asignar";   // el instructor queda fuera por ahora

    TalleresService talleresService = new();
    PeriodosService periodosService = new();
    AlumnosService alumnosService = new();
    InscripcionesService inscripcionesService = new();

    private readonly MainViewModel _main;
    private List<TallerDTO> _todos = new();
    private TallerDTO? _editando;

    // ---------- Filtros (Disponibilidad, Agenda y catálogo) ----------

    public ObservableCollection<PeriodoDTO> Periodos { get; } = new();

    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado
    {
        get => _periodoSeleccionado;
        set { if (SetField(ref _periodoSeleccionado, value)) Aplicar(); }
    }

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Aplicar(); }
    }

    private bool _soloConCupo;
    public bool SoloConCupo
    {
        get => _soloConCupo;
        set { if (SetField(ref _soloConCupo, value)) Aplicar(); }
    }

    private int _conteoConCupo;
    public int ConteoConCupo { get => _conteoConCupo; set => SetField(ref _conteoConCupo, value); }

    private int _conteoPocos;
    public int ConteoPocos { get => _conteoPocos; set => SetField(ref _conteoPocos, value); }

    private int _conteoLlenos;
    public int ConteoLlenos { get => _conteoLlenos; set => SetField(ref _conteoLlenos, value); }

    public ObservableCollection<TallerFila> Disponibles { get; } = new();   // Disponibilidad
    public ObservableCollection<TallerFila> Catalogo { get; } = new();      // Talleres (CRUD)
    public ObservableCollection<FranjaFila> Franjas { get; } = new();       // Agenda

    // ---------- Formulario ----------

    private bool _isFormularioAbierto;
    public bool IsFormularioAbierto { get => _isFormularioAbierto; set => SetField(ref _isFormularioAbierto, value); }

    private string _tituloFormulario = "";
    public string TituloFormulario { get => _tituloFormulario; set => SetField(ref _tituloFormulario, value); }

    private string _nombre = "";
    public string Nombre { get => _nombre; set => SetField(ref _nombre, value); }

    // Se llama NombresCategorias para no chocar con el enum Categorias del Core
    public List<string> NombresCategorias { get; } = new() { "Arte", "Deportes", "Tecnología" };

    private string? _categoriaSeleccionada;
    public string? CategoriaSeleccionada { get => _categoriaSeleccionada; set => SetField(ref _categoriaSeleccionada, value); }

    private PeriodoDTO? _periodoFormulario;
    public PeriodoDTO? PeriodoFormulario { get => _periodoFormulario; set => SetField(ref _periodoFormulario, value); }

    // Cupo = lugares disponibles (el API lo baja con cada inscripción)
    private string _cupo = "";
    public string Cupo { get => _cupo; set => SetField(ref _cupo, value); }

    private string _costo = "";
    public string Costo { get => _costo; set => SetField(ref _costo, value); }

    public List<string> Horas { get; } = Lista(SemanaPrimera, SemanaUltima);
    public List<string> HorasSabadoInicio { get; } = Lista(SabadoInicioPrimera, SabadoInicioUltima);
    public List<string> HorasSabadoFin { get; } = Lista(SabadoFinPrimera, SabadoFinUltima);

    private string? _horaInicio;
    public string? HoraInicio { get => _horaInicio; set => SetField(ref _horaInicio, value); }

    private string? _horaFin;
    public string? HoraFin { get => _horaFin; set => SetField(ref _horaFin, value); }

    private string? _horaInicioSabado;
    public string? HoraInicioSabado { get => _horaInicioSabado; set => SetField(ref _horaInicioSabado, value); }

    private string? _horaFinSabado;
    public string? HoraFinSabado { get => _horaFinSabado; set => SetField(ref _horaFinSabado, value); }

    // ---------- Detalle ----------

    private bool _isDetalleAbierto;
    public bool IsDetalleAbierto { get => _isDetalleAbierto; set => SetField(ref _isDetalleAbierto, value); }

    private TallerFila? _detalle;
    public TallerFila? Detalle { get => _detalle; set => SetField(ref _detalle, value); }

    private string _detallePeriodo = "";
    public string DetallePeriodo { get => _detallePeriodo; set => SetField(ref _detallePeriodo, value); }

    public ObservableCollection<InscripcionFila> Inscritos { get; } = new();

    // ---------- Commands ----------

    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand VerDetalleCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand CerrarCommand { get; }
    public ICommand InscribirCommand { get; }

    public TalleresViewModel(MainViewModel main)
    {
        _main = main;

        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(Editar);
        VerDetalleCommand = new RelayCommand(VerDetalle);
        EliminarCommand = new RelayCommand(Eliminar);
        GuardarCommand = new RelayCommand(_ => Guardar());
        CerrarCommand = new RelayCommand(_ => Cerrar());
        InscribirCommand = new RelayCommand(Inscribir);
    }

    private static List<string> Lista(int primera, int ultima) =>
        Enumerable.Range(primera, ultima - primera + 1).Select(Formato.Hora).ToList();

    // Del texto elegido en el ComboBox saca la hora (posición en la lista + primera hora)
    private static int Valor(List<string> horas, string? elegida, int primera) =>
        elegida is null ? 0 : primera + horas.IndexOf(elegida);

    public async void Cargar()
    {
        try
        {
            _todos = await talleresService.GetAll();
            var lista = await periodosService.GetAll();

            // Se conserva el período elegido si sigue existiendo; si no, el más reciente
            var idActual = PeriodoSeleccionado?.Id;
            Periodos.Clear();
            lista.ForEach(Periodos.Add);
            PeriodoSeleccionado = Periodos.FirstOrDefault(p => p.Id == idActual) ?? Periodos.OrderBy(p => p.Id).LastOrDefault();
            Aplicar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private void Aplicar()
    {
        bool Coincide(TallerDTO t) => string.IsNullOrWhiteSpace(TextoBusqueda) || Formato.Contiene(t.Nombre, TextoBusqueda);

        // Catálogo: todos los talleres
        Catalogo.Clear();
        foreach (var t in _todos.Where(Coincide)) Catalogo.Add(new TallerFila(t));

        // Disponibilidad: los del período elegido. Los contadores no dependen de "solo con cupo".
        var delPeriodo = _todos.Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id).ToList();
        var filas = delPeriodo.Where(Coincide).Select(t => new TallerFila(t)).ToList();
        ConteoConCupo = filas.Count(f => f.Tono == "Verde");
        ConteoPocos = filas.Count(f => f.Tono == "Ambar");
        ConteoLlenos = filas.Count(f => f.Tono == "Rojo");

        Disponibles.Clear();
        foreach (var f in filas.Where(f => !SoloConCupo || f.TieneCupo)) Disponibles.Add(f);

        // Agenda: talleres del período con el mismo horario quedan juntos
        Franjas.Clear();
        foreach (var g in delPeriodo.GroupBy(t => (t.HoraInicio, t.HoraFin)).OrderBy(g => g.Key.HoraInicio).ThenBy(g => g.Key.HoraFin))
        {
            Franjas.Add(new FranjaFila
            {
                Hora = Formato.Rango(g.Key.HoraInicio, g.Key.HoraFin),
                Talleres = g.Select(t => new TallerFila(t)).ToList()
            });
        }
    }

    private void Nuevo()
    {
        _editando = null;
        TituloFormulario = "Nuevo taller";
        Nombre = "";
        CategoriaSeleccionada = null;
        PeriodoFormulario = PeriodoSeleccionado;
        Cupo = "";
        Costo = "";
        HoraInicio = HoraFin = HoraInicioSabado = HoraFinSabado = null;
        Errores.Clear();
        IsFormularioAbierto = true;
    }

    private void Editar(object? parametro)
    {
        if (parametro is not TallerFila fila) return;

        var t = fila.Taller;
        _editando = t;
        TituloFormulario = "Editar taller";
        Nombre = t.Nombre;
        CategoriaSeleccionada = NombresCategorias[Math.Clamp((int)t.Categoria - 1, 0, 2)];
        PeriodoFormulario = Periodos.FirstOrDefault(p => p.Id == t.IdPeriodo);
        Cupo = t.Cupo.ToString();
        Costo = t.Costo.ToString(CultureInfo.InvariantCulture);
        HoraInicio = Formato.Hora(t.HoraInicio);
        HoraFin = Formato.Hora(t.HoraFin);
        HoraInicioSabado = Formato.Hora(t.HoraInicioSabado);
        HoraFinSabado = Formato.Hora(t.HoraFinSabado);
        Errores.Clear();
        IsDetalleAbierto = false;
        IsFormularioAbierto = true;
    }

    private async void Guardar()
    {
        // Primero lo que no se puede convertir (texto a número o ComboBox vacío)
        var faltantes = new List<string>();
        if (!decimal.TryParse(Costo, NumberStyles.Number, CultureInfo.InvariantCulture, out var costo))
            faltantes.Add("Escribe el costo como número (ej. 420 o 420.50).");
        if (!int.TryParse(Cupo, out var cupo))
            faltantes.Add("Escribe el cupo como número entero.");
        if (CategoriaSeleccionada is null) faltantes.Add("Elige una categoría.");
        if (PeriodoFormulario is null) faltantes.Add("Elige un período.");
        if (HoraInicio is null || HoraFin is null) faltantes.Add("Elige la hora de inicio y de fin entre semana.");
        if (HoraInicioSabado is null || HoraFinSabado is null) faltantes.Add("Elige la hora de inicio y de fin del sábado.");
        if (faltantes.Count > 0)
        {
            MostrarErrores(faltantes);
            return;
        }

        // La posición en la lista coincide con el enum (arte=1, deportes=2, tecnología=3)
        var categoria = (Categorias)(NombresCategorias.IndexOf(CategoriaSeleccionada!) + 1);
        var instructor = string.IsNullOrWhiteSpace(_editando?.NombreInstructor) ? InstructorPendiente : _editando.NombreInstructor;

        var dto = new EditTallerDTO
        {
            Id = _editando?.Id ?? 0,
            Nombre = Nombre.Trim(),
            Costo = costo,
            Cupo = cupo,
            Categoria = categoria,
            IdPeriodo = PeriodoFormulario!.Id,
            NombreInstructor = instructor,
            HoraInicio = Valor(Horas, HoraInicio, SemanaPrimera),
            HoraFin = Valor(Horas, HoraFin, SemanaPrimera),
            HoraInicioSabado = Valor(HorasSabadoInicio, HoraInicioSabado, SabadoInicioPrimera),
            HoraFinSabado = Valor(HorasSabadoFin, HoraFinSabado, SabadoFinPrimera)
        };

        try
        {
            var errores = new ValidatorClass();
            bool listo;

            if (_editando is null)
            {
                // EditTallerDTO hereda de CreateTallerDTO, así que sirve para crear
                var validacion = new CreateTallerValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await talleresService.Create(dto, errores) != null;
            }
            else
            {
                var validacion = new EditTallerValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await talleresService.Update(dto, errores);
            }

            if (!listo)
            {
                MostrarErrores(errores.Errores);
                return;
            }

            IsFormularioAbierto = false;
            _main.ShowToast("Taller guardado", $"Se guardó \"{dto.Nombre}\".");
            Cargar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private async void Eliminar(object? parametro)
    {
        if (parametro is not TallerFila fila) return;

        var respuesta = MessageBox.Show($"¿Eliminar \"{fila.Nombre}\"?", "Eliminar taller",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (respuesta != MessageBoxResult.Yes) return;

        try
        {
            var errores = new ValidatorClass();
            if (await talleresService.Delete(fila.Taller.Id, errores))
            {
                IsDetalleAbierto = false;
                _main.ShowToast("Taller eliminado", $"Se eliminó \"{fila.Nombre}\".");
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
        if (parametro is not TallerFila fila) return;

        Detalle = fila;
        DetallePeriodo = Periodos.FirstOrDefault(p => p.Id == fila.Taller.IdPeriodo)?.ToString() ?? "";
        Inscritos.Clear();
        IsDetalleAbierto = true;

        try
        {
            var alumnos = await alumnosService.GetAll();
            var inscripciones = await inscripcionesService.GetAll();

            var filas = from i in inscripciones
                        where i.IdTaller == fila.Taller.Id
                        join a in alumnos on i.IdAlumno equals a.Id
                        select InscripcionFila.Crear(i, a, fila.Taller);
            foreach (var f in filas) Inscritos.Add(f);
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

    // Desde Disponibilidad: abre Inscribir con este taller ya agregado
    private void Inscribir(object? parametro)
    {
        if (parametro is not TallerFila fila) return;
        _main.Inscripciones.Preseleccionar(fila.Taller);
        _main.IrA("Inscribir");
    }
}
