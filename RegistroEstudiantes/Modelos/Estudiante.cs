namespace RegistroEstudiantes.Modelos;

/// <summary>
/// Representa un estudiante universitario del registro.
/// </summary>
public class Estudiante
{
    public int Id { get; set; }

    public string Documento { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; } = DateTime.Today;

    public string Telefono { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    /// <summary>
    /// Programa academico que cursa, por ejemplo "Ingenieria de Sistemas".
    /// </summary>
    public string Carrera { get; set; } = string.Empty;

    /// <summary>
    /// Semestre que cursa, de 1 a 12.
    /// </summary>
    public int Semestre { get; set; } = 1;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>
    /// Nombre completo, usado para mostrar en la grilla.
    /// </summary>
    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

    /// <summary>
    /// Edad cumplida a la fecha actual.
    /// </summary>
    public int Edad
    {
        get
        {
            var edad = DateTime.Today.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > DateTime.Today.AddYears(-edad))
            {
                edad--;
            }
            return edad;
        }
    }
}
