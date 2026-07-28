using RegistroEstudiantes.Datos;
using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Tests;

/// <summary>
/// Pruebas del repositorio: las operaciones CRUD, la busqueda y las reglas
/// que el formulario usa para validar.
///
/// El repositorio es estatico, asi que cada caso parte de un estado limpio.
/// xUnit ejecuta en paralelo por defecto; se desactiva para esta coleccion
/// porque todos los casos comparten el mismo almacenamiento.
/// </summary>
[Collection("Repositorio")]
public class RepositorioMemoriaTests
{
    public RepositorioMemoriaTests()
    {
        RepositorioMemoria.Limpiar();
    }

    private static Estudiante NuevoEstudiante(
        string documento = "1000000001",
        string nombres = "Pedro",
        string apellidos = "Ramirez Soto",
        string grado = "7A") => new()
        {
            Documento = documento,
            Nombres = nombres,
            Apellidos = apellidos,
            FechaNacimiento = new DateTime(2010, 5, 20),
            Telefono = "3001234567",
            Email = "pedro.ramirez@ejemplo.com",
            Direccion = "Calle 1 # 2-3",
            Grado = grado
        };

    // ---------- Crear ----------

    [Fact]
    public void Insertar_AsignaIdCorrelativo()
    {
        var primero = RepositorioMemoria.Insertar(NuevoEstudiante("1111111111"));
        var segundo = RepositorioMemoria.Insertar(NuevoEstudiante("2222222222"));

        Assert.Equal(1, primero);
        Assert.Equal(2, segundo);
    }

    [Fact]
    public void Insertar_GuardaTodosLosCampos()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        var guardado = Assert.Single(RepositorioMemoria.Listar());

