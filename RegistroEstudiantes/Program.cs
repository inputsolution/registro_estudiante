using RegistroEstudiantes.Formularios;

namespace RegistroEstudiantes;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormPrincipal());
    }
}
