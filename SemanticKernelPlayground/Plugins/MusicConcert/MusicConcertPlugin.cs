using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground.Plugins.MusicConcert;

public class MusicConcertPlugin
{
    [KernelFunction, Description("Get a list of upcoming concerts")]
    public static string GetTours()
    {
        var path = $"{Directory.GetCurrentDirectory()}/Plugins/MusicConcert/concertdates.json"; 
        return File.ReadAllText(path);
    }
}