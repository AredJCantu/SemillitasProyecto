using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// CAMBIO: sin toolkit y sin columna de instructor (queda fuera por ahora)
public class TalleresViewModel : ViewModelBase
{
    private readonly TalleresService _talleres = new();
    private List<TallerDTO> _todos = new();

    public TalleresViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        NuevoTallerCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(Rutas.TallerForm));
        VerDetalleTallerCommand = new AsyncRelayCommand<TallerAdminFila>(fila =>
            Shell.Current.GoToAsync(Rutas.TallerDetalle, new Dictionary<string, object> { ["Taller"] = fila.Taller }));
        EditarTallerCommand = new AsyncRelayCommand<TallerAdminFila>(fila =>
            Shell.Current.GoToAsync(Rutas.TallerForm, new Dictionary<string, object> { ["Taller"] = fila.Taller }));
        EliminarTallerCommand = new AsyncRelayCommand<TallerAdminFila>(Eliminar);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevoTallerCommand { get; }
    public AsyncRelayCommand<TallerAdminFila> VerDetalleTallerCommand { get; }
    public AsyncRelayCommand<TallerAdminFila> EditarTallerCommand { get; }
    public AsyncRelayCommand<TallerAdminFila> EliminarTallerCommand { get; }

    // Cada vez que se escribe en el buscador se vuelve a filtrar (sin ir al API)
    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Filtrar(); }
    }

    private List<TallerAdminFila> _talleresLista = new();
    public List<TallerAdminFila> Talleres { get => _talleresLista; set => SetField(ref _talleresLista, value); }

    private async Task Cargar()
    {
        _todos = await _talleres.GetAll();
        Filtrar();
    }

    private void Filtrar()
    {
        Talleres = _todos
            .Where(t => string.IsNullOrWhiteSpace(TextoBusqueda) || Formato.Contiene(t.Nombre, TextoBusqueda))
            .Select(t => new TallerAdminFila
            {
                Taller = t,
                Nombre = t.Nombre,
                Horario = Formato.Horario(t),
                Cupo = t.Cupo.ToString(),
                Costo = Formato.Dinero(t.Costo)
            }).ToList();
    }

    private async Task Eliminar(TallerAdminFila fila)
    {
        if (!await Confirmar("Eliminar taller", $"¿Eliminar \"{fila.Nombre}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _talleres.Delete(fila.Taller.Id, errores))
            {
                await Avisar("No se pudo eliminar", string.Join("\n", errores.Errores));
                return;
            }
            _todos.Remove(fila.Taller);
            Filtrar();
        });
    }
}
