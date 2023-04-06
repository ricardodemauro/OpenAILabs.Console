using ConsoleAppOpenAI.DALL_E.Models.Images;
using ConsoleAppOpenAI.DALL_E.Services;
using Microsoft.Extensions.Configuration;
using System.Reflection;

Console.WriteLine("Starting commandline for [underline bold green]DALL-E Open AI[/] World!");

var config = BuildConfig();

var aiClient = new OpenAIService(config);

Console.WriteLine("Type your first Prompt");
var msg = Console.ReadLine();

do
{
    var prompt = new GenerateImageRequest
    {
        N = 1,
        Prompt = msg,
        Size = "1024x1024"
    };

    var result = await aiClient.GenerateImages(prompt);

    foreach (var item in result.Data)
    {
        Console.WriteLine(item.Url);

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}.png");
        var img = await aiClient.DownloadImage(item.Url);

        await File.WriteAllBytesAsync(fullPath, img);
    }

    Console.WriteLine("Next Prompt:");
    msg = Console.ReadLine();
} while (msg != "q");


static IConfiguration BuildConfig()
{
    var dir = Directory.GetCurrentDirectory();
    var configBuilder = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(dir, "appsettings.json"), optional: false)
        .AddUserSecrets(Assembly.GetExecutingAssembly());

    return configBuilder.Build();
}

