using DeveloperConsole; // Developer Console namespace
using UnityEngine;
using System;

/// <summary>
/// Class to easily interact Console programmatically
/// </summary>
public static class ConsoleAPI {

    /// <summary>
    /// Log message directly into Developer Console window
    /// This is same as Console.Log()
    /// </summary>
    /// <param name="text">Text to print</param>
    public static void Log(string text) {
        ConsoleEvents.Log(text);
    }

    /// <summary>
    /// Register new command
    /// </summary>
    /// <param name="script">Reference to Monobehaviour</param>
    /// <param name="methodName">method name as string</param>
    /// <param name="commandName">command name as string</param>
    public static void RegisterCommand(MonoBehaviour script, string methodName, string commandName, string defaultValue = "", 
        bool isHiddenCommand = false, bool hiddenCommandMinimalGUI = false) {
        CommandDatabase.RegisterCommand(script, methodName, commandName, defaultValue, isHiddenCommand, hiddenCommandMinimalGUI);
    }

    /// <summary>
    /// Unregister command
    /// </summary>
    /// <param name="command">Command name</param>
    /// <param name="log">Log successfull Unregister event to Console</param>
    public static void RemoveCommand(string command, bool log = false) {
        CommandDatabase.RemoveCommand(command, log);
    }

    /// <summary>
    /// Get Console state
    /// </summary>
    /// <returns>boolean</returns>
    public static bool IsConsoleOpen() {
        return ConsoleManager.IsConsoleOpen();
    }

    /// <summary>
    /// Is Console fully initilized.
    /// Initializing takes ~1-2 seconds after scene load.
    /// </summary>
    /// <returns>boolean</returns>
    public static bool IsConsoleInitialized() {
        return ConsoleManager.IsConsoleInitialized();
    }

    /// <summary>
    /// Open Console
    /// </summary>
    public static void OpenConsole() {
        ConsoleEvents.OpenConsole();
    }

    /// <summary>
    /// Close Console
    /// </summary>
    public static void CloseConsole() {
        ConsoleEvents.CloseConsole();
    }

    /// <summary>
    /// Set Console state directly.
    /// </summary>
    public static void SetConsoleState(bool state) {
        ConsoleEvents.SetConsoleState(state);
    }

    /// <summary>
    /// Clear all console messages
    /// </summary>
    public static void ClearConsoleMessages() {
        ConsoleEvents.ClearConsoleMessages();
    }

    /// <summary>
    /// Rebind console activator to new key (Default: § / ½ - KeyCode.Backslash)
    /// </summary>
    /// <param name="newKey">New keycode</param>
    public static void RebindConsoleActivateKey(KeyCode newKey) {
        ConsoleEvents.ChangeActivateKeyCode(newKey);
    }

    /// <summary>
    /// enable/disable listening default Console activator key input.
    /// If disabled Developer Console doesn't handle opening or closing Console.
    /// Only use this if you plan to handle opening/closing console yourself.
    /// </summary>
    public static void ListenConsoleActivateKey(bool enabled) {
        ConsoleEvents.SetListenActivateKeyState(enabled);
    }

    /// <summary>
    /// Destroy Console Gameobject from the scene.
    /// </summary>
    /// <param name="time">Delay time</param>
    public static void DestroyConsole(float time = 0.0f) {
        ConsoleEvents.DestroyConsole(time);
    }

    /// <summary>
    /// Set whether console can print message timestamps.
    /// Messages that have already been printed are unaffected. 
    /// </summary>
    public static void PrintMessageTimeStamps(bool print) {
        var settings = ConsoleManager.GetSettings();
        settings.printMessageTimestamps = print;
        ConsoleManager.SetSettings(settings);
    }

    /// <summary>
    /// Set new Console settings
    /// </summary>
    public static void SetNewSettings(ConsoleSettings newSettings) {
        ConsoleManager.SetSettings(newSettings);
    }

    /// <summary>
    /// Get current Console settings
    /// </summary>
    public static ConsoleSettings GetSettings() {
        return ConsoleManager.GetSettings();
    }

    /// <summary>
    /// Get current Console GUI style
    /// </summary>
    public static ConsoleGUIStyle GetGUIStyle() {
        return ConsoleManager.GetGUIStyle();
    }

    /// <summary>
    /// Set Console GUI style
    /// </summary>
    public static void SetGUIStyle(ConsoleGUIStyle style) {
        ConsoleManager.SetGUIStyle(style);
    }

    /// <summary>
    /// Register to receive Developer Console state change event
    /// See FreeCamera.cs for example usage
    /// </summary>
    public static event Action<bool> RegisterConsoleStateChangeEvent;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialization() {
        var settings = ConsoleManager.GetSettings();
        if (!settings.íncludeConsoleInBuild && !Application.isEditor) return;
        
        ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChanged;
        Application.quitting += () => ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleStateChanged;
    }

    private static void ConsoleStateChanged(bool enabled) {
        RegisterConsoleStateChangeEvent?.Invoke(enabled);
    }
}