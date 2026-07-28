using Microsoft.Data.SqlClient;
using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Datos;

/// <summary>
/// Acceso a los estudiantes guardados en SQL Server.
///
/// Expone los mismos metodos que RepositorioMemoria, de modo que el
/// formulario funciona igual con cualquiera de los dos.
/// </summary>
public static class RepositorioSqlServer
{
    /// <summary>
    /// Columnas en el orden que espera <see cref="Mapear"/>.
    /// </summary>
    private const string Columnas =
        "Id, Documento, Nombres, Apellidos, FechaNacimiento, " +
        "Telefono, Email, Direccion, Grado, FechaRegistro";

    /// <summary>
    /// Comprueba que se pueda abrir la conexion. Se llama al arrancar para
    /// avisar de inmediato si el servidor no responde, en vez de fallar mas
    /// tarde en medio de una operacion.
    /// </summary>
    public static void VerificarConexion()
    {
        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText =
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
            "WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Estudiantes';";

        if (Convert.ToInt32(comando.ExecuteScalar()) == 0)
        {
            throw new InvalidOperationException(
                "La base de datos respondio, pero no existe la tabla dbo.Estudiantes.\n\n" +
                "Ejecuta el script scripts/01_crear_base_datos.sql en SQL Server " +
                "Management Studio.");
        }
    }

    /// <summary>
    /// Devuelve los estudiantes, opcionalmente filtrados por documento,
    /// nombres o apellidos.
    /// </summary>
    public static List<Estudiante> Listar(string filtro = "")
    {
        var lista = new List<Estudiante>();

        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();

        if (string.IsNullOrWhiteSpace(filtro))
        {
            comando.CommandText =
                $"SELECT {Columnas} FROM dbo.Estudiantes ORDER BY Apellidos, Nombres;";
        }
        else
        {
            comando.CommandText =
                $"SELECT {Columnas} FROM dbo.Estudiantes " +
                "WHERE Documento LIKE @filtro " +
                "   OR Nombres   LIKE @filtro " +
                "   OR Apellidos LIKE @filtro " +
                "ORDER BY Apellidos, Nombres;";

            // El parametro evita inyeccion de SQL; los comodines se escapan
            // para que un guion bajo o un porcentaje se busquen literalmente.
            comando.Parameters.AddWithValue("@filtro", $"%{Escapar(filtro.Trim())}%");
        }

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(Mapear(lector));
        }

        return lista;
    }

    /// <summary>
    /// Inserta un estudiante y devuelve el Id que le asigno la base de datos.
    /// </summary>
    public static int Insertar(Estudiante e)
    {
        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText =
            "INSERT INTO dbo.Estudiantes " +
            "    (Documento, Nombres, Apellidos, FechaNacimiento, " +
            "     Telefono, Email, Direccion, Grado, FechaRegistro) " +
            "OUTPUT INSERTED.Id " +
            "VALUES " +
            "    (@documento, @nombres, @apellidos, @fechaNacimiento, " +
            "     @telefono, @email, @direccion, @grado, SYSDATETIME());";

        AgregarParametros(comando, e);

        try
        {
            return Convert.ToInt32(comando.ExecuteScalar());
        }
        catch (SqlException ex) when (EsDocumentoDuplicado(ex))
        {
            throw new InvalidOperationException(
                $"Ya existe un estudiante con el documento {e.Documento.Trim()}.", ex);
        }
    }

    /// <summary>
    /// Actualiza los datos de un estudiante existente.
    /// </summary>
    public static void Actualizar(Estudiante e)
    {
        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText =
            "UPDATE dbo.Estudiantes SET " +
            "    Documento       = @documento, " +
            "    Nombres         = @nombres, " +
            "    Apellidos       = @apellidos, " +
            "    FechaNacimiento = @fechaNacimiento, " +
            "    Telefono        = @telefono, " +
            "    Email           = @email, " +
            "    Direccion       = @direccion, " +
            "    Grado           = @grado " +
            "WHERE Id = @id;";

        AgregarParametros(comando, e);
        comando.Parameters.AddWithValue("@id", e.Id);

        try
        {
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex) when (EsDocumentoDuplicado(ex))
        {
            throw new InvalidOperationException(
                $"Ya existe otro estudiante con el documento {e.Documento.Trim()}.", ex);
        }
    }

    /// <summary>
    /// Elimina un estudiante por Id.
    /// </summary>
    public static void Eliminar(int id)
    {
        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM dbo.Estudiantes WHERE Id = @id;";
        comando.Parameters.AddWithValue("@id", id);
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Indica si ya existe otro estudiante con el mismo documento.
    /// Se excluye <paramref name="idExcluir"/> para permitir editar sin que el
    /// propio registro cuente como duplicado.
    /// </summary>
    public static bool ExisteDocumento(string documento, int idExcluir = 0)
    {
        using var conexion = Conectar();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText =
            "SELECT COUNT(*) FROM dbo.Estudiantes " +
            "WHERE Documento = @documento AND Id <> @id;";

        comando.Parameters.AddWithValue("@documento", documento.Trim());
        comando.Parameters.AddWithValue("@id", idExcluir);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    private static SqlConnection Conectar() => new(Configuracion.CadenaConexion);

    private static void AgregarParametros(SqlCommand comando, Estudiante e)
    {
        comando.Parameters.AddWithValue("@documento", e.Documento.Trim());
        comando.Parameters.AddWithValue("@nombres", e.Nombres.Trim());
        comando.Parameters.AddWithValue("@apellidos", e.Apellidos.Trim());
        comando.Parameters.AddWithValue("@fechaNacimiento", e.FechaNacimiento.Date);
        comando.Parameters.AddWithValue("@telefono", e.Telefono.Trim());
        comando.Parameters.AddWithValue("@email", e.Email.Trim());
        comando.Parameters.AddWithValue("@direccion", e.Direccion.Trim());
        comando.Parameters.AddWithValue("@grado", e.Grado.Trim());
    }

    private static Estudiante Mapear(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Documento = lector.GetString(1),
        Nombres = lector.GetString(2),
        Apellidos = lector.GetString(3),
        FechaNacimiento = lector.GetDateTime(4),
        Telefono = lector.GetString(5),
        Email = lector.GetString(6),
        Direccion = lector.GetString(7),
        Grado = lector.GetString(8),
        FechaRegistro = lector.GetDateTime(9)
    };

    /// <summary>
    /// 2627 y 2601 son los errores de SQL Server para clave duplicada.
    /// </summary>
    private static bool EsDocumentoDuplicado(SqlException ex) =>
        ex.Number is 2627 or 2601;

    /// <summary>
    /// Neutraliza los comodines de LIKE para que se busquen como texto.
    /// </summary>
    private static string Escapar(string valor) => valor
        .Replace("[", "[[]")
        .Replace("%", "[%]")
        .Replace("_", "[_]");
}
