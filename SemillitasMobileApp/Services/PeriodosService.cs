using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

// CAMBIO: se completa con ActualizarAsync; tu DeleteAsync pasa a llamarse EliminarAsync (igual que los demás servicios)
public class PeriodosService
{
    public Task<List<PeriodoDTO>> ObtenerAsync() => ApiClient.GetAsync<List<PeriodoDTO>>("api/periodos");

    public Task<PeriodoDTO?> CrearAsync(CreatePeriodoDTO p) => ApiClient.PostAsync<PeriodoDTO>("api/periodos", p);

    public Task ActualizarAsync(EditPeriodoDTO p) => ApiClient.PutAsync($"api/periodos/{p.Id}", p);

    public Task EliminarAsync(int id) => ApiClient.DeleteAsync($"api/periodos/{id}");
}
