using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Se abre con GoToAsync(Rutas.AlumnoDetalle, { "Alumno": alumno }). IQueryAttributable recibe ese parámetro.
// CAMBIO: sin toolkit; el adeudo se calcula aquí sumando el costo de los talleres que debe.
public class AlumnoDetalleViewModel : ViewModelBase, IQueryAttributable
{
    private readonly AlumnosService _alumnos = new();
    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();
    private readonly InscripcionesService _inscripciones = new();

    private AlumnoDTO? _alumno;

    public AlumnoDetalleViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        VolverCommand = new AsyncRelayCommand(Regresar);
        EditarAlumnoCommand = new AsyncRelayCommand(() =>
            Shell.Current.GoToAsync(Rutas.AlumnoForm, new Dictionary<string, object> { ["Alumno"] = _alumno! }));
        MarcarPagadoCommand = new AsyncRelayCommand<InscripcionAlumnoFila>(MarcarPagado);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand VolverCommand { get; }
    public AsyncRelayCommand EditarAlumnoCommand { get; }
    public AsyncRelayCommand<InscripcionAlumnoFila> MarcarPagadoCommand { get; }

    private string _nombreCompleto = "";
    public string NombreCompleto { get => _nombreCompleto; set => SetField(ref _nombreCompleto, value); }

    private string _tutor = "";
    public string Tutor { get => _tutor; set => SetField(ref _tutor, value); }

    private string _telefono = "";
    public string Telefono { get => _telefono; set => SetField(ref _telefono, value); }

    private string _totalAdeudo = "$0";
    public string TotalAdeudo { get => _totalAdeudo; set => SetField(ref _totalAdeudo, value); }

    private List<InscripcionAlumnoFila> _inscripcionesLista = new();
    public List<InscripcionAlumnoFila> Inscripciones { get => _inscripcionesLista; set => SetField(ref _inscripcionesLista, value); }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Alumno", out var valor) && valor is AlumnoDTO a)
        {
            _alumno = a;
            Mostrar(a);
        }
    }

    private void Mostrar(AlumnoDTO a)
    {
        NombreCompleto = Formato.NombreCompleto(a);
        Tutor = a.NombreTutor;
        Telefono = a.Telefono;
    }

    private async Task Cargar()
    {
        if (_alumno is null) return;

        // Se vuelve a pedir el alumno por si se editó en el formulario
        var actual = (await _alumnos.GetAll()).FirstOrDefault(a => a.Id == _alumno.Id);
        if (actual is not null) { _alumno = actual; Mostrar(actual); }

        var talleres = await _talleres.GetAll();
        var periodos = await _periodos.GetAll();
        var suyas = (await _inscripciones.GetAll()).Where(i => i.IdAlumno == _alumno.Id).ToList();

        var filas = (from i in suyas
                     join t in talleres on i.IdTaller equals t.Id
                     select new { i, t }).ToList();

        Inscripciones = filas.Select(x => new InscripcionAlumnoFila
        {
            Inscripcion = x.i,
            Taller = x.t.Nombre,
            Periodo = periodos.FirstOrDefault(p => p.Id == x.t.IdPeriodo)?.ToString() ?? "",
            Costo = Formato.Dinero(x.t.Costo),
            Estado = x.i.Debe == true ? "Debe" : "Al corriente",
            Tono = x.i.Debe == true ? "Rojo" : "Verde",
            PuedeMarcarPago = x.i.Debe == true
        }).ToList();

        TotalAdeudo = Formato.Dinero(filas.Where(x => x.i.Debe == true).Sum(x => x.t.Costo));
    }

    private async Task MarcarPagado(InscripcionAlumnoFila fila)
    {
        if (!await Confirmar("Marcar pagado", $"¿Marcar \"{fila.Taller}\" como pagado?", "Sí, pagó")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _inscripciones.MarcarPagada(fila.Inscripcion, errores))
            {
                await Avisar("No se pudo marcar el pago", string.Join("\n", errores.Errores));
                return;
            }
            await Cargar();   // refresca la lista y el adeudo
        });
    }
}
