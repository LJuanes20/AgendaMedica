using AMAPI.Infrastructure;
using AMShared.Models;
using AMAPI.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace AMAPI.Services
{
    public class CitasService : ICitasService
    {
        private readonly DbContext _dbContext;
        private readonly string _GET_ALL_CITAS_QUERY = "SELECT c.IdCita, c.Estado, c.Motivo, c.MotivoCancelacion, c.InicioCita, c.FinCita, p.IdPaciente, p.NombreCompleto AS PacienteNombre, p.Telefono AS PacienteTelefono, p.Correo AS PacienteCorreo, m.IdMedico, m.NombreCompleto AS MedicoNombre, e.IdEspecialidad, e.Nombre AS Especialidad, e.Duracion AS DuracionEspecialidad FROM dbo.Cita c INNER JOIN dbo.Paciente p ON c.PacienteId = p.IdPaciente INNER JOIN dbo.Medico m ON c.MedicoId = m.IdMedico INNER JOIN dbo.Especialidad e ON m.EspecialidadId = e.IdEspecialidad ";     
        private readonly string _CANCELAR_CITA_QUERY = "UPDATE dbo.Cita SET Estado = 'Cancelada', MotivoCancelacion = @MotivoCancelacion WHERE IdCita = @IdCita;";
        private readonly string _GET_EXIST_CITA = "IF EXISTS (SELECT 1 FROM Cita WHERE MedicoId = @MedicoId AND PacienteId = @PacienteId AND Estado <> 'Cancelada' AND @InicioCita >= InicioCita AND @FinCita <= FinCita) SELECT 1 AS Agendada; ELSE SELECT 0 AS Agendada;";
        private readonly string _GET_CANCELACIONES_MES_ANTERIOR = "SELECT COUNT(*) as Cancelaciones FROM [agendamedica].[dbo].[Cita] WHERE Estado = 'Cancelada' AND PacienteId = @PacienteId AND InicioCita >= DATEADD(DAY, -30, GETDATE());";
        private readonly string _AGENDAR_CITA_SP = "sp_InsertarCita";
        private readonly string _PROGRAMADA = "Programada";
        public CitasService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public OperationResult AgendarCita(CitaCreateDto cita)
        {
            var result = new OperationResult();
            try
            {
                using var command = new SqlCommand(_AGENDAR_CITA_SP, _dbContext.Connection());
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MedicoId", cita.MedicoId);
                command.Parameters.AddWithValue("@PacienteId", cita.PacienteId);
                command.Parameters.AddWithValue("@Estado", _PROGRAMADA);
                command.Parameters.AddWithValue("@Motivo", cita.Motivo);
                command.Parameters.AddWithValue("@InicioCita", cita.InicioCita);
                command.Parameters.AddWithValue("@FinCita", cita.FinCita);
                command.Parameters.AddWithValue("@NoCancelaciones", 3);

                var response = command.ExecuteNonQuery();

                result.Completed = true;
                result.Message = "Cita agendada correctamente.";
                return result;
            }
            catch (SqlException ex)
            {
                result.Completed = false;
                result.Message = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                result.Completed = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public bool CancelarCita(int idCita, string motivoCancelacion = "Cancelada por el paciente sin especificar motivo")
        {
            var command = new SqlCommand(_CANCELAR_CITA_QUERY, _dbContext.Connection());
            command.Parameters.AddWithValue("@IdCita", idCita);
            command.Parameters.AddWithValue("@MotivoCancelacion", motivoCancelacion);
            return command.ExecuteNonQuery() > 0;
        }

        public List<CitaDto> ConsultarCitas()
        {
            var citas = new List<CitaDto>();
            var command = new SqlCommand($"{_GET_ALL_CITAS_QUERY};", _dbContext.Connection());
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    var cita = new CitaDto()
                    {
                        Id = reader.GetInt32(0),
                        Estado = reader.GetString(1),
                        Motivo = reader.GetString(2),
                        MotivoCancelacion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        InicioCita = reader.GetDateTime(4),
                        FinCita = reader.GetDateTime(5),
                        Paciente = new PacienteDto
                        {
                            Id = reader.GetInt32(6),
                            Nombre = reader.GetString(7),
                            Telefono = reader.GetString(8),
                            Correo = reader.GetString(9)
                        },
                        Medico = new MedicoDto
                        {
                            Id = reader.GetInt32(10),
                            Nombre = reader.GetString(11),
                            Especialidad = new EspecialidadDto
                            {
                                Id = reader.GetInt32(12),
                                Nombre = reader.GetString(13),
                                DuracionMinutos = reader.GetInt32(14)
                            }
                        }

                    };
                    citas.Add(cita);
                }
                catch (Exception)
                {
                }

            }

            return citas;

        }

        // Todas las citas (pasadas y futuras) de un paciente
        public List<CitaDto> ConsultarCitasPorPaciente(int pacienteId)
        {
            var citasPaciente = new List<CitaDto>();
            var command = new SqlCommand($"{_GET_ALL_CITAS_QUERY} WHERE p.IdPaciente = @PacienteId",
                _dbContext.Connection());
            command.Parameters.AddWithValue("@PacienteId", pacienteId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    var cita = new CitaDto
                    {
                        Id = reader.GetInt32(0),
                        Estado = reader.GetString(1),
                        Motivo = reader.GetString(2),
                        MotivoCancelacion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        InicioCita = reader.GetDateTime(4),
                        FinCita = reader.GetDateTime(5),
                        Paciente = new PacienteDto
                        {
                            Id = reader.GetInt32(6),
                            Nombre = reader.GetString(7),
                            Telefono = reader.GetString(8),
                            Correo = reader.GetString(9)
                        },
                        Medico = new MedicoDto
                        {
                            Id = reader.GetInt32(10),
                            Nombre = reader.GetString(11),
                            Especialidad = new EspecialidadDto
                            {
                                Id = reader.GetInt32(12),
                                Nombre = reader.GetString(13),
                                DuracionMinutos = reader.GetInt32(14)
                            }
                        }
                    };

                    citasPaciente.Add(cita);
                }
                catch (Exception) { }
            }
            return citasPaciente;
        }

        // Los próximos 5 horarios disponibles del médico a partir de la fecha indicada
        public List<DateTime> ConsultarProximosHorariosDisponibles(int medicoId, DateTime fecha)
        {
            // TODO: Implementar cinco proximos horarios disponibles.
            var horariosDisponibles = new List<DateTime>();
            return horariosDisponibles;
        }

        public bool ExisteCitaEnHorario(int medicoId, int pacienteId, DateTime inicioCita, DateTime finCita)
        {
            bool existe = false;
            var command = new SqlCommand(_GET_EXIST_CITA, _dbContext.Connection());
            command.Parameters.AddWithValue("@MedicoId", medicoId);
            command.Parameters.AddWithValue("@PacienteId", pacienteId);
            command.Parameters.AddWithValue("@InicioCita", inicioCita);
            command.Parameters.AddWithValue("@FinCita", finCita);
            var result = command.ExecuteScalar();
            existe = Convert.ToInt32(result) == 1;
            return existe;
        }

        public int ContarCancelacionesPaciente(int pacienteId)
        {
            int cancelaciones = 0;
            var command = new SqlCommand(_GET_CANCELACIONES_MES_ANTERIOR, _dbContext.Connection());
            command.Parameters.AddWithValue("@PacienteId", pacienteId);
            var result = command.ExecuteScalar();
            cancelaciones = Convert.ToInt32(result);
            return cancelaciones;
        }
    }
}
