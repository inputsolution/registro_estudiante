using System.Text.Json;

namespace RegistroEstudiantes.Datos;

/// <summary>
/// Lee la cadena de conexion de appsettings.json, que queda junto al
/// ejecutable. Cambiar de servidor no obliga a recompilar.
/// </summary>
public static class Configuracion
{
    private const string CadenaPorDefecto =
        "Server=.;Database=RegistroEstudiantes;Trusted_Connection=True;TrustServerCertificate=True;";

    private static string? _cadena;

    public static string CadenaConexion => _cadena ??= Leer();

    /// <summary>
    /// Ruta del archivo de configuracion, para poder mostrarla en los
    /// mensajes de error.
    /// </summary>
    public static string RutaArchivo =>
        Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    private static string Leer()
    {
        if (!File.Exists(RutaArchivo))
        {
            return CadenaPorDefecto;
        }

        try
        {
            using var documento = JsonDocument.Parse(File.ReadAllText(RutaArchivo));

            if (documento.RootElement.TryGetProperty("ConnectionStrings", out var cadenas) &&
                cadenas.TryGetProperty("SqlServer", out var valor) &&
                valor.GetString() is { Length: > 0 } cadena)
            {
                return cadena;
            }
        }
        catch (JsonException)
        {
            // Un archivo mal formado no debe impedir que la aplicacion abra:
            // se usa la cadena por defecto y el error de conexion, si lo hay,
            // se reporta al arrancar.
        }

        return CadenaPorDefecto;
    }
}
