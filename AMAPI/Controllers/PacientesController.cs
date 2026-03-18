using AMAPI.Models;
using AMAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly PacientesService _pacientesService;

        public PacientesController(PacientesService pacientesService)
        {
            _pacientesService = pacientesService;
        }

        [HttpGet()]
        public ActionResult<List<PacienteDto>> GetPacientes()
        {
            var pacientes = _pacientesService.GetAllPacientes();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public ActionResult<PacienteDto> GetPacienteById(int id)
        {
            var paciente = _pacientesService.GetPacienteById(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return Ok(paciente);
        }

        [HttpPost("crear")]
        public ActionResult<OperationResult> CrearPaciente(PacienteDto paciente)
        {
            var resultado = _pacientesService.CrearPaciente(paciente);
            if (resultado.Completed)
            {
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Paciente creado correctamente."
                });
            }
            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = resultado.Message
            });
        }

        [HttpPut("actualizar")]
        public ActionResult<OperationResult> ActualizarPaciente(PacienteDto paciente)
        {
            var resultado = _pacientesService.ActualizarDatosPaciente(paciente);

            if (resultado.Completed)
            {
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Paciente modificado correctamente."
                });
            }

            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = resultado.Message
            });
        }


        [HttpDelete("eliminar/{id}")]
        public ActionResult<OperationResult> EliminarPaciente(int id)
        {
            var resultado = _pacientesService.EliminarPaciente(id);

            if (resultado.Completed)
            {
                return Ok(new OperationResult
                {
                    Completed = true,
                    Message = "Paciente eliminado correctamente."
                });
            }

            return BadRequest(new OperationResult
            {
                Completed = false,
                Message = resultado.Message
            });
        }
    }
}
