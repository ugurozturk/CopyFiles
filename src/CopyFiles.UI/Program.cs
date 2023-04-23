using System.Text.Json;
using CopyFiles.Library;

#if DEBUG
string? currentDirectory = Directory.GetCurrentDirectory();
#else
            string? currentDirectory = Path.GetDirectoryName(Environment.ProcessPath);
#endif
System.Console.WriteLine($"Current Directory : {currentDirectory}");

List<string> files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories).ToList();

string json = File.ReadAllText(currentDirectory + "/settings.json");

JsonSerializerOptions _options = new()
{
    PropertyNameCaseInsensitive = true
};

SettingsType? settings = JsonSerializer.Deserialize<SettingsType>(json, _options);

if (settings == null)
{
    System.Console.WriteLine("Ayarlar boş olamaz");
    return;
}

files = files.Where(x => !settings.IgnoreFiles?.Any(y => x.Contains(y)) ?? true).ToList();

List<string> alreadyExistsFiles = new();

for (int i = 0; i < files.Count; i++)
{
    string filePath = files[i].Replace(currentDirectory + "/", "");

    string[] filePathSplit = filePath.Split("/");

    string directoryPaths = string.Empty;
    for (int j = 0; j < filePathSplit.Length; j++)
    {
        directoryPaths += "/" + filePathSplit[j];
        FileAttributes fileAttributes = File.GetAttributes(currentDirectory + directoryPaths);
        if (fileAttributes.HasFlag(FileAttributes.Directory))
        {
            string directoryPath = settings.CopyDestination + directoryPaths;
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

System.Console.WriteLine("Already exists:");
for (int i = 0; i < alreadyExistsFiles.Count; i++)
{
    System.Console.WriteLine(alreadyExistsFiles[i]);
}

System.Console.WriteLine("Finished.");
Console.ReadLine();