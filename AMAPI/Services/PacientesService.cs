using AMAPI.Infrastructure;
using AMAPI.Models;
using AMAPI.Services.Interfaces;
using System.Data.SqlClient;

namespace AMAPI.Services
{
    public class PacientesService : IPacientesService
    {
        private readonly DbContext _dbContext;
        private readonly string _GET_PACIENTES_QUERY = "SELECT IdPaciente, NombreCompleto, FechaNacimiento, Telefono, Correo FROM [Paciente]";
        private readonly string _INSERT_PACIENTE_QUERY = "INSERT INTO [Paciente] (NombreCompleto, FechaNacimiento, Telefono, Correo) VALUES (@NombreCompleto, @FechaNacimiento, @Telefono, @Correo)";
        private readonly string _DELETE_PACIENTE_QUERY = "DELETE FROM [Paciente] WHERE IdPaciente = @IdPaciente";
        private readonly string _UPDATE_PACIENTE_QUERY = "UPDATE [Paciente] SET NombreCompleto = @NombreCompleto, FechaNacimiento = @FechaNacimiento, Telefono = @Telefono, Correo = @Correo WHERE IdPaciente = @IdPaciente";

        public PacientesService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PacienteDto> GetAllPacientes()
        {
            var command = new SqlCommand(_GET_PACIENTES_QUERY, _dbContext.Connection());
            var reader = command.ExecuteReader();
            var pacientes = new List<PacienteDto>();
            while (reader.Read())
            {
                pacientes.Add(new PacienteDto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    FechaNacimiento = reader.GetDateTime(2),
                    Telefono = reader.GetString(3),
                    Correo = reader.GetString(4)
                });
            }
            return pacientes;
        }

        public PacienteDto? GetPacienteById(int id)
        {
            var command = new SqlCommand($"{_GET_PACIENTES_QUERY} WHERE IdPaciente = @IdPaciente", _dbContext.Connection());
            command.Parameters.AddWithValue("@IdPaciente", id);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new PacienteDto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    FechaNacimiento = reader.GetDateTime(2),
                    Telefono = reader.GetString(3),
                    Correo = reader.GetString(4)
                };
            }
            return null;
        }

        public OperationResult CrearPaciente(PacienteDto paciente)
        {
            OperationResult result = new OperationResult();
            try
            {
                var command = new SqlCommand(_INSERT_PACIENTE_QUERY, _dbContext.Connection());
                command.Parameters.AddWithValue("@NombreCompleto", paciente.Nombre);
                command.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                command.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                command.Parameters.AddWithValue("@Correo", paciente.Correo);
                result.Completed = command.ExecuteNonQuery() > 0;
                return result;
            }
            catch (Exception)
            {
                result.Completed = false;
                result.Message = "Ocurrió un error al crear el paciente.";
                return result;
            }
           
        }

        public OperationResult EliminarPaciente(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                var command = new SqlCommand(_DELETE_PACIENTE_QUERY, _dbContext.Connection());
                command.Parameters.AddWithValue("@IdPaciente", id);
                result.Completed = command.ExecuteNonQuery() > 0;
                return result;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                result.Completed = false;
                result.Message = "No se puede eliminar el paciente porque tiene citas registradas.";
                return result;
            }
            catch (Exception ex)
            {
                result.Completed = false;
                result.Message = "Ocurrió un error al eliminar el paciente.";
                return result;
            }
        }

        public OperationResult ActualizarDatosPaciente(PacienteDto paciente)
        {
            OperationResult result = new OperationResult();
            try
            {
                var command = new SqlCommand(_UPDATE_PACIENTE_QUERY, _dbContext.Connection());
                command.Parameters.AddWithValue("@IdPaciente", paciente.Id);
                command.Parameters.AddWithValue("@NombreCompleto", paciente.Nombre);
                command.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                command.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                command.Parameters.AddWithValue("@Correo", paciente.Correo);
                result.Completed = command.ExecuteNonQuery() > 0;
                return result;
            }
            catch (Exception ex)
            {
                result.Completed = false;
                result.Message = "Ocurrió un error al actualizar los datos del paciente.";
                return result;
            }
          
            
        }
    }
}
