using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground.Plugins.MusicLibrary;

public class MusicLibraryPlugin
{
    private static string _folderPath = $"{Directory.GetCurrentDirectory()}/Plugins/MusicLibrary";
    private static string _recentlyPlayedFilePath = $"{_folderPath}/recentlyplayed.json";
    private static string _musicLibraryFilePath = $"{_folderPath}/musiclibrary.json";

    [KernelFunction,
     Description("Get a list of music recently played by the user")]
    public static string GetRecentPlays()
    {
        string content = File.ReadAllText(_recentlyPlayedFilePath);
        return content;
    }

    [KernelFunction, Description("Add a song to the recently played list")]
    public static string AddToRecentlyPlayed(
        [Description("The name of the artist")] string artist,
        [Description("The title of the song")] string song,
        [Description("The song genre")] string genre)
    {
        string jsonContent = File.ReadAllText(_recentlyPlayedFilePath);
        var recentlyPlayed = (JsonArray)JsonNode.Parse(jsonContent);

        var newSong = new JsonObject
        {
            ["title"] = song,
            ["artist"] = artist,
            ["genre"] = genre
        };

        recentlyPlayed.Insert(0, newSong);
        File.WriteAllText(_recentlyPlayedFilePath,
            JsonSerializer.Serialize(recentlyPlayed,
                new JsonSerializerOptions { WriteIndented = true }));

        return $"Added '{song}' to recently played";
    }

    [KernelFunction, Description("Get a list of music available to the user")]
    public static string GetMusicLibrary()
    {
        string dir = Directory.GetCurrentDirectory();
        string content = File.ReadAllText(_musicLibraryFilePath);
        return content;
    }
}