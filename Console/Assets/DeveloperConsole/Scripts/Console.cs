using DeveloperConsole; // Developer Console namespace

/// <summary>
/// Class to easily log to Developer Console from anywhere 
/// without adding DeveloperConsole namespace
/// </summary>
public static class Console {

    /// <summary>
    /// Log message directly into Developer Console window
    /// </summary>
    /// <param name="text">Text to print</param>
    public static void Log(string text) {
        ConsoleEvents.Log(text);
    }
}