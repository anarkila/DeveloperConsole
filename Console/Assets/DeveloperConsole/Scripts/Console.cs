using Anarkila.DeveloperConsole; // Developer Console namespace
using UnityEngine;
using System;

/// <summary>
/// static class to interact with Developer Console from anywhere
/// </summary>
public static class Console {

    /// <summary>
    ///  Log message directly into Developer Console window
    /// </summary>
    /// <param name="message">Message to print</param>
    /// <param name="textColor">Text color</param>
    /// <param name="forceIgnoreTimestamp">force ignore timestamp</param>
    public static void Log(string message, Color? textColor = null, bool forceIgnoreTimestamp = false) {
        ConsoleEvents.Log(message, textColor, forceIgnoreTimestamp);
    }

    /// <summary>
    ///  Log message directly into Developer Console window
    /// </summary>
    /// <param name="message">Message to print</param>
    /// <param name="textColor">Text color</param>
    /// <param name="forceIgnoreTimestamp">force ignore timestamp</param>
    public static void Log(System.Object message, Color? textColor = null, bool forceIgnoreTimestamp = false) {
        ConsoleEvents.Log(message.ToString(), textColor, forceIgnoreTimestamp);
    }

    /// <summary>
    /// Log empty message without timestamp
    /// </summary>
    public static void LogEmpty() {
        ConsoleEvents.Log(ConsoleConstants.SPACE, forceIgnoreTimeStamp:true);
    }

    /// <summary>
    /// Register new command
    /// </summary>
    /// <param name="script">Reference to Monobehaviour</param>
    /// <param name="methodName">method name as string</param>
    /// <param name="commandName">command name as string</param>
    public static void RegisterCommand(MonoBehaviour script, string methodName, string commandName, string defaultValue = "", string info = "",
        bool debugCommandOnly = false, bool isHiddenCommand = false, bool hiddenCommandMinimalGUI = false) {
        CommandDatabase.RegisterCommand(script, methodName, commandName, defaultValue, info, debugCommandOnly, isHiddenCommand, hiddenCommandMinimalGUI);
    }

    /// <summary>
    /// Remove command
    /// If there's multiple instances of the same command, all of them will be removed.
    /// </summary>
    /// <param name="commandToRemove">Command to remove</param>
    /// <param name="logResult">Log result to console (Editor only)</param>
    public static void RemoveCommand(string commandToRemove, bool logResult = false) {
        CommandDatabase.RemoveCommand(commandToRemove, logResult);
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
    /// enable/disable listening default Console activator key input.
    /// If disabled Developer Console doesn't handle opening or closing Console.
    /// Only use this if you plan to handle opening/closing console yourself.
    /// </summary>
    public static void AllowConsoleActivateKey(bool enabled) {
        ConsoleEvents.SetListenActivateKeyState(enabled);
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
    /// Destroy Console Gameobject from the scene.
    /// </summary>
    /// <param name="time">Delay time</param>
    public static void DestroyConsole(float time = 0.0f) {
        ConsoleEvents.DestroyConsole(time);
    }

    /// <summary>
    /// Set new Console settings
    /// </summary>
    public static void SetSettings(ConsoleSettings newSettings) {
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
    /// </summary>
    public static event Action<bool> RegisterConsoleStateChangeEvent;

    /// <summary>
    /// Register to receive Developer Console initialized event
    /// </summary>
    public static event Action RegisterConsoleInitializedEvent;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialization() {
        var settings = ConsoleManager.GetSettings();
        if (!settings.includeConsoleInFinalBuild && !Debug.isDebugBuild) return;

        ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChanged;
        ConsoleEvents.RegisterConsoleInitializedEvent += ConsoleInitialized;

        Application.quitting += () => ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleStateChanged;
        Application.quitting += () => ConsoleEvents.RegisterConsoleInitializedEvent -= ConsoleInitialized;
    }

    private static void ConsoleStateChanged(bool enabled) {
        RegisterConsoleStateChangeEvent?.Invoke(enabled);
    }

    private static void ConsoleInitialized() {
        RegisterConsoleInitializedEvent?.Invoke();
    }
}