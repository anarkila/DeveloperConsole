﻿using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class MessagePrinter {

        private static Dictionary<LogType, string> LogTypes = new Dictionary<LogType, string>();
        private static List<string> messagesBeforeInitDone = new List<string>();
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
        private static void Init() {
            if (initDone) return;

            foreach (LogType logType in Enum.GetValues(typeof(LogType))) {
                LogTypes.Add(logType, logType.ToString());
            }

            Application.logMessageReceived += UnityLogEvent;
            Application.quitting += OnDestroy;

            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;
            ConsoleEvents.RegisterConsoleRefreshEvent += Refresh;
            ConsoleEvents.RegisterConsoleInitializedEvent += ConsoleIsInitialized;
            initDone = true;
        }

        private static void ConsoleIsInitialized() {
            ConsoleEvents.RegisterConsoleInitializedEvent -= ConsoleIsInitialized;

            consoleInitialized = ConsoleManager.IsConsoleInitialized();
            if (consoleInitialized) {
                for (int i = 0; i < messagesBeforeInitDone.Count; i++) {
                    if (!Application.isPlaying) continue;

                    ConsoleEvents.DirectLog(messagesBeforeInitDone[i]);
                }
                messagesBeforeInitDone.Clear();
            }
        }

        private static void Refresh() {
            settings = ConsoleManager.GetSettings();
            consoleInitialized = ConsoleManager.IsConsoleInitialized();
        }

        /// <summary>
        /// OnDestroy callback from Unity Engine.
        /// </summary>
        private static void OnDestroy() {

            Application.logMessageReceived -= UnityLogEvent;
            Application.quitting -= OnDestroy;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= GUIStyleChanged;

#if UNITY_EDITOR
            if (settings != null && settings.printConsoleDebugInfo) {
                if (messageCount != 0) Debug.Log(string.Format("Debug.Log and Debug.LogError was called {0} times.", messageCount));
            }
#endif
        }

        private static void GUIStyleChanged(ConsoleGUIStyle newStyle) {
            currentGUIStyle = newStyle;
        }

        private static void UnityLogEvent(string input, string stackTrace, LogType type) {
            if (settings == null || settings.interfaceStyle == ConsoleGUIStyle.Minimal
                || settings.UnityLogOptions == ConsoleLogOptions.DontPrintLogs) return;

            if (type == LogType.Error || type == LogType.Exception) {
                input = AppendStrackTrace(input, stackTrace, settings.UnityLogOptions);
            }

            if (settings.printLogType && Debug.isDebugBuild) {
                input = AddMessagePrefix(LogTypes[type], input);
            }

            ConsoleEvents.Log(input);
            ++messageCount;
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
        public static void PrintLog(string text, Action<string> subscribers) {
            if (!ConsoleManager.IsRunningOnMainThread(Thread.CurrentThread)
                || !Application.isPlaying
                || currentGUIStyle == ConsoleGUIStyle.Minimal
                || subscribers == null) return;

            if (printMessageTimestamps) {
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
    }
}