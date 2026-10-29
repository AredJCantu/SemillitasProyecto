using SemillitasMobileApp.Models;

namespace SemillitasMobileApp.Services;

// Habla con TalleresController. Rutas supuestas: confirmar con quien hace el API.
public class TalleresService
{
    public Task<List<Taller>> ObtenerAsync() => ApiClient.GetAsync<List<Taller>>("api/talleres");

    public Task<Taller?> CrearAsync(Taller t) => ApiClient.PostAsync<Taller>("api/talleres", t);

    public Task ActualizarAsync(Taller t) => ApiClient.PutAsync($"api/talleres/{t.Id}", t);

    public Task EliminarAsync(int id) => ApiClient.DeleteAsync($"api/talleres/{id}");
}
