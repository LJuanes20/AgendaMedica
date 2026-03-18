USE [agendamedica];
GO

/* ====================================
   LIMPIAR TABLAS EN ORDEN
   ==================================== */
DELETE FROM dbo.Cita;
DELETE FROM dbo.HorarioMedico;
DELETE FROM dbo.Medico;
DELETE FROM dbo.Paciente;
DELETE FROM dbo.Especialidad;
GO

/* ====================================
   REINICIAR IDENTITY
   ==================================== */
DBCC CHECKIDENT ('dbo.Cita', RESEED, 0);
DBCC CHECKIDENT ('dbo.HorarioMedico', RESEED, 0);
DBCC CHECKIDENT ('dbo.Medico', RESEED, 0);
DBCC CHECKIDENT ('dbo.Paciente', RESEED, 0);
DBCC CHECKIDENT ('dbo.Especialidad', RESEED, 0);
GO

/* ====================================
   ESPECIALIDADES
   ==================================== */
SET IDENTITY_INSERT dbo.Especialidad ON;

INSERT INTO dbo.Especialidad (IdEspecialidad, Nombre, Duracion)
VALUES
    (1, 'Medicina General', 20),
    (2, 'Cardiología', 30),
    (3, 'Cirugía', 45),
    (4, 'Pediatría', 20),
    (5, 'Ginecología', 30);

SET IDENTITY_INSERT dbo.Especialidad OFF;
GO

/* ====================================
   15 PACIENTES
   ==================================== */
INSERT INTO dbo.Paciente (NombreCompleto, FechaNacimiento, Telefono, Correo)
VALUES
('Juan Pérez',        '1990-01-15', '6001000001', 'juan.perez@mail.com'),
('María Gómez',       '1992-03-21', '6001000002', 'maria.gomez@mail.com'),
('Carlos Rodríguez',  '1988-07-11', '6001000003', 'carlos.rodriguez@mail.com'),
('Ana Martínez',      '1995-05-09', '6001000004', 'ana.martinez@mail.com'),
('Luis Hernández',    '1987-08-14', '6001000005', 'luis.hernandez@mail.com'),
('Sofía López',       '1998-11-22', '6001000006', 'sofia.lopez@mail.com'),
('Pedro Sánchez',     '1991-02-17', '6001000007', 'pedro.sanchez@mail.com'),
('Laura Ramírez',     '1993-06-30', '6001000008', 'laura.ramirez@mail.com'),
('Diego Torres',      '1989-09-05', '6001000009', 'diego.torres@mail.com'),
('Valeria Flores',    '1996-12-01', '6001000010', 'valeria.flores@mail.com'),
('Miguel Castro',     '1985-04-12', '6001000011', 'miguel.castro@mail.com'),
('Daniela Morales',   '1997-10-19', '6001000012', 'daniela.morales@mail.com'),
('Jorge Ortiz',       '1994-01-28', '6001000013', 'jorge.ortiz@mail.com'),
('Elena Vargas',      '1990-07-07', '6001000014', 'elena.vargas@mail.com'),
('Ricardo Navarro',   '1986-03-03', '6001000015', 'ricardo.navarro@mail.com');
GO

/* ====================================
   10 MÉDICOS
   ==================================== */
INSERT INTO dbo.Medico (NombreCompleto, EspecialidadId)
VALUES
('Dr. Andrés Ruiz',      1),
('Dra. Patricia Mendoza',2),
('Dr. Fernando Silva',   3),
('Dra. Gabriela Reyes',  4),
('Dr. Óscar Cruz',       5),
('Dra. Natalia Herrera', 1),
('Dr. Roberto Medina',   2),
('Dra. Paola Rojas',     3),
('Dr. Héctor Guerrero',  4),
('Dra. Lucía Peña',      5);
GO

/* ====================================
   HORARIOS DE MÉDICOS
   ==================================== */
INSERT INTO dbo.HorarioMedico (MedicoId, DiaSemana, HoraInicio, HoraFin)
SELECT
    m.IdMedico,
    d.DiaSemana,
    b.HoraInicio,
    b.HoraFin
