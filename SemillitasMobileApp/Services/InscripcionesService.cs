using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

public class InscripcionesService
{
    public Task<List<InscripcionesDTO>> ObtenerAsync() => ApiClient.GetAsync<List<InscripcionesDTO>>("api/inscripciones");

    public Task<InscripcionesDTO?> CrearAsync(CreateInscripcionesDTO i) =>
        ApiClient.PostAsync<InscripcionesDTO>("api/inscripciones", i);

    // Marcar pagado = guardar la inscripción con Debe = false (el PUT necesita IdAlumno e IdTaller también)
    public Task MarcarPagadaAsync(InscripcionesDTO i)
    {
        i.Debe = false;
        var edicion = new EditInscripcionesDTO { Id = i.Id, IdAlumno = i.IdAlumno, IdTaller = i.IdTaller, Debe = false };
        return ApiClient.PutAsync($"api/inscripciones/{i.Id}", edicion);
    }
}
