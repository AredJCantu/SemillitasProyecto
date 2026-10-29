using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo alumno (sin parámetro) o editar (GoToAsync con { "Alumno": alumno })
public partial class AlumnoFormViewModel : BaseViewModel, IQueryAttributable
{
    private readonly AlumnosService _alumnos = new();
    private Alumno? _alumno;

    [ObservableProperty] private string titulo = "Nuevo alumno";
    [ObservableProperty] private string nombreAlumno = "";
    [ObservableProperty] private string apellidoAlumno = "";
    [ObservableProperty] private string nombreTutor = "";
    [ObservableProperty] private string apellidoTutor = "";
    [ObservableProperty] private string telefono = "";
    [ObservableProperty] private bool tieneErrorNombre;
    [ObservableProperty] private string errorNombre = "";
    [ObservableProperty] private bool tieneErrorTelefono;
    [ObservableProperty] private string errorTelefono = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Alumno", out var valor) && valor is Alumno a)
        {
            _alumno = a;
            Titulo = "Editar alumno";
            NombreAlumno = a.Nombre;
            ApellidoAlumno = a.Apellido;
            NombreTutor = a.NombreTutor;
            ApellidoTutor = a.ApellidoTutor;
            Telefono = a.Telefono;
        }
    }

    [RelayCommand]
    private Task Cancelar() => Regresar();

    [RelayCommand]
    private Task Guardar() => Ejecutar(async () =>
    {
        TieneErrorNombre = TieneErrorTelefono = false;

        var alumno = new Alumno
        {
            Id = _alumno?.Id ?? 0,
            Nombre = NombreAlumno.Trim(),
            Apellido = ApellidoAlumno.Trim(),
            NombreTutor = NombreTutor.Trim(),
            ApellidoTutor = ApellidoTutor.Trim(),
            Telefono = Telefono.Trim()
        };

        try
        {
            if (_alumno is null) await _alumnos.CrearAsync(alumno);
            else await _alumnos.ActualizarAsync(alumno);
        }
        catch (ApiException ex) when (ex.Errores.Count > 0)
        {
            // Los mensajes los generan los validators del API; aquí solo se colocan en su campo
            if (ex.Errores.TryGetValue("Nombre", out var nombre))
            {
                ErrorNombre = string.Join(" ", nombre);
                TieneErrorNombre = true;
            }
            if (ex.Errores.TryGetValue("Telefono", out var tel))
            {
                ErrorTelefono = string.Join(" ", tel);
                TieneErrorTelefono = true;
            }
            if (!TieneErrorNombre && !TieneErrorTelefono) throw;   // otro campo: lo muestra Ejecutar
            return;
        }

        await Regresar();
    });
}
