using SemillasCore.DTOs;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan MainView de Windows y MainView de Android.
// CAMBIO: sin toolkit (propiedades manuales con SetField y comandos propios), sin chips de categoría
// y sin leer inscripciones: el API ya manda CantidadAlumnos en cada TallerDTO.
public class DisponibilidadViewModel : ViewModelBase
{
    private const int TamañoPagina = 8;

    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();

    private List<TallerDTO> _todos = new();
    private int _pagina;
    private int _totalFiltrados;

    public DisponibilidadViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        NuevaInscripcionCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync($"//{Rutas.Inscripcion}"));
        // La inscripción es una sección raíz del Shell ("//"), se le manda el taller como parámetro
        InscribirTallerCommand = new AsyncRelayCommand<TallerFila>(fila =>
            Shell.Current.GoToAsync($"//{Rutas.Inscripcion}", new Dictionary<string, object> { ["Taller"] = fila.Taller }));
        PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(PaginaSiguiente);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevaInscripcionCommand { get; }
    public AsyncRelayCommand<TallerFila> InscribirTallerCommand { get; }
    public RelayCommand PaginaAnteriorCommand { get; }
    public RelayCommand PaginaSiguienteCommand { get; }

    private List<PeriodoDTO> _periodosLista = new();
    public List<PeriodoDTO> Periodos { get => _periodosLista; set => SetField(ref _periodosLista, value); }

    // Cualquier filtro que cambie vuelve a calcular la lista (primera página)
    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado
    {
        get => _periodoSeleccionado;
        set { if (SetField(ref _periodoSeleccionado, value)) Aplicar(reiniciarPagina: true); }
    }

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Aplicar(reiniciarPagina: true); }
    }

    private bool _soloConCupo;
    public bool SoloConCupo
    {
        get => _soloConCupo;
        set { if (SetField(ref _soloConCupo, value)) Aplicar(reiniciarPagina: true); }
    }

    private int _conteoConCupo;
    public int ConteoConCupo { get => _conteoConCupo; set => SetField(ref _conteoConCupo, value); }

    private int _conteoPocos;
    public int ConteoPocos { get => _conteoPocos; set => SetField(ref _conteoPocos, value); }

    private int _conteoLlenos;
    public int ConteoLlenos { get => _conteoLlenos; set => SetField(ref _conteoLlenos, value); }

    private List<TallerFila> _talleresLista = new();
    public List<TallerFila> Talleres { get => _talleresLista; set => SetField(ref _talleresLista, value); }

    private string _resumenListado = "";
    public string ResumenListado { get => _resumenListado; set => SetField(ref _resumenListado, value); }

    private async Task Cargar()
    {
        _todos = await _talleres.ObtenerAsync();
        var lista = await _periodos.ObtenerAsync();

        // Se conserva el período elegido si sigue existiendo; si no, el más reciente
        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    }

    private void Aplicar(bool reiniciarPagina = false)
    {
        if (reiniciarPagina) _pagina = 0;

        // CAMBIO: la búsqueda es solo por nombre de taller (el instructor queda fuera por ahora)
        var filas = _todos
            .Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id)
            .Where(t => string.IsNullOrWhiteSpace(TextoBusqueda) || Formato.Contiene(t.Nombre, TextoBusqueda))
            .Select(CrearFila)
            .ToList();

        // Los contadores cuentan lo que hay con la búsqueda, antes de "solo con cupo"
        ConteoConCupo = filas.Count(f => f.Tono == "Verde");
        ConteoPocos = filas.Count(f => f.Tono == "Ambar");
        ConteoLlenos = filas.Count(f => f.Tono == "Rojo");

        if (SoloConCupo) filas = filas.Where(f => f.TieneCupo).ToList();

        var pagina = filas.Skip(_pagina * TamañoPagina).Take(TamañoPagina).ToList();
        Talleres = pagina;
        ResumenListado = filas.Count == 0
            ? "Sin resultados"
            : $"Mostrando {_pagina * TamañoPagina + 1}–{_pagina * TamañoPagina + pagina.Count} de {filas.Count}";
        _totalFiltrados = filas.Count;
    }

    private static TallerFila CrearFila(TallerDTO t)
    {
        // Cupo = lugares que quedan; el máximo es lo que quedan más los inscritos
        var maximo = Formato.Maximo(t);
        var (badge, tono) = Formato.Lugares(t.Cupo, maximo);

        return new TallerFila
        {
            Taller = t,
            Nombre = t.Nombre,
            Meta = $"{Formato.Categoria(t.Categoria)} · {Formato.Dinero(t.Costo)}",
            Horario = Formato.Horario(t),
            Ocupacion = $"{t.CantidadAlumnos}/{maximo}",
            Porcentaje = maximo == 0 ? 1 : Math.Min((double)t.CantidadAlumnos / maximo, 1),
            Badge = badge,
            Tono = tono,
            TieneCupo = t.Cupo > 0
        };
    }

    private void PaginaAnterior()
    {
        if (_pagina == 0) return;
        _pagina--;
        Aplicar();
    }

    private void PaginaSiguiente()
    {
        if ((_pagina + 1) * TamañoPagina >= _totalFiltrados) return;
        _pagina++;
        Aplicar();
    }
}
