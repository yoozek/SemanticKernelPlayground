using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground.Plugins.Todoist;

public class TodoistPlugin(string apiKey)
{
    readonly HttpClient _client = new() { DefaultRequestHeaders = { { "Authorization", $"Bearer {apiKey}" } } };

    [KernelFunction, Description("Gets the list of all projects")]
    [return: Description("List all projects")]
    public async Task<string> GetProjects()
    {
        string url = $"https://api.todoist.com/rest/v2/projects";

        HttpResponseMessage response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }
}

public record TodoistConfig(string ApiKey);