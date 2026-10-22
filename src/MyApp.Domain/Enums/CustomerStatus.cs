namespace MyApp.Domain.Enums;

/// <summary>
/// Estados posibles de una cuenta de cliente en el dominio.
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// Cliente activo para realizar operaciones.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Cliente inactivo o dado de baja lógica.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Cliente suspendido por incumplimiento de términos.
    /// </summary>
    Suspended = 3
}
