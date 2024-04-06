using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Refit;
using TodoistApi;

namespace SemanticKernelPlayground.Plugins.Todoist;

public class TodoistPlugin(string apiKey)
{
    readonly HttpClient _client = new() { 
        BaseAddress = new Uri("https://api.todoist.com/rest/v2"),
        DefaultRequestHeaders = { { "Authorization", $"Bearer {apiKey}" } }
    };

    [KernelFunction, Description("Gets the list of all projects")]
    [return: Description("List all projects")]
    public async Task<string> GetProjects()
    {
        var projects = await GetClient().GetAllProjectsAsync();
        return JsonSerializer.Serialize(projects);
    }

    [KernelFunction, Description("Creates a new task")]
    [return: Description("Created task")]
    public async Task<string> CreateTask(string taskDescription)
    {
        // TODO: How to create Task object?
        var projects = await GetClient().CreateTaskAsync(new TodoistModels.Task());
        return JsonSerializer.Serialize(projects);
    }

    private ITodoistApi GetClient()
    {
        return RestService.For<ITodoistApi>(_client);
    }
}

public record TodoistConfig(string ApiKey);