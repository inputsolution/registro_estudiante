# Registro de Estudiantes

Aplicación de escritorio (Windows Forms, .NET 8) para registrar estudiantes.

**Estado actual: maqueta.** La interfaz está completa y funcional, pero los datos
se guardan en memoria — al cerrar la aplicación se pierden. La base de datos
definitiva (SQL Server) se conecta en el siguiente paso.

## Cómo ejecutarla en Windows

Requiere el [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
(o Visual Studio 2022, que ya lo incluye).

**Opción A — línea de comandos:**

```
cd RegistroEstudiantes
dotnet run
```

**Opción B — Visual Studio:**

Abrir `RegistroEstudiantes.sln` y presionar F5.

## Generar un .exe portable

Para entregar un ejecutable único que no requiera instalar .NET:

```
cd RegistroEstudiantes
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

El `.exe` queda en `bin\Release\net8.0-windows\win-x64\publish\`.

## Pruebas

```
cd RegistroEstudiantes.Tests
dotnet test
```

40 casos sobre el repositorio y el modelo: altas, bajas, modificaciones,
búsqueda, documento único y cálculo de edad. Corren en cualquier sistema
operativo — no dependen de Windows Forms.

Lo que **no** cubren: la interfaz. Que un botón esté bien posicionado o que la
grilla se dibuje correctamente solo se verifica ejecutando la aplicación en
Windows.

## Qué hace

- Formulario con los datos básicos del estudiante (arriba)
- Grilla con todos los registros (abajo)
- Guardar, actualizar y eliminar
- Búsqueda en vivo por documento, nombres o apellidos
- Validaciones: campos obligatorios, documento no repetido, fecha no futura,
  formato de email
- Al seleccionar una fila, sus datos se cargan en el formulario para editar
- Arranca con 2 registros de ejemplo para ver la grilla con contenido

## Estructura

```
RegistroEstudiantes/
├── Modelos/
│   └── Estudiante.cs              Datos del estudiante
├── Datos/
│   ├── RepositorioMemoria.cs      Almacenamiento temporal (maqueta)
│   └── BaseDatos.cs.sqlite-pendiente   Versión SQLite, sin usar
├── Formularios/
│   ├── FormPrincipal.cs           Lógica de la ventana
│   └── FormPrincipal.Designer.cs  Diseño de la ventana
└── Program.cs                     Punto de entrada

RegistroEstudiantes.Tests/
├── RepositorioMemoriaTests.cs     CRUD, búsqueda, documento único
└── EstudianteTests.cs             Nombre completo y cálculo de edad
```

## Conectar la base de datos definitiva

`RepositorioMemoria` expone estos métodos:

```csharp
List<Estudiante> Listar(string filtro = "")
int  Insertar(Estudiante e)
void Actualizar(Estudiante e)
void Eliminar(int id)
bool ExisteDocumento(string documento, int idExcluir = 0)
```

Para pasar a SQL Server se crea una clase con esos mismos métodos, se agrega el
paquete `Microsoft.Data.SqlClient` al `.csproj` y se cambian las llamadas en
`FormPrincipal.cs`. El resto del formulario no se toca.

El archivo `BaseDatos.cs.sqlite-pendiente` ya tiene esa misma interfaz
implementada contra SQLite, por si se prefiere esa opción (no requiere instalar
nada en la máquina donde corre).

## Nota

`EnableWindowsTargeting` en el `.csproj` permite compilar el proyecto desde
macOS o Linux para verificar el código. La aplicación se ejecuta únicamente en
Windows. En Windows esa línea es inofensiva.
