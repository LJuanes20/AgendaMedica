namespace AMAPI.Models
{
    public class AgendaItemDto
    {
        public int MedicoId { get; set; }

        public DateOnly FechaAgenda { get; set; }

        public string DiaSemana { get; set; }
        public string BloqueInicio { get; set; }
        public string BloqueFin { get; set; }
        public string EstadoBloque { get; set; }
        public int? IdCita { get; set; }
        public int? PacienteId { get; set; }
        public string? NombrePaciente { get; set; }
        public string? EstadoCita { get; set; }
        public string? Motivo { get; set; }

    }
}
