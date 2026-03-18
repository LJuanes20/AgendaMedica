namespace AMAPI.Models
{
    public class CitaDto
    {
        public int Id { get; set; }
        public MedicoDto Medico { get; set; }
        public PacienteDto Paciente { get; set; }

        public string Estado { get; set; }

        public DateTime InicioCita { get; set; }

        public DateTime FinCita { get; set; }

        public string Motivo { get; set; }

        public string? MotivoCancelacion { get; set; }
    }

    public class CitaCreateDto
    {
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public string Estado { get; set; } = "Agendada";
        public DateTime InicioCita { get; set; }
        public DateTime FinCita { get; set; }
        public string Motivo { get; set; }
    }

}
