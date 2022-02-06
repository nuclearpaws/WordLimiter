namespace WordLimiter.Cli.Helpers;

internal static class FileWriter
{
    public static void WriteFile(string filePath, string text)
    {
        using(var sw = new StreamWriter(filePath))
        {
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
        }
    }
}