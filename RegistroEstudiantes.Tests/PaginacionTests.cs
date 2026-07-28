using RegistroEstudiantes.Datos;
using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Tests;

/// <summary>
/// Pruebas del calculo de paginacion.
///
/// El formulario no se puede instanciar aqui (Windows Forms solo corre en
/// Windows), asi que se replica la misma aritmetica que usa RefrescarGrilla
/// y se verifica contra los datos reales del repositorio. Si la formula del
/// formulario cambia, estas pruebas deben cambiar con ella.
/// </summary>
[Collection("Repositorio")]
public class PaginacionTests
{
    public PaginacionTests()
    {
        RepositorioMemoria.Limpiar();
    }

    private static int TotalPaginas(int registros, int porPagina) =>
        Math.Max(1, (int)Math.Ceiling(registros / (double)porPagina));

    private static List<Estudiante> Pagina(List<Estudiante> todos, int pagina, int porPagina) =>
        todos.Skip((pagina - 1) * porPagina).Take(porPagina).ToList();

    private static void Sembrar(int cantidad)
    {
        for (var i = 1; i <= cantidad; i++)
        {
            RepositorioMemoria.Insertar(new Estudiante
            {
                Documento = $"DOC{i:D4}",
                Nombres = $"Nombre{i:D3}",
                Apellidos = $"Apellido{i:D3}",
                FechaNacimiento = new DateTime(2010, 1, 1),
                Grado = "7A"
            });
        }
    }

    // ---------- Total de paginas ----------

