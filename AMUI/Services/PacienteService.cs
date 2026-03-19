using AMShared.Models;
using System.Text.Json;

namespace AMUI.Services
{
    public class PacienteService
    {
        private readonly HttpClient _httpClient;

        public PacienteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PacienteDto>> GetPacientesAsync()
        {
            var response = await _httpClient.GetAsync("Pacientes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PacienteDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PacienteDto>();
        }

        public async Task<PacienteDto> GetPacienteByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Pacientes/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PacienteDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PacienteDto();
        }

        public async Task<OperationResult> CreatePacienteAsync(PacienteDto paciente)
        {
            var content = new StringContent(JsonSerializer.Serialize(paciente), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"Pacientes/crear", content);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al crear el paciente." };
        }

        public async Task<OperationResult> UpdatePacienteAsync(PacienteDto paciente)
        {
            var content = new StringContent(JsonSerializer.Serialize(paciente), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Pacientes/actualizar", content);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al actualizar el paciente." };
        }

        public async Task<OperationResult> DeletePacienteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Pacientes/eliminar/{id}");            
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al eliminar el paciente." };
        }
    }
}
