# Registro de Estudiantes Universitarios

Aplicación de escritorio (Windows Forms, .NET 8) para registrar estudiantes
de la universidad: datos personales, carrera y semestre.

Los datos se guardan en **SQL Server**.

## Puesta en marcha

**1. Crear la base de datos.** Abrir `scripts/01_crear_base_datos.sql` en SQL
Server Management Studio y ejecutarlo una vez. Crea la base, la tabla y 12
estudiantes de ejemplo. Se puede volver a ejecutar sin duplicar nada.

> Si la base ya existía de la versión anterior (registro escolar, columna
> `Grado`), ejecutar además `scripts/02_migrar_a_universidad.sql`: agrega
> `Carrera` y `Semestre` y elimina `Grado` sin tocar el resto de los datos.

**2. Revisar la conexión.** Está en `RegistroEstudiantes/appsettings.json`:

```json
"SqlServer": "Server=.;Database=RegistroEstudiantes;Trusted_Connection=True;TrustServerCertificate=True;"
```

`Server=.` es la instancia local por defecto. Otras opciones comunes:

| Instancia | Valor |
|---|---|
| SQL Server Express | `.\SQLEXPRESS` |
| LocalDB de Visual Studio | `(localdb)\MSSQLLocalDB` |
| Servidor en la red | `NOMBRE-PC\INSTANCIA` |

`Trusted_Connection=True` usa la cuenta de Windows, sin usuario ni contraseña.
Para autenticación SQL, reemplazar por `User Id=usuario;Password=clave;`.

El archivo se copia junto al ejecutable, así que se puede cambiar el servidor
sin recompilar.

## Cómo ejecutarla en Windows

Requiere **.NET 8**. Visual Studio 2022 lo incluye si se marca la carga de
trabajo **"Desarrollo de escritorio de .NET"** durante la instalación. Sin esa
carga de trabajo el proyecto abre pero no compila.

Sin Visual Studio, basta el [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

**Opción A — Visual Studio 2022:**

Abrir `RegistroEstudiantes.sln` (no el `.csproj`) y presionar F5.

La solución contiene dos proyectos. Si F5 arranca el equivocado, clic derecho
sobre **RegistroEstudiantes** → *Establecer como proyecto de inicio*.

**Opción B — línea de comandos:**

```
cd RegistroEstudiantes
dotnet run
```

**Versiones anteriores de Visual Studio no sirven.** VS 2012 llega hasta .NET
Framework 4.5 y VS 2019 hasta .NET 5; ninguna reconoce el formato de proyecto
que usa .NET 8.

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

155 casos sobre el repositorio, el modelo, las validaciones y la paginación.
Corren en cualquier sistema operativo y **no necesitan SQL Server**: usan el
repositorio en memoria, que expone la misma interfaz.

Lo que **no** cubren: la interfaz. Que un botón esté bien posicionado o que la
grilla se dibuje correctamente solo se verifica ejecutando la aplicación en
Windows.

## Qué hace

- Formulario con los datos del estudiante: documento, nombres, apellidos,
  fecha de nacimiento, teléfono, email, dirección, carrera y semestre
- Grilla con todos los registros (abajo)
- Guardar, actualizar y eliminar
- Búsqueda en vivo por documento, nombres, apellidos o carrera
- Paginación con selector de 10, 25, 50 o 100 filas por página
- Validaciones: campos obligatorios, documento único, fecha coherente,
  semestre entre 1 y 12, y formato de email, teléfono, documento, nombres
  y carrera
- Al seleccionar una fila, sus datos se cargan en el formulario para editar

## Estructura

```
scripts/
├── 01_crear_base_datos.sql        Crea base, tabla y datos de ejemplo
└── 02_migrar_a_universidad.sql    Migra una base de la versión escolar

RegistroEstudiantes/
├── appsettings.json               Cadena de conexión
├── Modelos/
│   ├── Estudiante.cs              Datos del estudiante
│   └── Validaciones.cs            Reglas de los campos
├── Datos/
│   ├── RepositorioSqlServer.cs    Acceso a SQL Server (el que usa la app)
│   ├── Configuracion.cs           Lectura de appsettings.json
│   ├── RepositorioMemoria.cs      Equivalente sin base de datos, para pruebas
│   └── BaseDatos.cs.sqlite-pendiente   Versión SQLite, sin usar
├── Formularios/
│   ├── FormPrincipal.cs           Lógica de la ventana
│   └── FormPrincipal.Designer.cs  Diseño de la ventana
└── Program.cs                     Punto de entrada

RegistroEstudiantes.Tests/
├── RepositorioMemoriaTests.cs     CRUD, búsqueda, documento único
├── ValidacionesTests.cs           Email, documento, teléfono, nombres
├── PaginacionTests.cs             Cálculo de páginas y rangos
├── EstudianteTests.cs             Nombre completo y cálculo de edad
└── InterfazRepositoriosTests.cs   Firma común de los repositorios
```

## Cambiar de motor de base de datos

`RepositorioMemoria` expone estos métodos:

```csharp
List<Estudiante> Listar(string filtro = "")
int  Insertar(Estudiante e)
void Actualizar(Estudiante e)
void Eliminar(int id)
bool ExisteDocumento(string documento, int idExcluir = 0)
```

`RepositorioSqlServer` implementa esos métodos contra SQL Server y es el que
usa la aplicación. `RepositorioMemoria` mantiene la misma forma sin base de
datos, y es lo que ejecutan las pruebas.

El archivo `BaseDatos.cs.sqlite-pendiente` tiene la misma interfaz contra
SQLite, por si alguna vez conviene una opción que no requiera instalar nada en
la máquina donde corre.

## Nota

`EnableWindowsTargeting` en el `.csproj` permite compilar el proyecto desde
macOS o Linux para verificar el código. La aplicación se ejecuta únicamente en
Windows. En Windows esa línea es inofensiva.
