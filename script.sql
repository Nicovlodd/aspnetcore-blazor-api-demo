
-- ==========================================
-- MEDICAL CARE API - SCRIPT DE BASE DE DATOS
-- ==========================================

-- 1. CREACIÓN DE TABLAS LIMPIAS Y OPTIMIZADAS
CREATE TABLE Speciality (
    Speciality_Id INT IDENTITY(1,1) PRIMARY KEY,
    Speciality_Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Doctor (
    Doctor_Id INT IDENTITY(1,1) PRIMARY KEY,
    Doctor_FirstName NVARCHAR(100) NOT NULL,
    Doctor_LastName NVARCHAR(100) NOT NULL,
    Speciality_Id INT NOT NULL,
    FOREIGN KEY (Speciality_Id) REFERENCES Speciality(Speciality_Id)
);

CREATE TABLE Patient (
    Patient_Id INT IDENTITY(1,1) PRIMARY KEY,
    Patient_RUT NVARCHAR(20) NOT NULL,
    Patient_FirstName NVARCHAR(100) NOT NULL,
    Patient_LastName NVARCHAR(100) NOT NULL,
    Patient_DateOfBirth DATE NOT NULL
);

CREATE TABLE Appointment (
    Appointment_Id INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    Appointment_StartUtc DATETIME2(7) NOT NULL,
    Appointment_EndUtc DATETIME2(7) NOT NULL,
    Appointment_Diagnosis NVARCHAR(500),
    Appointment_Room NVARCHAR(30),
    Appointment_Status NVARCHAR(20),
    Appointment_CreatedBy NVARCHAR(128) NOT NULL,
    Appointment_CreatedAt DATETIME2(7) NOT NULL,
    Appointment_ModifiedBy NVARCHAR(128),
    Appointment_ModifiedAt DATETIME2(7),
    FOREIGN KEY (PatientId) REFERENCES Patient(Patient_Id),
    FOREIGN KEY (DoctorId) REFERENCES Doctor(Doctor_Id)
);

-- =======================================================
-- 2. Nunca permitir dos RUT iguales
-- =======================================================

ALTER TABLE Patient
ADD CONSTRAINT UQ_Patient_RUT UNIQUE (Patient_RUT);

-- =======================================================
-- 2. DATOS DE PRUEBA (POBLADO INICIAL)
-- =======================================================

-- 2.1 Especialidades
SET IDENTITY_INSERT Speciality ON;
INSERT INTO Speciality (Speciality_Id, Speciality_Name) VALUES 
(1, N'EspecialidadTest'),
(2, N'Cientifico'),
(3, N'Cirujano');
SET IDENTITY_INSERT Speciality OFF;
GO

-- 2.2 Pacientes
SET IDENTITY_INSERT Patient ON;
INSERT INTO Patient (Patient_Id, Patient_RUT, Patient_FirstName, Patient_LastName, Patient_DateOfBirth) VALUES 
(1, N'12.123.123-1', N'pacienteTest', N'pacienteTest', CAST(N'2003-06-05' AS Date));
SET IDENTITY_INSERT Patient OFF;
GO

-- 2.3 Doctores
SET IDENTITY_INSERT Doctor ON;
INSERT INTO Doctor (Doctor_Id, Doctor_FirstName, Doctor_LastName, Speciality_Id, Doctor_RUT) VALUES 
(3, N'doctest1', N'doctest1', 2, N'13.445.345.1'),
(4, N'doc', N'doc', 3, N'14.123.123-4');
SET IDENTITY_INSERT Doctor OFF;
GO

-- 2.4 Atenciones Médicas
SET IDENTITY_INSERT Appointment ON;
INSERT INTO Appointment (Appointment_Id, PatientId, DoctorId, Appointment_StartUtc, Appointment_EndUtc, Appointment_Diagnosis, Appointment_Room, Appointment_Status, Appointment_CreatedBy, Appointment_CreatedAt) VALUES 
(2, 1, 3, CAST(N'2026-06-06T13:31:58.7566667' AS DateTime2), CAST(N'2026-06-06T15:31:58.7566667' AS DateTime2), N'nada', N'string', N'string', N'AdminTest', CAST(N'2026-06-05T03:33:28.2459249' AS DateTime2)),
(3, 1, 3, CAST(N'2026-06-05T01:42:00.0000000' AS DateTime2), CAST(N'2026-06-05T03:43:00.0000000' AS DateTime2), N'asd', N'Box 1', N'Agendada', N'AdminTest', CAST(N'2026-06-05T03:40:45.2754717' AS DateTime2)),
(4, 1, 3, CAST(N'2026-06-06T16:47:00.0000000' AS DateTime2), CAST(N'2026-06-06T17:47:00.0000000' AS DateTime2), N'solapada', N'Box 1', N'Agendada', N'AdminTest', CAST(N'2026-06-05T13:48:33.3664727' AS DateTime2)),
(5, 1, 4, CAST(N'2026-06-05T10:06:17.0566667' AS DateTime2), CAST(N'2026-06-05T11:06:17.0566667' AS DateTime2), N'pierna rota', N'Box 1', N'Agendada', N'AdminTest', CAST(N'2026-06-05T14:06:30.5433940' AS DateTime2));
SET IDENTITY_INSERT Appointment OFF;
GO



-- =========================================================================
-- Procedimiento Almacenado para Crear Cita con Validaciones de Solapamiento
-- =========================================================================

CREATE OR ALTER PROCEDURE sp_CreateAppointment
    @PatientId INT,
    @DoctorId INT,
    @StartUtc DATETIME2(7),
    @EndUtc DATETIME2(7),
    @Diagnosis NVARCHAR(500),
    @Room NVARCHAR(30),
    @Status NVARCHAR(20),
    @CreatedBy NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validar sentido lógico: El inicio no puede ser después del fin
    IF @StartUtc >= @EndUtc
    BEGIN
        THROW 51000, 'La fecha y hora de inicio debe ser menor a la fecha y hora de fin.', 1;
    END

    -- 2. Validar Solapamiento para el mismo Doctor
    -- Lógica: (NuevoInicio < ViejoFin) Y (NuevoFin > ViejoInicio)
    IF EXISTS (
        SELECT 1 
        FROM Appointment 
        WHERE DoctorId = @DoctorId 
          AND Appointment_StartUtc < @EndUtc 
          AND Appointment_EndUtc > @StartUtc
          AND Appointment_Status != 'Cancelada' -- Ignoramos las citas canceladas
    )
    BEGIN
        -- Si entra aquí, significa que el horario choca. Lanzamos un error.
        THROW 51001, 'El doctor ya tiene una cita programada que se solapa con este horario.', 1;
    END

    -- 3. Si pasó las validaciones, insertamos la cita de forma segura
    INSERT INTO Appointment (
        PatientId, 
        DoctorId, 
        Appointment_StartUtc, 
        Appointment_EndUtc, 
        Appointment_Diagnosis, 
        Appointment_Room, 
        Appointment_Status, 
        Appointment_CreatedBy, 
        Appointment_CreatedAt
    )
    VALUES (
        @PatientId, 
        @DoctorId, 
        @StartUtc, 
        @EndUtc, 
        @Diagnosis, 
        @Room, 
        @Status, 
        @CreatedBy, 
        SYSUTCDATETIME()
    );

    -- 4. Devolver el ID generado para que Dapper lo capture
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END;




