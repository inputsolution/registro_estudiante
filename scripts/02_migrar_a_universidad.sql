/* ============================================================
   Migracion: de registro escolar a registro universitario
   ------------------------------------------------------------
   Ejecutar SOLO si la base de datos ya existia con la version
   anterior de la aplicacion (la que tenia la columna Grado).

   Si la base se crea desde cero con 01_crear_base_datos.sql,
   este script no hace falta (y si se ejecuta, no daña nada:
   detecta que no hay nada que migrar y termina).

   Que hace:
   1. Agrega las columnas Carrera y Semestre si no existen.
   2. Elimina la columna Grado y su restriccion por defecto.

   El valor de Grado no se copia a Carrera: son datos de otro
   dominio (un "7B" escolar no es un programa academico). Los
   registros existentes quedan con Carrera vacia y Semestre 1,
   listos para completarse desde la aplicacion.
   ============================================================ */

USE RegistroEstudiantes;
GO

/* ---------- 1. Carrera ---------- */

IF COL_LENGTH('dbo.Estudiantes', 'Carrera') IS NULL
BEGIN
    ALTER TABLE dbo.Estudiantes
        ADD Carrera NVARCHAR(80) NOT NULL
            CONSTRAINT DF_Estudiantes_Carrera DEFAULT ('');
    PRINT 'Columna Carrera agregada.';
END
ELSE
BEGIN
    PRINT 'La columna Carrera ya existia.';
END
GO

/* ---------- 2. Semestre ---------- */

IF COL_LENGTH('dbo.Estudiantes', 'Semestre') IS NULL
BEGIN
    ALTER TABLE dbo.Estudiantes
        ADD Semestre TINYINT NOT NULL
            CONSTRAINT DF_Estudiantes_Semestre DEFAULT (1),
            CONSTRAINT CK_Estudiantes_Sem CHECK (Semestre BETWEEN 1 AND 12);
    PRINT 'Columna Semestre agregada.';
END
ELSE
BEGIN
    PRINT 'La columna Semestre ya existia.';
END
GO

/* ---------- 3. Eliminar Grado ---------- */

IF COL_LENGTH('dbo.Estudiantes', 'Grado') IS NOT NULL
BEGIN
    /* La restriccion DEFAULT impide borrar la columna; se quita primero.
       Se busca por nombre real por si no se llama DF_Estudiantes_Grado. */
    DECLARE @restriccion NVARCHAR(128);

    SELECT @restriccion = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c
        ON c.object_id = dc.parent_object_id
       AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Estudiantes')
      AND c.name = 'Grado';

    IF @restriccion IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE dbo.Estudiantes DROP CONSTRAINT [' + @restriccion + '];');
    END

    ALTER TABLE dbo.Estudiantes DROP COLUMN Grado;
    PRINT 'Columna Grado eliminada.';
END
ELSE
BEGIN
    PRINT 'La columna Grado ya no existe: nada que migrar.';
END
GO

/* ---------- 4. Comprobacion ---------- */

SELECT TOP (5) Id, Documento, Nombres, Apellidos, Carrera, Semestre
FROM dbo.Estudiantes
ORDER BY Apellidos, Nombres;
GO
