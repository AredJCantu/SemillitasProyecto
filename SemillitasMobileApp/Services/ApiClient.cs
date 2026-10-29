using System.Net.Http.Json;
using System.Text.Json;

namespace SemillitasMobileApp.Services;

// Único HttpClient de la app. Los servicios lo usan para no repetir código.
public static class ApiClient
{
    // CAMBIO: URL del API (perfil IIS Express, puerto 44391). El emulador de Android no ve "localhost":
    // la PC se ve como 10.0.2.2.
    private static readonly string BaseUrl =
        DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:44391/" : "https://localhost:44391/";

    private static readonly HttpClient Http = CrearCliente();

    private static HttpClient CrearCliente()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        // Solo en desarrollo: acepta el certificado de prueba (el emulador no lo reconoce). Quitar para producción.
        handler.ServerCertificateCustomValidationCallback = (mensaje, certificado, cadena, errores) => true;
#endif
        return new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    public static async Task<T> GetAsync<T>(string ruta) where T : new()
    {
        var resp = await Http.GetAsync(ruta);
        await Verificar(resp);
        return await resp.Content.ReadFromJsonAsync<T>() ?? new T();
    }

    public static async Task<T?> PostAsync<T>(string ruta, object cuerpo)
    {
        var resp = await Http.PostAsJsonAsync(ruta, cuerpo);
        await Verificar(resp);
        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public static async Task PutAsync(string ruta, object cuerpo)
    {
        var resp = await Http.PutAsJsonAsync(ruta, cuerpo);
        await Verificar(resp);
    }

    public static async Task DeleteAsync(string ruta)
    {
        var resp = await Http.DeleteAsync(ruta);
        await Verificar(resp);
    }

    // CAMBIO: lee el formato real de error del API
    private static async Task Verificar(HttpResponseMessage resp)
    {
        if (resp.IsSuccessStatusCode) return;

        var texto = await resp.Content.ReadAsStringAsync();
        var errores = new List<string>();

        try
        {
            using var doc = JsonDocument.Parse(texto);
            var raiz = doc.RootElement;

            if (raiz.ValueKind == JsonValueKind.Array)
            {
                // Los controllers hacen BadRequest(result.Errores): una lista de textos
                errores.AddRange(raiz.EnumerateArray().Select(x => x.GetString() ?? ""));
            }
            else if (raiz.ValueKind == JsonValueKind.Object && raiz.TryGetProperty("errors", out var e)
                     && e.ValueKind == JsonValueKind.Object)
            {
                // Errores automáticos de ASP.NET (por ejemplo JSON mal formado): { "errors": { "Campo": ["texto"] } }
                foreach (var campo in e.EnumerateObject())
                    errores.AddRange(campo.Value.EnumerateArray().Select(x => x.GetString() ?? ""));
            }
            else if (raiz.ValueKind == JsonValueKind.Object && raiz.TryGetProperty("title", out var t))
            {
                errores.Add(t.GetString() ?? "");
            }
        }
        catch (JsonException)
        {
            if (!string.IsNullOrWhiteSpace(texto) && texto.Length < 300) errores.Add(texto);
        }

        if (errores.Count == 0) errores.Add($"El servidor respondió {(int)resp.StatusCode}.");
        throw new ApiException(errores);
    }
}
