using ConsoleAppOpenAI;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Reflection;

AnsiConsole.MarkupLine("Starting commandline for [underline bold green]Chat GPT[/] World!");

AnsiConsole.Write(new FigletText("Console GPT").Color(Color.Blue));

var config = BuildConfig();
IOpenAIProxy chatOpenAI = new OpenAIProxy(
    config["OpenAI:ApiKey"],
    config["OpenAI:OrganizationId"]);

chatOpenAI.SetSystemMessage("You are a helpful assistant called Felix AI");

var msg = AnsiConsole.Ask<string>("[bold blue]Type your first Prompt[/]:");
do
{
    var results = await chatOpenAI.SendChatMessage(msg);

    foreach (var item in results)
    {
        AnsiConsole.MarkupLineInterpolated($"[red]{item.Role.Humanize(LetterCasing.Title)}: [/] {item.Content}");
    }

    msg = AnsiConsole.Ask<string>("[bold blue]Next Prompt[/]:");
} while (msg != "q");


static IConfiguration BuildConfig()
{
    var dir = Directory.GetCurrentDirectory();
    var configBuilder = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(dir, "appsettings.json"), optional: false)
        .AddUserSecrets(Assembly.GetExecutingAssembly());

    return configBuilder.Build();
}