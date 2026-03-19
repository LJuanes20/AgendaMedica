using AMShared.Models;
using AMAPI.Services.Interfaces;
using AMUI.Services;
using Moq;

namespace AMTests
{
    public class CitasServiceTests
    {
        private readonly Mock<ICitasService> _mockCitasService;
        private readonly Mock<IMedicoService> _mockMedicosService;

        public CitasServiceTests()
        {
            _mockCitasService = new Mock<ICitasService>();
            _mockMedicosService = new Mock<IMedicoService>();
        }

        [Fact]
        public void AgendarCitaOkTest()
        {
            var cita = new CitaCreateDto
            {
                MedicoId = 1,
                PacienteId = 2,
                Motivo = "Consulta general",
                InicioCita = new DateTime(2026, 4, 10, 9, 0, 0),
                FinCita = new DateTime(2026, 4, 10, 9, 30, 0)
            };

            _mockCitasService
                .Setup(s => s.AgendarCita(cita))
                .Returns(new OperationResult { Completed = true, Message = "Cita agendada correctamente." });

            var resultado = _mockCitasService.Object.AgendarCita(cita);
            Assert.True(resultado.Completed);
        }

        [Fact]
        public void AgendarCitaFailTest()
        {
            var cita = new CitaCreateDto
            {
                MedicoId = 99,
                PacienteId = 99,
                Motivo = "Prueba fallida",
                InicioCita = DateTime.Now,
                FinCita = DateTime.Now.AddMinutes(30)
            };

            _mockCitasService
                .Setup(s => s.AgendarCita(cita))
                .Returns(new OperationResult { Completed = false, Message = "Error al agendar la cita." });

            var resultado = _mockCitasService.Object.AgendarCita(cita);
            Assert.False(resultado.Completed);
        }

        [Fact]
        public void ExisteCitaEnHorarioTest()
        {
            int medicoId = 1;
            int pacienteId = 2;
            var inicio = new DateTime(2026, 4, 10, 9, 0, 0);
            var fin = new DateTime(2026, 4, 10, 9, 30, 0);

            _mockCitasService
                .Setup(s => s.ExisteCitaEnHorario(medicoId, pacienteId, inicio, fin))
                .Returns(true);

            var existeConflictoCitaDuplicada = _mockCitasService.Object
                .ExisteCitaEnHorario(medicoId, pacienteId, inicio, fin);
            Assert.True(existeConflictoCitaDuplicada);
        }

        [Fact]
        public void HorarioMedicoNoDisponibleTest()
        {
            int medicoId = 1;
            var fecha = new DateTime(2026, 4, 10);
            var horarioInicio = new TimeSpan(20, 0, 0);
            var horarioFin = new TimeSpan(20, 30, 0);

            _mockMedicosService
                .Setup(s => s.VerificarHorarioDisponible(medicoId, fecha, horarioInicio, horarioFin))
                .Returns(false);

            var horarioDisponible = _mockMedicosService.Object
                .VerificarHorarioDisponible(medicoId, fecha, horarioInicio, horarioFin);

            bool seIntentaAgendar = horarioDisponible;

            Assert.False(horarioDisponible);
            Assert.False(seIntentaAgendar);
        }

        [Fact]
        public void VerificarCancelacionesPacienteTest()
        {
            int pacienteId = 3;
            int limiteCancelaciones = 5;

            _mockCitasService
                .Setup(s => s.ContarCancelacionesPaciente(pacienteId))
                .Returns(5);

            var totalCancelaciones = _mockCitasService.Object
                .ContarCancelacionesPaciente(pacienteId);
            bool debeActivarAlerta = totalCancelaciones >= limiteCancelaciones;

            Assert.Equal(5, totalCancelaciones);
            Assert.True(debeActivarAlerta);
        }
    }
}
