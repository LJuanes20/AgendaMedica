using AMUI.Models;
using System.Text.Json;

namespace AMUI.Services
{
    public class CitasService
    {
        private readonly HttpClient _httpClient;

        public CitasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AgendarCita(CitaCreateDto cita)
        {
            var content = new StringContent(JsonSerializer.Serialize(cita), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Citas/agendar", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CitaDto>> GetCitasByPacienteIdAsync(int pacienteId)
        {
            var response = await _httpClient.GetAsync($"Citas/paciente/{pacienteId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CitaDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CitaDto>();
        }

        public async Task<List<CitaDto>> GetAgendaMedico(int medicoId)
        {
            var response = await _httpClient.GetAsync($"Citas/medico/{medicoId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CitaDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CitaDto>();
        } 

        public async Task<List<CitaDto>> GetAllCitas()
        {
            var response = await _httpClient.GetAsync("Citas");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CitaDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CitaDto>();
        }

        public async Task<bool> CancelarCita(CitaDto citaDto)
        {
            var content = new StringContent(JsonSerializer.Serialize(citaDto.MotivoCancelacion), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"Citas/cancelar/{citaDto.Id}", content);
            return response.IsSuccessStatusCode;
        } 

        public async Task<bool> ExisteCita(int medicoId, int pacienteId, DateTime inicioCita, DateTime finCita)
        {
            var response = await _httpClient.GetAsync($"Citas/existe?medicoId={medicoId}&pacienteId={pacienteId}&inicioCita={inicioCita:o}&finCita={finCita:o}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }   

        public async Task<int> GetNumeroCancelaciones(int pacienteId)
        {
            var response = await _httpClient.GetAsync($"Citas/numerocancelaciones/{pacienteId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
