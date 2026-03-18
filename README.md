# Agenda Médica

**Prueba Técnica — Desarrollador C# .NET**

Sistema web para la gestión de citas médicas, desarrollado con **.NET 8**, **Blazor Server** y **SQL Server**.  
Permite administrar médicos, pacientes y citas de forma centralizada desde una sola interfaz.

---

## Estructura de la Solución

La solución `AgendaMedica.slnx` contiene tres proyectos dentro del mismo repositorio:

```
AgendaMedica/
├── AMAPI/          → ASP.NET Core Web API (backend REST)
├── AMUI/           → Blazor Server (frontend)
├── AMTests/        → Proyecto de pruebas unitarias (xUnit + Moq)
└── scripts/        → Scripts SQL para crear y poblar la base de datos
```

Los proyectos AMAPI y AMUI son contracciones de "Agenda Médica API" y "Agenda Médica UI", respectivamente.

> **Decisión de diseño:** Se optó por mantener los tres proyectos en la misma solución para simplificar el flujo de desarrollo, facilitar la navegación entre capas y evitar configuraciones adicionales de repositorios o referencias entre soluciones separadas.

---

## Instrucciones para ejecutar el proyecto

### 1. Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local o remoto)
- Visual Studio 2022+ (recomendado) o VS Code

---

### 2. Configuración del `appsettings`

#### `AMAPI/appsettings.json` — Cadena de conexión

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=agendamedica;TrustServerCertificate=True;Trusted_Connection=True;"
  }
}
```

> Modifica `Server=(local)` si tu instancia de SQL Server tiene un nombre diferente (ej. `Server=.\\SQLEXPRESS`).
> De igual forma si de decide cambiar el nombre de la base de datos, actualizar `Database=agendamedica` por el nuevo nombre.
---

#### `AMUI/appsettings.json` — URL base de la API

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7206/api/"
  }
}
```

> Asegúrate de colocar el puerto correcto que corresponde al que usa la API al ejecutarse. Puedes verificarlo en `AMAPI/Properties/launchSettings.json`.

---

### 3. Configurar Multiple Startup Projects

Para lanzar la API y la UI simultáneamente desde Visual Studio:

1. Clic derecho sobre la **solución** → **"Set Startup Projects..."**
2. Seleccionar **"Multiple startup projects"**
3. Asignar la acción **"Start"** a:
   - `AMAPI`
   - `AMUI`
4. Clic en **Aceptar**
5. Presionar **F5** para iniciar ambos proyectos

---

## Base de Datos

### Ejecución de scripts

Ejecutar los scripts en orden desde SQL Server Management Studio (SSMS) u otra herramienta compatible:

| # | Archivo                 | Descripción                              |
|---|-------------------------|------------------------------------------|
| 1 | `01_CrearBD.sql`        | Crea la base de datos `agendamedica`     |
| 2 | `02_Tablas.sql`         | Crea todas las tablas y llaves foráneas  |
| 3 | `03_Procedimientos.sql` | Crea los stored procedures               |
| 4 | `04_DatosPrueba.sql`    | Inserta datos de prueba (limpia primero) |
|---|-------------------------|------------------------------------------|

> Nota: El script `04_DatosPrueba.sql` **elimina y reinicia** todos los datos existentes antes de insertar.

---

### Estructura de la base de datos

```
Especialidad
────────────────────────────────
PK  IdEspecialidad  int
    Nombre          varchar(50)
    Duracion        int ← duración en minutos de la consulta

Medico
────────────────────────────────
PK  IdMedico        int
    NombreCompleto  varchar(150)
FK  EspecialidadId  int  → Especialidad.IdEspecialidad

HorarioMedico
────────────────────────────────
PK  IdHorario       int
FK  MedicoId        int  → Medico.IdMedico
    DiaSemana       varchar(15)    ← "Lunes", "Martes", etc.
    HoraInicio      time(0)
    HoraFin         time(0)

Paciente
────────────────────────────────
PK  IdPaciente      int
    NombreCompleto  varchar(150)
    FechaNacimiento date
    Telefono        varchar(10)
    Correo          varchar(100)

Cita
────────────────────────────────
PK  IdCita              int
FK  MedicoId            int  → Medico.IdMedico
FK  PacienteId          int  → Paciente.IdPaciente
    Estado              varchar(15)     ← "Programada" | "Cancelada"
    Motivo              varchar(150)
    MotivoCancelacion   varchar(150)    ← nullable
    InicioCita          datetime
    FinCita             datetime
```

**Relaciones:**

```
Especialidad ──< Medico ──< HorarioMedico
                Medico  ──< Cita >── Paciente
```

---

### Stored Procedures

