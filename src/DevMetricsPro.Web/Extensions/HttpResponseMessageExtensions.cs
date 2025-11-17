namespace DevMetricsPro.Web.Extensions;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public static class HttpResponseMessageExtensions
{
    public static async Task<string?> ReadProblemDetailsMessageAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response is null || response.Content is null)
        {
            return null;
        }

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            if (problem is not null)
            {
                if (!string.IsNullOrWhiteSpace(problem.Detail))
                {
                    return problem.Detail;
                }

                if (!string.IsNullOrWhiteSpace(problem.Title))
                {
                    return problem.Title;
                }
            }
        }
        catch (NotSupportedException)
        {
            // Content type not JSON; fall back to raw string.
        }
        catch (JsonException)
        {
            // Invalid JSON payload; fall back to raw string.
        }

        try
        {
            var raw = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(raw) ? null : raw;
        }
        catch
        {
            return null;
        }
    }
}

