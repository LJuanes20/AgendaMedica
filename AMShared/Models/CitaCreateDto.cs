namespace AMShared.Models
{
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
