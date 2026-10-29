using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

// CAMBIO: usa los DTOs de SemillasCore; se agrega EliminarAsync
public class AlumnosService
{
    public Task<List<AlumnoDTO>> ObtenerAsync() => ApiClient.GetAsync<List<AlumnoDTO>>("api/alumnos");

    public Task<AlumnoDTO?> CrearAsync(CreateAlumnoDTO a) => ApiClient.PostAsync<AlumnoDTO>("api/alumnos", a);

    public Task ActualizarAsync(EditAlumnoDTO a) => ApiClient.PutAsync($"api/alumnos/{a.Id}", a);

    public Task EliminarAsync(int id) => ApiClient.DeleteAsync($"api/alumnos/{id}");
}
