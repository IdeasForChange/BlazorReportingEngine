using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Converters;

internal sealed class LocalDateNulalbleConverter : ValueConverter<LocalDate?, DateTime?>
{
    public LocalDateNulalbleConverter() : base(
        d => d.HasValue ? d.Value.ToDateTimeUnspecified() : null,
        d => d.HasValue ? LocalDate.FromDateTime(d.Value) : null)
    {
    }
}
