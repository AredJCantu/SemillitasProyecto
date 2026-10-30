using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using SemillasCore.DTOs;
using SemillasCore.Validators;

namespace SemillitasDesktopApp.Services;

// Como en Sonrisas: cada service tiene su HttpClient.
// Create/Update/Delete reciben un ValidatorClass y, si el API responde error, le llenan Errores.
public class AlumnosService
{
    HttpClient cliente;

    public AlumnosService()
    {
        // Debe ser la misma dirección que usas en Postman
        cliente = new() { BaseAddress = new Uri("https://localhost:44391/") };
    }

    public async Task<List<AlumnoDTO>> GetAll()
    {
        var data = await cliente.GetFromJsonAsync<List<AlumnoDTO>>("api/alumnos");
        return data ?? [];
    }

    // Devuelve el alumno creado, o null si el API respondió error
    public async Task<AlumnoDTO?> Create(CreateAlumnoDTO dto, ValidatorClass errores)
    {
        var response = await cliente.PostAsJsonAsync("api/alumnos", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<AlumnoDTO>();

        await LeerErrores(response, errores);
        return null;
    }

    public async Task<bool> Update(EditAlumnoDTO dto, ValidatorClass errores)
    {
        var response = await cliente.PutAsJsonAsync($"api/alumnos/{dto.Id}", dto);
        if (response.IsSuccessStatusCode) return true;

        await LeerErrores(response, errores);
        return false;
    }

    public async Task<bool> Delete(int id, ValidatorClass errores)
    {
        var response = await cliente.DeleteAsync($"api/alumnos/{id}");
        if (response.IsSuccessStatusCode) return true;

        await LeerErrores(response, errores);
        return false;
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
