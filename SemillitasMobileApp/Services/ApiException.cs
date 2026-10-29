namespace SemillitasMobileApp.Services;

// Error devuelto por el API. Errores: campo -> mensajes (cuando el API responde 400 con "errors")
public class ApiException : Exception
{
    public Dictionary<string, string[]> Errores { get; }

    public ApiException(string mensaje, Dictionary<string, string[]>? errores = null) : base(mensaje)
    {
        Errores = errores ?? new();
    }
}