        Assert.Equal("1000000001", guardado.Documento);
        Assert.Equal("Pedro", guardado.Nombres);
        Assert.Equal("Ramirez Soto", guardado.Apellidos);
        Assert.Equal(new DateTime(2010, 5, 20), guardado.FechaNacimiento);
        Assert.Equal("3001234567", guardado.Telefono);
        Assert.Equal("pedro.ramirez@ejemplo.com", guardado.Email);
        Assert.Equal("Calle 1 # 2-3", guardado.Direccion);
        Assert.Equal("7A", guardado.Grado);
    }

    [Fact]
    public void Insertar_RegistraLaFechaDeRegistro()
    {
        var antes = DateTime.Now.AddSeconds(-1);

        RepositorioMemoria.Insertar(NuevoEstudiante());

        var guardado = Assert.Single(RepositorioMemoria.Listar());
        Assert.InRange(guardado.FechaRegistro, antes, DateTime.Now.AddSeconds(1));
    }

    // ---------- Leer ----------

    [Fact]
    public void Listar_SinRegistros_DevuelveListaVacia()
    {
        Assert.Empty(RepositorioMemoria.Listar());
    }

    [Fact]
    public void Listar_OrdenaPorApellidoYLuegoNombre()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("1", "Zulma", "Vargas"));
        RepositorioMemoria.Insertar(NuevoEstudiante("2", "Ana", "Acosta"));
        RepositorioMemoria.Insertar(NuevoEstudiante("3", "Beto", "Acosta"));

        var apellidos = RepositorioMemoria.Listar()
            .Select(e => $"{e.Apellidos} {e.Nombres}")
            .ToArray();

        Assert.Equal(
            new[] { "Acosta Ana", "Acosta Beto", "Vargas Zulma" },
            apellidos);
    }

    [Fact]
    public void Listar_DevuelveCopias_NoPermiteMutarElAlmacen()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        var copia = RepositorioMemoria.Listar()[0];
        copia.Nombres = "MODIFICADO POR FUERA";

        var real = RepositorioMemoria.Listar()[0];
        Assert.Equal("Pedro", real.Nombres);
    }

    // ---------- Buscar ----------

    [Theory]
    [InlineData("Pedro")]        // por nombre
    [InlineData("Ramirez")]      // por apellido
    [InlineData("1000000001")]   // por documento
    [InlineData("pedro")]        // sin distinguir mayusculas
    [InlineData("RAMIREZ")]
    [InlineData("amire")]        // coincidencia parcial
    public void Listar_ConFiltro_EncuentraElRegistro(string filtro)
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());
        RepositorioMemoria.Insertar(NuevoEstudiante("9999999999", "Otra", "Persona"));

        var resultado = RepositorioMemoria.Listar(filtro);

        Assert.Single(resultado);
        Assert.Equal("Pedro", resultado[0].Nombres);
    }

    [Fact]
    public void Listar_ConFiltroSinCoincidencias_DevuelveVacio()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        Assert.Empty(RepositorioMemoria.Listar("noexiste"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Listar_ConFiltroVacio_DevuelveTodos(string filtro)
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("1"));
        RepositorioMemoria.Insertar(NuevoEstudiante("2", "Otra", "Persona"));

        Assert.Equal(2, RepositorioMemoria.Listar(filtro).Count);
    }

    [Fact]
    public void Listar_ConFiltroConEspacios_IgnoraLosExtremos()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        Assert.Single(RepositorioMemoria.Listar("  Pedro  "));
    }

    [Fact]
    public void Listar_NoBuscaEnCamposQueNoCorresponden()
    {
        // El email y la direccion no participan en la busqueda.
        RepositorioMemoria.Insertar(NuevoEstudiante());

        Assert.Empty(RepositorioMemoria.Listar("ejemplo.com"));
        Assert.Empty(RepositorioMemoria.Listar("Calle 1"));
    }

    // ---------- Actualizar ----------

    [Fact]
    public void Actualizar_CambiaLosDatos()
    {
        var id = RepositorioMemoria.Insertar(NuevoEstudiante());

        RepositorioMemoria.Actualizar(new Estudiante
        {
            Id = id,
            Documento = "5555555555",
            Nombres = "Pedro Jose",
            Apellidos = "Ramirez Soto",
            FechaNacimiento = new DateTime(2011, 1, 1),
            Telefono = "3009998877",
            Email = "nuevo@ejemplo.com",
            Direccion = "Carrera 9 # 8-7",
            Grado = "8B"
        });

        var actualizado = Assert.Single(RepositorioMemoria.Listar());
        Assert.Equal("5555555555", actualizado.Documento);
        Assert.Equal("Pedro Jose", actualizado.Nombres);
        Assert.Equal("nuevo@ejemplo.com", actualizado.Email);
        Assert.Equal("8B", actualizado.Grado);
    }

    [Fact]
    public void Actualizar_NoCreaRegistrosNuevos()
    {
        var id = RepositorioMemoria.Insertar(NuevoEstudiante());

        var modificado = NuevoEstudiante(nombres: "Pedro Antonio");
        modificado.Id = id;
        RepositorioMemoria.Actualizar(modificado);

        Assert.Single(RepositorioMemoria.Listar());
        Assert.Equal(id, RepositorioMemoria.Listar()[0].Id);
    }

    [Fact]
    public void Actualizar_ConIdInexistente_NoHaceNada()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        var fantasma = NuevoEstudiante("7777777777", "Fantasma", "Inexistente");
        fantasma.Id = 999;
        RepositorioMemoria.Actualizar(fantasma);

        var lista = RepositorioMemoria.Listar();
        Assert.Single(lista);
        Assert.Equal("Pedro", lista[0].Nombres);
    }

    [Fact]
    public void Actualizar_RecortaEspaciosEnBlanco()
    {
        var id = RepositorioMemoria.Insertar(NuevoEstudiante());

        var conEspacios = NuevoEstudiante("  8888  ", "  Luis  ", "  Perez  ");
        conEspacios.Id = id;
        RepositorioMemoria.Actualizar(conEspacios);

        var actualizado = Assert.Single(RepositorioMemoria.Listar());
        Assert.Equal("8888", actualizado.Documento);
        Assert.Equal("Luis", actualizado.Nombres);
        Assert.Equal("Perez", actualizado.Apellidos);
    }

    // ---------- Eliminar ----------

    [Fact]
    public void Eliminar_QuitaSoloElRegistroIndicado()
    {
        var id = RepositorioMemoria.Insertar(NuevoEstudiante("1111111111"));
        RepositorioMemoria.Insertar(NuevoEstudiante("2222222222", "Otra", "Persona"));

        RepositorioMemoria.Eliminar(id);

        var restante = Assert.Single(RepositorioMemoria.Listar());
        Assert.Equal("2222222222", restante.Documento);
    }

    [Fact]
    public void Eliminar_ConIdInexistente_NoAfectaNada()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante());

        RepositorioMemoria.Eliminar(999);

        Assert.Single(RepositorioMemoria.Listar());
    }

    // ---------- Documento unico ----------

    [Fact]
    public void ExisteDocumento_DetectaDuplicado()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("1234567890"));

        Assert.True(RepositorioMemoria.ExisteDocumento("1234567890"));
    }

    [Fact]
    public void ExisteDocumento_ConDocumentoLibre_DevuelveFalso()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("1234567890"));

        Assert.False(RepositorioMemoria.ExisteDocumento("0987654321"));
    }

    [Fact]
    public void ExisteDocumento_AlEditarSePermiteConservarElPropio()
    {
        // Este es el caso que evita el falso positivo: al editar un estudiante
        // sin cambiarle el documento, no debe reportarse como duplicado.
        var id = RepositorioMemoria.Insertar(NuevoEstudiante("1234567890"));

        Assert.False(RepositorioMemoria.ExisteDocumento("1234567890", id));
    }

    [Fact]
    public void ExisteDocumento_AlEditarDetectaElDeOtroEstudiante()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("1111111111"));
        var id = RepositorioMemoria.Insertar(NuevoEstudiante("2222222222", "Otra", "Persona"));

        Assert.True(RepositorioMemoria.ExisteDocumento("1111111111", id));
    }

    [Fact]
    public void ExisteDocumento_IgnoraEspaciosYMayusculas()
    {
        RepositorioMemoria.Insertar(NuevoEstudiante("ABC123"));

        Assert.True(RepositorioMemoria.ExisteDocumento("  abc123  "));
    }

    // ---------- Datos de ejemplo ----------

    [Fact]
    public void CargarEjemplos_InsertaDoceRegistros()
    {
        RepositorioMemoria.CargarEjemplos();

        Assert.Equal(12, RepositorioMemoria.Listar().Count);
    }

    [Fact]
    public void CargarEjemplos_NoDuplicaSiSeLlamaDosVeces()
    {
        RepositorioMemoria.CargarEjemplos();
        RepositorioMemoria.CargarEjemplos();

        Assert.Equal(12, RepositorioMemoria.Listar().Count);
    }

    [Fact]
    public void CargarEjemplos_TodosLosDocumentosSonUnicos()
    {
        RepositorioMemoria.CargarEjemplos();

        var documentos = RepositorioMemoria.Listar().Select(e => e.Documento).ToList();

        Assert.Equal(documentos.Count, documentos.Distinct().Count());
    }

    [Fact]
    public void CargarEjemplos_NingunCampoObligatorioQuedaVacio()
    {
        RepositorioMemoria.CargarEjemplos();

        foreach (var e in RepositorioMemoria.Listar())
        {
            Assert.False(string.IsNullOrWhiteSpace(e.Documento));
            Assert.False(string.IsNullOrWhiteSpace(e.Nombres));
            Assert.False(string.IsNullOrWhiteSpace(e.Apellidos));
        }
    }

    [Fact]
    public void CargarEjemplos_NingunaFechaDeNacimientoEsFutura()
    {
        RepositorioMemoria.CargarEjemplos();

        Assert.All(
            RepositorioMemoria.Listar(),
            e => Assert.True(e.FechaNacimiento.Date <= DateTime.Today));
    }
}
