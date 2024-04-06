using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelPlayground.Plugins.Todoist;

namespace SemanticKernelPlayground;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        var openAiKey = config.GetSection(nameof(OpenAiConfig)).GetValue<string>(nameof(OpenAiConfig.ApiKey))
                        ?? throw new InvalidOperationException(nameof(OpenAiConfig.ApiKey));
        
        var todoistApiKey = config.GetSection(nameof(TodoistConfig)).GetValue<string>(nameof(TodoistConfig.ApiKey))
                        ?? throw new InvalidOperationException(nameof(TodoistConfig.ApiKey));

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
         "gpt-3.5-turbo",
         openAiKey);

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Plugins.AddFromType<TimePlugin>();
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();
        builder.Plugins.AddFromObject(new TodoistPlugin(todoistApiKey));

        var kernel = builder.Build();

        //await HelloWorld(kernel);
        //await CurrentDayTimePluginExample(kernel);
        //await ConversationSummaryPluginExamples(kernel);

        await TodoistPluginExamples(kernel);
    }

    private static async Task TodoistPluginExamples(Kernel kernel)
    {
        var prompt = kernel.CreateFunctionFromPrompt(@"{{TodoistPlugin.GetProjects}}
for the given list project names in markdown bullet list
Projects:");
        var getTodoistProjects = await kernel.InvokeAsync(prompt);
        Console.WriteLine(getTodoistProjects);
    }


    private static async Task HelloWorld(Kernel kernel)
    {
        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = text1 }));
    }

    private static async Task CurrentDayTimePluginExample(Kernel kernel)
    {
        string prompt = @"Current date {{TimePlugin.Now}} 
        print me current date in most popular formats";

        var result = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(result);
    }

    private static async Task ConversationSummaryPluginExamples(Kernel kernel)
    {
        var recipesPrompt = @"User background: 
            {{ConversationSummaryPlugin.SummarizeConversation $input}}
            Given this user's background, provide a list of relevant recipes.";

        string input = @"I'm a vegan in search of new recipes. 
I love spicy food! Can you give me a list of breakfast 
recipes that are vegan friendly?";

        var suggestRecipes = kernel.CreateFunctionFromPrompt(recipesPrompt);
        var suggestRecipesResult = await kernel.InvokeAsync(suggestRecipes,
            new KernelArguments
            {
                { "input", input }
            });
        Console.WriteLine(suggestRecipesResult);

        var summarizeResult = await kernel.InvokeAsync(
            nameof(ConversationSummaryPlugin),
            "SummarizeConversation",
            new() { { "input", input } });

        Console.WriteLine(summarizeResult);

        var getTopicsResult = await kernel.InvokeAsync(
            nameof(ConversationSummaryPlugin),
            "GetConversationTopics",
            new() { { "input", input } });

        Console.WriteLine(getTopicsResult);

        var actionItemsResult = await kernel.InvokeAsync(
            nameof(ConversationSummaryPlugin),
            "GetConversationActionItems",
            new() { { "input", input } });

        Console.WriteLine(actionItemsResult);
    }

    private record OpenAiConfig(string ApiKey);
}

#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