FROM dbo.Medico m
CROSS JOIN (
    VALUES
        ('Lunes'),
        ('Martes'),
        ('Miércoles'),
        ('Jueves'),
        ('Viernes')
) d(DiaSemana)
CROSS JOIN (
    VALUES
        (CAST('08:00' AS time), CAST('08:30' AS time)),
        (CAST('08:30' AS time), CAST('09:00' AS time)),
        (CAST('09:00' AS time), CAST('09:30' AS time)),
        (CAST('09:30' AS time), CAST('10:00' AS time)),
        (CAST('10:00' AS time), CAST('10:30' AS time)),
        (CAST('10:30' AS time), CAST('11:00' AS time)),
        (CAST('11:00' AS time), CAST('11:30' AS time)),
        (CAST('11:30' AS time), CAST('12:00' AS time)),
        (CAST('12:00' AS time), CAST('12:30' AS time)),
        (CAST('12:30' AS time), CAST('13:00' AS time)),
        (CAST('13:00' AS time), CAST('13:30' AS time)),
        (CAST('13:30' AS time), CAST('14:00' AS time)),
        (CAST('14:00' AS time), CAST('14:30' AS time)),
        (CAST('14:30' AS time), CAST('15:00' AS time)),
        (CAST('15:00' AS time), CAST('15:30' AS time)),
        (CAST('15:30' AS time), CAST('16:00' AS time)),
        (CAST('16:00' AS time), CAST('16:30' AS time)),
        (CAST('16:30' AS time), CAST('17:00' AS time))
) b(HoraInicio, HoraFin);
GO

/* ====================================
   12 CITAS NORMALES
   ==================================== */
INSERT INTO dbo.Cita
(
    MedicoId,
    PacienteId,
    Estado,
    Motivo,
    MotivoCancelacion,
    InicioCita,
    FinCita
)
VALUES
(1,  1,  'Programada', 'Consulta general',        NULL, '2026-03-16T08:00:00', '2026-03-16T08:30:00'),
(2,  2,  'Programada', 'Dolor en el pecho',       NULL, '2026-03-16T09:00:00', '2026-03-16T09:30:00'),
(3,  3,  'Programada', 'Valoración quirúrgica',   NULL, '2026-03-16T10:00:00', '2026-03-16T10:30:00'),
(4,  4,  'Programada', 'Control pediátrico',      NULL, '2026-03-16T11:00:00', '2026-03-16T11:30:00'),
(5,  5,  'Programada', 'Control ginecológico',    NULL, '2026-03-16T14:00:00', '2026-03-16T14:30:00'),
(6,  6,  'Programada', 'Consulta general',        NULL, '2026-03-17T08:30:00', '2026-03-17T09:00:00'),
(7,  7,  'Programada', 'Chequeo cardiológico',    NULL, '2026-03-17T09:30:00', '2026-03-17T10:00:00'),
(8,  8,  'Programada', 'Seguimiento postcirugía', NULL, '2026-03-17T10:30:00', '2026-03-17T11:00:00'),
(9,  9,  'Programada', 'Consulta pediátrica',     NULL, '2026-03-17T13:00:00', '2026-03-17T13:30:00'),
(10, 10, 'Programada', 'Revisión anual',          NULL, '2026-03-17T15:00:00', '2026-03-17T15:30:00'),
(1,  11, 'Programada', 'Dolor de cabeza',         NULL, '2026-03-18T08:00:00', '2026-03-18T08:30:00'),
(2,  12, 'Programada', 'Control cardiaco',        NULL, '2026-03-18T09:00:00', '2026-03-18T09:30:00');
GO

/* ====================================
   3 CITAS CANCELADAS
   ==================================== */
INSERT INTO dbo.Cita
(
    MedicoId,
    PacienteId,
    Estado,
    Motivo,
    MotivoCancelacion,
    InicioCita,
    FinCita
)
VALUES
(3, 13, 'Cancelada', 'Valoración quirúrgica', 'Paciente no asistirá',       '2026-03-18T10:00:00', '2026-03-18T10:30:00'),
(4, 14, 'Cancelada', 'Consulta pediátrica',   'Emergencia personal',        '2026-03-19T11:00:00', '2026-03-19T11:30:00'),
(5, 15, 'Cancelada', 'Control ginecológico',  'Reprogramada por el médico', '2026-03-19T14:00:00', '2026-03-19T14:30:00');
GO