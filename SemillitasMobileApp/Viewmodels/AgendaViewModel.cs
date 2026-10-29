using SemillasCore.DTOs;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// CAMBIO: sin toolkit; el detalle de cada taller es inscritos/máximo (sin instructor, que queda fuera por ahora)
public class AgendaViewModel : ViewModelBase
{
    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();

    private List<TallerDTO> _todos = new();

    public AgendaViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
    }

    public AsyncRelayCommand CargarCommand { get; }

    private List<PeriodoDTO> _periodosLista = new();
    public List<PeriodoDTO> Periodos { get => _periodosLista; set => SetField(ref _periodosLista, value); }

    private PeriodoDTO? _periodoSeleccionado;
    public PeriodoDTO? PeriodoSeleccionado
    {
        get => _periodoSeleccionado;
        set { if (SetField(ref _periodoSeleccionado, value)) Aplicar(); }
    }

    private List<FranjaFila> _franjas = new();
    public List<FranjaFila> Franjas { get => _franjas; set => SetField(ref _franjas, value); }

    private async Task Cargar()
    {
        _todos = await _talleres.ObtenerAsync();
        var lista = await _periodos.ObtenerAsync();

        var idActual = PeriodoSeleccionado?.Id;
        Periodos = lista;
        PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
        Aplicar();
    }

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
                    Detalle = $"{t.CantidadAlumnos}/{Formato.Maximo(t)} inscritos",
                    Categoria = Formato.CategoriaClave(t.Categoria)
                }).ToList()
            }).ToList();
    }
}
