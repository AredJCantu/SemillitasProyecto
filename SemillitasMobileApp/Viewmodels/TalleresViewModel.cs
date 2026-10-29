using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

public partial class TalleresViewModel : BaseViewModel
{
    private readonly TalleresService _talleres = new();
    private List<Models.Taller> _todos = new();

    [ObservableProperty] private string textoBusqueda = "";
    [ObservableProperty] private List<TallerAdminFila> talleres = new();

    // Cada vez que se escribe en el buscador se vuelve a filtrar (sin ir al API)
    partial void OnTextoBusquedaChanged(string value) => Filtrar();

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todos = await _talleres.ObtenerAsync();
        Filtrar();
    });

    private void Filtrar()
    {
        Talleres = _todos
            .Where(t => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(t.Nombre, TextoBusqueda)
                        || Formato.Contiene(t.NombreInstructor, TextoBusqueda))
            .Select(t => new TallerAdminFila
            {
                Taller = t,
                Nombre = t.Nombre,
                Categoria = Formato.Categoria(t.Categoria),
                Instructor = t.NombreInstructor,
                Horario = Formato.Rango(t),
                Cupo = t.Cupo.ToString(),
                Costo = Formato.Dinero(t.Costo)
            }).ToList();
    }

    [RelayCommand]
    private Task NuevoTaller() => Shell.Current.GoToAsync(Rutas.TallerForm);

    [RelayCommand]
    private Task VerDetalleTaller(TallerAdminFila fila) =>
        Shell.Current.GoToAsync(Rutas.TallerDetalle, new Dictionary<string, object> { ["Taller"] = fila.Taller });

    [RelayCommand]
    private Task EditarTaller(TallerAdminFila fila) =>
        Shell.Current.GoToAsync(Rutas.TallerForm, new Dictionary<string, object> { ["Taller"] = fila.Taller });

    [RelayCommand]
    private async Task EliminarTaller(TallerAdminFila fila)
    {
        if (!await Confirmar("Eliminar taller", $"¿Eliminar \"{fila.Nombre}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            await _talleres.EliminarAsync(fila.Taller.Id);
            _todos.Remove(fila.Taller);
            Filtrar();
        });
    }
}
