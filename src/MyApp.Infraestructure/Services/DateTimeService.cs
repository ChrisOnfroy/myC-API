using MyApp.Application.Interfaces;

namespace MyApp.Infraestructure.Services;

/// <summary>
/// Servicio de infraestructura para proveer la fecha y hora del sistema en UTC.
/// </summary>
public sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
