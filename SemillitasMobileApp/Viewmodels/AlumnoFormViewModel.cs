using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo alumno (sin parámetro) o editar (GoToAsync con { "Alumno": alumno }).
// CAMBIO: sin ApellidoTutor (el DTO solo tiene NombreTutor). Se validan los datos con los validators de
// SemillasCore antes de llamar al API y los errores salen en una sola lista.
public class AlumnoFormViewModel : ViewModelBase, IQueryAttributable
{
    private readonly AlumnosService _alumnos = new();
    private AlumnoDTO? _alumno;

    public AlumnoFormViewModel()
    {
        CancelarCommand = new AsyncRelayCommand(Regresar);
        GuardarCommand = new AsyncRelayCommand(() => Ejecutar(Guardar));
    }

    public AsyncRelayCommand CancelarCommand { get; }
    public AsyncRelayCommand GuardarCommand { get; }

    private string _titulo = "Nuevo alumno";
    public string Titulo { get => _titulo; set => SetField(ref _titulo, value); }

    private string _nombreAlumno = "";
    public string NombreAlumno { get => _nombreAlumno; set => SetField(ref _nombreAlumno, value); }

    private string _apellidoAlumno = "";
    public string ApellidoAlumno { get => _apellidoAlumno; set => SetField(ref _apellidoAlumno, value); }

    private string _nombreTutor = "";
    public string NombreTutor { get => _nombreTutor; set => SetField(ref _nombreTutor, value); }

    private string _telefono = "";
    public string Telefono { get => _telefono; set => SetField(ref _telefono, value); }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Alumno", out var valor) && valor is AlumnoDTO a)
        {
            _alumno = a;
            Titulo = "Editar alumno";
            NombreAlumno = a.Nombre;
            ApellidoAlumno = a.Apellido;
            NombreTutor = a.NombreTutor;
            Telefono = a.Telefono;
        }
    }

    private async Task Guardar()
    {
        LimpiarErrores();

        if (_alumno is null)
        {
            var nuevo = new CreateAlumnoDTO
            {
                Nombre = NombreAlumno.Trim(),
                Apellido = ApellidoAlumno.Trim(),
                NombreTutor = NombreTutor.Trim(),
                Telefono = Telefono.Trim()
            };
            if (!EsValido(new CreateAlumnoValidator().IsValid(nuevo))) return;
            await _alumnos.CrearAsync(nuevo);
        }
        else
        {
            var edicion = new EditAlumnoDTO
            {
                Id = _alumno.Id,
                Nombre = NombreAlumno.Trim(),
                Apellido = ApellidoAlumno.Trim(),
                NombreTutor = NombreTutor.Trim(),
                Telefono = Telefono.Trim()
            };
            if (!EsValido(new EditAlumnoValidator().IsValid(edicion))) return;
            await _alumnos.ActualizarAsync(edicion);
        }

        await Regresar();
    }
}
