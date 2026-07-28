/* ============================================================
   Registro de Estudiantes - creacion de la base de datos
   ------------------------------------------------------------
   Ejecutar una sola vez en SQL Server Management Studio (SSMS)
   sobre la instancia donde vaya a vivir la aplicacion.

   El script se puede volver a ejecutar sin peligro: no borra
   nada de lo que ya exista.
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
        Grado           NVARCHAR(30)  NOT NULL CONSTRAINT DF_Estudiantes_Grado     DEFAULT (''),
        FechaRegistro   DATETIME2(0)  NOT NULL CONSTRAINT DF_Estudiantes_FechaReg  DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_Estudiantes        PRIMARY KEY (Id),
        CONSTRAINT UQ_Estudiantes_Doc    UNIQUE (Documento)
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
        (Documento, Nombres, Apellidos, FechaNacimiento, Telefono, Email, Direccion, Grado)
    VALUES
        ('1001234567', N'Ana Maria',      N'Torres Ruiz',     '2010-03-14', '3001112233', 'ana.torres@ejemplo.com',        N'Calle 12 # 4-56',      '7B'),
        ('1007654321', N'Carlos Andres',  N'Gomez Diaz',      '2009-11-02', '3104445566', 'carlos.gomez@ejemplo.com',      N'Carrera 30 # 8-12',    '8A'),
        ('1009887766', N'Valentina',      N'Alvarez Mesa',    '2011-06-25', '3125558899', 'valentina.alvarez@ejemplo.com', N'Calle 45 # 22-10',     '6A'),
        ('1002233445', N'Juan Sebastian', N'Bermudez Pena',   '2008-09-08', '3007776655', 'juan.bermudez@ejemplo.com',     N'Avenida 3 # 15-40',    '9C'),
        ('1004455667', N'Laura Sofia',    N'Castro Nino',     '2010-01-30', '3113334422', 'laura.castro@ejemplo.com',      N'Carrera 18 # 60-05',   '7B'),
        ('1006677889', N'Mateo',          N'Duarte Salazar',  '2009-04-17', '3189991122', 'mateo.duarte@ejemplo.com',      N'Diagonal 27 # 9-33',   '8A'),
        ('1003344556', N'Isabella',       N'Herrera Lopez',   '2011-12-03', '3162224433', 'isabella.herrera@ejemplo.com',  N'Calle 8 # 31-77',      '6A'),
        ('1008899001', N'Santiago',       N'Jimenez Rojas',   '2008-07-21', '3145556677', 'santiago.jimenez@ejemplo.com',  N'Transversal 5 # 40-18','9C'),
        ('1005566778', N'Camila Andrea',  N'Moreno Vargas',   '2010-10-11', '3208887744', 'camila.moreno@ejemplo.com',     N'Carrera 50 # 12-90',   '7A'),
        ('1000112233', N'Nicolas',        N'Ospina Guerrero', '2009-02-05', '3171113355', 'nicolas.ospina@ejemplo.com',    N'Calle 33 # 7-21',      '8B'),
        ('1002244668', N'Sara Lucia',     N'Quintero Marin',  '2011-08-29', '3196668811', 'sara.quintero@ejemplo.com',     N'Carrera 9 # 55-14',    '6B'),
        ('1007788990', N'Daniel Felipe',  N'Rincon Cardenas', '2008-05-19', '3134447799', 'daniel.rincon@ejemplo.com',     N'Avenida 68 # 24-06',   '9A');

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
