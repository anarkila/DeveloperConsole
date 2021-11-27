﻿using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class listens Unity Debug.Log/LogError messages that were
    /// called from other than Unity main thread.
    /// Most of the Unity API's are not thread safe so we need to be sure we call
    /// Developer Console Log events from the Unity main thread!
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class ListenThreadedLogs : MonoBehaviour {

        private static ListenThreadedLogs Instance;

        private static ConsoleLogOptions logOption = ConsoleLogOptions.DontPrintLogs;
        private static List<string> messageBacklog = new List<string>(8);
        private static List<string> messages = new List<string>(8);
        private static volatile bool messagesQueued = false;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(this);
            }
            Application.logMessageReceivedThreaded += UnityLogEventThreaded;
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();
            if (settings == null) return;

            // Get current log option setting for threaded logs
            logOption = settings.unityThreadedLogOptions;

            // Don't print logs if ConsoleLogOptions.DontPrintLogs enum is selected
            if (logOption == ConsoleLogOptions.DontPrintLogs) {
                this.enabled = false;
                UnRegister();
            }
        }

        private void OnDestroy() {
            UnRegister();
            Instance = null;
        }

        private void UnRegister() {
            Application.logMessageReceivedThreaded -= UnityLogEventThreaded;
        }

        private void UnityLogEventThreaded(string message, string stackTrace, LogType type) {
            if (ConsoleManager.IsRunningOnMainThread(Thread.CurrentThread)) return;

            lock (messageBacklog) {
                if (type == LogType.Error || type == LogType.Exception) {
                    message = MessagePrinter.AppendStrackTrace(message, stackTrace, logOption);
                }
                messageBacklog.Add(message);
                messagesQueued = true;
            }
        }

        private void Update() {

            // early return if there's no messages to log!
            if (!messagesQueued) return;

            lock (messageBacklog) {
                var temp = messages;
                messages = messageBacklog;
                messageBacklog = temp;
                messagesQueued = false;
            }

            for (int i = 0; i < messages.Count; i++) {
                ConsoleEvents.Log(messages[i]);
            }
            messages.Clear();
        }
    }
}