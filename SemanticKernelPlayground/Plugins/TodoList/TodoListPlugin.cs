using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace SemanticKernelPlayground.Plugins.TodoList;

public class TodoListPlugin
{
    [KernelFunction, Description("Mark a todo list item as complete")]
    public static string CompleteTask(
        [Description("The task to complete")] string task
    )
    {
        // Read the JSON file
        string jsonFilePath = $"{Directory.GetCurrentDirectory()}/Plugins/TodoList/todo.txt";
        string jsonContent = File.ReadAllText(jsonFilePath);

        // Parse the JSON content
        JsonNode todoData = JsonNode.Parse(jsonContent);

        // Find the task and mark it as complete
        JsonArray todoList = (JsonArray)todoData["todoList"];
        foreach (JsonNode taskNode in todoList)
        {
            if (taskNode["task"].ToString() == task)
            {
                taskNode["completed"] = true;
                break;
            }
        }

        // Save the modified JSON back to the file
        File.WriteAllText(jsonFilePath, JsonSerializer.Serialize(todoData));
        return $"Task '{task}' marked as complete.";
    }
}