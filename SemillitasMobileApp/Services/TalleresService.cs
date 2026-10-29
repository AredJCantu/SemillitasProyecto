using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

// CAMBIO: usa los DTOs de SemillasCore directamente (ya no existe la carpeta Models)
public class TalleresService
{
    public Task<List<TallerDTO>> ObtenerAsync() => ApiClient.GetAsync<List<TallerDTO>>("api/talleres");

    public Task<TallerDTO?> CrearAsync(CreateTallerDTO t) => ApiClient.PostAsync<TallerDTO>("api/talleres", t);

    public Task ActualizarAsync(EditTallerDTO t) => ApiClient.PutAsync($"api/talleres/{t.Id}", t);

    public Task EliminarAsync(int id) => ApiClient.DeleteAsync($"api/talleres/{id}");
}
