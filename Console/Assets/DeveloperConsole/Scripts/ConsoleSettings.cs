﻿using System;
using UnityEngine;

namespace DeveloperConsole {

    [Serializable]
    public class ConsoleSettings {

        [Header("GUI Settings")]
        public ConsoleGUIStyle InterfaceStyle = ConsoleGUIStyle.Large;
        [Space(5)]

        [Tooltip("Large Developer Console background opacity")]
        [Range(1, 100)]
        public float consoleBackgroundOpacity = 55;

        [Tooltip("Large Developer Console scroll sensitivity")]
        [Range(2, 100)]
        public float scrollSensitivity = 30;

        [Header("Console Settings")]

        public PrintOptions unityPrintOptions = PrintOptions.PrintDebugLogExpectionsWithStackTrace;

        [Tooltip("Max message count before starting to recycle from beginning")]
        [Range(2, 500)]
        public int maxMessageCount = 150;

        [Tooltip("Show Developer Console in build")]
        public bool íncludeConsoleInBuild = true;

        [Tooltip("Whether to register static commands only (No Monobehaviour commands). This can be done asynchronously expect in WebGL builds.")]
        public bool registerStaticCommandsOnly = false;

        [Tooltip("Show closests matching commands")]
        public bool showInputPredictions = true;

        [Tooltip("Allow Console Resizing")]
        public bool allowConsoleResize = true;

        [Tooltip("Show Cursor when Console is opened")]
        public bool showCursorOnEnable = true;

        [Tooltip("Whether to move scroll bar to bottom when console opens")]
        public bool scrollToBottomOnEnable = true;

        [Tooltip("Whether to clear all consoles messages when scene changes")]
        public bool clearMessagesOnSceneChange = true;

        [Tooltip("Print timestamp always when printing message to Developer Console")]
        public bool printMessageTimestamps = true;

        [Tooltip("Should help info be printed on startup")]
        public bool printHelpTextOnStartup = true;

        [Tooltip("Print console debug info like startup time etc.")]
        public bool printConsoleDebugInfo = true;

        [Header("KeyBindings")]
        public KeyCode consoleToggleKey = KeyCode.Backslash;        // Key to open/close console
        public KeyCode consoleSubmitKey = KeyCode.Return;           // Key to submit command
        public KeyCode consoleSearchCommandKey = KeyCode.UpArrow;   // key to search previous command
        public KeyCode consoleFillCommandKey = KeyCode.DownArrow;   // Key to fill suggestion
        public KeyCode ConsoleFillCommandKeyAlt = KeyCode.Tab;      // key to fill suggestion alternative key


        [Header("Editor only Settings")]

        [Tooltip("Print Editor debug info")]
        public bool printEditorDebugInfo = true;

        [Tooltip("whether to collect render information in editor. This can be printed to console with command: 'debug.print.renderinfo' ")]
        public bool collectRenderInfoEditor = true;

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