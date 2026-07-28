using RegistroEstudiantes.Datos;
using RegistroEstudiantes.Formularios;

namespace RegistroEstudiantes;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Se comprueba la conexion antes de abrir la ventana: es preferible un
        // mensaje claro al arrancar que un fallo a mitad de una operacion.
        try
        {
            RepositorioSqlServer.VerificarConexion();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "No se pudo conectar con SQL Server.\n\n" +
                $"Detalle: {ex.Message}\n\n" +
                "Comprueba que:\n" +
                "  1. El servicio de SQL Server este iniciado.\n" +
                "  2. La base de datos exista (ejecuta scripts/01_crear_base_datos.sql).\n" +
                "  3. La cadena de conexion sea correcta en:\n" +
                $"     {Configuracion.RutaArchivo}\n\n" +
                $"Cadena en uso:\n{Configuracion.CadenaConexion}",
                "Error de conexion",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormPrincipal());
    }
}
