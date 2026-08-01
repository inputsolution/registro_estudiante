using System.Text.RegularExpressions;

namespace RegistroEstudiantes.Modelos;

/// <summary>
/// Reglas de validacion de los campos del formulario.
///
/// Vive aparte del formulario para poder probarse sin abrir la ventana:
/// Windows Forms solo se ejecuta en Windows, pero estas reglas se verifican
/// en cualquier sistema.
/// </summary>
public static class Validaciones
{
    /// <summary>
    /// Estructura de correo: algo, arroba, dominio, punto y extension de al
    /// menos dos letras. No pretende cubrir el estandar completo, sino
    /// descartar lo que evidentemente no es una direccion.
    /// </summary>
    private static readonly Regex FormatoEmail = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Documento: letras y numeros, sin espacios ni signos.
    /// </summary>
    private static readonly Regex FormatoDocumento = new(
        @"^[A-Za-z0-9]{5,20}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Telefono: numeros y los signos habituales de separacion.
    /// </summary>
    private static readonly Regex FormatoTelefono = new(
        @"^[0-9+()\s-]+$",
        RegexOptions.Compiled);

    /// <summary>
    /// El email es opcional; si se escribe algo, debe tener forma de correo.
    /// </summary>
    public static bool EsEmailValido(string? email)
    {
        var valor = email?.Trim() ?? string.Empty;

        if (valor.Length == 0)
        {
            return true;
        }

        // Un punto al inicio o al final del dominio, o dos seguidos, pasan la
        // expresion regular pero no corresponden a una direccion real.
        if (valor.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        return valor.Length <= 80 && FormatoEmail.IsMatch(valor);
    }

    /// <summary>
    /// El documento es obligatorio y solo admite letras y numeros.
    /// </summary>
    public static bool EsDocumentoValido(string? documento)
    {
        var valor = documento?.Trim() ?? string.Empty;
        return FormatoDocumento.IsMatch(valor);
    }

    /// <summary>
    /// El telefono es opcional; si se escribe algo, debe tener al menos siete
    /// digitos ademas de los signos de separacion.
    /// </summary>
    public static bool EsTelefonoValido(string? telefono)
    {
        var valor = telefono?.Trim() ?? string.Empty;

        if (valor.Length == 0)
        {
            return true;
        }

        if (valor.Length > 20 || !FormatoTelefono.IsMatch(valor))
        {
            return false;
        }

        return valor.Count(char.IsDigit) >= 7;
    }

    /// <summary>
    /// La carrera es obligatoria: letras, espacios y los signos habituales de
    /// los nombres de programas academicos ("Diseño Grafico", "Administracion
    /// de Empresas - Nocturna").
    /// </summary>
    public static bool EsCarreraValida(string? carrera)
    {
        var valor = carrera?.Trim() ?? string.Empty;

        if (valor.Length is < 3 or > 80)
        {
            return false;
        }

        return valor.All(c => char.IsLetter(c) || c == ' ' || c == '.' || c == '-');
    }

    /// <summary>
    /// El semestre va de 1 a 12, que cubre las carreras mas largas
    /// (medicina llega a 12).
    /// </summary>
    public static bool EsSemestreValido(int semestre) =>
        semestre is >= 1 and <= 12;

    /// <summary>
    /// Nombres y apellidos: obligatorios, sin numeros.
    /// </summary>
    public static bool EsNombreValido(string? nombre)
    {
        var valor = nombre?.Trim() ?? string.Empty;

        if (valor.Length is 0 or > 60)
        {
            return false;
        }

        return valor.All(c => char.IsLetter(c) || c == ' ' || c == '\'' || c == '-');
    }
}
