﻿using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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

#pragma warning disable SKEXP0050
        builder.Plugins.AddFromType<TimePlugin>();
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050
        builder.Plugins.AddFromObject(new TodoistPlugin(todoistApiKey));

        var kernel = builder.Build();

        //await Examples.HelloWorld(kernel);
        //await Examples.CurrentDayTimePluginExample(kernel);
        //await Examples.ConversationSummaryPluginExamples(kernel);
        //await Examples.TodoistPluginExamples(kernel);

        // APL-2005 https://learn.microsoft.com/en-us/training/paths/develop-ai-agents-azure-open-ai-semantic-kernel-sdk/

        //await SuggestChordsPromptExample(kernel);

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
