using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

public class InscripcionesService
{
    public Task<List<InscripcionesDTO>> ObtenerAsync() => ApiClient.GetAsync<List<InscripcionesDTO>>("api/inscripciones");

    public Task<InscripcionesDTO?> CrearAsync(CreateInscripcionesDTO i) => ApiClient.PostAsync<InscripcionesDTO>("api/inscripciones", i);

    // Marcar pagado = guardar la inscripción con Debe = false
    public Task MarcarPagadaAsync(EditInscripcionesDTO i)
    {
        i.Debe = false;
        return ApiClient.PutAsync($"api/inscripciones/{i.Id}", i);
    }
}
