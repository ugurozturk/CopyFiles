namespace CopyFiles.Library;
public class SettingsType
{
    public string CopyDestination { get; set; } = null!;
    public List<string>? IgnoreFiles { get; set; }
}
