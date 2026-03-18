using AMUI.Models;
using System.Text.Json;

namespace AMUI.Services
{
    public class EspecialidadesService
    {
        private readonly HttpClient _httpClient;

        public EspecialidadesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EspecialidadDto>> GetEspecialidadesAsync()
        {
            var response = await _httpClient.GetAsync("Especialidades/all");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<EspecialidadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<EspecialidadDto>();
        }
    }
}
