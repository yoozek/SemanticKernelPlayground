using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelPlayground.Plugins.ConvertCurrency;
using SemanticKernelPlayground.Plugins.Ingredients;
using SemanticKernelPlayground.Plugins.MusicConcert;
using SemanticKernelPlayground.Plugins.MusicLibrary;
using Serilog.Core;

namespace SemanticKernelPlayground;

#pragma warning disable SKEXP0060
#pragma warning disable SKEXP0060
#pragma warning disable SKEXP0050
public class MsLearnApl2005
{
    // APL-2005 https://learn.microsoft.com/en-us/training/paths/develop-ai-agents-azure-open-ai-semantic-kernel-sdk/


    public static async Task TravelBotExample(ILoggerFactory loggerFactory, string openAiKey, Logger logger)
    {
        // Create new kernel to do not include plugins for this example (reduces prompt for planner)
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(loggerFactory);
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo-0125",
            openAiKey);
        var kernel = builder.Build();

        kernel.ImportPluginFromType<CurrencyConverter>();
        kernel.ImportPluginFromType<ConversationSummaryPlugin>();

        var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");
        OpenAIPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        Console.WriteLine("What would you like to do?");
        var input = Console.ReadLine();

        var intent = await kernel.InvokeAsync<string>(
            prompts["GetIntent"],
            new() { { "input", input } }
        );

        logger.Information($"User's intent: {intent}");
        switch (intent)
        {
            case "ConvertCurrency":
                var currencyText = await kernel.InvokeAsync<string>(
                    prompts["GetTargetCurrencies"],
                    new() { { "input", input } }
                );
                var currencyInfo = currencyText!.Split("|");
                var result = await kernel.InvokeAsync("CurrencyConverter",
                    "ConvertAmount",
                    new() {
                        {"targetCurrencyCode", currencyInfo[0]},
                        {"baseCurrencyCode", currencyInfo[1]},
                        {"amount", currencyInfo[2]},
                    }
                );
                logger.Information(result.ToString());
                break;
            case "SuggestDestinations":
            case "SuggestActivities":
            case "HelpfulPhrases":
            case "Translate":
                var autoInvokeResult = await kernel.InvokePromptAsync(input!, new(settings));
                logger.Information(autoInvokeResult.ToString());
                break;
            default:
                logger.Information("Sure, I can help with that.");
                var otherIntentResult = await kernel.InvokePromptAsync(input!, new(settings));
                logger.Information(otherIntentResult.ToString());
                break;
        }
    }

    public static async Task AutoInvokeKernelFunctionsExample(ILoggerFactory loggerFactory, string openAiKey, Logger logger)
    {
        // Create new kernel to do not include plugins for this example (reduces prompt for planner)
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(loggerFactory);
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo-0125",
            openAiKey);
        var kernel = builder.Build();
        kernel.ImportPluginFromType<MusicLibraryPlugin>();
        kernel.ImportPluginFromType<MusicConcertPlugin>();
        kernel.ImportPluginFromPromptDirectory("Prompts");

        OpenAIPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        string prompt = @"I live in Portland OR USA. Based on my recently 
    played songs and a list of upcoming concerts, which concert 
    do you recommend?";

        var result = await kernel.InvokePromptAsync(prompt, new(settings));

        logger.Information(result.ToString());
    }

    public static async Task PlannerExample(ILoggerFactory loggerFactory, string openAiKey, Logger logger)
    {
        // Create new kernel to do not include plugins for this example (reduces prompt for planner)
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(loggerFactory);
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo-0125",
            openAiKey);
        var kernel = builder.Build();

        kernel.ImportPluginFromType<MusicLibraryPlugin>();
        kernel.ImportPluginFromType<MusicConcertPlugin>();
        kernel.ImportPluginFromPromptDirectory("Prompts");

        var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

        string location = "Redmond WA USA";
        string goal = @$"Based on the user's recently played music, suggest a 
    concert for the user living in ${location}";

        var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
            promptTemplate: @"Based on the user's recently played music:
        {{$recentlyPlayedSongs}}
        recommend a song to the user from the music library:
        {{$musicLibrary}}",
            functionName: "SuggestSong",
            description: "Suggest a song to the user"
        );

        kernel.Plugins.AddFromFunctions("SuggestSongPlugin", [songSuggesterFunction]);

        var songSuggestPlan = await planner.CreatePlanAsync(kernel, @"Suggest a song from the 
    music library to the user based on their recently played songs");

        logger.Information("Song Plan:");
        logger.Information(songSuggestPlan.ToString());

        var plan = await planner.CreatePlanAsync(kernel, goal);
        logger.Information($"Concert Plan: {plan}");
        var result = await plan.InvokeAsync(kernel);

        logger.Information($"Results: {result}");
    }

    public static async Task CombinePromptsWithPlugins(Kernel kernel)
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

    public static async Task IngredientsPluginExample(Kernel kernel)
    {
        kernel.ImportPluginFromType<IngredientsPlugin>();

        string prompt = @"This is a list of ingredients available to the user:
    {{IngredientsPlugin.GetIngredients}} 
    
    Please suggest a recipe the user could make with 
    some of the ingredients they have available";

        var result = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(result);
    }

    public static async Task MusicLibraryPluginExample(Kernel kernel)
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

    public static async Task CompleteTaskExample(Kernel kernel)
    {
        var result = await kernel.InvokeAsync<string>(
            "TodoListPlugin",
            "CompleteTask",
            new() { { "task", "Buy groceries" } }
        );
        Console.WriteLine(result);
    }

    public static async Task TravelPluginsExample(Kernel kernel)
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

    public static async Task SuggestChordsPromptExample(Kernel kernel)
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
}