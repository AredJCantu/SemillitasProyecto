using System.Windows.Input;
using SemillasCore.DTOs;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Infrastructure;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.ViewModels;

// Lectura de inscripciones: la Cobranza del período (sin marcar pagos)
public class InscripcionesViewModel : ViewModelBase
{
    TalleresService talleresService = new();
    AlumnosService alumnosService = new();
    InscripcionesService inscripcionesService = new();
    PeriodosService periodosService = new();

    private List<TallerDTO> _talleres = new();
    private List<AlumnoDTO> _alumnos = new();
    private List<InscripcionesDTO> _inscripciones = new();

    private List<PeriodoDTO> _periodos = new();
    public List<PeriodoDTO> Periodos { get => _periodos; set => SetField(ref _periodos, value); }

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

    public List<string> Filtros { get; } = new() { "Todos", "Deben", "Al corriente" };

    private string _filtroSeleccionado = "Todos";
    public string FiltroSeleccionado
    {
        get => _filtroSeleccionado;
        set { if (SetField(ref _filtroSeleccionado, value)) Aplicar(); }
    }

    private List<CobroFila> _cobros = new();
    public List<CobroFila> Cobros { get => _cobros; set => SetField(ref _cobros, value); }

    public ICommand CargarCommand { get; }

    public InscripcionesViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
    }

    private async void Cargar()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            _talleres = await talleresService.GetAll();
            _alumnos = await alumnosService.GetAll();
            _inscripciones = await inscripcionesService.GetAll();
            var lista = await periodosService.GetAll();

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

    // Una fila por inscripción de un taller del período; el costo sale del taller
    private void Aplicar()
    {
        var filas = (from i in _inscripciones
                     join t in _talleres on i.IdTaller equals t.Id
                     join a in _alumnos on i.IdAlumno equals a.Id
                     where t.IdPeriodo == PeriodoSeleccionado?.Id
                     select new { i, t, a }).ToList();

        // Los totales no dependen del filtro
        Cobrado = Formato.Dinero(filas.Where(x => x.i.Debe != true).Sum(x => x.t.Costo));
        PorCobrar = Formato.Dinero(filas.Where(x => x.i.Debe == true).Sum(x => x.t.Costo));
        AlumnosConAdeudo = filas.Where(x => x.i.Debe == true).Select(x => x.a.Id).Distinct().Count();

        Cobros = filas
            .Where(x => FiltroSeleccionado switch
            {
                "Deben" => x.i.Debe == true,
                "Al corriente" => x.i.Debe != true,
                _ => true
            })
            .Select(x => new CobroFila
            {
                Alumno = Formato.NombreCompleto(x.a),
                Tutor = x.a.NombreTutor,
                Taller = x.t.Nombre,
                Costo = Formato.Dinero(x.t.Costo),
                Estado = x.i.Debe == true ? "Debe" : "Al corriente",
                Tono = x.i.Debe == true ? "Rojo" : "Verde"
            })
            .ToList();
    }
}
