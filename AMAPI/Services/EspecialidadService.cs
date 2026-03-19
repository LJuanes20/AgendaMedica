using AMAPI.Infrastructure;
using AMShared.Models;
using AMAPI.Services.Interfaces;
using System.Data.SqlClient;

namespace AMAPI.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly DbContext _dbContext;

        private readonly string _GET_ESPECIALIDADES_QUERY = "SELECT IdEspecialidad, Nombre, Duracion FROM [Especialidad]";

        public EspecialidadService(DbContext context)
        {
            _dbContext = context;
        }

        public List<EspecialidadDto> GetAllEspecialidades()
        {
            var command = new SqlCommand(_GET_ESPECIALIDADES_QUERY, _dbContext.Connection());
            var reader = command.ExecuteReader();
            var especialidades = new List<EspecialidadDto>();
            while (reader.Read())
            {
                especialidades.Add(new EspecialidadDto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    DuracionMinutos = reader.GetInt32(2)
                });
            }
            return especialidades;
        }
    }
}
