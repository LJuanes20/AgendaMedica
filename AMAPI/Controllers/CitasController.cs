using AMShared.Models;
using AMAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitasService _citasService;
        public CitasController(ICitasService citasService)
        {
            _citasService = citasService;
        }

        [HttpGet]
        public ActionResult<List<CitaDto>> GetCitas()
        {
            var citas = _citasService.ConsultarCitas();

            citas = citas
                .OrderByDescending(c => c.InicioCita)
                .ToList();

            return Ok(citas);
        }

        [HttpPost("agendar")]
        public ActionResult<OperationResult> AgendarCita(CitaCreateDto cita)
        {
            var resultado = _citasService.AgendarCita(cita);
            if (resultado.Completed)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }

        [HttpPost("cancelar/{id}")]
        public ActionResult<bool> CancelarCita(int id, [FromBody] string motivoCancelacion)
        {
            var resultado = _citasService.CancelarCita(id, motivoCancelacion);
            if (resultado)
            {
                return Ok(true);
            }

            return BadRequest(false);
        }

        [HttpGet("paciente/{id}")]
        public ActionResult<List<CitaDto>> GetCitasPorPaciente(int id)
        {
            var citas = _citasService.ConsultarCitasPorPaciente(id);
            return Ok(citas);
        }

        [HttpGet("existe")]
        public ActionResult<bool> GetExisteCita(int medicoId, int pacienteId, DateTime inicioCita, DateTime finCita)
        {
            var existeCita = _citasService.ExisteCitaEnHorario(medicoId, pacienteId, inicioCita, finCita);
            return Ok(existeCita);
        }

        [HttpGet("numerocancelaciones/{pacienteId}")]
        public ActionResult<int> GetNumeroCancelaciones(int pacienteId)
        {
            var cancelaciones = _citasService.ContarCancelacionesPaciente(pacienteId);
            return Ok(cancelaciones);
        }
    }
}
