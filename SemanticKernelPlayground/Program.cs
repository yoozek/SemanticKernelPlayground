using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelPlayground.Plugins.ConvertCurrency;
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

        //await MsLearnApl2005.SuggestChordsPromptExample(kernel);
        //await MsLearnApl2005.TravelPluginsExample(kernel);
        //await MsLearnApl2005.CompleteTaskExample(kernel);
        //await MsLearnApl2005.MusicLibraryPluginExample(kernel);
        //await MsLearnApl2005.IngredientsPluginExample(kernel);
        //await MsLearnApl2005.CombinePromptsWithPlugins(kernel);
        //await MsLearnApl2005.PlannerExample(loggerFactory, openAiKey, logger);
        //await MsLearnApl2005.AutoInvokeKernelFunctionsExample(loggerFactory, openAiKey, logger);
        await MsLearnApl2005.TravelBotExample(loggerFactory, openAiKey, logger);

    }


    private record OpenAiConfig(string ApiKey);
}

#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
