using UnityEngine.UI;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    [Serializable]
    public class ConsoleSettings {

        [Header("GUI Settings")]
        [Tooltip("GUI style to use")]
        public ConsoleGUIStyle interfaceStyle = ConsoleGUIStyle.Large;

        [Tooltip("GUI Theme to use")]
        public ConsoleGUITheme interfaceTheme = ConsoleGUITheme.Dark;


        [Header("Large GUI Settings")]
        [Tooltip("Console window size multiplier (Large GUI only)")]
        [Range(0.50f, 1.2f)]
        public float consoleWindowDefaultSize = 0.9f;

        [Tooltip("Large Developer Console scroll sensitivity")]
        [Range(2, 100)]
        public float scrollSensitivity = 32;

        [Tooltip("Reset console window position back to center of the screen when console is opened (Large GUI only)")]
        public bool resetWindowPositionOnEnable = false;

        [Tooltip("Reset console window size back to default when console is opened (Large GUI only)")]
        public bool resetWindowSizeOnEnable = false;

        [Tooltip("Force console to be inside screen bounds when it's dragged (Large GUI only)")]
        public bool forceConsoleInsideScreenBounds = false;

        [Tooltip("ScrollRect scrollbar visibility")]
        public ScrollRect.ScrollbarVisibility ScrollRectVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        public ConsoleColors consoleColors = new ConsoleColors {
            minimalGUIBackgroundColor = new Color(0.16f, 0.16f, 0.16f, 1f),
            minimalGUITextColor = new Color(1f, 1f, 1f, 1f),

            largeGUIBackgroundColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 0.97f),
            largeGUIBorderColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
            largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
            largeGUIControlsColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 1f),
            largeGUIScrollbarBackgroundColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
            largeGUIScrollbarHandleColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
            largeGUITextColor = new Color(1f, 1f, 1f, 1f),
        };


        [Header("General Settings")]
        [Tooltip("Whether to include Developer Console in final release build. Default is false.")]
        public bool includeConsoleInFinalBuild = false;

        [Tooltip("Initialize Console on first open instead of when game starts.")]
        public bool initializeConsoleOnFirstOpen = false;

        [Tooltip("Whether to register static commands only (No Monobehaviour commands with [ConsoleCommand()] attributes). " +
           "To register MonoBehaviour commands use Console.RegisterCommand() method.")]
        public bool registerStaticCommandsOnly = false;

        [Tooltip("Whether to look for all C# assemblies for Console Commands. Enabling this increases the Initialization a lot, from ~20 ms to ~1500 ms.")]
        public bool scanAllAssemblies = false;

        [Tooltip("Whether commands are case sensetive'")] 
        public bool commandsAreCaseSensetive = false;

        [Tooltip("Whether to allow GUI style change during runtime.")]
        public bool allowGUIStyleChangeRuntime = true;

        // If you only use Minimal GUI, consider changing these to ConsoleLogOptions.DontPrintLogs
        [Tooltip("Whether to print Debug.Log and Debug.LogError messages into Developer Console (Large GUI only)")]
        public ConsoleLogOptions UnityLogOption = ConsoleLogOptions.LogExceptionWithStackTrace;

        [Tooltip("Whether to print Debug.Log and Debug.LogError message into Developer Console from another threads (Large GUI only)")]
        public ConsoleLogOptions unityThreadedLogOption = ConsoleLogOptions.LogExceptionsWithStackTraceEditorOnly;

        [Tooltip("Max message count before starting to recycle from beginning. This cannot be changed runtime.")]
        [Range(2, 1024)]
        public int maxMessageCount = 128;

        [Tooltip("Allow multiple commands to be executed in one go like: 'test.int 1 & test.int 2'")] // test.int 1 && test.int 2 also works
        public bool allowMultipleCommands = true;

        [Tooltip("Show input predictions")]
        public bool showInputPredictions = true;

        [Tooltip("Show input predictions")]
        public bool showContextMenuOnMessageRightClick = false;

        [Tooltip("Allow Console Resizing (Large GUI only)")]
        public bool allowConsoleResize = true;

        [Tooltip("Allow Console Dragging (Large GUI only)")]
        public bool allowConsoleWindowDrag = true;

        [Tooltip("Show Cursor when Console is opened")]
        public bool showCursorOnEnable = true;

        [Tooltip("Hide Cursor when Console is closed")]
        public bool hideCursorOnDisable = true;

        [Tooltip("Whether to move scroll bar to bottom when console opens")]
        public bool scrollToBottomOnEnable = true;

        [Tooltip("Whether to clear all consoles messages when scene changes")]
        public bool clearMessagesOnSceneChange = false;

        [Tooltip("Print timestamp always when printing message to Developer Console")]
        public bool printMessageTimestamps = true;

        [Tooltip("Should help info be printed on startup")]
        public bool printStartupHelpText = true;

        [Tooltip("Whether to print unrecognized command info to console. 'Command [command name] was not recognized'")]
        public bool printUnrecognizedCommandInfo = true;

        [Tooltip("Whether to print available commands in alphabetical order.'")]
        public bool printCommandsAlphabeticalOrder = true;

        [Tooltip("Whether to print commands info text with 'help' command")]
        public bool printCommandInfoTexts = true;

        [Tooltip("Whether to print loaded scene name and LoadSceneMode")]
        public bool printLoadedSceneName = true;

        [Tooltip("Print Developer Console debug info like startup time etc.")]
        public bool printInitializationTime = false;

        [Tooltip("Whether to keep track of logged messages " +
            "These messages can then be retrieved with Console.GetConsoleMessages()")]
        public bool keepTrackOfMessages = true;

        [Tooltip("Whether to keep track of successfully executed commands that already " +
        "exists in the executed commands list.")]
        public bool trackDuplicateCommands = false;

        [Tooltip("Whether to keep track of commands that failed or does not exists.")]
        public bool trackFailedCommands = true;


        [Header("KeyBindings")]

        // Key to open/close console
        public KeyCode consoleToggleKey = KeyCode.Backslash; 

        // Key to submit command
        public KeyCode consoleSubmitKey = KeyCode.Return;      

        // key to search previously executed command
        public KeyCode consoleSearchCommandKey = KeyCode.UpArrow;

        // Key to fill next command from suggestion
        // however if consoleSearchCommandKey clicked then this key goes reverse direction of that key
        // just like Terminal/CMD does
        public KeyCode NextSuggestedCommandKey = KeyCode.DownArrow;

        // key to fill next command from suggestion alternative key
        public KeyCode NextSuggestedCommandKeyAlt = KeyCode.Tab;

        [Header("Debug Settings")]
        [Tooltip("Whether to print message counts after stopping play mode")]
        public bool printMessageCountOnStopPlay = false;

        [Tooltip("Print Play button click to playable scene time")]
        public bool printPlayButtonToSceneTime = true;

        [Tooltip("whether to collect render information in editor. This can be printed to console with command: 'debug.print.renderinfo' ")]
        public bool collectRenderInfoEditor = true;

        [Tooltip("Whether to clear Unity Console messages when 'clear' command called")]
        public bool clearUnityConsoleOnConsoleClear = false;

        [Tooltip("whether to print Unity log type. ")]
        public bool printLogType = false;

        public void ApplyColors() {
            if (interfaceTheme == ConsoleGUITheme.Custom) return;

            switch (interfaceTheme) {

                case ConsoleGUITheme.Dark: // default theme

                    consoleColors = new ConsoleColors {
                        minimalGUIBackgroundColor = new Color(0.16f, 0.16f, 0.16f, 1f),
                        minimalGUITextColor = new Color(1f, 1f, 1f, 1f),

                        largeGUIBackgroundColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 0.97f),
                        largeGUIBorderColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
                        largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
                        largeGUIControlsColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f),
                        largeGUIScrollbarBackgroundColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 0.9f),
                        largeGUIScrollbarHandleColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f),
                        largeGUITextColor = new Color(1f, 1f, 1f, 1f),
                    };
                    break;

                case ConsoleGUITheme.Darker:

                    consoleColors = new ConsoleColors {
                        minimalGUIBackgroundColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 1f),
                        minimalGUITextColor = new Color(1f, 1f, 1f, 1f),

                        largeGUIBackgroundColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 1f),
                        largeGUIBorderColor = new Color(0.09803922f, 0.09803922f, 0.09803922f, 1f),
                        largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
                        largeGUIControlsColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 0.9f),
                        largeGUIScrollbarBackgroundColor = new Color(0.09803922f, 0.09803922f, 0.09803922f, 1f),
                        largeGUIScrollbarHandleColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
                        largeGUITextColor = new Color(1f, 1f, 1f, 1f),
                    };
                    break;

                case ConsoleGUITheme.Red:

                    consoleColors = new ConsoleColors {
                        minimalGUIBackgroundColor = new Color(0f, 0f, 0f, 1f),
                        minimalGUITextColor = new Color(1f, 0f, 0f, 1f),

                        largeGUIBackgroundColor = new Color(0f, 0f, 0f, 0.97f),
                        largeGUIBorderColor = new Color(0.2392157f, 0f, 0f, 1f),
                        largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
                        largeGUIControlsColor = new Color(0f, 0f, 0f, 0.5f),
                        largeGUIScrollbarBackgroundColor = new Color(0.2392157f, 0f, 0f, 1f),
                        largeGUIScrollbarHandleColor = new Color(0f, 0f, 0f, 0.5f),
                        largeGUITextColor = new Color(1f, 0f, 0f, 1f),
                    };
                    break;

                default:

                    consoleColors = new ConsoleColors {
                        minimalGUIBackgroundColor = new Color(0.16f, 0.16f, 0.16f, 1f),
                        minimalGUITextColor = new Color(1f, 1f, 1f, 1f),

                        largeGUIBackgroundColor = new Color(0.04705882f, 0.04705882f, 0.04705882f, 0.97f),
                        largeGUIBorderColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f),
                        largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
                        largeGUIControlsColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f),
                        largeGUIScrollbarBackgroundColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 0.9f),
                        largeGUIScrollbarHandleColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f),
                        largeGUITextColor = new Color(1f, 1f, 1f, 1f),
                    };
                    break;
            }
        }
    }
}

public enum ConsoleGUIStyle {
    Large,
    Minimal
}

public enum ConsoleGUITheme {
    Dark,
    Darker,
    Red,
    Custom
}

public enum ConsoleLogOptions {
    // Don't print any Debug.Log/LogError to Console
    DontPrintLogs,

    // Print expections to Console such as
    // "UnityException: Transform child out of bounds"
    // In Unity Editor only!
    LogWithExceptionsEditorOnly,

    // Print expections expections with stack trace to Console such as
    // "UnityException: Transform child out of bounds YourScript.Start () (at Assets/Example/ExampleScene/YourScript.cs:42)"
    // in Unity Editor only!
    LogExceptionsWithStackTraceEditorOnly,

    // Print Debug logs without expections or stack traces to Console
    // In Editor and Build!
    LogWithoutExceptions,

    // Print expections to Console such
    // as "UnityException: Transform child out of bounds"
    // In Editor and Build!
    LogWithExceptions,

    // Print expections with stack trace to Console such as
    // "UnityException: Transform child out of bounds YourScript.Start () (at Assets/Example/ExampleScene/YourScript.cs:42)"
    // In Editor and Build!
    LogExceptionWithStackTrace
}