using System.Windows.Input;
using SemillasCore.DTOs;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Infrastructure;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.ViewModels;

// Lectura de alumnos con cuántos talleres tienen y si deben
public class AlumnosViewModel : ViewModelBase
{
    AlumnosService alumnosService = new();
    InscripcionesService inscripcionesService = new();

    private List<AlumnoDTO> _todos = new();
    private List<InscripcionesDTO> _inscripciones = new();

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Filtrar(); }
    }

    private List<AlumnoFila> _alumnos = new();
    public List<AlumnoFila> Alumnos { get => _alumnos; set => SetField(ref _alumnos, value); }

    public ICommand CargarCommand { get; }

    public AlumnosViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
    }

    private async void Cargar()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            _todos = await alumnosService.GetAll();
            _inscripciones = await inscripcionesService.GetAll();
            Filtrar();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Sin conexión", ex.Message, "Aceptar");
        }
        IsBusy = false;
    }

    private void Filtrar()
    {
        Alumnos = _todos
            .Where(a => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(Formato.NombreCompleto(a), TextoBusqueda)
                        || Formato.Contiene(a.NombreTutor, TextoBusqueda)
                        || Formato.Contiene(a.Telefono, TextoBusqueda))
            .Select(a =>
            {
                var suyas = _inscripciones.Where(i => i.IdAlumno == a.Id).ToList();
                var debe = suyas.Any(i => i.Debe == true);
                return new AlumnoFila
                {
                    NombreCompleto = Formato.NombreCompleto(a),
                    Tutor = a.NombreTutor,
                    Telefono = a.Telefono,
                    CantidadTalleres = suyas.Count,
                    Estado = debe ? "Debe" : "Al corriente",
                    Tono = debe ? "Rojo" : "Verde"
                };
            })
            .ToList();
    }
}
