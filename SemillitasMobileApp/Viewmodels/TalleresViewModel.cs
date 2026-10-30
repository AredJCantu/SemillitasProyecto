using System.Windows.Input;
using SemillasCore.DTOs;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Infrastructure;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.ViewModels;

// Lectura de talleres: lo usan Disponibilidad y Agenda
public class TalleresViewModel : ViewModelBase
{
    TalleresService talleresService = new();
    PeriodosService periodosService = new();

    private List<TallerDTO> _todos = new();

    private List<PeriodoDTO> _periodos = new();
    public List<PeriodoDTO> Periodos { get => _periodos; set => SetField(ref _periodos, value); }

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

    private int _conteoConCupo;
    public int ConteoConCupo { get => _conteoConCupo; set => SetField(ref _conteoConCupo, value); }

    private int _conteoPocos;
    public int ConteoPocos { get => _conteoPocos; set => SetField(ref _conteoPocos, value); }

    private int _conteoLlenos;
    public int ConteoLlenos { get => _conteoLlenos; set => SetField(ref _conteoLlenos, value); }

    private List<TallerFila> _disponibles = new();
    public List<TallerFila> Disponibles { get => _disponibles; set => SetField(ref _disponibles, value); }

    private List<FranjaFila> _franjas = new();
    public List<FranjaFila> Franjas { get => _franjas; set => SetField(ref _franjas, value); }

    public ICommand CargarCommand { get; }

    public TalleresViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
    }

    private async void Cargar()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            _todos = await talleresService.GetAll();
            var lista = await periodosService.GetAll();

            // Se conserva el período elegido si sigue existiendo; si no, el más reciente
            var idActual = PeriodoSeleccionado?.Id;
            Periodos = lista;
            PeriodoSeleccionado = lista.FirstOrDefault(p => p.Id == idActual) ?? lista.OrderBy(p => p.Id).LastOrDefault();
            Aplicar();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Sin conexión", ex.Message, "Aceptar");
        }
        IsBusy = false;
    }

    private void Aplicar()
    {
        var delPeriodo = _todos.Where(t => t.IdPeriodo == PeriodoSeleccionado?.Id).ToList();

        // Disponibilidad: filtrada por nombre
        Disponibles = delPeriodo
            .Where(t => string.IsNullOrWhiteSpace(TextoBusqueda) || Formato.Contiene(t.Nombre, TextoBusqueda))
            .Select(t => new TallerFila(t))
            .ToList();
        ConteoConCupo = Disponibles.Count(f => f.Tono == "Verde");
        ConteoPocos = Disponibles.Count(f => f.Tono == "Ambar");
        ConteoLlenos = Disponibles.Count(f => f.Tono == "Rojo");

        // Agenda: talleres con el mismo horario entre semana quedan juntos
        Franjas = delPeriodo
            .GroupBy(t => (t.HoraInicio, t.HoraFin))
            .OrderBy(g => g.Key.HoraInicio).ThenBy(g => g.Key.HoraFin)
            .Select(g => new FranjaFila
            {
                Hora = Formato.Rango(g.Key.HoraInicio, g.Key.HoraFin),
                Talleres = g.Select(t => new TallerFila(t)).ToList()
            })
            .ToList();
    }
}
