namespace CopyFiles;

public static class CopyFilesExtension
{
    public static bool IsRunningOnDebugMode()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}
