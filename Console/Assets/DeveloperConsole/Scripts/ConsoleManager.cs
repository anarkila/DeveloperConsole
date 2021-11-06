using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using UnityEngine;
using System;

namespace DeveloperConsole {

    public static class ConsoleManager {

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

            UnityMainThreadID = thread;

            Application.logMessageReceived += UnityLogEventCallback;                    // Get logMessageReceived callback from Unity Engine
            SceneManager.sceneLoaded += SceneLoadCallback;                              // Register OnSceneChanged  event from Unity Engine
            Application.quitting += OnDestroy;                                          // Register ApplicationQuit event from Unity Engine

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent += RebindActivateKeyEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;

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
            if (settings.printConsoleDebugInfo) {

                var executedCommandCount = CommandDatabase.GetExcecutedCommandCount();
                var failedCommandCount = CommandDatabase.GetFailedCommandCount();

                if (messageCount != 0) Debug.Log(string.Format("Debug.Log/LogError was called {0} times.", messageCount));
                if (executedCommandCount != 0) Debug.Log(string.Format("Console Commands was called {0} times.", executedCommandCount));
                if (failedCommandCount != 0) Debug.Log(string.Format("Failed / not recognized commands was called {0} times.", failedCommandCount));
            }
#endif
        }

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
            return settings.InterfaceStyle;
        }

        /// <summary>
        /// Set new ConsoleSettings
        /// </summary>
        /// <param name="newsettings"></param>
        public static void SetSettings(ConsoleSettings newsettings) {
            if (newsettings == null) return;

            settings = newsettings;
            SetPredictions(settings.showInputPredictions);
            GUIStyleChanged(settings.InterfaceStyle);
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

        /// <summary>
        /// Print message to Developer Console
        /// </summary>
        public static void PrintLog(string text, Action<string> subscribers) {
            if (!Application.isPlaying) return;

            if (!IsUnityThread(Thread.CurrentThread)) return;

            if (!consoleInitialized) {
                messagesBeforeInitDone.Add(text);
                return;
            }

            if (settings.printMessageTimestamps) {
                sb.Clear();
                sb.Append(ConsoleConstants.OPENBRACKET);
                sb.Append(DateTime.Now.ToString(ConsoleConstants.DATETIMEFORMAT));
                sb.Append(ConsoleConstants.CLOSEDBRACKET);
                sb.Append(text);
                text = sb.ToString();
            }

            subscribers.Invoke(text);
        }

        /// <summary>
        /// Toggle GUI style between Large and minimal
        /// </summary>
        public static void ToggleInterfaceStyle(bool reOpenConsole = true) {
            if (reOpenConsole) ConsoleEvents.CloseConsole();

            if (settings.InterfaceStyle == ConsoleGUIStyle.Large) {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Minimal);
            }
            else {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Large);
            }

            if (reOpenConsole) ConsoleEvents.OpenConsole();
        }

        /// <summary>
        /// Checks if we are in Unity main thread
        /// </summary>
        private static bool IsUnityThread(Thread thread) {
            return thread == UnityMainThreadID;
        }

        private static void RebindActivateKeyEvent(KeyCode key) {
            settings.consoleToggleKey = key;

            SetSettings(settings);
        }

        private static void GUIStyleChanged(ConsoleGUIStyle style) {
            settings.InterfaceStyle = style;

            if (consoleInitialized) {
                CommandDatabase.UpdateLists();
                ConsoleEvents.ConsoleRefresh();
            }

          
        }

        private static void SetPredictions(bool show) {
            showSuggestions = show;
        }

        private static void ConsoleState(bool enabled) {
            consoleIsOpen = enabled;

            if (consoleIsOpen && settings.showCursorOnEnable) {
                ConsoleUtils.ShowCursor(true);
            }
        }

        private static void UnityLogEventCallback(string text, string stackTrace, LogType type) {
            if (settings == null || settings.InterfaceStyle == ConsoleGUIStyle.Minimal || settings.unityPrintOptions == PrintOptions.DontPrintDebugLogs) return; // minimal console doesn't have logging capability

            if (type != LogType.Log) {
                switch (settings.unityPrintOptions) {

                    case PrintOptions.PrintDebugLogsWithoutExpections:
                        return;
                    case PrintOptions.PrintDebugLogExpectionsWithStackTrace:
                        text = string.Format("<color=red>{0} {1} </color>", text, stackTrace);
                        break;
                    case PrintOptions.PrintDebugLogsWithExpections:
                        text = string.Format("<color=red>{0} </color>", text);
                        break;

                    case PrintOptions.PrintDebugLogsExpectionsWithStackTraceEditorOnly:
#if UNITY_EDITOR
                        text = string.Format("<color=red>{0} {1} </color>", text, stackTrace);
#endif
                        break;
                    case PrintOptions.PrintDebugLogsWithExpectionsEditorOnly:
#if UNITY_EDITOR
                        text = string.Format("<color=red>{0} </color>", text);
#endif
                        break;
                }
            }

            ConsoleEvents.Log(text);
            ++messageCount;
        }

        private static async void SceneLoadCallback(Scene scene, LoadSceneMode mode) {
            // TODO: additive scenes? This could get slow..

            CommandDatabase.ClearLists();

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

#if UNITY_WEBGL
            commands = GetConsoleCommandAttributes(registerStaticOnly);
#else    
            commands = await Task.Run(() => CommandDatabase.GetConsoleCommandAttributes(registerStaticOnly)); // Threaded work
#endif

            timer.Stop();
            var ms = timer.Elapsed.TotalMilliseconds;
            timer.Reset();

            if (!registerStaticOnly) {
                timer.Start();
                CommandDatabase.RegisterCommandsPartTwo(commands); // Rest of the work must be in done in Unity main thread
                timer.Stop();
            }
            else {
                CommandDatabase.UpdateLists();
            }
            consoleInitialized = true;

            var partOne = Math.Round(ms, 1);
            var partTwo = Math.Round(timer.Elapsed.TotalMilliseconds, 1);

            string staticOnly = string.Empty;

#if UNITY_EDITOR
            if (settings.printEditorDebugInfo && settings.registerStaticCommandAttributesOnly) {
                staticOnly = "(static commands only) ";
            }
#endif
            if (settings.printConsoleDebugInfo) {

                Debug.Log("Console Initialized.");
#if UNITY_WEBGL
                var total = partOne + partTwo;
                Debug.Log(string.Format("Console Initialization work {0} took: {1} ms", staticOnly, total));
#else
                Debug.Log(string.Format("Console Initialization work (Threaded) {0}took: {1} ms", staticOnly, partOne));

                if (!registerStaticOnly) {
                    Debug.Log(string.Format("Console Initialization work (Non-threaded) took: {0} ms", partTwo));
                }
#endif
            }

            if (settings.printHelpTextOnStartup) {
                Debug.Log("Type 'help' and press Enter to print all available console commands.");
            }

            // Print all messages that were called before console wasn't fully initialized.
            // For example if Debug.Log/Console.Log was called in Awake..
            for (int i = 0; i < messagesBeforeInitDone.Count; i++) {
                Console.Log(messagesBeforeInitDone[i]);
            }
            messagesBeforeInitDone.Clear();

            ConsoleEvents.ConsoleRefresh();
        }
    }
}