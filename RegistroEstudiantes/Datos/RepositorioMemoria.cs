using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Datos;

/// <summary>
/// Almacenamiento en memoria, equivalente a RepositorioSqlServer pero sin
/// base de datos.
///
/// La aplicacion ya no lo usa: quedo como referencia de la interfaz y, sobre
/// todo, para las pruebas, que se ejecutan en cualquier sistema operativo y
/// sin necesidad de un SQL Server disponible.
/// </summary>
public static class RepositorioMemoria
{
    private static readonly List<Estudiante> Datos = new();
    private static int _siguienteId = 1;

    /// <summary>
    /// Vacia el almacenamiento. Usado por las pruebas para partir de un estado
    /// conocido en cada caso.
    /// </summary>
    public static void Limpiar()
    {
        Datos.Clear();
        _siguienteId = 1;
    }

    /// <summary>
    /// Carga unos registros de ejemplo para poder ver la grilla con contenido.
    /// </summary>
    public static void CargarEjemplos()
    {
        if (Datos.Count > 0)
        {
            return;
        }

        var ejemplos = new[]
        {
            new Estudiante
            {
                Documento = "1001234567",
                Nombres = "Ana Maria",
                Apellidos = "Torres Ruiz",
                FechaNacimiento = new DateTime(2010, 3, 14),
                Telefono = "3001112233",
                Email = "ana.torres@ejemplo.com",
                Direccion = "Calle 12 # 4-56",
                Grado = "7B"
            },
            new Estudiante
            {
                Documento = "1007654321",
                Nombres = "Carlos Andres",
                Apellidos = "Gomez Diaz",
                FechaNacimiento = new DateTime(2009, 11, 2),
                Telefono = "3104445566",
                Email = "carlos.gomez@ejemplo.com",
                Direccion = "Carrera 30 # 8-12",
                Grado = "8A"
            },
            new Estudiante
            {
                Documento = "1009887766",
                Nombres = "Valentina",
                Apellidos = "Alvarez Mesa",
                FechaNacimiento = new DateTime(2011, 6, 25),
                Telefono = "3125558899",
                Email = "valentina.alvarez@ejemplo.com",
                Direccion = "Calle 45 # 22-10",
                Grado = "6A"
            },
            new Estudiante
            {
                Documento = "1002233445",
                Nombres = "Juan Sebastian",
                Apellidos = "Bermudez Pena",
                FechaNacimiento = new DateTime(2008, 9, 8),
                Telefono = "3007776655",
                Email = "juan.bermudez@ejemplo.com",
                Direccion = "Avenida 3 # 15-40",
                Grado = "9C"
            },
            new Estudiante
            {
                Documento = "1004455667",
                Nombres = "Laura Sofia",
                Apellidos = "Castro Nino",
                FechaNacimiento = new DateTime(2010, 1, 30),
                Telefono = "3113334422",
                Email = "laura.castro@ejemplo.com",
                Direccion = "Carrera 18 # 60-05",
                Grado = "7B"
            },
            new Estudiante
            {
                Documento = "1006677889",
                Nombres = "Mateo",
                Apellidos = "Duarte Salazar",
                FechaNacimiento = new DateTime(2009, 4, 17),
                Telefono = "3189991122",
                Email = "mateo.duarte@ejemplo.com",
                Direccion = "Diagonal 27 # 9-33",
                Grado = "8A"
            },
            new Estudiante
            {
                Documento = "1003344556",
                Nombres = "Isabella",
                Apellidos = "Herrera Lopez",
                FechaNacimiento = new DateTime(2011, 12, 3),
                Telefono = "3162224433",
                Email = "isabella.herrera@ejemplo.com",
                Direccion = "Calle 8 # 31-77",
                Grado = "6A"
            },
            new Estudiante
            {
                Documento = "1008899001",
                Nombres = "Santiago",
                Apellidos = "Jimenez Rojas",
                FechaNacimiento = new DateTime(2008, 7, 21),
                Telefono = "3145556677",
                Email = "santiago.jimenez@ejemplo.com",
                Direccion = "Transversal 5 # 40-18",
                Grado = "9C"
            },
            new Estudiante
            {
                Documento = "1005566778",
                Nombres = "Camila Andrea",
                Apellidos = "Moreno Vargas",
                FechaNacimiento = new DateTime(2010, 10, 11),
                Telefono = "3208887744",
                Email = "camila.moreno@ejemplo.com",
                Direccion = "Carrera 50 # 12-90",
                Grado = "7A"
            },
            new Estudiante
            {
                Documento = "1000112233",
                Nombres = "Nicolas",
                Apellidos = "Ospina Guerrero",
                FechaNacimiento = new DateTime(2009, 2, 5),
                Telefono = "3171113355",
                Email = "nicolas.ospina@ejemplo.com",
                Direccion = "Calle 33 # 7-21",
                Grado = "8B"
            },
            new Estudiante
            {
                Documento = "1002244668",
                Nombres = "Sara Lucia",
                Apellidos = "Quintero Marin",
                FechaNacimiento = new DateTime(2011, 8, 29),
                Telefono = "3196668811",
                Email = "sara.quintero@ejemplo.com",
                Direccion = "Carrera 9 # 55-14",
                Grado = "6B"
            },
            new Estudiante
            {
                Documento = "1007788990",
                Nombres = "Daniel Felipe",
                Apellidos = "Rincon Cardenas",
                FechaNacimiento = new DateTime(2008, 5, 19),
                Telefono = "3134447799",
                Email = "daniel.rincon@ejemplo.com",
                Direccion = "Avenida 68 # 24-06",
                Grado = "9A"
            }
        };

        foreach (var estudiante in ejemplos)
        {
            Insertar(estudiante);
        }
    }

