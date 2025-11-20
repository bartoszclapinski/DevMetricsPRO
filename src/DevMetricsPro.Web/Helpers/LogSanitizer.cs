namespace DevMetricsPro.Web.Helpers;

/// <summary>
/// Helper class for sanitizing sensitive data before logging
/// </summary>
public static class LogSanitizer
{
    private static readonly string[] SensitivePropertyNames = 
    [
        "password",
        "accesstoken",
        "githubAccessToken",
        "token",
        "secret",
        "apikey",
        "connectionstring"
    ];

    /// <summary>
    /// Masks sensitive string values for logging
    /// </summary>
    /// <param name="value">The value to mask</param>
    /// <param name="visibleChars">Number of characters to show at the start</param>
    /// <returns>Masked string (e.g., "ghp_abc***")</returns>
    public static string MaskSensitiveData(string? value, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "[empty]";
        }

        if (value.Length <= visibleChars)
        {
            return "***";
        }

        return $"{value[..visibleChars]}***";
    }

    /// <summary>
    /// Checks if a property name indicates sensitive data
    /// </summary>
    /// <param name="propertyName">The property name to check</param>
    /// <returns>True if the property is considered sensitive</returns>
    public static bool IsSensitiveProperty(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return false;
        }

        return SensitivePropertyNames.Any(sensitive => 
            propertyName.Contains(sensitive, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Sanitizes an object by masking sensitive properties
    /// </summary>
    /// <param name="obj">The object to sanitize</param>
    /// <returns>A sanitized representation suitable for logging</returns>
    public static string SanitizeForLogging(object? obj)
    {
        if (obj == null)
        {
            return "[null]";
        }

        var type = obj.GetType();
        var properties = type.GetProperties();
        var sanitized = new Dictionary<string, string>();

        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);
                var propName = prop.Name;

                if (IsSensitiveProperty(propName))
                {
                    sanitized[propName] = MaskSensitiveData(value?.ToString());
                }
                else
                {
                    sanitized[propName] = value?.ToString() ?? "[null]";
                }
            }
            catch
            {
                sanitized[prop.Name] = "[error reading value]";
            }
        }

        return string.Join(", ", sanitized.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}

