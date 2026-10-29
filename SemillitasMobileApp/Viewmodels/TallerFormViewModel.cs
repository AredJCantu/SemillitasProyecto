using System.Globalization;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;
using SemillitasProyecto.Commons;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo taller (sin parámetro) o editar (GoToAsync con { "Taller": taller }).
// CAMBIOS: sin toolkit; sin campo de instructor (se manda "Por asignar" al crear y se conserva al editar);
// se agregan las horas de sábado; los rangos de horas salen de los validators de SemillasCore;
// errores en una sola lista.
public class TallerFormViewModel : ViewModelBase, IQueryAttributable
{
    // Rangos permitidos por los validators: entre semana 12–22, sábado inicio 5–12 y fin 6–13
    private const int SemanaPrimera = 12, SemanaUltima = 22;
    private const int SabadoInicioPrimera = 5, SabadoInicioUltima = 12;
    private const int SabadoFinPrimera = 6, SabadoFinUltima = 13;
    private const string InstructorPendiente = "Por asignar";

    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();
    private TallerDTO? _taller;

    public TallerFormViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        CancelarCommand = new AsyncRelayCommand(Regresar);
        GuardarCommand = new AsyncRelayCommand(() => Ejecutar(Guardar));
        EliminarCommand = new AsyncRelayCommand(Eliminar);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand CancelarCommand { get; }
    public AsyncRelayCommand GuardarCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }

    private string _titulo = "Nuevo taller";
    public string Titulo { get => _titulo; set => SetField(ref _titulo, value); }

    private bool _esEdicion;
    public bool EsEdicion { get => _esEdicion; set => SetField(ref _esEdicion, value); }

    private string _nombre = "";
    public string Nombre { get => _nombre; set => SetField(ref _nombre, value); }

    // CAMBIO: se llama NombresCategorias para no chocar con el enum Categorias de SemillasCore
    public List<string> NombresCategorias { get; } = new() { "Arte", "Deportes", "Tecnología" };

    private string? _categoriaSeleccionada;
    public string? CategoriaSeleccionada { get => _categoriaSeleccionada; set => SetField(ref _categoriaSeleccionada, value); }

    private List<PeriodoDTO> _periodosLista = new();
    public List<PeriodoDTO> Periodos { get => _periodosLista; set => SetField(ref _periodosLista, value); }

    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado { get => _periodoSeleccionado; set => SetField(ref _periodoSeleccionado, value); }

    // Cupo = lugares disponibles (el API lo baja con cada inscripción)
    private string _cupo = "";
    public string Cupo { get => _cupo; set => SetField(ref _cupo, value); }

    private string _costo = "";
    public string Costo { get => _costo; set => SetField(ref _costo, value); }

    // Listas de horas para los Picker
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

    private static List<string> Lista(int primera, int ultima) =>
        Enumerable.Range(primera, ultima - primera + 1).Select(Formato.Hora).ToList();

    // Del texto elegido en el Picker saca la hora (posición en la lista + primera hora). 0 si no eligió.
    private static int Valor(List<string> horas, string? elegida, int primera) =>
        elegida is null ? 0 : primera + horas.IndexOf(elegida);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is TallerDTO t)
        {
            _taller = t;
            EsEdicion = true;
            Titulo = "Editar taller";
            Nombre = t.Nombre;
            CategoriaSeleccionada = NombresCategorias[Math.Clamp((int)t.Categoria - 1, 0, 2)];
            Cupo = t.Cupo.ToString();
            Costo = t.Costo.ToString(CultureInfo.InvariantCulture);
            HoraInicio = Formato.Hora(t.HoraInicio);
            HoraFin = Formato.Hora(t.HoraFin);
            HoraInicioSabado = Formato.Hora(t.HoraInicioSabado);
            HoraFinSabado = Formato.Hora(t.HoraFinSabado);
        }
    }

    private async Task Cargar()
    {
        Periodos = await _periodos.GetAll();
        PeriodoSeleccionado = Periodos.FirstOrDefault(p => p.Id == _taller?.IdPeriodo)
                              ?? PeriodoSeleccionado
                              ?? Periodos.OrderBy(p => p.Id).LastOrDefault();
    }

    private async Task Guardar()
    {
        LimpiarErrores();

        // Primero lo que no se puede convertir (texto a número o Picker vacío)
        var faltantes = new List<string>();
        if (!decimal.TryParse(Costo, NumberStyles.Number, CultureInfo.InvariantCulture, out var costo))
            faltantes.Add("Escribe el costo como número (ej. 420 o 420.50).");
        if (!int.TryParse(Cupo, out var cupo))
            faltantes.Add("Escribe el cupo como número entero.");
        if (CategoriaSeleccionada is null) faltantes.Add("Elige una categoría.");
        if (PeriodoSeleccionado is null) faltantes.Add("Elige un período.");
        if (HoraInicio is null || HoraFin is null) faltantes.Add("Elige la hora de inicio y de fin entre semana.");
        if (HoraInicioSabado is null || HoraFinSabado is null) faltantes.Add("Elige la hora de inicio y de fin del sábado.");
        if (faltantes.Count > 0) { MostrarErrores(faltantes); return; }

        var nombre = Nombre.Trim();
        // Categoría: la posición en la lista coincide con el enum (arte=1, deportes=2, tecnología=3)
        var categoria = (Categorias)(IndiceCategoria() + 1);
        var instructor = _taller is null || string.IsNullOrWhiteSpace(_taller.NombreInstructor)
            ? InstructorPendiente : _taller.NombreInstructor;
        var inicio = Valor(Horas, HoraInicio, SemanaPrimera);
        var fin = Valor(Horas, HoraFin, SemanaPrimera);
        var inicioSab = Valor(HorasSabadoInicio, HoraInicioSabado, SabadoInicioPrimera);
        var finSab = Valor(HorasSabadoFin, HoraFinSabado, SabadoFinPrimera);

        if (_taller is null)
        {
            var nuevo = new CreateTallerDTO
            {
                Nombre = nombre, Costo = costo, Cupo = cupo, Categoria = categoria, IdPeriodo = PeriodoSeleccionado!.Id,
                NombreInstructor = instructor, HoraInicio = inicio, HoraFin = fin,
                HoraInicioSabado = inicioSab, HoraFinSabado = finSab
            };
            if (!EsValido(new CreateTallerValidator().IsValid(nuevo))) return;
            var errores = new ValidatorClass();
            if (await _talleres.Create(nuevo, errores) is null) { MostrarErrores(errores.Errores); return; }
        }
        else
        {
            var edicion = new EditTallerDTO
            {
                Id = _taller.Id,
                Nombre = nombre, Costo = costo, Cupo = cupo, Categoria = categoria, IdPeriodo = PeriodoSeleccionado!.Id,
                NombreInstructor = instructor, HoraInicio = inicio, HoraFin = fin,
                HoraInicioSabado = inicioSab, HoraFinSabado = finSab
            };
            if (!EsValido(new EditTallerValidator().IsValid(edicion))) return;
            var errores = new ValidatorClass();
            if (!await _talleres.Update(edicion, errores)) { MostrarErrores(errores.Errores); return; }
        }

        await Regresar();
    }

    private int IndiceCategoria() => Math.Max(NombresCategorias.IndexOf(CategoriaSeleccionada!), 0);

    private async Task Eliminar()
    {
        if (_taller is null) return;
        if (!await Confirmar("Eliminar taller", $"¿Eliminar \"{_taller.Nombre}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _talleres.Delete(_taller.Id, errores)) { MostrarErrores(errores.Errores); return; }
            await Regresar();
        });
    }
}
