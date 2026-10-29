using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan CobranzaView de Windows y de Android (Android no usa MarcarPagado).
// CAMBIO: cobranza se calcula en el cliente cruzando inscripciones, talleres y alumnos
// (el costo sale del taller de cada inscripción).
public class CobranzaViewModel : ViewModelBase
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<TallerDTO> _todosTalleres = new();
    private List<AlumnoDTO> _todosAlumnos = new();
    private List<InscripcionesDTO> _todasInscripciones = new();

    public CobranzaViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        MarcarPagadoCommand = new AsyncRelayCommand<CobroFila>(MarcarPagado);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand<CobroFila> MarcarPagadoCommand { get; }

    private List<PeriodoDTO> _periodosLista = new();
    public List<PeriodoDTO> Periodos { get => _periodosLista; set => SetField(ref _periodosLista, value); }

    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado
    {
        get => _periodoSeleccionado;
        set { if (SetField(ref _periodoSeleccionado, value)) Aplicar(); }
    }

    private string _cobrado = "$0";
    public string Cobrado { get => _cobrado; set => SetField(ref _cobrado, value); }

    private string _porCobrar = "$0";
    public string PorCobrar { get => _porCobrar; set => SetField(ref _porCobrar, value); }

    private int _alumnosConAdeudo;
    public int AlumnosConAdeudo { get => _alumnosConAdeudo; set => SetField(ref _alumnosConAdeudo, value); }

    private List<Chip> _filtros = new();
    public List<Chip> Filtros { get => _filtros; set => SetField(ref _filtros, value); }

    private Chip? _filtroSeleccionado;
    public Chip? FiltroSeleccionado
    {
        get => _filtroSeleccionado;
        set { if (SetField(ref _filtroSeleccionado, value)) Aplicar(); }
    }

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Aplicar(); }
    }

    private List<CobroFila> _cobros = new();
    public List<CobroFila> Cobros { get => _cobros; set => SetField(ref _cobros, value); }

    private async Task Cargar()
    {
        _todosTalleres = await _talleres.GetAll();
        _todosAlumnos = await _alumnos.GetAll();
        _todasInscripciones = await _inscripciones.GetAll();
        var lista = await _periodos.GetAll();

        Filtros = new() { new() { Texto = "Todos" }, new() { Texto = "Deben" }, new() { Texto = "Al corriente" } };
        FiltroSeleccionado ??= Filtros[0];

        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    }

    private void Aplicar()
    {
        // Una fila por inscripción de un taller del período elegido
        var filas = (from i in _todasInscripciones
                     join t in _todosTalleres on i.IdTaller equals t.Id
                     join a in _todosAlumnos on i.IdAlumno equals a.Id
                     where t.IdPeriodo == PeriodoSeleccionado?.Id
                     select new { i, t, a }).ToList();

        // Debe es bool? en el DTO: solo cuenta como adeudo cuando es true
        Cobrado = Formato.Dinero(filas.Where(x => x.i.Debe != true).Sum(x => x.t.Costo));
        PorCobrar = Formato.Dinero(filas.Where(x => x.i.Debe == true).Sum(x => x.t.Costo));
        AlumnosConAdeudo = filas.Where(x => x.i.Debe == true).Select(x => x.a.Id).Distinct().Count();

        Cobros = filas
            .Where(x => FiltroSeleccionado?.Texto switch
            {
                "Deben" => x.i.Debe == true,
                "Al corriente" => x.i.Debe != true,
                _ => true
            })
            .Where(x => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(Formato.NombreCompleto(x.a), TextoBusqueda)
                        || Formato.Contiene(x.a.NombreTutor, TextoBusqueda)
                        || Formato.Contiene(x.t.Nombre, TextoBusqueda))
            .Select(x => new CobroFila
            {
                Inscripcion = x.i,
                Tutor = x.a.NombreTutor,
                Alumno = Formato.NombreCompleto(x.a),
                Taller = x.t.Nombre,
                Costo = Formato.Dinero(x.t.Costo),
                Estado = x.i.Debe == true ? "Debe" : "Al corriente",
                Tono = x.i.Debe == true ? "Rojo" : "Verde",
                PuedeMarcarPago = x.i.Debe == true
            }).ToList();
    }

    private async Task MarcarPagado(CobroFila fila)
    {
        if (!await Confirmar("Marcar pagado", $"¿{fila.Alumno} pagó {fila.Taller}?", "Sí, pagó")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _inscripciones.MarcarPagada(fila.Inscripcion, errores))
            {
                await Avisar("No se pudo marcar el pago", string.Join("\n", errores.Errores));
                return;
            }
            Aplicar();
        });
    }
}