    [Theory]
    [InlineData(0, 10, 1)]    // sin registros sigue habiendo una pagina
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]   // exacto, no debe sobrar una pagina vacia
    [InlineData(11, 10, 2)]
    [InlineData(12, 10, 2)]
    [InlineData(20, 10, 2)]
    [InlineData(21, 10, 3)]
    [InlineData(12, 25, 1)]
    [InlineData(100, 25, 4)]
    [InlineData(101, 25, 5)]
    public void TotalPaginas_EsCorrecto(int registros, int porPagina, int esperado)
    {
        Assert.Equal(esperado, TotalPaginas(registros, porPagina));
    }

    // ---------- Contenido de cada pagina ----------

    [Fact]
    public void PrimeraPagina_TraeLasPrimerasDiezFilas()
    {
        Sembrar(12);
        var todos = RepositorioMemoria.Listar();

        var pagina = Pagina(todos, 1, 10);

        Assert.Equal(10, pagina.Count);
        Assert.Equal("Apellido001", pagina[0].Apellidos);
        Assert.Equal("Apellido010", pagina[9].Apellidos);
    }

    [Fact]
    public void UltimaPagina_TraeSoloElResto()
    {
        Sembrar(12);
        var todos = RepositorioMemoria.Listar();

        var pagina = Pagina(todos, 2, 10);

        Assert.Equal(2, pagina.Count);
        Assert.Equal("Apellido011", pagina[0].Apellidos);
        Assert.Equal("Apellido012", pagina[1].Apellidos);
    }

    [Fact]
    public void LasPaginas_NoRepitenNiOmitenRegistros()
    {
        Sembrar(25);
        var todos = RepositorioMemoria.Listar();

        var recorridos = new List<string>();
        for (var p = 1; p <= TotalPaginas(todos.Count, 10); p++)
        {
            recorridos.AddRange(Pagina(todos, p, 10).Select(e => e.Documento));
        }

        Assert.Equal(25, recorridos.Count);
        Assert.Equal(25, recorridos.Distinct().Count());
    }

    [Fact]
    public void PaginaMasAllaDelFinal_QuedaVacia()
    {
        Sembrar(5);
        var todos = RepositorioMemoria.Listar();

        Assert.Empty(Pagina(todos, 3, 10));
    }

    // ---------- Ajuste al cambiar de filtro o eliminar ----------

    [Theory]
    [InlineData(3, 1, 1)]   // la pagina 3 ya no existe: se ajusta a la ultima
    [InlineData(2, 2, 2)]   // sigue siendo valida
    [InlineData(0, 1, 1)]   // por debajo del minimo
    public void PaginaActual_SeAjustaAlRangoValido(int pedida, int totalPaginas, int esperada)
    {
        Assert.Equal(esperada, Math.Clamp(pedida, 1, totalPaginas));
    }

    [Fact]
    public void AlEliminarElUnicoDeLaUltimaPagina_LaPaginaSeAjusta()
    {
        Sembrar(11); // 2 paginas de 10: la segunda tiene un solo registro
        var todos = RepositorioMemoria.Listar();
        Assert.Equal(2, TotalPaginas(todos.Count, 10));

        var ultimo = todos[10];
        RepositorioMemoria.Eliminar(ultimo.Id);

        var restantes = RepositorioMemoria.Listar().Count;
        var paginasAhora = TotalPaginas(restantes, 10);

        Assert.Equal(1, paginasAhora);
        Assert.Equal(1, Math.Clamp(2, 1, paginasAhora));
    }

    // ---------- Cambio de filas por pagina ----------

    [Fact]
    public void AlAumentarFilasPorPagina_HayMenosPaginas()
    {
        Sembrar(30);
        var total = RepositorioMemoria.Listar().Count;

        Assert.Equal(3, TotalPaginas(total, 10));
        Assert.Equal(2, TotalPaginas(total, 25));
        Assert.Equal(1, TotalPaginas(total, 50));
    }

    [Fact]
    public void ConCienFilasPorPagina_LosDoceEjemplosCabenEnUna()
    {
        RepositorioMemoria.CargarEjemplos();
        var total = RepositorioMemoria.Listar().Count;

        Assert.Equal(1, TotalPaginas(total, 100));
    }

    // ---------- Interaccion con la busqueda ----------

    [Fact]
    public void AlFiltrar_LasPaginasSeCalculanSobreElResultado()
    {
        Sembrar(30);

        var filtrados = RepositorioMemoria.Listar("Apellido01"); // 01x -> 10 coincidencias

        Assert.Equal(10, filtrados.Count);
        Assert.Equal(1, TotalPaginas(filtrados.Count, 10));
    }

    [Fact]
    public void FiltroSinResultados_DejaUnaPaginaVacia()
    {
        Sembrar(30);

        var filtrados = RepositorioMemoria.Listar("noexiste");

        Assert.Empty(filtrados);
        Assert.Equal(1, TotalPaginas(filtrados.Count, 10));
    }

    // ---------- Rango mostrado en la barra de estado ----------

    [Theory]
    [InlineData(1, 10, 12, 1, 10)]    // pagina 1: 1-10 de 12
    [InlineData(2, 10, 12, 11, 12)]   // pagina 2: 11-12 de 12
    [InlineData(1, 25, 12, 1, 12)]    // todo en una pagina
    public void RangoMostrado_EsCorrecto(
        int pagina, int porPagina, int total, int desdeEsperado, int hastaEsperado)
    {
        Sembrar(total);
        var todos = RepositorioMemoria.Listar();
        var enPagina = Pagina(todos, pagina, porPagina).Count;

        var desde = ((pagina - 1) * porPagina) + 1;
        var hasta = desde + enPagina - 1;

        Assert.Equal(desdeEsperado, desde);
        Assert.Equal(hastaEsperado, hasta);
    }

    // ---------- Ubicar un registro recien guardado ----------

    [Theory]
    [InlineData("Apellido001", 1)]
    [InlineData("Apellido010", 1)]
    [InlineData("Apellido011", 2)]
    [InlineData("Apellido020", 2)]
    [InlineData("Apellido021", 3)]
    public void PaginaDeUnRegistro_SeCalculaPorSuPosicion(string apellido, int paginaEsperada)
    {
        Sembrar(25);
        var todos = RepositorioMemoria.Listar();

        var posicion = todos.FindIndex(e => e.Apellidos == apellido);

        Assert.Equal(paginaEsperada, (posicion / 10) + 1);
    }
}
