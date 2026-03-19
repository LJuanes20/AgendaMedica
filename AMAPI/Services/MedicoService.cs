using AMAPI.Infrastructure;
using AMShared.Models;
using AMAPI.Services.Interfaces;
using System.Data.SqlClient;

namespace AMAPI.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly DbContext _dbContext;
        private readonly string _GET_MEDICOS_QUERY = "SELECT m.IdMedico, m.NombreCompleto, m.EspecialidadId, e.Nombre AS Especialidad, e.Duracion FROM dbo.Medico m INNER JOIN dbo.Especialidad e ON m.EspecialidadId = e.IdEspecialidad";
        private readonly string _GET_MEDICO_BY_ID_QUERY = "{0} WHERE m.IdMedico = @Id";
        private readonly string _POST_CREATE_MEDICO_QUERY = "INSERT INTO Medico (NombreCompleto, EspecialidadId) VALUES(@NombreCompleto, @EspecialidadId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        private readonly string _POST_UPDATE_MEDICO_QUERY = "UPDATE dbo.Medico SET NombreCompleto = @NombreCompleto, EspecialidadId = @EspecialidadId WHERE IdMedico = @Id";
        private readonly string _DELETE_MEDICO_QUERY = "DELETE FROM dbo.Medico WHERE IdMedico = @Id";       
        private readonly string _SP_AGENDA_MEDICO_BY_DATE = "EXEC dbo.sp_ObtenerAgendaMedicoPorFecha  @MedicoId = @MedicoId, @Fecha = @Fecha;";
        private readonly string _DELETE_HORARIO_MEDICO = "DELETE FROM HorarioMedico WHERE MedicoId = @MedicoId";
        private readonly string _GET_HORARIOS_MEDICO = "SELECT DiaSemana, HoraInicio, HoraFin FROM HorarioMedico WHERE MedicoId = @MedicoId";
        private string _VERIFICAR_HORARIO_DISPONIBLE = "EXEC dbo.sp_HorarioDisponibleAgendaMedico @MedicoId = @MedicoId, @Fecha = @Fecha, @HorarioInicio = @HorarioInicio, @HorarioFin = @HorarioFin;";
        private readonly string _GET_HORARIOS_MEDICO_DISPONIBLES_BY_DIA = "SELECT IdHorario, HoraInicio, HoraFin FROM HorarioMedico WHERE MedicoId = @MedicoId AND DiaSemana = @DiaSemana";
        public MedicoService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MedicoDto> GetAllMedicos()
        {
            var command = new SqlCommand(_GET_MEDICOS_QUERY, _dbContext.Connection());
            using (var reader = command.ExecuteReader())
            {
                var medicos = new List<MedicoDto>();
                while (reader.Read())
                {
                    medicos.Add(new MedicoDto
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Especialidad = new EspecialidadDto { Id = reader.GetInt32(2), Nombre = reader.GetString(3), DuracionMinutos = reader.GetInt32(4) }
                    });
                }
                return medicos;
            }
        }

        public MedicoDto? GetMedicoById(int id)
        {
            var command = new SqlCommand(string.Format(_GET_MEDICO_BY_ID_QUERY, _GET_MEDICOS_QUERY), _dbContext.Connection());
            command.Parameters.AddWithValue("@Id", id);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new MedicoDto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Especialidad = new EspecialidadDto { Id = reader.GetInt32(2), Nombre = reader.GetString(3), DuracionMinutos = reader.GetInt32(4) }
                };
            }
            return null;
        }

        public List<MedicoDto> GetMedicosByEspecialidad(int especialidadId)
        {
            var command = new SqlCommand(string.Format(_GET_MEDICOS_QUERY + " WHERE m.EspecialidadId = @EspecialidadId"), _dbContext.Connection());
            command.Parameters.AddWithValue("@EspecialidadId", especialidadId);
            using (var reader = command.ExecuteReader())
            {
                var medicos = new List<MedicoDto>();
                while (reader.Read())
                {
                    medicos.Add(new MedicoDto
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Especialidad = new EspecialidadDto { Id = reader.GetInt32(2), Nombre = reader.GetString(3), DuracionMinutos = reader.GetInt32(4) }
                    });
                }
                return medicos;
            }
        }

        public OperationResult CrearMedico(MedicoCreacionDto medico)
        {
            if (medico is null)
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "La información del médico es inválida."
                };
            }

            try
            {
                using var connection = _dbContext.Connection();
                if (connection is not null)
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }
                }

                using var transaction = connection.BeginTransaction();

                try
                {
                    using var command = new SqlCommand(_POST_CREATE_MEDICO_QUERY, connection, transaction);
                    command.Parameters.AddWithValue("@NombreCompleto", medico.Nombre);
                    command.Parameters.AddWithValue("@EspecialidadId", medico.EspecialidadId);

                    var result = command.ExecuteScalar();

                    if (result is null || !int.TryParse(result.ToString(), out int newId) || newId <= 0)
                    {
                        transaction.Rollback();
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "Ocurrió un error al crear el médico."
                        };
                    }

                    var rHorarios = GuardarHorariosMedico(newId, medico.Horarios, connection, transaction);

                    if (!rHorarios.Completed)
                    {
                        transaction.Rollback();
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "Ocurrió un error al guardar los horarios."
                        };
                    }

                    transaction.Commit();

                    return new OperationResult
                    {
                        Completed = true,
                        Message = "Médico creado exitosamente."
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception)
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "Ocurrió un error al crear el médico."
                };
            }
        }

        public OperationResult ActualizarMedico(MedicoCreacionDto medico)
        {
            if (medico is null)
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "La información del médico es inválida."
                };
            }

            try
            {
                using var connection = _dbContext.Connection();
                if (connection == null)
                {
                    return new OperationResult
                    {
                        Completed = false,
                        Message = "No se pudo obtener la conexión a la base de datos."
                    };
                }

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                using var transaction = connection.BeginTransaction();

                try
                {
                    using var command = new SqlCommand(_POST_UPDATE_MEDICO_QUERY, connection, transaction);
                    command.Parameters.AddWithValue("@Id", medico.Id);
                    command.Parameters.AddWithValue("@NombreCompleto", medico.Nombre);
                    command.Parameters.AddWithValue("@EspecialidadId", medico.EspecialidadId);

                    var rows = command.ExecuteNonQuery();

                    if (rows <= 0)
                    {
                        transaction.Rollback();
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "No se pudo actualizar el médico."
                        };
                    }

                    var rEliminar = EliminarHorariosMedico(medico.Id, connection, transaction);
                    if (!rEliminar.Completed)
                    {
                        transaction.Rollback();
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "No se pudieron actualizar los horarios del médico."
                        };
                    }

                    var rHorarios = GuardarHorariosMedico(medico.Id, medico.Horarios, connection, transaction);

                    if (!rHorarios.Completed)
                    {
                        transaction.Rollback();
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "Ocurrió un error al guardar los horarios."
                        };
                    }

                    transaction.Commit();

                    return new OperationResult
                    {
                        Completed = true,
                        Message = "Médico actualizado exitosamente."
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "Ocurrió un error al actualizar el médico."
                };
            }
        }

        private OperationResult EliminarHorariosMedico(int medicoId, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                using var command = new SqlCommand(_DELETE_HORARIO_MEDICO, connection, transaction);
                command.Parameters.AddWithValue("@MedicoId", medicoId);
                command.ExecuteNonQuery();

                return new OperationResult
                {
                    Completed = true,
                    Message = "Horarios eliminados correctamente."
                };
            }
            catch
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "Ocurrió un error al eliminar los horarios."
                };
            }
        }

        private OperationResult GuardarHorariosMedico(int medicoId, List<HorarioMedicoCreacionDto>? horarios, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                if (horarios is null || !horarios.Any())
                {
                    return new OperationResult
                    {
                        Completed = true,
                        Message = "Sin horarios para guardar."
                    };
                }

                const string query = @"INSERT INTO HorarioMedico (MedicoId, DiaSemana, HoraInicio, HoraFin) 
                    VALUES (@MedicoId, @DiaSemana, @HoraInicio, @HoraFin);";

                foreach (var horario in horarios)
                {
                    using var command = new SqlCommand(query, connection, transaction);
                    command.Parameters.AddWithValue("@MedicoId", medicoId);
                    command.Parameters.AddWithValue("@DiaSemana", horario.DiaSemana);
                    command.Parameters.AddWithValue("@HoraInicio", horario.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", horario.HoraFin);

                    var rows = command.ExecuteNonQuery();

                    if (rows <= 0)
                    {
                        return new OperationResult
                        {
                            Completed = false,
                            Message = "No se pudo guardar uno de los horarios."
                        };
                    }
                }

                return new OperationResult
                {
                    Completed = true,
                    Message = "Horarios guardados correctamente."
                };
            }
            catch
            {
                return new OperationResult
                {
                    Completed = false,
                    Message = "Ocurrió un error al guardar los horarios."
                };
            }
        }

        public List<HorarioMedicoCreacionDto> GetHorariosMedico(int medicoId)
        {
            var horarios = new List<HorarioMedicoCreacionDto>();

            var command = new SqlCommand(_GET_HORARIOS_MEDICO, _dbContext.Connection());
            command.Parameters.AddWithValue("@MedicoId", medicoId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                horarios.Add(new HorarioMedicoCreacionDto
                {
                    DiaSemana = reader.GetString(0),
                    HoraInicio = reader.GetTimeSpan(1),
                    HoraFin = reader.GetTimeSpan(2),
                    MedicoId = medicoId
                });
            }
            return horarios;
        }

        // Agenda del día para un médico específico. Todas las citas de un médico en un día, ordenadas por hora (Excepto canceladas, esto puede cambiar si se quiere).
        public List<AgendaItemDto> GetAgendaDiaria(int medicoId, DateTime fecha)
        {
            var agenda = new List<AgendaItemDto>();
            var command = new SqlCommand(_SP_AGENDA_MEDICO_BY_DATE, _dbContext.Connection());
            command.Parameters.AddWithValue("@MedicoId", medicoId);
            command.Parameters.AddWithValue("@Fecha", fecha.Date);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    agenda.Add(new AgendaItemDto
                    {
                        MedicoId = reader.GetInt32(0),
                        FechaAgenda = DateOnly.FromDateTime(reader.GetDateTime(1)),
                        DiaSemana = reader.GetString(2),
                        BloqueInicio = reader.GetTimeSpan(3).ToString(@"hh\:mm"),
                        BloqueFin = reader.GetTimeSpan(4).ToString(@"hh\:mm"),
                        EstadoBloque = reader.GetString(5),
                        IdCita = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                        PacienteId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                        NombrePaciente = reader.IsDBNull(8) ? null : reader.GetString(8),
                        EstadoCita = reader.IsDBNull(9) ? null : reader.GetString(9),
                        Motivo = reader.IsDBNull(10) ? null : reader.GetString(10)
                    });
                }
            }

            return agenda;
        }

        public OperationResult EliminarMedico(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                var command = new SqlCommand(_DELETE_MEDICO_QUERY, _dbContext.Connection());
                command.Parameters.AddWithValue("@Id", id);
                result.Completed = command.ExecuteNonQuery() > 0;
                return result;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                result.Completed = false;
                result.Message = "No se puede eliminar el médico porque tiene citas registradas.";
                return result;
            }
            catch (Exception)
            {
                result.Completed = false;
                result.Message = "Ocurrió un error al eliminar el médico.";
                return result;
            }
        }

        public bool VerificarHorarioDisponible(int medicoId, DateTime fecha, TimeSpan horarioInicio, TimeSpan horarioFin)
        {
            var command = new SqlCommand(_VERIFICAR_HORARIO_DISPONIBLE, _dbContext.Connection());
            command.Parameters.AddWithValue("@MedicoId", medicoId);
            command.Parameters.AddWithValue("@Fecha", fecha.Date);
            command.Parameters.AddWithValue("@HorarioInicio", horarioInicio);
            command.Parameters.AddWithValue("@HorarioFin", horarioFin);
            var result = command.ExecuteScalar();
            return result != null && Convert.ToInt32(result) > 0;
        }

        public List<HorarioMedicoCreacionDto> GetHorariosMedicoDisponiblesByDia(int medicoId, string diaSemana)
        {
            var horarios = new List<HorarioMedicoCreacionDto>();
            var command = new SqlCommand(_GET_HORARIOS_MEDICO_DISPONIBLES_BY_DIA, _dbContext.Connection());
            command.Parameters.AddWithValue("@MedicoId", medicoId);
            command.Parameters.AddWithValue("@DiaSemana", diaSemana);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                horarios.Add(new HorarioMedicoCreacionDto
                {
                    Id = reader.GetInt32(0),                    
                    HoraInicio = reader.GetTimeSpan(1),
                    HoraFin = reader.GetTimeSpan(2),
                    DiaSemana = diaSemana,
                    MedicoId = medicoId
                });
            }
            return horarios;
        }
    }
}
