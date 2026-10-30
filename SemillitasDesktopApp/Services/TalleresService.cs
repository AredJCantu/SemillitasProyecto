using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using SemillasCore.DTOs;
using SemillasCore.Validators;

namespace SemillitasDesktopApp.Services;

// Como en Sonrisas: cada service tiene su HttpClient.
// Create/Update/Delete reciben un ValidatorClass y, si el API responde error, le llenan Errores.
public class TalleresService
{
    HttpClient cliente;

    public TalleresService()
    {
        // Debe ser la misma dirección que usas en Postman
        cliente = new() { BaseAddress = new Uri("https://localhost:44391/") };
    }

    public async Task<List<TallerDTO>> GetAll()
    {
        var data = await cliente.GetFromJsonAsync<List<TallerDTO>>("api/talleres");
        Console.Write(data);
        return data ?? [];
    }

    // Devuelve el taller creado, o null si el API respondió error
    public async Task<TallerDTO?> Create(CreateTallerDTO dto, ValidatorClass errores)
    {
        var response = await cliente.PostAsJsonAsync("api/talleres", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TallerDTO>();

        await LeerErrores(response, errores);
        return null;
    }

    public async Task<bool> Update(EditTallerDTO dto, ValidatorClass errores)
    {
        var response = await cliente.PutAsJsonAsync($"api/talleres/{dto.Id}", dto);
        if (response.IsSuccessStatusCode) return true;

        await LeerErrores(response, errores);
        return false;
    }

    public async Task<bool> Delete(int id, ValidatorClass errores)
    {
        var response = await cliente.DeleteAsync($"api/talleres/{id}");
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
