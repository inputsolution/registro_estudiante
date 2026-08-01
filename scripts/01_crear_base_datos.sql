/* ============================================================
   Registro de Estudiantes Universitarios - creacion de la base
   ------------------------------------------------------------
   Ejecutar una sola vez en SQL Server Management Studio (SSMS)
   sobre la instancia donde vaya a vivir la aplicacion.

   El script se puede volver a ejecutar sin peligro: no borra
   nada de lo que ya exista.

   Si la base se creo con la version anterior (colegio, columna
   Grado), ejecutar despues scripts/02_migrar_a_universidad.sql.
   ============================================================ */

/* ---------- 1. Base de datos ---------- */

IF DB_ID('RegistroEstudiantes') IS NULL
BEGIN
    CREATE DATABASE RegistroEstudiantes;
    PRINT 'Base de datos RegistroEstudiantes creada.';
END
ELSE
BEGIN
    PRINT 'La base de datos RegistroEstudiantes ya existia.';
END
GO

USE RegistroEstudiantes;
GO

/* ---------- 2. Tabla ---------- */

IF OBJECT_ID('dbo.Estudiantes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Estudiantes
    (
        Id              INT           IDENTITY(1,1) NOT NULL,
        Documento       NVARCHAR(20)  NOT NULL,
        Nombres         NVARCHAR(60)  NOT NULL,
        Apellidos       NVARCHAR(60)  NOT NULL,
        FechaNacimiento DATE          NOT NULL,
        Telefono        NVARCHAR(20)  NOT NULL CONSTRAINT DF_Estudiantes_Telefono  DEFAULT (''),
        Email           NVARCHAR(80)  NOT NULL CONSTRAINT DF_Estudiantes_Email     DEFAULT (''),
        Direccion       NVARCHAR(120) NOT NULL CONSTRAINT DF_Estudiantes_Direccion DEFAULT (''),
        Carrera         NVARCHAR(80)  NOT NULL CONSTRAINT DF_Estudiantes_Carrera   DEFAULT (''),
        Semestre        TINYINT       NOT NULL CONSTRAINT DF_Estudiantes_Semestre  DEFAULT (1),
        FechaRegistro   DATETIME2(0)  NOT NULL CONSTRAINT DF_Estudiantes_FechaReg  DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_Estudiantes        PRIMARY KEY (Id),
        CONSTRAINT UQ_Estudiantes_Doc    UNIQUE (Documento),
        CONSTRAINT CK_Estudiantes_Sem    CHECK (Semestre BETWEEN 1 AND 12)
    );

    /* La grilla ordena por apellido y nombre. */
    CREATE INDEX IX_Estudiantes_Nombre
        ON dbo.Estudiantes (Apellidos, Nombres);

    PRINT 'Tabla dbo.Estudiantes creada.';
END
ELSE
BEGIN
    PRINT 'La tabla dbo.Estudiantes ya existia.';
END
GO

/* ---------- 3. Datos de ejemplo ---------- */
/* Solo se insertan si la tabla esta vacia, para no duplicar
   registros si el script se ejecuta mas de una vez.        */

IF NOT EXISTS (SELECT 1 FROM dbo.Estudiantes)
BEGIN
    INSERT INTO dbo.Estudiantes
        (Documento, Nombres, Apellidos, FechaNacimiento, Telefono, Email, Direccion, Carrera, Semestre)
    VALUES
        ('1001234567', N'Ana Maria',      N'Torres Ruiz',     '2003-03-14', '3001112233', 'ana.torres@ejemplo.com',        N'Calle 12 # 4-56',      N'Ingenieria de Sistemas',      6),
        ('1007654321', N'Carlos Andres',  N'Gomez Diaz',      '2001-11-02', '3104445566', 'carlos.gomez@ejemplo.com',      N'Carrera 30 # 8-12',    N'Medicina',                    9),
        ('1009887766', N'Valentina',      N'Alvarez Mesa',    '2005-06-25', '3125558899', 'valentina.alvarez@ejemplo.com', N'Calle 45 # 22-10',     N'Derecho',                     2),
        ('1002233445', N'Juan Sebastian', N'Bermudez Pena',   '2000-09-08', '3007776655', 'juan.bermudez@ejemplo.com',     N'Avenida 3 # 15-40',    N'Administracion de Empresas', 10),
        ('1004455667', N'Laura Sofia',    N'Castro Nino',     '2004-01-30', '3113334422', 'laura.castro@ejemplo.com',      N'Carrera 18 # 60-05',   N'Psicologia',                  4),
        ('1006677889', N'Mateo',          N'Duarte Salazar',  '2002-04-17', '3189991122', 'mateo.duarte@ejemplo.com',      N'Diagonal 27 # 9-33',   N'Ingenieria Civil',            7),
        ('1003344556', N'Isabella',       N'Herrera Lopez',   '2006-12-03', '3162224433', 'isabella.herrera@ejemplo.com',  N'Calle 8 # 31-77',      N'Diseño Grafico',              1),
        ('1008899001', N'Santiago',       N'Jimenez Rojas',   '2000-07-21', '3145556677', 'santiago.jimenez@ejemplo.com',  N'Transversal 5 # 40-18',N'Contaduria Publica',          8),
        ('1005566778', N'Camila Andrea',  N'Moreno Vargas',   '2003-10-11', '3208887744', 'camila.moreno@ejemplo.com',     N'Carrera 50 # 12-90',   N'Enfermeria',                  5),
        ('1000112233', N'Nicolas',        N'Ospina Guerrero', '2002-02-05', '3171113355', 'nicolas.ospina@ejemplo.com',    N'Calle 33 # 7-21',      N'Ingenieria de Sistemas',      7),
        ('1002244668', N'Sara Lucia',     N'Quintero Marin',  '2005-08-29', '3196668811', 'sara.quintero@ejemplo.com',     N'Carrera 9 # 55-14',    N'Comunicacion Social',         3),
        ('1007788990', N'Daniel Felipe',  N'Rincon Cardenas', '2001-05-19', '3134447799', 'daniel.rincon@ejemplo.com',     N'Avenida 68 # 24-06',   N'Arquitectura',                9);

    PRINT 'Se insertaron 12 estudiantes de ejemplo.';
END
ELSE
BEGIN
    PRINT 'La tabla ya tenia datos: no se inserto nada.';
END
GO

/* ---------- 4. Comprobacion ---------- */

SELECT COUNT(*) AS TotalEstudiantes FROM dbo.Estudiantes;
GO
