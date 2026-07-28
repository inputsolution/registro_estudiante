using System.Reflection;
using RegistroEstudiantes.Datos;

namespace RegistroEstudiantes.Tests;

/// <summary>
/// El formulario habla con el repositorio a traves de un conjunto fijo de
/// metodos. Estas pruebas verifican que RepositorioMemoria mantenga esa misma
/// forma, para que siga siendo un sustituto valido en las pruebas y como
/// referencia de la interfaz.
///
/// RepositorioSqlServer no se puede inspeccionar aqui: vive en el proyecto
/// net8.0-windows, que no se referencia desde este.
/// </summary>
public class InterfazRepositoriosTests
{
    private static MethodInfo? Metodo(string nombre, params Type[] parametros) =>
        typeof(RepositorioMemoria).GetMethod(
            nombre, BindingFlags.Public | BindingFlags.Static, parametros);

    [Fact]
    public void Listar_TieneLaFirmaEsperada()
    {
        var metodo = Metodo("Listar", typeof(string));

        Assert.NotNull(metodo);
        Assert.Equal(typeof(List<Modelos.Estudiante>), metodo!.ReturnType);
    }

    [Fact]
    public void Insertar_DevuelveElIdAsignado()
    {
        var metodo = Metodo("Insertar", typeof(Modelos.Estudiante));

        Assert.NotNull(metodo);
        Assert.Equal(typeof(int), metodo!.ReturnType);
    }

    [Fact]
    public void Actualizar_TieneLaFirmaEsperada()
    {
        var metodo = Metodo("Actualizar", typeof(Modelos.Estudiante));

        Assert.NotNull(metodo);
        Assert.Equal(typeof(void), metodo!.ReturnType);
    }

    [Fact]
    public void Eliminar_RecibeUnId()
    {
        var metodo = Metodo("Eliminar", typeof(int));

        Assert.NotNull(metodo);
        Assert.Equal(typeof(void), metodo!.ReturnType);
    }

    [Fact]
    public void ExisteDocumento_RecibeDocumentoEIdAExcluir()
    {
        var metodo = Metodo("ExisteDocumento", typeof(string), typeof(int));

        Assert.NotNull(metodo);
        Assert.Equal(typeof(bool), metodo!.ReturnType);
    }

    [Fact]
    public void ListarYExisteDocumento_TienenParametrosOpcionales()
    {
        // El formulario llama Listar() sin argumentos y ExisteDocumento con
        // uno solo; los valores por defecto deben conservarse.
        Assert.True(Metodo("Listar", typeof(string))!
            .GetParameters()[0].HasDefaultValue);

        Assert.True(Metodo("ExisteDocumento", typeof(string), typeof(int))!
            .GetParameters()[1].HasDefaultValue);
    }
}
