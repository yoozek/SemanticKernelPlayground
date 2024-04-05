using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelPlayground;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        var openAiKey = config.GetSection(nameof(OpenAi)).GetValue<string>(nameof(OpenAi.ApiKey))
                        ?? throw new InvalidOperationException(nameof(OpenAi.ApiKey));
        
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
         "gpt-3.5-turbo",
         openAiKey);
        
        var kernel = builder.Build();

        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

        Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = text1 }));

        Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = text2 }));
        
        Console.WriteLine("Hello, World!");
    }

    private record OpenAi(string ApiKey);
}