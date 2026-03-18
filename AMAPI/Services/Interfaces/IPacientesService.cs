using AMAPI.Models;

namespace AMAPI.Services.Interfaces
{
    public interface IPacientesService
    {
        List<PacienteDto> GetAllPacientes();
        PacienteDto? GetPacienteById(int id);
        OperationResult CrearPaciente(PacienteDto paciente);
        OperationResult EliminarPaciente(int id);
        OperationResult ActualizarDatosPaciente(PacienteDto paciente);
    }
}
