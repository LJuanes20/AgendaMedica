namespace AMShared.Models
{
    public class MedicoCreacionDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int EspecialidadId { get; set; }

        public List<HorarioMedicoCreacionDto> Horarios { get; set; } = new List<HorarioMedicoCreacionDto>();
    }

}