#### `sp_HorarioDisponibleAgendaMedico`
Valida si un médico tiene disponibilidad real en su horario registrado para una fecha y rango horario específico.  
**Parámetros:** `@MedicoId`, `@Fecha`, `@HorarioInicio`, `@HorarioFin`  
**Retorna:** `Disponible = 1` (disponible) o `0` (no disponible)

#### `sp_ObtenerAgendaMedicoPorFecha`
Obtiene la agenda completa de un médico para un día específico, mostrando cada bloque horario con su estado (`Libre` / `Ocupado`) e información del paciente si existe cita.  
**Parámetros:** `@MedicoId`, `@Fecha`  
**Retorna:** Lista de bloques con datos de paciente, estado de cita y motivo.

---

## Decisiones de Diseño y Arquitectura

### Organización del backend (AMAPI)

El proyecto API sigue una arquitectura en capas con separación clara de responsabilidades:

```
AMAPI/
├── Controllers/     → Endpoints REST. Reciben la solicitud, delegan al servicio y retornan la respuesta HTTP.
├── Services/        → Lógica de negocio. Contiene las reglas de validación, consultas y operaciones.
│   └── Interfaces/  → Contratos de los servicios. Permiten desacoplar implementaciones y facilitar pruebas con mocks.
├── Models/          → DTOs y entidades usadas para comunicación entre capas.
└── Infrastructure/  → Configuración de infraestructura, como el DbContext (conexión a BD con Dapper).
```

Esta separación tiene ventajas concretas:

- **Mantenibilidad:** cada carpeta tiene una única responsabilidad; cambiar la lógica de negocio no toca los controladores.
- **Testeabilidad:** al depender de interfaces, los servicios pueden mockearse en pruebas unitarias sin tocar la base de datos.
- **Escalabilidad:** agregar un nuevo módulo (ej. Expedientes) solo requiere añadir su controller, service e interface, sin modificar lo existente.
- **Legibilidad:** cualquier desarrollador nuevo puede orientarse rápidamente por la estructura de carpetas.

### Frontend (AMUI — Blazor Server)

El frontend consume la API REST mediante servicios HTTP inyectados. Se optó por **Blazor Server** como framework UI por el conocimiento previo del desarrollador con el ecosistema .NET, lo que permitió mayor agilidad en el desarrollo sin necesidad de aprender un framework JavaScript adicional.

### Módulos implementados

Se definieron **tres módulos principales** para mantener el alcance del sistema como una agenda sencilla y funcional:

- **Médicos** — CRUD con asignación de especialidad y horarios semanales. Incluye accesos directos a agenda e historial desde la tabla.
- **Pacientes** — CRUD con datos de contacto. Incluye accesos directos a agenda e historial desde la tabla.
- **Citas** — Agendar, cancelar y consultar citas con validación de disponibilidad.

> La decisión de concentrar las funcionalidades de agenda e historial directamente en las tablas de médicos y pacientes (mediante botones de acción) permite centralizar el flujo de trabajo sin necesidad de navegar por menús adicionales.

---

## Notas del Desarrollador

- **Framework UI:** Se eligió Blazor Server por el dominio previo del framework dentro del ecosistema .NET, priorizando la velocidad de desarrollo sobre la exploración de nuevas tecnologías en el frontend.

- **Horarios médicos:** Se implementó la tabla `HorarioMedico` para registrar por día de la semana los bloques en que el médico trabaja. Esto simplifica significativamente la consulta de disponibilidad: al agendar una cita, basta con cruzar la fecha solicitada contra los bloques registrados del médico para ese día de la semana.

- **Alertas y validaciones:** Se usaron alertas nativas de JavaScript para avisos y validaciones del lado del usuario. Se reconoce que esto puede mejorarse con modales personalizados y validaciones declarativas con el atributo `[Required]` en los formularios, pero se dejó pendiente por restricciones de tiempo.

- **Swagger:** Se habilitó Swagger en la API para facilitar la prueba y documentación de endpoints durante el desarrollo, antes de integrarlos en el frontend.

- **Sistema de resultados (`OptionResult`):** Se diseñó un mecanismo de respuesta unificado (`OptionResult`) que tenía como objetivo comunicar al usuario el tipo exacto de error ocurrido (duplicidad de cita, horario no disponible, exceso de cancelaciones, etc.). Por restricciones de tiempo, este sistema quedó implementado de forma parcial.

- **Stored Procedures:** Se implementaron los dos procedures sugeridos (`sp_HorarioDisponibleAgendaMedico` y `sp_ObtenerAgendaMedicoPorFecha`). Durante el desarrollo se identificó que otras operaciones del sistema (alertas, historial, reportes) también se beneficiarían de procedures dedicados, quedando como mejora futura.

