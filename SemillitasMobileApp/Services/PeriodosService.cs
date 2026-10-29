using SemillasCore.DTOs;


namespace SemillitasMobileApp.Services;

public class PeriodosService
{
    public Task<List<PeriodoDTO>> ObtenerAsync() => ApiClient.GetAsync<List<PeriodoDTO>>("api/periodos");

    public Task<PeriodoDTO?> CrearAsync(CreatePeriodoDTO p) => ApiClient.PostAsync<PeriodoDTO>("api/periodos", p);

    //public Task ActualizarAsync(Periodo p) => ApiClient.PutAsync($"api/periodos/{p.Id}", p);
    public Task DeleteAsync(int id) => ApiClient.DeleteAsync($"api/periodos/{id}");
}
