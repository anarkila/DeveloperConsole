﻿using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class ConsoleManager {

        private static ConsoleSettings settings = new ConsoleSettings();
        private static bool showInputPredictions = true;
        private static bool consoleInitialized = false;
        private static int sceneChangeCount = 0;
        private static Thread UnityMainThreadID;
        private static bool initDone = false;
        private static bool consoleIsOpen;

        /// <summary>
        /// Initilize Developer Console
        /// </summary>
        public static void InitializeDeveloperConsole(ConsoleSettings settings, Thread thread) {
            if (initDone) return;

            UnityMainThreadID = thread;

            Application.quitting += OnDestroy;
            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent += RebindActivateKeyEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;
            ConsoleEvents.RegisterDestroyEvent += ConsoleDestroyed;
#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent += ConsoleClearEvent;
#endif

            MessagePrinter.Init();
            SetSettings(settings);
            RegisterCommands(logMessage: false);

            // Register to sceneloaded callback from UnityEngine after small delay
            // otherwise RegisterCommand might get called twice.
            ConsoleUtils.DelayedCall(() =>
            {
                SceneManager.sceneLoaded += SceneLoadCallback;
            }, 0.2f);

            initDone = true;
        }

        /// <summary>
        /// OnDestroy callback from Unity Engine.
        /// </summary>
        private static void OnDestroy() {
            SceneManager.sceneLoaded -= SceneLoadCallback;
            Application.quitting -= OnDestroy;

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent -= RebindActivateKeyEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= GUIStyleChanged;
            ConsoleEvents.RegisterDestroyEvent -= ConsoleDestroyed;

#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent -= ConsoleClearEvent;
#endif

#if UNITY_EDITOR
            if (settings != null && settings.printMessageCount) {
                var executedCommandCount = CommandDatabase.GetExcecutedCommandCount();
                var failedCommandCount = CommandDatabase.GetFailedCommandCount();

                if (executedCommandCount != 0) Debug.Log(string.Format("Console Commands was called {0} times.", executedCommandCount));
                if (failedCommandCount != 0) Debug.Log(string.Format("Failed or not recognized commands was called {0} times.", failedCommandCount));
            }

            // for domain reload purposes
            showInputPredictions = true;
            consoleInitialized = false;
            consoleIsOpen = false;
            sceneChangeCount = 0;
            initDone = false;
#endif
        }

        private static void ConsoleDestroyed(float obj) {
            OnDestroy();
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
        public static ConsoleSettings GetSettings() {
            return settings;
        }

        /// <summary>
        /// Show Console Predictions
        /// </summary>
        public static bool ShowConsolePredictions() {
            return showInputPredictions;
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
        public static ConsoleGUIStyle GetGUIStyle() {
            return settings.interfaceStyle;
        }

        /// <summary>
        /// Get current setting printUnrecognizedCommandInfo
        /// </summary>
        public static bool PrintUnrecognizedCommandInfo() {
            return settings.printUnrecognizedCommandInfo;
        }

        /// <summary>
        /// Get current setting allowMultipleCommands
        /// </summary>
        public static bool AllowMultipleCommands() {
            return settings.allowMultipleCommands;
        }

        /// <summary>
        /// Get current setting caseSensetive
        /// </summary>
        public static bool IsCaseSensetive() {
            return settings.commandsAreCaseSensetive;
        }

        /// <summary>
        /// Set new ConsoleSettings
        /// </summary>
        public static void SetSettings(ConsoleSettings newsettings) {
            if (newsettings == null) return;

            settings = newsettings;
            showInputPredictions = settings.showInputPredictions;
            GUIStyleChanged(settings.interfaceStyle);
            ConsoleEvents.NewSettingsSet();
        }

        /// <summary>
        /// Set GUI style to use
        /// </summary>
        public static void SetGUIStyle(ConsoleGUIStyle style) {
            if (style != GetGUIStyle()) {
                ToggleInterfaceStyle(false);
            }
        }

        /// <summary>
        /// Toggle GUI style between Large and minimal
        /// </summary>
        public static void ToggleInterfaceStyle(bool reOpen = true) {
            if (!settings.allowGUIChangeRuntime) return;

            if (reOpen) ConsoleEvents.CloseConsole();

            if (settings.interfaceStyle == ConsoleGUIStyle.Large) {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Minimal);
            }
            else {
                ConsoleEvents.ChangeGUIStyle(ConsoleGUIStyle.Large);
            }

            if (reOpen) ConsoleEvents.OpenConsole();
        }

        /// <summary>
        /// Check if we are in Unity main thread
        /// </summary>
        public static bool IsRunningOnMainThread(Thread thread) {
            if (UnityMainThreadID == null) return false;

            return thread == UnityMainThreadID;
        }

        /// <summary>
        /// Rebind console activate key (default §)
        /// </summary>
        /// <param name="key"></param>
        private static void RebindActivateKeyEvent(KeyCode key) {
            settings.consoleToggleKey = key;
            SetSettings(settings);
        }

        private static void GUIStyleChanged(ConsoleGUIStyle style) {
            settings.interfaceStyle = style;
            if (consoleInitialized) {
                CommandDatabase.UpdateLists();
                ConsoleEvents.ListsChanged();
            }
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

        private static void SceneLoadCallback(Scene scene, LoadSceneMode mode) {
            RegisterCommands(scene.name, mode);
        }

        private static void RegisterCommands(string sceneName = "", LoadSceneMode mode = LoadSceneMode.Single, bool logMessage = true) {
            consoleInitialized = false;

            if (logMessage) {
                if (!settings.clearMessagesOnSceneChange) {
                    ConsoleEvents.Log(ConsoleConstants.SPACE, forceIgnoreTimeStamp: true);
                }
                else {
                    ConsoleEvents.ClearConsoleMessages();
                }

                if (settings.printLoadedSceneName) {
                    // https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.html
                    string additive = mode == LoadSceneMode.Additive ? "(Additive)" : null;
                    Console.Log(string.Format("Loaded Scene: [{0}] {1}", sceneName, additive));
                }
            }
            else if (!settings.printPlayButtonToSceneTime && settings.printLoadedSceneName) {
                Console.Log(string.Format("Loaded Scene: [{0}]", sceneName));
            }

            ++sceneChangeCount;

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            List<ConsoleCommandData> commands = null;
            bool registerStaticOnly = settings.registerStaticCommandsOnly;

            if (registerStaticOnly && CommandDatabase.StaticCommandsRegistered()) {
                timer.Stop();
                return;
            }

            bool isDebugBuild = Debug.isDebugBuild;
            bool scanAllAssemblies = settings.scanAllAssemblies;

#if UNITY_EDITOR
            if (scanAllAssemblies) {
                Debug.Log(ConsoleConstants.EDITORWARNING + "option ScanAllAssemblies is set to true. This increases initialization time.");
            }
#endif

#if UNITY_WEBGL
            commands = CommandDatabase.GetConsoleCommandAttributes(isDebugBuild, registerStaticOnly, scanAllAssemblies);
#else    
            commands = CommandDatabase.GetConsoleCommandAttributes(isDebugBuild, registerStaticOnly, scanAllAssemblies);
#endif
            if (!registerStaticOnly) {
                CommandDatabase.RegisterMonoBehaviourCommands(commands); // Rest of the work must be in done in Unity main thread
                timer.Stop();
            }
            else {
                CommandDatabase.UpdateLists();
            }

            var time = Math.Round(timer.Elapsed.TotalMilliseconds, 1);
            string suffix = ".";

            if (settings.printInitializationTime) {
#if UNITY_EDITOR
                if (settings.registerStaticCommandsOnly) {
                    suffix = ConsoleConstants.REGISTEREDSTATIC;
                }
#endif
                string message = ConsoleConstants.CONSOLEINIT;
#if UNITY_WEBGL
                message += string.Format("Initialization took: {0} ms {1}", time, suffix);
                ConsoleEvents.Log(message);
#else
                message += string.Format("Initialization took {0} ms{1}", time, suffix);
                ConsoleEvents.Log(message);
#endif
            }

            if (settings.printStartupHelpText) {
                ConsoleEvents.Log(ConsoleConstants.HELPTEXT);
            }

            consoleInitialized = true;

            ConsoleEvents.ConsoleInitialized();
            ConsoleEvents.ListsChanged();
        }
    }
}