using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 1998
    public static class ConsoleManager {

        private static ConsoleSettings settings = new ConsoleSettings();
        private static bool registeredSceneCallback = false;
        private static bool consoleInitialized = false;
        private static bool consoleIsEnabled = true;
        private static Thread UnityMainThreadID;
        private static bool initDone = false;
        private static bool consoleIsOpen;

        /// <summary>
        /// Initialize these regardless whether DeveloperConsole.prefab exists in the scene or not.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            if (initDone) return;

            UnityMainThreadID = System.Threading.Thread.CurrentThread;
            Application.quitting += OnDestroy;
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleState;
        }

        /// <summary>
        /// Initialize Developer Console
        /// </summary>
        public static void InitializeDeveloperConsole(ConsoleSettings settings) {
            if (initDone) return;

            MessageTracker.Init();
            MessagePrinter.Init();

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent += RebindActivateKeyEvent;
            ConsoleEvents.RegisterInputPredctionChanged += InputPredictionSettingChanged;
            ConsoleEvents.RegisterConsoleLogOptionsChanged += LogOptionsChanged;
            ConsoleEvents.RegisterConsoleEnabledEvent += ConsoleEnabledChanged;
            ConsoleEvents.RegisterGUIStyleChangeEvent += GUIStyleChanged;
            ConsoleEvents.RegisterDestroyEvent += ConsoleDestroyed;

#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent += ClearUnityConsole;
#endif
            SetSettings(settings);
            RegisterCommands(logMessage: false);
           
            initDone = true;
        }

        /// <summary>
        /// OnDestroy callback from Unity Engine.
        /// </summary>
        private static void OnDestroy() {
            SceneManager.sceneLoaded -= SceneLoadCallback;
            Application.quitting -= OnDestroy;

            ConsoleEvents.RegisterConsoleActivateKeyChangeEvent -= RebindActivateKeyEvent;
            ConsoleEvents.RegisterInputPredctionChanged -= InputPredictionSettingChanged;
            ConsoleEvents.RegisterConsoleLogOptionsChanged -= LogOptionsChanged;
            ConsoleEvents.RegisterConsoleEnabledEvent -= ConsoleEnabledChanged;
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleState;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= GUIStyleChanged;
            ConsoleEvents.RegisterDestroyEvent -= ConsoleDestroyed;

#if UNITY_EDITOR
            ConsoleEvents.RegisterConsoleClearEvent -= ClearUnityConsole;
#endif

#if UNITY_EDITOR
            if (settings != null && settings.printMessageCountOnStopPlay) {
                var executedCommandCount = CommandDatabase.GetExcecutedCommandCount();
                var failedCommandCount = CommandDatabase.GetFailedCommandCount();

                if (executedCommandCount != 0) Debug.Log(string.Format("Successfully called Console Commands {0} times.", executedCommandCount));
                if (failedCommandCount != 0) Debug.Log(string.Format("Failed to execute {0} Console Commands.", failedCommandCount));
            }

            // for domain reload purposes
            registeredSceneCallback = false;
            consoleInitialized = false;
            consoleIsOpen = false;
            initDone = false;
#endif
        }

        private static void ConsoleEnabledChanged(bool enabled) {
            consoleIsEnabled = enabled;
        }

        private static void ConsoleDestroyed(float time) {
            OnDestroy();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Clear Unity Editor console
        /// </summary>
        private static void ClearUnityConsole() {
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
            if (settings == null) {
#if UNITY_EDITOR
                Debug.LogError("ConsoleSettings is null! Creating new settings..");
#endif
                settings = new ConsoleSettings();
            }

            return settings;
        }

        private static void InputPredictionSettingChanged(bool showPredictions) {
            settings.showInputPredictions = showPredictions;
        }

        /// <summary>
        /// Is Console enabled and can be opened
        /// </summary>
        public static bool IsConsoleEnabled() {
            return consoleIsEnabled;
        }

        /// <summary>
        /// Show Console Predictions
        /// </summary>
        public static bool ShowConsolePredictions() {
            return settings.showInputPredictions;
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
        /// Should duplicate commands be tracked
        /// </summary>
        public static bool TrackDuplicates() {
            return settings.trackDuplicateCommands;
        }

        /// <summary>
        /// Should failed commands we tracked
        /// </summary>
        public static bool TrackFailedCommands() {
            return settings.trackFailedCommands;
        }

        /// <summary>
        /// Get current setting caseSensetive
        /// </summary>
        public static bool IsCaseSensetive() {
            return settings.commandsAreCaseSensetive;
        }

        private static void LogOptionsChanged(ConsoleLogOptions logOption) {
            if (!consoleInitialized) {
#if UNITY_EDITOR
                Debug.LogError("Console is not yet initialized.");
#endif
                return;
            }
            settings.UnityLogOption = logOption;
            ConsoleEvents.NewSettingsSet();
        }

        /// <summary>
        /// Set new ConsoleSettings
        /// </summary>
        public static void SetSettings(ConsoleSettings newsettings) {
            if (newsettings == null) {
#if UNITY_EDITOR
                Debug.LogError("Trying to set new ConsoleSettings but it's null!");
#endif
                return;
            }

            settings = newsettings;
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
        /// Set GUI theme to use and apply colors
        /// </summary>
        public static void SetGUITheme(ConsoleGUITheme theme) {
            if (settings == null || theme == ConsoleGUITheme.Custom) return;

            settings.interfaceTheme = theme;
            settings.ApplyColors();
            ConsoleEvents.ConsoleColorsChanged();
        }

        /// <summary>
        /// Set custom GUI colors
        /// </summary>
        public static void SetCustomGUITheme(ConsoleColors newColors) {
            if (settings == null) return;

            settings.interfaceTheme = ConsoleGUITheme.Custom;
            settings.consoleColors = newColors;
            ConsoleEvents.ConsoleColorsChanged();
        }

        /// <summary>
        /// Toggle GUI style between Large and minimal
        /// </summary>
        public static void ToggleInterfaceStyle(bool openConsole = true) {
            if (!settings.allowGUIStyleChangeRuntime) return;

            var style = settings.interfaceStyle == ConsoleGUIStyle.Large ? ConsoleGUIStyle.Minimal : ConsoleGUIStyle.Large;
            if (openConsole) {
                ConsoleEvents.CloseConsole();
                ConsoleEvents.ChangeGUIStyle(style);
                ConsoleEvents.OpenConsole();
            }
            else {
                ConsoleEvents.ChangeGUIStyle(style);
            }
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
            if (settings.initializeConsoleOnFirstOpen && !consoleInitialized) {
                InitializeDeveloperConsole(settings);
            }

            if (!consoleInitialized) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "Console is not yet initialized. Make sure DeveloperConsole.prefab exists in the scene.");
#endif
                return;
            }

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
                CommandDatabase.RegisterMonoBehaviourCommands(commands);
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
                message += string.Format("Initialization took {0} ms{1}", time, suffix);
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

            var delayUtil = DelayHelper.GetInstance();
            if (delayUtil != null) {
                // Add 10 frame artificial delay before calling console is initialized events
                delayUtil.DelayedCallFrames(NotifyConsoleIsReady, 10);
            }
            else {
                // just in case DelayHelper Instance is null for whatever reason, let's call this without delay
                // this might cause some messages not to be printed that were called on Awake/Start but otherwise console should work fine.
#if UNITY_EDITOR
                Debug.Log("DelayHelper Instance is null. Initializing without delay. You might miss some messages that were called in Awake/Start methods.");
#endif
                NotifyConsoleIsReady();
            }
        }

        private static void NotifyConsoleIsReady() {
            // Check if console was destroyed during those 10 frames
            if (!consoleInitialized) return;
  
            ConsoleEvents.ConsoleInitialized();
            ConsoleEvents.ListsChanged();

            if (!registeredSceneCallback) {
                SceneManager.sceneLoaded += SceneLoadCallback;
                registeredSceneCallback = true;
            }
        }
    }
}