using System.Collections.Generic;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class MessageTracker {

        private static List<string> messages;
        private static bool initDone = false;
        private static int currentIndex = 0;
        private static int maxMessageCount;

        /// <summary>
        /// Init Message Printer
        /// </summary>
        public static void Init() {
            if (initDone) return;

            var settings = ConsoleManager.GetSettings();
            if (settings == null || !settings.keepTrackOfMessages) {
                return;
            }

            currentIndex = 0;
            maxMessageCount = settings.maxMessageCount;
            messages = new List<string>();
            ConsoleEvents.RegisterDeveloperConsoleLogEvent += LogEvent;
            ConsoleEvents.RegisterConsoleClearEvent += ClearEvent;
            Application.quitting += OnDestroy;

            initDone = true;
        }

        private static void OnDestroy() {
            ConsoleEvents.RegisterDeveloperConsoleLogEvent -= LogEvent;
            ConsoleEvents.RegisterConsoleClearEvent -= ClearEvent;
            Application.quitting -= OnDestroy;

#if UNITY_EDITOR
            initDone = false;
#endif
        }

        private static void ClearEvent() {
            messages.Clear();
        }

        private static void LogEvent(string message, Color? arg2) {

            // Add new messages till maxMessageCount reached
            if (messages.Count <= maxMessageCount - 1) {
                messages.Add(message);
            }
            // After that replace message at current index
            else {
                messages[currentIndex] = message;
            }

            ++currentIndex;

            // Reset index after maxMessageCount reached
            if (currentIndex >= maxMessageCount) {
                currentIndex = 0;
            }
        }

        public static List<string> GetConsoleMessagesList() {
            if (messages == null) {
                messages = new List<string>();
            }

            return messages;
        }

        public static string[] GetConsoleMessagesArray() {
            if (messages == null) {
                messages = new List<string>();
            }

            return messages.ToArray();
        }

    }
}