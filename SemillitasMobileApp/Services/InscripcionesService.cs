using SemillitasMobileApp.Models;

namespace SemillitasMobileApp.Services;

public class InscripcionesService
{
    public Task<List<Inscripcion>> ObtenerAsync() => ApiClient.GetAsync<List<Inscripcion>>("api/inscripciones");

    public Task<Inscripcion?> CrearAsync(Inscripcion i) => ApiClient.PostAsync<Inscripcion>("api/inscripciones", i);

    // Marcar pagado = guardar la inscripción con Debe = false
    public Task MarcarPagadaAsync(Inscripcion i)
    {
        i.Debe = false;
        return ApiClient.PutAsync($"api/inscripciones/{i.Id}", i);
    }
}
