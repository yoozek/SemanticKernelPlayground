using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

namespace SemanticKernelPlayground;
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class Examples
{
    public static async Task HelloWorld(Kernel kernel)
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

    public static async Task CurrentDayTimePluginExample(Kernel kernel)
    {
        string prompt = @"Current date {{TimePlugin.Now}} 
        print me current date in most popular formats";

        var result = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(result);
    }

    public static async Task ConversationSummaryPluginExamples(Kernel kernel)
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
    public static async Task TodoistPluginExamples(Kernel kernel)
    {
        var getProjectsPrompt = kernel.CreateFunctionFromPrompt(@"{{TodoistPlugin.GetProjects}}
for the given list project names in markdown bullet list. Preserve hierarchy
Projects:");
        var getProjectsResult = await kernel.InvokeAsync(getProjectsPrompt);
        Console.WriteLine(getProjectsResult);
    }
}