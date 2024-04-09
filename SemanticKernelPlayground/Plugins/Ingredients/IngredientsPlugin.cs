using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground.Plugins.Ingredients;

public class IngredientsPlugin
{
    [KernelFunction, Description("Get a list of the user's ingredients")]
    public static string GetIngredients()
    {
        var path = $"{Directory.GetCurrentDirectory()}/Plugins/Ingredients/ingredients.json";
        string content = File.ReadAllText(path);
        return content;
    }
}