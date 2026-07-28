using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Tests;

/// <summary>
/// Pruebas de las reglas de validacion del formulario.
///
/// Pertenece a la coleccion del repositorio porque el ultimo caso lee los
/// datos de ejemplo, que son estado compartido.
/// </summary>
[Collection("Repositorio")]
public class ValidacionesTests
{
    // ---------- Email ----------

    [Theory]
    [InlineData("ana.torres@ejemplo.com")]
    [InlineData("a@b.co")]
    [InlineData("nombre+etiqueta@dominio.com.co")]
    [InlineData("con_guion-bajo@sub.dominio.org")]
    [InlineData("123@456.com")]
    public void Email_FormatosValidos_SeAceptan(string email)
    {
        Assert.True(Validaciones.EsEmailValido(email));
    }

    [Theory]
    [InlineData(".@")]                  // pasaba la validacion anterior
    [InlineData("@ejemplo.com")]        // sin destinatario
    [InlineData("ana@")]                // sin dominio
    [InlineData("ana@ejemplo")]         // sin extension
    [InlineData("ana@ejemplo.")]        // extension vacia
    [InlineData("ana@ejemplo.c")]       // extension de una sola letra
    [InlineData("ana ejemplo.com")]     // sin arroba
    [InlineData("ana@@ejemplo.com")]    // dos arrobas
    [InlineData("ana@ejem plo.com")]    // espacio en el dominio
    [InlineData("ana..torres@x.com")]   // dos puntos seguidos
    [InlineData("ana@ejemplo..com")]
    public void Email_FormatosInvalidos_SeRechazan(string email)
    {
        Assert.False(Validaciones.EsEmailValido(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Email_EsOpcional(string? email)
    {
        Assert.True(Validaciones.EsEmailValido(email));
    }

    [Fact]
    public void Email_IgnoraEspaciosAlrededor()
    {
        Assert.True(Validaciones.EsEmailValido("  ana@ejemplo.com  "));
    }

    [Fact]
    public void Email_DemasiadoLargo_SeRechaza()
    {
        var largo = new string('a', 75) + "@ejemplo.com";
        Assert.False(Validaciones.EsEmailValido(largo));
    }

    // ---------- Documento ----------

    [Theory]
    [InlineData("1001234567")]
    [InlineData("ABC12345")]
    [InlineData("12345")]                  // el minimo
    [InlineData("12345678901234567890")]   // el maximo
    public void Documento_ValoresValidos_SeAceptan(string documento)
    {
        Assert.True(Validaciones.EsDocumentoValido(documento));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1234")]                    // muy corto
    [InlineData("123456789012345678901")]   // muy largo
    [InlineData("100-123")]                 // con guion
    [InlineData("100 123")]                 // con espacio
    [InlineData("100.123")]
    public void Documento_ValoresInvalidos_SeRechazan(string? documento)
    {
        Assert.False(Validaciones.EsDocumentoValido(documento));
    }

    // ---------- Telefono ----------

    [Theory]
    [InlineData("3001112233")]
    [InlineData("+57 300 111 2233")]
    [InlineData("(601) 234-5678")]
    [InlineData("1234567")]   // el minimo de digitos
    public void Telefono_ValoresValidos_SeAceptan(string telefono)
    {
        Assert.True(Validaciones.EsTelefonoValido(telefono));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Telefono_EsOpcional(string? telefono)
    {
        Assert.True(Validaciones.EsTelefonoValido(telefono));
    }

    [Theory]
    [InlineData("123456")]            // menos de siete digitos
    [InlineData("300-ABC-1234")]      // con letras
    [InlineData("300@111@2233")]      // con simbolos no permitidos
    [InlineData("+++---()")]          // sin ningun digito
    public void Telefono_ValoresInvalidos_SeRechazan(string telefono)
    {
        Assert.False(Validaciones.EsTelefonoValido(telefono));
    }

    // ---------- Nombres ----------

    [Theory]
    [InlineData("Ana")]
    [InlineData("Ana Maria")]
    [InlineData("Torres Ruiz")]
    [InlineData("O'Connor")]
    [InlineData("Garcia-Lopez")]
    [InlineData("Jose Nino")]
    public void Nombre_ValoresValidos_SeAceptan(string nombre)
    {
        Assert.True(Validaciones.EsNombreValido(nombre));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("Ana2")]        // con numero
    [InlineData("Ana@Maria")]   // con simbolo
    [InlineData("123")]
    public void Nombre_ValoresInvalidos_SeRechazan(string? nombre)
    {
        Assert.False(Validaciones.EsNombreValido(nombre));
    }

    [Fact]
    public void Nombre_AceptaTildesYEnie()
    {
        Assert.True(Validaciones.EsNombreValido("Muñoz Peña"));
        Assert.True(Validaciones.EsNombreValido("Jose Andres"));
    }

    [Fact]
    public void Nombre_DemasiadoLargo_SeRechaza()
    {
        Assert.False(Validaciones.EsNombreValido(new string('a', 61)));
    }

    // ---------- Los datos de ejemplo cumplen las reglas ----------

    [Fact]
    public void LosEjemplosDelRepositorio_PasanTodasLasValidaciones()
    {
        RepositorioMemoriaFixture.Reiniciar();

        foreach (var e in Datos.RepositorioMemoria.Listar())
        {
            Assert.True(Validaciones.EsDocumentoValido(e.Documento), $"Documento: {e.Documento}");
            Assert.True(Validaciones.EsNombreValido(e.Nombres), $"Nombres: {e.Nombres}");
            Assert.True(Validaciones.EsNombreValido(e.Apellidos), $"Apellidos: {e.Apellidos}");
            Assert.True(Validaciones.EsTelefonoValido(e.Telefono), $"Telefono: {e.Telefono}");
            Assert.True(Validaciones.EsEmailValido(e.Email), $"Email: {e.Email}");
        }
    }
}

/// <summary>
/// Utilidad para dejar el repositorio con los datos de ejemplo cargados.
/// </summary>
internal static class RepositorioMemoriaFixture
{
    public static void Reiniciar()
    {
        Datos.RepositorioMemoria.Limpiar();
        Datos.RepositorioMemoria.CargarEjemplos();
    }
}