- **Errores HTTP:** Se tenía previsto un manejo formal de códigos de respuesta HTTP apropiados: `400` para datos inválidos, `404` para recursos no encontrados y `409` para conflictos de horario, con mensajes descriptivos en español. Por falta de tiempo no se terminó de implementar de manera consistente en todos los endpoints.

- **Validaciones al agendar:** La disponibilidad y detección de duplicidad se verifican mediante consultas directas en el flujo de agendar. Esto podría centralizarse en un módulo de validaciones dedicado o en un único stored procedure que agrupe todas las reglas de negocio de una cita.

- **Bootstrap en botones:** Se identificó un problema visual con algunos botones que aparecen con opacidad reducida (opacos) debido a clases CSS de Bootstrap. Queda pendiente su corrección.

---

## Pruebas Unitarias (`AMTests`)

Se implementaron **cinco pruebas unitarias** con **Moq** para simular los servicios sin afectar la base de datos real.

AgendarCitaOkTest: Verifica que agendar una cita con datos válidos retorna `true` 
AgendarCitaFailTest: Verifica que agendar con datos inválidos (médico/paciente inexistente) retorna `false` 
ExisteCitaEnHorarioTest: Verifica que se detecta correctamente un conflicto de horario duplicado 
HorarioMedicoNoDisponibleTest: Verifica que un horario fuera del rango del médico retorna no disponible 
VerificarCancelacionesPacienteTest: Verifica que se activa la alerta al alcanzar el límite de cancelaciones del paciente 
> Las pruebas usan **mocks simples** sobre las interfaces de servicio (`ICitasService`, `IMedicoService`), siguiendo el principio de inyección de dependencias para aislar la lógica sin depender de infraestructura externa.

**Pendiente:** Pruebas unitarias para CRUD de médicos y pacientes.

---

## Pendientes y propuestas de mejora

**Sugerencia de horarios disponibles** — Si el horario solicitado no está libre, mostrar los próximos 5 slots disponibles del médico Consultar los bloques libres de `HorarioMedico` para esa semana, cruzar contra `Cita` para descartar ocupados, y retornar los primeros 5 resultados ordenados por fecha/hora
**Autenticación y seguridad** JWT en la API + login en Blazor. Roles: Recepcionista, Médico, Admin 
**Manejo formal de errores HTTP** Middleware global de excepciones + códigos 400/404/409 consistentes con mensajes en español
**Módulo de validaciones centralizado** Extraer todas las reglas de agendar cita a un `CitaValidationService` o un stored procedure unificado
**Reparar Bootstrap en botones** Revisar clases CSS conflictivas (`disabled`, `opacity`) en los componentes Blazor
**Pruebas unitarias para CRUD** Agregar tests para `MedicoService` y `PacientesService` cubriendo altas, ediciones y eliminaciones
**Modales y validaciones de formulario** Reemplazar `alert()` de JavaScript por modales Blazor y atributos `[Required]` /`DataAnnotationsValidator` 
**Proyecto Shared** Proyecto para compartir modelos y DTOs entre API y UI, evitando duplicación de clases como `MedicoDto`, `PacienteDto`, etc.

---

## Tecnologías utilizadas

ASP.NET Core 8 para API REST (AMAPI)
Blazor Server para interfaz de usuario (AMUI)
SQL Server para base de datos relacional
Dapper como micro-ORM para acceso a datos
xUnit como framework de pruebas unitarias
Moq para mocking en pruebas unitarias
Swagger / Swashbuckle para documentación y prueba de endpoints
Bootstrap para estilos en el frontend

## Notas sobre Migración desde Delphi Legacy

Para la migración de estos módulos desde una aplicación Delphi legacy, se requeriría un análisis detallado de la arquitectura actual.

Iimplicaría varios puntos de atención.
- No detener el sistema legacy, sino desarrollar el nuevo sistema en paralelo, migrando módulo por módulo (Pacientes → Médicos → Citas, etc.) para minimizar riesgos y validar cada etapa.
- Los stored procedures existentes en SQL Server podrían reutilizarse casi directamente, lo cual representa una ventaja significativa.
- Analisis de la lógica de negocio actual en Delphi para identificar reglas de validación, flujos de trabajo y casos de uso que deben ser replicados en el nuevo sistema.
- Replicar estos modelos al sistema en C# con una arquitectura en capas similar a la propuesta (Controllers, Services, Models) para mantener la separación de responsabilidades y facilitar el mantenimiento futuro.
- La lógica de validación de horarios y cancelaciones, si estuviera documentada, sería trasladable con relativa fidelidad a servicios en el nuevo sistema en C#.
- La interfaz gráfica requeriría una reescritura completa hacia el nuevo framework que se decida usar (Blazor, Angular, React, etc.).

---
