using AMShared.Models;

namespace AMAPI.Services.Interfaces
{
    public interface ICitasService
    {
        OperationResult AgendarCita(CitaCreateDto cita);
        bool CancelarCita(int idCita, string motivoCancelacion = "Cancelada por el paciente sin especificar motivo");
        List<CitaDto> ConsultarCitas();
        List<CitaDto> ConsultarCitasPorPaciente(int pacienteId);
        List<DateTime> ConsultarProximosHorariosDisponibles(int medicoId, DateTime fecha);
        bool ExisteCitaEnHorario(int medicoId, int pacienteId, DateTime inicioCita, DateTime finCita);
        int ContarCancelacionesPaciente(int pacienteId);
    }
}
