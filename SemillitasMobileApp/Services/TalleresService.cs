using System.Net.Http.Json;
using SemillasCore.DTOs;

namespace SemillitasMobileApp.Services;

// Solo lectura: la app móvil no crea, edita ni borra (eso lo hace la app de escritorio)
public class TalleresService
{
    HttpClient cliente;

    public TalleresService()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        // Solo en desarrollo: acepta el certificado de prueba (el emulador no lo reconoce)
        handler.ServerCertificateCustomValidationCallback = (mensaje, certificado, cadena, errores) => true;
#endif
        // El emulador de Android ve la PC como 10.0.2.2. Debe ser el mismo puerto que usas en Postman.
        cliente = new(handler) { BaseAddress = new Uri("https://10.0.2.2:44391/") };
    }

    public async Task<List<TallerDTO>> GetAll()
    {
        var data = await cliente.GetFromJsonAsync<List<TallerDTO>>("api/talleres");
        return data ?? [];
    }
}
