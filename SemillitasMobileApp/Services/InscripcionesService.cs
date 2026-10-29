using System.Net.Http.Json;
using System.Text.Json;
using SemillasCore.DTOs;
using SemillasCore.Validators;

namespace SemillitasMobileApp.Services;

// CAMBIO: sin ApiClient ni ApiException. Como en Sonrisas, cada service crea su HttpClient.
// Create/Update/Delete reciben un ValidatorClass y, si el API responde error, le llenan Errores.
public class InscripcionesService
{
    HttpClient cliente;

    public InscripcionesService()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        // Solo en desarrollo: acepta el certificado de prueba (el emulador no lo reconoce)
        handler.ServerCertificateCustomValidationCallback = (mensaje, certificado, cadena, errores) => true;
#endif
        // El emulador de Android ve la PC como 10.0.2.2 en lugar de localhost
        var direccion = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:44391/" : "https://localhost:44391/";
        cliente = new(handler) { BaseAddress = new Uri(direccion) };
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

    // Marcar pagado = guardar la inscripción con Debe = false (el PUT necesita IdAlumno e IdTaller también)
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

        inscripcion.Debe = false;   // la lista en pantalla ya queda como pagada
        return true;
    }

    // Si el API respondió error, guarda su lista de textos en el ValidatorClass
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
