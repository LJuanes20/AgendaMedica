namespace AMUI.Models
{
    public class HorarioMedicoDto
    {
        public int Id { get; set; }

        public string DiaSemana { get; set; }

        public MedicoDto Medico { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }
    }

    public class HorarioMedicoCreacionDto
    {
        public int Id { get; set; }

        public string DiaSemana { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public int MedicoId { get; set; }
    }

}
