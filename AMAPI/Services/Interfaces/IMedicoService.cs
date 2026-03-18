using AMAPI.Models;

namespace AMAPI.Services.Interfaces
{
    public interface IMedicoService
    {
        List<MedicoDto> GetAllMedicos();
        MedicoDto? GetMedicoById(int id);
        List<MedicoDto> GetMedicosByEspecialidad(int especialidadId);
        OperationResult CrearMedico(MedicoCreacionDto medico);
        OperationResult ActualizarMedico(MedicoCreacionDto medico);
        OperationResult EliminarMedico(int id);
        List<HorarioMedicoCreacionDto> GetHorariosMedico(int medicoId);
        List<AgendaItemDto> GetAgendaDiaria(int medicoId, DateTime fecha);
        bool VerificarHorarioDisponible(int medicoId, DateTime fecha, TimeSpan horarioInicio, TimeSpan horarioFin);
        List<HorarioMedicoCreacionDto> GetHorariosMedicoDisponiblesByDia(int medicoId, string diaSemana);
    }
}
