using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class MessagePrinter {

        private static Dictionary<LogType, string> LogTypes = new Dictionary<LogType, string>();
        private static List<TempMessage> messagesBeforeInitDone = new List<TempMessage>(32);
        private static ConsoleGUIStyle currentGUIStyle = ConsoleGUIStyle.Large;
        private static ConsoleSettings settings = new ConsoleSettings();
        private static StringBuilder sb = new StringBuilder();
        private static bool printMessageTimestamps = true;
        private static bool consoleInitialized = false;
        private static bool initDone = false;
        private static int messageCount;

        /// <summary>
        /// Init Message Printer
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() {
            if (initDone) return;

            foreach (LogType logType in Enum.GetValues(typeof(LogType))) {
                LogTypes.Add(logType, logType.ToString());
            }

            Application.logMessageReceived += UnityLogEvent;
            Application.quitting += OnDestroy;
            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;
            ConsoleEvents.RegisterSettingsChangedEvent += GetSettings;
            ConsoleEvents.RegisterConsoleInitializedEvent += ConsoleIsInitialized;
            initDone = true;
        }


        private static void ConsoleIsInitialized() {
            consoleInitialized = ConsoleManager.IsConsoleInitialized();
            GetSettings();

            if (messagesBeforeInitDone.Count != 0) {
                // Add slight delay before logging messages
                ConsoleUtils.DelayedCall(() => {
                    for (int i = 0; i < messagesBeforeInitDone.Count; i++) {
                        ConsoleEvents.Log(messagesBeforeInitDone[i].message, messagesBeforeInitDone[i].messageColor, forceIgnoreTimeStamp: true);
                    }
                    messagesBeforeInitDone.Clear();
                }, 0.1f);
            }
        }

        private static void GetSettings() {
            settings = ConsoleManager.GetSettings();
            printMessageTimestamps = settings.printMessageTimestamps;
        }

        /// <summary>
        /// OnDestroy callback from Unity Engine.
        /// </summary>
        private static void OnDestroy() {
            Application.logMessageReceived -= UnityLogEvent;
            Application.quitting -= OnDestroy;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= GUIStyleChanged;
            ConsoleEvents.RegisterSettingsChangedEvent -= GetSettings;
            ConsoleEvents.RegisterConsoleInitializedEvent -= ConsoleIsInitialized;

#if UNITY_EDITOR
            if (settings != null && settings.printMessageCount) {
                if (messageCount != 0) Debug.Log(string.Format("Debug.Log and Debug.LogError was called {0} times.", messageCount));
            }

            // for domain reload purposes
            LogTypes.Clear();
            messagesBeforeInitDone.Clear();
            currentGUIStyle = ConsoleGUIStyle.Large;
            sb = new StringBuilder();
            printMessageTimestamps = true;
            consoleInitialized = false;
            initDone = false;
            messageCount = 0;
#endif
        }

        private static void GUIStyleChanged(ConsoleGUIStyle newStyle) {
            currentGUIStyle = newStyle;
        }

        private static void UnityLogEvent(string input, string stackTrace, LogType type) {
            ConsoleEvents.UnityLog(input, stackTrace, type, settings.defaultMessageTextColor);
        }

        /// <summary>
        /// Add message prefix
        /// </summary>
        public static string AddMessagePrefix(string prefixMessage, string msg) {
            sb.Clear();
            sb.Append(ConsoleConstants.OPENBRACKET);
            sb.Append(prefixMessage);
            sb.Append(ConsoleConstants.CLOSEDBRACKET);
            sb.Append(msg);

            return sb.ToString();
        }

        /// <summary>
        /// Append stack trace into string depending on print setting
        /// </summary>
        public static string AppendStrackTrace(string message, string stackTrace, ConsoleLogOptions printOption) {
            switch (printOption) {
                case ConsoleLogOptions.LogWithoutExpections:
                    return message;

                case ConsoleLogOptions.LogExpectionsWithStackTrace:
                    return string.Format("{0}{1} {2} {3}", ConsoleConstants.COLOR_RED_START, message, stackTrace, ConsoleConstants.COLOR_END);

                case ConsoleLogOptions.LogWithExpections:
                    return string.Format("{0}{1} {2}", ConsoleConstants.COLOR_RED_START, message, ConsoleConstants.COLOR_END);

                case ConsoleLogOptions.LogExpectionsWithStackTraceEditorOnly:
#if UNITY_EDITOR
                    return string.Format("{0}{1} {2} {3}", ConsoleConstants.COLOR_RED_START, message, stackTrace, ConsoleConstants.COLOR_END);
#endif
                case ConsoleLogOptions.LogWithExpectionsEditorOnly:
#if UNITY_EDITOR
                    return string.Format("{0}{1} {2}", ConsoleConstants.COLOR_RED_START, message, ConsoleConstants.COLOR_END);
#endif
                default:
                    return message;
            }
        }

        /// <summary>
        /// Print message to Developer Console
        /// </summary>
        public static void PrintLog(string text, Action<string, Color?> subscribers, Color? textColor = null,
            bool appendStackTrace = false, LogType type = LogType.Error, string stackTrace = "",
            bool forceIgnoreTimestamp = false) {

            if (!ConsoleManager.IsRunningOnMainThread(Thread.CurrentThread)
                || !Application.isPlaying
                || currentGUIStyle == ConsoleGUIStyle.Minimal
                || settings.UnityLogOption == ConsoleLogOptions.DontPrintLogs) return;

            if (appendStackTrace) {
                if (type == LogType.Error || type == LogType.Exception) {
                    text = AppendStrackTrace(text, stackTrace, settings.UnityLogOption);
                }

                if (settings.printLogType && Debug.isDebugBuild) {
                    text = AddMessagePrefix(LogTypes[type], text);
                }
            }

            if (!forceIgnoreTimestamp && printMessageTimestamps) {
                text = AddMessagePrefix(DateTime.Now.ToString(ConsoleConstants.DATETIMEFORMAT), text);
            }

            if (!consoleInitialized) {
                var temp = new TempMessage();
                temp.message = text;
                temp.messageColor = textColor;
                messagesBeforeInitDone.Add(temp);
                return;
            }

            if (subscribers != null && Application.isPlaying) {
                subscribers.Invoke(text, textColor);
            }
        }
    }
}