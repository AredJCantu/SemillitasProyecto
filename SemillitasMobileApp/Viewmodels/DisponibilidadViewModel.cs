using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan MainView de Windows y MainView de Android
public partial class DisponibilidadViewModel : BaseViewModel
{
    private const int TamañoPagina = 8;

    private readonly TalleresService _talleres = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<Taller> _todos = new();
    private List<Inscripcion> _inscritos = new();
    private int _pagina;

    [ObservableProperty] private List<Periodo> periodos = new();
    [ObservableProperty] private Periodo? periodoSeleccionado;
    [ObservableProperty] private string textoBusqueda = "";
    [ObservableProperty] private bool soloConCupo;
    [ObservableProperty] private List<Chip> categorias = new();
    [ObservableProperty] private Chip? categoriaSeleccionada;
    [ObservableProperty] private int conteoConCupo;
    [ObservableProperty] private int conteoPocos;
    [ObservableProperty] private int conteoLlenos;
    [ObservableProperty] private List<TallerFila> talleres = new();
    [ObservableProperty] private string resumenListado = "";

    // Cualquier filtro que cambie vuelve a calcular la lista (primera página)
    partial void OnPeriodoSeleccionadoChanged(Periodo? value) => Aplicar(reiniciarPagina: true);
    partial void OnTextoBusquedaChanged(string value) => Aplicar(reiniciarPagina: true);
    partial void OnSoloConCupoChanged(bool value) => Aplicar(reiniciarPagina: true);
    partial void OnCategoriaSeleccionadaChanged(Chip? value) => Aplicar(reiniciarPagina: true);

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todos = await _talleres.ObtenerAsync();
        _inscritos = await _inscripciones.ObtenerAsync();
        var lista = await _periodos.ObtenerAsync();

        Categorias = new() { new() { Texto = "Todas" }, new() { Texto = "Arte" }, new() { Texto = "Deportes" }, new() { Texto = "Tecnología" } };
        CategoriaSeleccionada ??= Categorias[0];

        // Se conserva el período elegido si sigue existiendo; si no, el más reciente
        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    });

    private void Aplicar(bool reiniciarPagina = false)
    {
        if (reiniciarPagina) _pagina = 0;

        var delPeriodo = _todos.Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id).ToList();

        var filas = delPeriodo
            .Where(t => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(t.Nombre, TextoBusqueda)
                        || Formato.Contiene(t.NombreInstructor, TextoBusqueda))
            .Where(t => CategoriaSeleccionada is null || CategoriaSeleccionada.Texto == "Todas"
                        || Formato.Categoria(t.Categoria) == CategoriaSeleccionada.Texto)
            .Select(CrearFila)
            .ToList();

        // Los contadores cuentan lo que hay con los filtros de arriba, antes de "solo con cupo"
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

    private int _totalFiltrados;

    private TallerFila CrearFila(Taller t)
    {
        var inscritos = _inscritos.Count(i => i.IdTaller == t.Id);
        var libres = Math.Max(t.Cupo - inscritos, 0);

        var (badge, tono) = Formato.Lugares(t.Cupo, inscritos);

        return new TallerFila
        {
            Taller = t,
            Nombre = t.Nombre,
            Meta = $"{Formato.Categoria(t.Categoria)} · {t.NombreInstructor} · {Formato.Dinero(t.Costo)}",
            Horario = Formato.Rango(t),
            Ocupacion = $"{inscritos}/{t.Cupo}",
            Porcentaje = t.Cupo == 0 ? 1 : Math.Min((double)inscritos / t.Cupo, 1),
            Badge = badge,
            Tono = tono,
            TieneCupo = libres > 0
        };
    }

    [RelayCommand]
    private Task NuevaInscripcion() => Shell.Current.GoToAsync($"//{Rutas.Inscripcion}");

    // La inscripción es una sección raíz del Shell ("//"), se le manda el taller como parámetro
    [RelayCommand]
    private Task InscribirTaller(TallerFila fila) =>
        Shell.Current.GoToAsync($"//{Rutas.Inscripcion}", new Dictionary<string, object> { ["Taller"] = fila.Taller });

    [RelayCommand]
    private void PaginaAnterior()
    {
        if (_pagina == 0) return;
        _pagina--;
        Aplicar();
    }

    [RelayCommand]
    private void PaginaSiguiente()
    {
        if ((_pagina + 1) * TamañoPagina >= _totalFiltrados) return;
        _pagina++;
        Aplicar();
    }
}
