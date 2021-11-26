using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class ConsoleManager {

        public static Dictionary<LogType, string> LogTypes = new Dictionary<LogType, string>();
        private static List<string> messagesBeforeInitDone = new List<string>();
        private static ConsoleSettings settings = new ConsoleSettings();
        private static StringBuilder sb = new StringBuilder();
        private static bool consoleInitialized = false;
        private static bool showSuggestions = true;
        private static int sceneChangeCount = 0;
        private static Thread UnityMainThreadID;
        private static bool initDone = false;
        private static bool consoleIsOpen;
        private static int messageCount;

        /// <summary>
        /// Initilize Developer Console
        /// </summary>
        public static void InitilizeDeveloperConsole(ConsoleSettings settings, Thread thread) {
            if (initDone) return;

            foreach (LogType logType in Enum.GetValues(typeof(LogType))) {
                LogTypes.Add(logType, logType.ToString());
            }

            UnityMainThreadID = thread;

            Application.logMessageReceived += UnityLogEventCallback;                    // Get logMessageReceived callback from Unity Engine
            SceneManager.sceneLoaded += SceneLoadCallback;                              // Register OnSceneChanged  event from Unity Engine
            Application.quitting += OnDestroy;                                          // Register ApplicationQuit event from Unity Engine

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent += RebindActivateKeyEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;
#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent += ConsoleClearEvent;
#endif

            SetSettings(settings);

            initDone = true;
        }

        /// <summary>
        /// OnDestroy callback from Unity Engine.
        /// </summary>
        private static void OnDestroy() {

            Application.logMessageReceived -= UnityLogEventCallback;
            SceneManager.sceneLoaded -= SceneLoadCallback;
            Application.quitting -= OnDestroy;

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent -= RebindActivateKeyEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= GUIStyleChanged;

#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent -= ConsoleClearEvent;
#endif

#if UNITY_EDITOR
            if (settings.printConsoleDebugInfo) {

                var executedCommandCount = CommandDatabase.GetExcecutedCommandCount();
                var failedCommandCount = CommandDatabase.GetFailedCommandCount();

                if (messageCount != 0) Debug.Log(string.Format("Debug.Log and Debug.LogError was called {0} times.", messageCount));
                if (executedCommandCount != 0) Debug.Log(string.Format("Console Commands was called {0} times.", executedCommandCount));
                if (failedCommandCount != 0) Debug.Log(string.Format("Failed or not recognized commands was called {0} times.", failedCommandCount));
            }
#endif
        }

#if UNITY_EDITOR
        private static void ConsoleClearEvent() {
            if (settings == null) return;

            if (settings.clearUnityConsoleOnConsoleClear) {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);
            }
        }
