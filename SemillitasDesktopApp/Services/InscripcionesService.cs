using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using SemillasCore.DTOs;
using SemillasCore.Validators;

namespace SemillitasDesktopApp.Services;

// Como en Sonrisas: cada service tiene su HttpClient.
// Create/Update/Delete reciben un ValidatorClass y, si el API responde error, le llenan Errores.
public class InscripcionesService
{
    HttpClient cliente;

    public InscripcionesService()
    {
        // Debe ser la misma dirección que usas en Postman
        cliente = new() { BaseAddress = new Uri("https://localhost:44391/") };
    }

    public async Task<List<InscripcionesDTO>> GetAll()
    {
        var data = await cliente.GetFromJsonAsync<List<InscripcionesDTO>>("api/inscripciones");
        return data ?? [];
    }

    // Devuelve la inscripción creada, o null si el API respondió error
    public async Task<InscripcionesDTO?> Create(CreateInscripcionesDTO dto, ValidatorClass errores)
    {
        var response = await cliente.PostAsJsonAsync("api/inscripciones", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<InscripcionesDTO>();

        await LeerErrores(response, errores);
        return null;
    }

    // Marcar pagado = guardar la inscripción con Debe = false (el PUT pide también IdAlumno e IdTaller)
    public async Task<bool> MarcarPagada(InscripcionesDTO inscripcion, ValidatorClass errores)
    {
        var dto = new EditInscripcionesDTO
        {
            Id = inscripcion.Id,
            IdAlumno = inscripcion.IdAlumno,
            IdTaller = inscripcion.IdTaller,
            Debe = false
        };
        var response = await cliente.PutAsJsonAsync($"api/inscripciones/{dto.Id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            await LeerErrores(response, errores);
            return false;
        }

        inscripcion.Debe = false;
        return true;
    }

    // Guarda en el ValidatorClass la lista de textos que manda el API (BadRequest(result.Errores))
    private static async Task LeerErrores(HttpResponseMessage response, ValidatorClass errores)
    {
        var texto = await response.Content.ReadAsStringAsync();
        try
        {
            var lista = JsonSerializer.Deserialize<List<string>>(texto);
            if (lista != null && lista.Count > 0)
            {
                errores.Errores.AddRange(lista);
                return;
            }
        }
        catch (JsonException) { }

        errores.Errores.Add($"El servidor respondió {(int)response.StatusCode}.");
    }
}
