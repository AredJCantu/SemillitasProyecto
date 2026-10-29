using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan CobranzaView de Windows y de Android (Android no usa MarcarPagado)
public partial class CobranzaViewModel : BaseViewModel
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<Taller> _todosTalleres = new();
    private List<Alumno> _todosAlumnos = new();
    private List<Inscripcion> _todasInscripciones = new();

    [ObservableProperty] private List<Periodo> periodos = new();
    [ObservableProperty] private Periodo? periodoSeleccionado;
    [ObservableProperty] private string cobrado = "$0";
    [ObservableProperty] private string porCobrar = "$0";
    [ObservableProperty] private int alumnosConAdeudo;
    [ObservableProperty] private List<Chip> filtros = new();
    [ObservableProperty] private Chip? filtroSeleccionado;
    [ObservableProperty] private string textoBusqueda = "";
    [ObservableProperty] private List<CobroFila> cobros = new();

    partial void OnPeriodoSeleccionadoChanged(Periodo? value) => Aplicar();
    partial void OnFiltroSeleccionadoChanged(Chip? value) => Aplicar();
    partial void OnTextoBusquedaChanged(string value) => Aplicar();

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todosTalleres = await _talleres.ObtenerAsync();
        _todosAlumnos = await _alumnos.ObtenerAsync();
        _todasInscripciones = await _inscripciones.ObtenerAsync();
        var lista = await _periodos.ObtenerAsync();

        Filtros = new() { new() { Texto = "Todos" }, new() { Texto = "Deben" }, new() { Texto = "Al corriente" } };
        FiltroSeleccionado ??= Filtros[0];

        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    });

    private void Aplicar()
    {
        // Una fila por inscripción de un taller del período elegido
        var filas = (from i in _todasInscripciones
                     join t in _todosTalleres on i.IdTaller equals t.Id
                     join a in _todosAlumnos on i.IdAlumno equals a.Id
                     where t.IdPeriodo == PeriodoSeleccionado?.Id
                     select new { i, t, a }).ToList();

        // Los totales no dependen del filtro ni de la búsqueda
        Cobrado = Formato.Dinero(filas.Where(x => !x.i.Debe).Sum(x => x.t.Costo));
        PorCobrar = Formato.Dinero(filas.Where(x => x.i.Debe).Sum(x => x.t.Costo));
        AlumnosConAdeudo = filas.Where(x => x.i.Debe).Select(x => x.a.Id).Distinct().Count();

        Cobros = filas
            .Where(x => FiltroSeleccionado?.Texto switch
            {
                "Deben" => x.i.Debe,
                "Al corriente" => !x.i.Debe,
                _ => true
            })
            .Where(x => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(Formato.NombreCompleto(x.a), TextoBusqueda)
                        || Formato.Contiene(Formato.Tutor(x.a), TextoBusqueda)
                        || Formato.Contiene(x.t.Nombre, TextoBusqueda))
            .Select(x => new CobroFila
            {
                Inscripcion = x.i,
                Tutor = Formato.Tutor(x.a),
                Alumno = Formato.NombreCompleto(x.a),
                Taller = x.t.Nombre,
                Costo = Formato.Dinero(x.t.Costo),
                Estado = x.i.Debe ? "Debe" : "Al corriente",
                Tono = x.i.Debe ? "Rojo" : "Verde",
                PuedeMarcarPago = x.i.Debe
            }).ToList();
    }

    [RelayCommand]
    private async Task MarcarPagado(CobroFila fila)
    {
        if (!await Confirmar("Marcar pagado", $"¿{fila.Alumno} pagó {fila.Taller}?", "Sí, pagó")) return;

        await Ejecutar(async () =>
        {
            await _inscripciones.MarcarPagadaAsync(fila.Inscripcion);   // cambia Debe en el mismo objeto
            Aplicar();
        });
    }
}
