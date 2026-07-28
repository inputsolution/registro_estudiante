namespace RegistroEstudiantes.Tests;

/// <summary>
/// El repositorio en memoria es estatico, asi que sus pruebas no pueden correr
/// en paralelo: se pisarian el estado entre si. Agrupar los casos en una misma
/// coleccion obliga a xUnit a ejecutarlos uno tras otro.
/// </summary>
[CollectionDefinition("Repositorio", DisableParallelization = true)]
public class ColeccionRepositorio
{
}
