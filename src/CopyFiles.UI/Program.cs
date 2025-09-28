using System.Text.Json;
using CopyFiles.Library;

namespace CopyFiles;

public class Program
{
    private static void Main(string[] args)
    {

        var currentDirectory = string.Empty;
        if (CopyFilesExtension.IsRunningOnDebugMode())
        {
            currentDirectory = Directory.GetCurrentDirectory();
        }
        else
        {
            currentDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new ArgumentException("Current Directory Could not get.");
        }

        Console.WriteLine($"Current Directory : {currentDirectory}");

        var json = File.ReadAllText(currentDirectory + "/settings.json");

        JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        var settings = JsonSerializer.Deserialize<SettingsType>(json, _options);

        if (settings == null)
        {
            Console.WriteLine("Settings could not be loaded.");
            return;
        }

        if (!(args.Length >= 2 && args[0] == "run-without-ask" && args[1].ToLower() == "yes"))
        {
            Console.WriteLine($"You are going to copy files from '{currentDirectory}' to '{settings.CopyDestination}'. Are you sure?");
            Console.Write("Type 'yes' to continue: ");
            var answer = Console.ReadLine();
            if (answer == null || !answer.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Aborted.");
                Console.ReadKey();
                return;
            }
        }

        List<string> files = [.. Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories)];

        files = [.. files.Where(x => !settings.IgnoreFiles?.Any(y => x.Contains(y)) ?? true)];

        List<string> alreadyExistsFiles = new();

        for (var i = 0; i < files.Count; i++)
        {
            var filePath = files[i].Replace(currentDirectory + "/", "");

            var filePathSplit = filePath.Split("/");

            var directoryPaths = string.Empty;
            for (var j = 0; j < filePathSplit.Length; j++)
            {
                directoryPaths += "/" + filePathSplit[j];
                FileAttributes fileAttributes = File.GetAttributes(currentDirectory + directoryPaths);
                if (fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    var directoryPath = settings.CopyDestination + directoryPaths;
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }
                else
                {
                    if (!File.Exists(settings.CopyDestination + directoryPaths))
                    {
                        File.Copy(currentDirectory + directoryPaths, settings.CopyDestination + directoryPaths);
                    }
                    else
                    {
                        alreadyExistsFiles.Add(settings.CopyDestination + directoryPaths);
                    }
                }
            }
        }

        if (alreadyExistsFiles.Count > 0)
        {
            Console.WriteLine("Already exists:");
            for (var i = 0; i < alreadyExistsFiles.Count; i++)
            {
                Console.WriteLine(alreadyExistsFiles[i]);
            }
        }

        Console.WriteLine("Finished.");
        Console.ReadKey();
    }
}
