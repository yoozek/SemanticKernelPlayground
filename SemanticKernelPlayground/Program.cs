using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelPlayground.Plugins.IngredientsPlugin;
using SemanticKernelPlayground.Plugins.MusicLibrary;
using SemanticKernelPlayground.Plugins.Todoist;
using SemanticKernelPlayground.Plugins.TodoList;
using Serilog;
using Serilog.Events;

namespace SemanticKernelPlayground;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0060

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.SemanticKernel", LogEventLevel.Debug)
            .WriteTo.Console()
            .CreateLogger();

        var services = new ServiceCollection();

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger));
        services.AddSingleton(loggerFactory);
        var serviceProvider = services.BuildServiceProvider();


        var openAiKey = config.GetSection(nameof(OpenAiConfig)).GetValue<string>(nameof(OpenAiConfig.ApiKey))
                        ?? throw new InvalidOperationException(nameof(OpenAiConfig.ApiKey));
        
        var todoistApiKey = config.GetSection(nameof(TodoistConfig)).GetValue<string>(nameof(TodoistConfig.ApiKey))
                        ?? throw new InvalidOperationException(nameof(TodoistConfig.ApiKey));

        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(loggerFactory);
        builder.AddOpenAIChatCompletion(
         "gpt-3.5-turbo-0125",
         openAiKey);

        builder.Plugins.AddFromType<TimePlugin>();
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();
        builder.Plugins.AddFromObject(new TodoistPlugin(todoistApiKey));
        builder.Plugins.AddFromType<TodoListPlugin>();
        builder.Plugins.AddFromType<MusicLibraryPlugin>();

        var kernel = builder.Build();

        //await Examples.HelloWorld(kernel);
        //await Examples.CurrentDayTimePluginExample(kernel);
        //await Examples.ConversationSummaryPluginExamples(kernel);
        //await Examples.TodoistPluginExamples(kernel);

        // APL-2005 https://learn.microsoft.com/en-us/training/paths/develop-ai-agents-azure-open-ai-semantic-kernel-sdk/

        //await SuggestChordsPromptExample(kernel);
        //await TravelPluginsExample(kernel);
        //await CompleteTaskExample(kernel);
        //await MusicLibraryPluginExample(kernel);
        //await IngredientsPluginExample(kernel);
        //await CombinePromptsWithPlugins(kernel);


        // Create new kernel to do not include plugins for this example (reduces prompt for planner)
        builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(loggerFactory);
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo-0125",
            openAiKey);
        kernel = builder.Build();

        kernel.ImportPluginFromPromptDirectory("Plugins");
        var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions { AllowLoops = true });
        
        string goal = @"What ingredients is the user missing from their 
   current ingredients list to make a recipe for blueberry muffins";

        var plan = await planner.CreatePlanAsync(kernel, goal);
        var result = await plan.InvokeAsync(kernel);
        Console.WriteLine(result);
    }

    private static async Task CombinePromptsWithPlugins(Kernel kernel)
    {
        string prompt = @"This is a list of music available to the user:
    {{MusicLibraryPlugin.GetMusicLibrary}} 

    This is a list of music the user has recently played:
    {{MusicLibraryPlugin.GetRecentPlays}}

    Based on their recently played music, suggest a song from
    the list to play next";

        var result = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(result);
    }

    private static async Task IngredientsPluginExample(Kernel kernel)
    {
        kernel.ImportPluginFromType<IngredientsPlugin>();

        string prompt = @"This is a list of ingredients available to the user:
    {{IngredientsPlugin.GetIngredients}} 
    
    Please suggest a recipe the user could make with 
    some of the ingredients they have available";

        var result = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(result);
    }

    private static async Task MusicLibraryPluginExample(Kernel kernel)
    {
        kernel.ImportPluginFromType<MusicLibraryPlugin>();
        var result = await kernel.InvokeAsync(
            "MusicLibraryPlugin",
            "AddToRecentlyPlayed",
            new()
            {
                ["artist"] = "Tiara",
                ["song"] = "Danse",
                ["genre"] = "French pop, electropop, pop"
            }
        );

        Console.WriteLine(result);
    }

    private static async Task CompleteTaskExample(Kernel kernel)
    {
        var result = await kernel.InvokeAsync<string>(
            "TodoListPlugin",
            "CompleteTask",
            new() { { "task", "Buy groceries" } }
        );
        Console.WriteLine(result);
    }

    private static async Task TravelPluginsExample(Kernel kernel)
    {
        var prompts = kernel.ImportPluginFromPromptDirectory("Prompts/TravelPlugins");

        ChatHistory history = [];
        string input = @"Planuję podróż do Mediolanu z moją narzeczoną. 
Przylot jest zaplanowany na niedzielę wieczór (do Beregamo), a wylot w środę w południe.
Lubimy zwiedzać miasto i jeść pyszne jedzenie";

        var result = await kernel.InvokeAsync<string>(prompts["SuggestDestinations"],
            new() {
                { "input", input },
            }
        );

        Console.WriteLine(result);
        history.AddUserMessage(input);
        history.AddAssistantMessage(result);

        Console.WriteLine("Where would you like to go?");
        input = Console.ReadLine();

        result = await kernel.InvokeAsync<string>(prompts["SuggestActivities"],
            new() {
                { "history", history },
                { "destination", input },
            }
        );
        Console.WriteLine(result);
    }

    private static async Task SuggestChordsPromptExample(Kernel kernel)
    {
        var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");
        string input = "G, C";

        var result = await kernel.InvokeAsync(
            prompts["SuggestChords"],
            new() {
                { "startingChords", input },
            }
        );
        Console.WriteLine(result);
    }


    private record OpenAiConfig(string ApiKey);
}

#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
