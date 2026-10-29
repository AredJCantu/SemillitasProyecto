using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

public partial class AgendaViewModel : BaseViewModel
{
    private readonly TalleresService _talleres = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<Taller> _todos = new();
    private List<Inscripcion> _inscritos = new();

    [ObservableProperty] private List<Periodo> periodos = new();
    [ObservableProperty] private Periodo? periodoSeleccionado;
    [ObservableProperty] private List<FranjaFila> franjas = new();

    partial void OnPeriodoSeleccionadoChanged(Periodo? value) => Aplicar();

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todos = await _talleres.ObtenerAsync();
        _inscritos = await _inscripciones.ObtenerAsync();
        var lista = await _periodos.ObtenerAsync();

        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    });

    // Talleres con la misma hora de inicio y fin quedan en la misma franja
    private void Aplicar()
    {
        Franjas = _todos
            .Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id)
            .GroupBy(t => (t.HoraInicio, t.HoraFin))
            .OrderBy(g => g.Key.HoraInicio).ThenBy(g => g.Key.HoraFin)
            .Select(g => new FranjaFila
            {
                Hora = Formato.Rango(g.Key.HoraInicio, g.Key.HoraFin),
                Talleres = g.Select(t => new EventoFila
                {
                    Nombre = t.Nombre,
                    Detalle = $"{t.NombreInstructor} · {_inscritos.Count(i => i.IdTaller == t.Id)}/{t.Cupo}",
                    Categoria = Formato.CategoriaClave(t.Categoria)
                }).ToList()
            }).ToList();
    }
}
