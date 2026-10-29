namespace SemillitasMobileApp.Services;

// CAMBIO: el API responde los errores como una lista de textos (BadRequest(result.Errores)),
// sin nombre de campo. Por eso Errores es una lista simple y no un diccionario.
public class ApiException : Exception
{
    public List<string> Errores { get; }

    public ApiException(List<string> errores) : base(string.Join("\n", errores))
    {
        Errores = errores;
    }
}
