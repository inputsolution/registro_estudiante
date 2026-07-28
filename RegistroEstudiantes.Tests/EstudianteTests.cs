using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Tests;

/// <summary>
/// Pruebas del modelo: el nombre completo y el calculo de la edad que se
/// muestra en la grilla.
/// </summary>
public class EstudianteTests
{
    [Fact]
    public void NombreCompleto_UneNombresYApellidos()
    {
        var e = new Estudiante { Nombres = "Ana Maria", Apellidos = "Torres Ruiz" };

        Assert.Equal("Ana Maria Torres Ruiz", e.NombreCompleto);
    }

    [Fact]
    public void NombreCompleto_SinApellidos_NoDejaEspacioSobrante()
    {
        var e = new Estudiante { Nombres = "Ana", Apellidos = "" };

        Assert.Equal("Ana", e.NombreCompleto);
    }

    [Fact]
    public void Edad_AntesDelCumpleanos_AunNoSuma()
    {
        // Cumple manana, asi que todavia tiene un ano menos.
        var manana = DateTime.Today.AddDays(1);
        var e = new Estudiante { FechaNacimiento = manana.AddYears(-15) };

        Assert.Equal(14, e.Edad);
    }

    [Fact]
    public void Edad_ElDiaDelCumpleanos_YaSuma()
    {
        var e = new Estudiante { FechaNacimiento = DateTime.Today.AddYears(-15) };

        Assert.Equal(15, e.Edad);
    }

    [Fact]
    public void Edad_DespuesDelCumpleanos_EsCorrecta()
    {
        var ayer = DateTime.Today.AddDays(-1);
        var e = new Estudiante { FechaNacimiento = ayer.AddYears(-15) };

        Assert.Equal(15, e.Edad);
    }

    [Fact]
    public void Edad_RecienNacido_EsCero()
    {
        var e = new Estudiante { FechaNacimiento = DateTime.Today };

        Assert.Equal(0, e.Edad);
    }

    [Fact]
    public void Estudiante_NuevoTraeValoresPorDefectoUsables()
    {
        // El formulario crea instancias vacias; ningun campo de texto debe ser
        // null porque se asignan directamente a los TextBox.
        var e = new Estudiante();

        Assert.Equal(string.Empty, e.Documento);
        Assert.Equal(string.Empty, e.Nombres);
        Assert.Equal(string.Empty, e.Apellidos);
        Assert.Equal(string.Empty, e.Telefono);
        Assert.Equal(string.Empty, e.Email);
        Assert.Equal(string.Empty, e.Direccion);
        Assert.Equal(string.Empty, e.Grado);
    }
}
