using SemillitasMobileApp.Models;

namespace SemillitasMobileApp.Services;

public class AlumnosService
{
    public Task<List<Alumno>> ObtenerAsync() => ApiClient.GetAsync<List<Alumno>>("api/alumnos");

    public Task<Alumno?> CrearAsync(Alumno a) => ApiClient.PostAsync<Alumno>("api/alumnos", a);

    public Task ActualizarAsync(Alumno a) => ApiClient.PutAsync($"api/alumnos/{a.Id}", a);
}
