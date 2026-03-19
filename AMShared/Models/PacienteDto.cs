namespace AMShared.Models
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public string Telefono { get; set; }
        public string Correo { get; set; }
    }

}
