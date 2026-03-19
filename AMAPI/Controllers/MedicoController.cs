using AMShared.Models;
using AMAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicoController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        public IActionResult GetAllMedicos()
        {
            var medicos = _medicoService.GetAllMedicos();
            return Ok(medicos);
        }

        [HttpGet("{id}")]
        public IActionResult GetMedicoById(int id)
        {
            var medico = _medicoService.GetMedicoById(id);
            if (medico == null)
            {
                return NotFound();
            }
            return Ok(medico);
        }

        [HttpGet("especialidad/{especialidadId}")]
        public IActionResult GetMedicosByEspecialidad(int especialidadId)
        {
            var medicos = _medicoService.GetMedicosByEspecialidad(especialidadId);
            return Ok(medicos);
        }

        [HttpPost("crear")]
        public ActionResult<OperationResult> CrearMedico(MedicoCreacionDto medico)
        {
            var result = _medicoService.CrearMedico(medico);
            if (result.Completed)
            {
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Médico creado correctamente."
                });
            }
            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = result.Message
            });
        }

        [HttpPut("actualizar/{id}")]
        public ActionResult<OperationResult> ActualizarMedico(int id, MedicoCreacionDto medico)
        {
            var result = _medicoService.ActualizarMedico(medico);
            if (result.Completed)
            {
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Médico actualizado correctamente."
                });
            }
            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = result.Message
            });
        }

        [HttpGet("horarios/{medicoId}")]
        public ActionResult<List<HorarioMedicoCreacionDto>> GetHorariosMedico(int medicoId)
        {
            var horarios = _medicoService.GetHorariosMedico(medicoId);
            return Ok(horarios);
        }

        [HttpDelete("eliminar/{id}")]
        public ActionResult<OperationResult> EliminarMedico(int id)
        {
            var result = _medicoService.EliminarMedico(id);
            if (result.Completed)
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Médico eliminado correctamente."
                });
            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = result.Message
            });
        }

        [HttpGet("agenda/{medicoId}/{fecha}")]
        public ActionResult<List<AgendaItemDto>> GetAgendaByMedicoIdAndFecha(int medicoId, DateTime fecha)
        {
            var agenda = _medicoService.GetAgendaDiaria(medicoId, fecha);
            return Ok(agenda);
        }

        [HttpGet("verificarhorario/{medicoId}/{fecha}/{horarioInicio}/{horarioFin}")]
        public ActionResult<bool> VerificarHorarioDisponible(int medicoId, DateTime fecha, TimeSpan horarioInicio, TimeSpan horarioFin)
        {
            var disponible = _medicoService.VerificarHorarioDisponible(medicoId, fecha, horarioInicio, horarioFin);
            return Ok(disponible);
        }

        [HttpGet("horariosdia/{medicoId}/{diaSemana}")]
        public ActionResult<List<HorarioMedicoCreacionDto>> GetHorariosMedicoDisponiblesByDia(int medicoId, string diaSemana)
        {
            var horarios = _medicoService.GetHorariosMedicoDisponiblesByDia(medicoId, diaSemana);
            return Ok(horarios);
        }
    }
}