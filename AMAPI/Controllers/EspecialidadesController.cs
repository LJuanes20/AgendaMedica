using AMAPI.Models;
using AMAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly EspecialidadService _especialidadService;

        public EspecialidadesController(EspecialidadService especialidadService)
        {
            _especialidadService = especialidadService;
        }

        [HttpGet("all")]
        public List<EspecialidadDto> GetEspecialidades()
        {
            return _especialidadService.GetAllEspecialidades();
        }
    }
}