    /// <summary>
    /// Devuelve los estudiantes, opcionalmente filtrados por documento,
    /// nombres o apellidos.
    /// </summary>
    public static List<Estudiante> Listar(string filtro = "")
    {
        IEnumerable<Estudiante> consulta = Datos;

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = filtro.Trim();
            consulta = consulta.Where(e =>
                e.Documento.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                e.Nombres.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                e.Apellidos.Contains(f, StringComparison.OrdinalIgnoreCase));
        }

        return consulta
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .Select(Clonar)
            .ToList();
    }

    /// <summary>
    /// Agrega un estudiante nuevo y devuelve el Id asignado.
    /// </summary>
    public static int Insertar(Estudiante e)
    {
        var copia = Clonar(e);
        copia.Id = _siguienteId++;
        copia.FechaRegistro = DateTime.Now;
        Datos.Add(copia);
        return copia.Id;
    }

    /// <summary>
    /// Actualiza los datos de un estudiante existente.
    /// </summary>
    public static void Actualizar(Estudiante e)
    {
        var actual = Datos.FirstOrDefault(x => x.Id == e.Id);
        if (actual is null)
        {
            return;
        }

        actual.Documento = e.Documento.Trim();
        actual.Nombres = e.Nombres.Trim();
        actual.Apellidos = e.Apellidos.Trim();
        actual.FechaNacimiento = e.FechaNacimiento;
        actual.Telefono = e.Telefono.Trim();
        actual.Email = e.Email.Trim();
        actual.Direccion = e.Direccion.Trim();
        actual.Grado = e.Grado.Trim();
    }

    /// <summary>
    /// Elimina un estudiante por Id.
    /// </summary>
    public static void Eliminar(int id)
    {
        Datos.RemoveAll(e => e.Id == id);
    }

    /// <summary>
    /// Indica si ya existe otro estudiante con el mismo documento.
    /// Se excluye <paramref name="idExcluir"/> para permitir editar sin falso positivo.
    /// </summary>
    public static bool ExisteDocumento(string documento, int idExcluir = 0)
    {
        var doc = documento.Trim();
        return Datos.Any(e =>
            e.Id != idExcluir &&
            e.Documento.Equals(doc, StringComparison.OrdinalIgnoreCase));
    }

    private static Estudiante Clonar(Estudiante e) => new()
    {
        Id = e.Id,
        Documento = e.Documento,
        Nombres = e.Nombres,
        Apellidos = e.Apellidos,
        FechaNacimiento = e.FechaNacimiento,
        Telefono = e.Telefono,
        Email = e.Email,
        Direccion = e.Direccion,
        Grado = e.Grado,
        FechaRegistro = e.FechaRegistro
    };
}
