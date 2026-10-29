using System.Net.Http.Json;
using System.Text.Json;

namespace SemillitasMobileApp.Services;

// Único HttpClient de la app. Los servicios lo usan para no repetir código.
public static class ApiClient
{
    // TODO: poner la URL/puerto reales del API. En el emulador de Android "localhost" es el
    // propio emulador; la PC se ve como 10.0.2.2.
    //private static readonly string BaseUrl =
    //    DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000/" : "http://localhost:5000/";
    private static readonly string BaseUrl = "https://localhost:44391/";

    private static readonly HttpClient Http = new() { BaseAddress = new Uri(BaseUrl) };

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

    private static async Task Verificar(HttpResponseMessage resp)
    {
        if (resp.IsSuccessStatusCode) return;

        var texto = await resp.Content.ReadAsStringAsync();
        var errores = new Dictionary<string, string[]>();
        var mensaje = $"El servidor respondió {(int)resp.StatusCode}.";

        // Formato estándar de ASP.NET para 400: { "errors": { "Campo": ["mensaje"] } }
        try
        {
            using var doc = JsonDocument.Parse(texto);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("errors", out var e) && e.ValueKind == JsonValueKind.Object)
            {
                foreach (var campo in e.EnumerateObject())
                    errores[campo.Name] = campo.Value.EnumerateArray().Select(x => x.GetString() ?? "").ToArray();
                mensaje = string.Join("\n", errores.Values.SelectMany(x => x));
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                     doc.RootElement.TryGetProperty("title", out var t))
            {
                mensaje = t.GetString() ?? mensaje;
            }
        }
        catch (JsonException)
        {
            if (!string.IsNullOrWhiteSpace(texto) && texto.Length < 300) mensaje = texto;
        }

        throw new ApiException(mensaje, errores);
    }
}
