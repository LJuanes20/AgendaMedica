using AMShared.Models;
using System.Text.Json;

namespace AMUI.Services
{
    public class MedicosService
    {
        private readonly HttpClient _httpClient;

        public MedicosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MedicoDto>> GetMedicosAsync()
        {
            var response = await _httpClient.GetAsync("Medico");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MedicoDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MedicoDto>();
        }

        public async Task<MedicoDto> GetMedicoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Medico/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MedicoDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new MedicoDto();
        }

        public async Task<List<MedicoDto>> GetMedicosByEspecialidadAsync(int especialidadId)
        {
            var response = await _httpClient.GetAsync($"Medico/especialidad/{especialidadId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MedicoDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MedicoDto>();
        }

        public async Task<OperationResult> CreateMedicoAsync(MedicoCreacionDto medico)
        {
            var content = new StringContent(JsonSerializer.Serialize(medico), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"Medico/crear", content);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al crear el médico." };
        }

        public async Task<OperationResult> DeleteMedicoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Medico/eliminar/{id}");
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al eliminar el médico." };
        }
        
        public async Task<OperationResult> UpdateMedicoAsync(MedicoCreacionDto medico)
        {
            var content = new StringContent(JsonSerializer.Serialize(medico), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Medico/actualizar/{medico.Id}", content);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            return result ?? new OperationResult { Completed = false, Message = "Ocurrió un error al actualizar el médico." };
        } 

        public async Task<List<EspecialidadDto>> GetEspecialidadesAsync()
        {
            var response = await _httpClient.GetAsync("Medico/especialidades");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<EspecialidadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<EspecialidadDto>();
        }

        public async Task<List<HorarioMedicoCreacionDto>> GetHorarioMedicodesAsync(int medicoId)
        {
            var response = await _httpClient.GetAsync($"Medico/horarios/{medicoId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<HorarioMedicoCreacionDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<HorarioMedicoCreacionDto>();
        }

        public async Task<List<AgendaItemDto>> GetAgendaMedicoByDate(int medicoId, DateTime date)
        {
            var response = await _httpClient.GetAsync($"Medico/agenda/{medicoId}/{date:yyyy-MM-dd}"); 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AgendaItemDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AgendaItemDto>();
        }

        public async Task<bool> GetDisponibilidadHorarioMedico(int medicoId, DateOnly fecha, TimeSpan horarioInicio, TimeSpan horarioFin)
        {
            var fechaStr = fecha.ToString("yyyy-MM-dd");
            var horarioInicioStr = horarioInicio.ToString(@"hh\:mm");
            var horarioFinStr = horarioFin.ToString(@"hh\:mm");
            var response = await _httpClient.GetAsync($"Medico/verificarhorario/{medicoId}/{fechaStr}/{horarioInicioStr}/{horarioFinStr}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); 
        }

        public async Task<List<HorarioMedicoCreacionDto>> GetHorarioMedicoByIdAsync(int medicoId, string diaSemana)
        {
            var response = await _httpClient.GetAsync($"Medico/horariosdia/{medicoId}/{diaSemana}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<HorarioMedicoCreacionDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<HorarioMedicoCreacionDto>();
        }
    }
}
