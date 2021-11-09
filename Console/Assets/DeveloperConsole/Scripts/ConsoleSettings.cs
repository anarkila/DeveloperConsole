using System;
using UnityEngine;

namespace DeveloperConsole {

    [Serializable]
    public class ConsoleSettings {


        [Header("GUI Settings")]
        public ConsoleGUIStyle InterfaceStyle = ConsoleGUIStyle.Large;
        [Space(5)]

        [Tooltip("Console window size multiplier on start (Large GUI only)")]
        [Range(0.50f, 1.2f)]
        public float consoleWindowDefaultSize = 0.8f;

        [Tooltip("Large Developer Console background opacity")]
        [Range(1, 100)]
        public float consoleBackgroundOpacity = 55;

        [Tooltip("Large Developer Console scroll sensitivity")]
        [Range(2, 100)]
        public float scrollSensitivity = 30;

        [Tooltip("Whether to reset console window position back to center of the screen when console is opened (Large GUI only)")]
        public bool resetWindowPositionOnEnable = false;

        [Tooltip("Whether to reset console window size back to default when console is opened (Large GUI only)")]
        public bool resetWindowSizeOnEnable = false;

        [Header("General Settings")]

        public PrintOptions unityPrintOptions = PrintOptions.PrintDebugLogExpectionsWithStackTrace;

        [Tooltip("Max message count before starting to recycle from beginning")]
        [Range(2, 500)]
        public int maxMessageCount = 150;

        [Tooltip("Whether to include Developer Console in final release build. Be careful whether you can actually want to include in final release build!")]
        public bool includeConsoleInFinalBuild = false;

        [Tooltip("Whether to register static command attributes only (No Monobehaviour commands with [ConsoleCommand()] attributes). " +
            "To register MonoBehaviour commands use Console.RegisterCommand() method.")]
        public bool registerStaticCommandAttributesOnly = false;

        [Tooltip("Show closests matching commands")]
        public bool showInputPredictions = true;

        [Tooltip("Allow Console Resizing (Large GUI only)")]
        public bool allowConsoleResize = true;

        [Tooltip("Allow Console Dragging (Large GUI only)")]
        public bool allowConsoleWindowDrag= true;

        [Tooltip("Whether force console to be inside screen bounds, both resize and drag")]
        public bool ForceConsoleInsideScreenBounds = false;

        [Tooltip("Show Cursor when Console is opened")]
        public bool showCursorOnEnable = true;

        [Tooltip("Hide Cursor when Console is closed")]
        public bool hideCursorOnDisable = true;

        [Tooltip("Whether to move scroll bar to bottom when console opens")]
        public bool scrollToBottomOnEnable = true;

        [Tooltip("Whether to clear all consoles messages when scene changes")]
        public bool clearMessagesOnSceneChange = true;

        [Tooltip("Print timestamp always when printing message to Developer Console")]
        public bool printMessageTimestamps = true;

        [Tooltip("Should help info be printed on startup")]
        public bool printStartupHelpText = true;

        [Tooltip("Print console debug info like startup time etc.")]
        public bool printConsoleDebugInfo = true;

        [Tooltip("Whether to print unrecognized command info to console. 'Command [command name] was not recognized'")]
        public bool printUnrecognizedCommandInfo = true;

        [Header("KeyBindings")]
        public KeyCode consoleToggleKey = KeyCode.Backslash;        // Key to open/close console
        public KeyCode consoleSubmitKey = KeyCode.Return;           // Key to submit command
        public KeyCode consoleSearchCommandKey = KeyCode.UpArrow;   // key to search previous command
        public KeyCode consoleFillCommandKey = KeyCode.DownArrow;   // Key to fill suggestion
        public KeyCode ConsoleFillCommandKeyAlt = KeyCode.Tab;      // key to fill suggestion alternative key

        [Header("Debug Settings")]
        [Tooltip("Print debug info (Editor and Development build)")]
        public bool printDebugInfo = true;

        [Tooltip("Print Play button click to playable scene time")]
        public bool printPlayButtonToSceneTime = true;

        [Tooltip("whether to collect render information in editor. This can be printed to console with command: 'debug.print.renderinfo' ")]
        public bool collectRenderInfoEditor = true;

        [Tooltip("Whether to clear Unity Console too when 'clear' command called (Editor only)'")]
        public bool ClearUnityConsoleOnConsoleClear = false;
    }

    public enum ConsoleGUIStyle {
        Minimal,
        Large
    }

    public enum PrintOptions {
        // Don't print any Debug.Log/LogError to Console
        DontPrintDebugLogs,

        // Print expections to Console such as
        // "UnityException: Transform child out of bounds"
        // In Unity Editor only!
        PrintDebugLogsWithExpectionsEditorOnly,

        // Print expections expections with stack trace to Console such as
        // "UnityException: Transform child out of bounds YourScript.Start () (at Assets/Example/ExampleScene/YourScript.cs:42)"
        // in Unity Editor only!
        PrintDebugLogsExpectionsWithStackTraceEditorOnly,

        // Print Debug logs without expections or stack traces to Console
        // In Editor and Build!
        PrintDebugLogsWithoutExpections,

        // Print expections to Console such
        // as "UnityException: Transform child out of bounds"
        // In Editor and Build!
        PrintDebugLogsWithExpections,

        // Print expections with stack trace to Console such as
        // "UnityException: Transform child out of bounds YourScript.Start () (at Assets/Example/ExampleScene/YourScript.cs:42)"
        // In Editor and Build!
        PrintDebugLogExpectionsWithStackTrace
    }
}