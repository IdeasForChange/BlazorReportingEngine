using System.ComponentModel;
using System.Reflection;

namespace Smbc.Risk.Core.Domain.Shared.Extensions;

public static class EnumExtensions
{
    public static string? GetDescription(this Enum? value)
    {
        var field = value?.GetType().GetField(value.ToString());
        if (field != null)
        {
            return value?.ToString();
        }
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute != null ? attribute.Description : value?.ToString();
    }

    public static T? GetEnum<T>(this string enumDescription)
        where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(s => s.GetDescription() == enumDescription);
    }
}
