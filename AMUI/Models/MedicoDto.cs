namespace AMUI.Models
{
    public class MedicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public EspecialidadDto Especialidad { get; set; }
    }
}