#endif
        /// <summary>
        /// Get current console settings
        /// </summary>
        /// <returns></returns>
        public static ConsoleSettings GetSettings() {
            return settings;
        }

        /// <summary>
        /// Show Console Predictions
        /// </summary>
        /// <returns>boolean</returns>
        public static bool ShowConsolePredictions() {
            return showSuggestions;
        }

        /// <summary>
        /// Restart (Re-open and reset) Developer Console
        /// </summary>
        public static void RestartDeveloperConsole() {
            ConsoleEvents.CloseConsole();
            ConsoleEvents.ClearConsoleMessages();
            ConsoleEvents.ResetConsole();
            ConsoleEvents.OpenConsole();
        }

        /// <summary>
        /// Is Developer Console currently open
        /// </summary>
        public static bool IsConsoleOpen() {
            return consoleIsOpen;
        }

        /// <summary>
        /// Is Developer Console initilized
        /// </summary>
        public static bool IsConsoleInitialized() {
            return consoleInitialized;
        }

        /// <summary>
        /// Get current GUI style
        /// </summary>
        /// <returns></returns>
        public static ConsoleGUIStyle GetGUIStyle() {
            return settings.interfaceStyle;
        }

        public static bool PrintUnrecognizedCommandInfo() {
            return settings.printUnrecognizedCommandInfo;
        }

        public static bool AllowMultipleCommands() {
            return settings.allowMultipleCommands;
        }

        public static bool IsCaseSensetive() {
            return settings.caseSensetive;
        }

        /// <summary>
        /// Set new ConsoleSettings
        /// </summary>
        /// <param name="newsettings"></param>
        public static void SetSettings(ConsoleSettings newsettings) {
            if (newsettings == null) return;

            settings = newsettings;
            SetPredictions(settings.showInputPredictions);
            GUIStyleChanged(settings.interfaceStyle);
            if (settings.unityPrintOptions == PrintOptions.DontPrintDebugLogs) {
                Application.logMessageReceived -= UnityLogEventCallback;
            }

            ConsoleEvents.NewSettingsSet();
        }

        /// <summary>
        /// Set new ConsoleSettings
        /// </summary>
        /// <param name="newsettings"></param>
        public static void SetGUIStyle(ConsoleGUIStyle style) {
            if (style != GetGUIStyle()) {
                ToggleInterfaceStyle(false);
            }
        }

        private static string AddMessagePrefix(string prefixMessage, string msg) {
            sb.Clear();
            sb.Append(ConsoleConstants.OPENBRACKET);
            sb.Append(prefixMessage);
            sb.Append(ConsoleConstants.CLOSEDBRACKET);
            sb.Append(msg);

            return sb.ToString();
        }

        /// <summary>
        /// Print message to Developer Console
        /// </summary>
        public static void PrintLog(string text, Action<string> subscribers) {
            if (!Application.isPlaying || !IsRunningOnMainThread(Thread.CurrentThread)) return;

            if (settings.printMessageTimestamps) {
                text = AddMessagePrefix(DateTime.Now.ToString(ConsoleConstants.DATETIMEFORMAT), text);
            }

            if (!consoleInitialized) {
                messagesBeforeInitDone.Add(text);
                return;
            }

            if (subscribers != null && Application.isPlaying) {
                subscribers.Invoke(text);
            }
        }

        /// <summary>
        /// Toggle GUI style between Large and minimal
        /// </summary>
        public static void ToggleInterfaceStyle(bool reOpenConsole = true) {
            if (reOpenConsole) ConsoleEvents.CloseConsole();

            if (settings.interfaceStyle == ConsoleGUIStyle.Large) {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Minimal);
            }
            else {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Large);
            }

            if (reOpenConsole) ConsoleEvents.OpenConsole();
        }

        /// <summary>
        /// Check if we are in Unity main thread
        /// </summary>
        private static bool IsRunningOnMainThread(Thread thread) {
            return thread == UnityMainThreadID;
        }

        private static void RebindActivateKeyEvent(KeyCode key) {
            settings.consoleToggleKey = key;

            SetSettings(settings);
        }

        private static void GUIStyleChanged(ConsoleGUIStyle style) {
            settings.interfaceStyle = style;

            if (consoleInitialized) {
                CommandDatabase.UpdateLists();
                ConsoleEvents.RefreshConsole();
            }
        }

        private static void SetPredictions(bool show) {
            showSuggestions = show;
        }

        private static void ConsoleState(bool enabled) {
            consoleIsOpen = enabled;

            if (consoleIsOpen) {
                if (settings.showCursorOnEnable) {
                    ConsoleUtils.ShowCursor(true);
                }
            }
            else {
                if (settings.hideCursorOnDisable) {
                    ConsoleUtils.ShowCursor(false);
                }
            }
        }

        private static void UnityLogEventCallback(string text, string stackTrace, LogType type) {
            if (settings == null || settings.interfaceStyle == ConsoleGUIStyle.Minimal
                || settings.unityPrintOptions == PrintOptions.DontPrintDebugLogs) return;


            if (type == LogType.Error || type == LogType.Exception) {
                switch (settings.unityPrintOptions) {

                    case PrintOptions.PrintDebugLogsWithoutExpections:
                        return;

                    case PrintOptions.PrintDebugLogExpectionsWithStackTrace:
                        text = string.Format("{0}{1} {2} {3}", ConsoleConstants.COLOR_RED_START, text, stackTrace, ConsoleConstants.COLOR_END);
                        break;

                    case PrintOptions.PrintDebugLogsWithExpections:
                        text = string.Format("{0}{1} {2}", ConsoleConstants.COLOR_RED_START, text, ConsoleConstants.COLOR_END);
                        break;

                    case PrintOptions.PrintDebugLogsExpectionsWithStackTraceEditorOnly:
#if UNITY_EDITOR
                        text = string.Format("{0}{1} {2} {3}", ConsoleConstants.COLOR_RED_START, text, stackTrace, ConsoleConstants.COLOR_END);
#endif
                        break;

                    case PrintOptions.PrintDebugLogsWithExpectionsEditorOnly:
#if UNITY_EDITOR
                        text = string.Format("{0}{1} {2}", ConsoleConstants.COLOR_RED_START, text, ConsoleConstants.COLOR_END);
#endif
                        break;
                }
            }

            if (settings.printLogType && Debug.isDebugBuild) {
                text = AddMessagePrefix(LogTypes[type], text);
            }

            ConsoleEvents.Log(text);
            ++messageCount;
        }

        private static async void SceneLoadCallback(Scene scene, LoadSceneMode mode) {
            CommandDatabase.ClearConsoleCommands();

            if (initDone) ++sceneChangeCount; // don't raise counter on start

            if (sceneChangeCount != 0 && settings.clearMessagesOnSceneChange) {
                ConsoleEvents.ClearConsoleMessages();
            }

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            List<ConsoleCommandData> commands = null;
            bool registerStaticOnly = settings.registerStaticCommandAttributesOnly;

            if (registerStaticOnly && CommandDatabase.StaticCommandsRegistered()) {
                timer.Stop();
                return;
            }

            bool isDebugBuild = Debug.isDebugBuild;

#if UNITY_WEBGL
            commands = CommandDatabase.GetConsoleCommandAttributes(isDebugBuild, registerStaticOnly);
#else    
            commands = await Task.Run(() => CommandDatabase.GetConsoleCommandAttributes(isDebugBuild, registerStaticOnly)); // Threaded work
#endif

            timer.Stop();
            var ms = timer.Elapsed.TotalMilliseconds;
            timer.Reset();

            if (!registerStaticOnly) {
                timer.Start();
                CommandDatabase.RegisterMonoBehaviourCommands(commands); // Rest of the work must be in done in Unity main thread
                timer.Stop();
            }
            else {
                CommandDatabase.UpdateLists();
            }
            consoleInitialized = true;

            // Print all messages that were called before console wasn't fully initialized.
            // For example if Debug.Log/Console.Log was called in Awake..
            for (int i = 0; i < messagesBeforeInitDone.Count; i++) {
                if (!Application.isPlaying) continue;

                ConsoleEvents.DirectLog(messagesBeforeInitDone[i]);
                //Console.Log(messagesBeforeInitDone[i]);
            }
            messagesBeforeInitDone.Clear();

            var partOne = Math.Round(ms, 1);
            var partTwo = Math.Round(timer.Elapsed.TotalMilliseconds, 1);

            string staticOnly = string.Empty;

#if UNITY_EDITOR
            if (settings.printConsoleDebugInfo && settings.registerStaticCommandAttributesOnly) {
                staticOnly = "(static commands only) ";
            }
#endif
            if (settings.printConsoleDebugInfo && Debug.isDebugBuild) {

                string message = ConsoleConstants.CONSOLEINIT;
#if UNITY_WEBGL
                var total = partOne + partTwo;
                //Debug.Log(string.Format("Console Initialization work {0} took: {1} ms", staticOnly, total));
                message += string.Format("Initialization work {0} took: {1} ms", staticOnly, total);
                Debug.Log(message);
#else
                message += string.Format("Threaded work took: {1} ms {0}", staticOnly, partOne);

                if (!registerStaticOnly) {
                    message += string.Format("and non-threaded work took: {0} ms.", partTwo);
                }

                // Uncomment below line if you wish to log this into Developer Console instead
                //Console.Log(message);
                Debug.Log(message);
#endif
            }

            if (settings.printStartupHelpText) {
                Console.Log("Type 'help' and press Enter to print all available commands.");
            }

            ConsoleEvents.RefreshConsole();
        }
    }
}