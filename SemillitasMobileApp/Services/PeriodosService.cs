using SemillitasMobileApp.Models;

namespace SemillitasMobileApp.Services;

public class PeriodosService
{
    public Task<List<Periodo>> ObtenerAsync() => ApiClient.GetAsync<List<Periodo>>("api/periodos");

    public Task<Periodo?> CrearAsync(Periodo p) => ApiClient.PostAsync<Periodo>("api/periodos", p);

    public Task ActualizarAsync(Periodo p) => ApiClient.PutAsync($"api/periodos/{p.Id}", p);
}
