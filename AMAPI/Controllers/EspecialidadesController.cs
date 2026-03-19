using AMShared.Models;
using AMAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadService _especialidadService;

        public EspecialidadesController(IEspecialidadService especialidadService)
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
