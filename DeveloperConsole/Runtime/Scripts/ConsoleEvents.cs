using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    public static class ConsoleEvents {

        public static event Action<bool> RegisterConsoleStateChangeEvent;
        public static void OpenConsole() => RegisterConsoleStateChangeEvent?.Invoke(true);
        public static void CloseConsole() => RegisterConsoleStateChangeEvent?.Invoke(false);
        public static void SetConsoleState(bool enabled) => RegisterConsoleStateChangeEvent?.Invoke(enabled);

        public static event Action<bool> RegisterConsoleEnabledEvent;
        public static void EnableConsole() => RegisterConsoleEnabledEvent?.Invoke(true);
        public static void DisableConsole() => RegisterConsoleEnabledEvent?.Invoke(false);
        public static void SetConsoleEnabledState(bool enabled) => RegisterConsoleEnabledEvent?.Invoke(enabled);

        public static event Action RegisterInputFieldSubmit;
        public static void InputFieldSubmit() => RegisterInputFieldSubmit?.Invoke();

        public static event Action RegisterPreviousCommandEvent;
        public static void SearchPreviousCommand() => RegisterPreviousCommandEvent?.Invoke();

        public static event Action RegisterFillCommandEvent;
        public static void FillCommand() => RegisterFillCommandEvent?.Invoke();

        public static event Action<ConsoleGUIStyle> RegisterGUIStyleChangeEvent;
        public static void ChangeGUIStyle(ConsoleGUIStyle style) => RegisterGUIStyleChangeEvent?.Invoke(style);
        public static void SwitchGUIStyle() => ConsoleManager.ToggleInterfaceStyle();

        public static event Action<string, Color?> RegisterDeveloperConsoleLogEvent;
        public static void Log(string text, Color? textColor = null, bool forceIgnoreTimeStamp = false) => MessagePrinter.PrintLog(text, RegisterDeveloperConsoleLogEvent, textColor, false, forceIgnoreTimestamp:forceIgnoreTimeStamp);
        public static void UnityLog(string text, string stacktrace, LogType type, Color? textColor = null, bool forceIgnoreTimeStamp = false) => MessagePrinter.PrintLog(text, RegisterDeveloperConsoleLogEvent, textColor, true, type, stacktrace, forceIgnoreTimeStamp);

        public static event Action<List<string>> RegisterConsolePredictionEvent;
        public static void Predictions(List<string> list) => RegisterConsolePredictionEvent?.Invoke(list);

        public static event Action RegisterConsoleResetEvent;
        public static void ResetConsole() => RegisterConsoleResetEvent?.Invoke();

        public static event Action RegisterConsoleClearEvent;
        public static void ClearConsoleMessages() => RegisterConsoleClearEvent?.Invoke();

        public static event Action RegisterListsChangedEvent;
        public static void ListsChanged() => RegisterListsChangedEvent?.Invoke();

        public static event Action RegisterSettingsChangedEvent;
        public static void NewSettingsSet() => RegisterSettingsChangedEvent?.Invoke();

        public static event Action RegisterConsoleInitializedEvent;
        public static void ConsoleInitialized() => RegisterConsoleInitializedEvent?.Invoke();

        public static event Action RegisterConsoleScrollMoveEvent;
        public static void ScrollToBottom() => RegisterConsoleScrollMoveEvent?.Invoke();

        public static event Action<string> RegisterSceneLoadByName;
        public static void LoadSceneByName(string name) => RegisterSceneLoadByName?.Invoke(name);

        public static event Action<string> RegisterSceneUnLoadByName;
        public static void UnLoadSceneByName(string name) => RegisterSceneUnLoadByName?.Invoke(name);

        public static event Action<int, UnityEngine.SceneManagement.LoadSceneMode> RegisterSceneLoadByIndex;
        public static void LoadSceneByIndexSingle(int index) => RegisterSceneLoadByIndex?.Invoke(index, UnityEngine.SceneManagement.LoadSceneMode.Single);
        public static void LoadSceneByIndexAdditive(int index) => RegisterSceneLoadByIndex?.Invoke(index, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        public static event Action<int> RegisterSceneUnLoadByIndex;
        public static void UnloadSceneByIndex(int index) => RegisterSceneUnLoadByIndex?.Invoke(index);

        public static event Action<bool> RegisterListenActivatStateEvent;
        public static void SetListenActivateKeyState(bool enabled) => RegisterListenActivatStateEvent?.Invoke(enabled);

        public static event Action<KeyCode> RegisterConsoleActivateKeyChangeEvent;
        public static void ChangeActivateKeyCode(KeyCode key) => RegisterConsoleActivateKeyChangeEvent?.Invoke(key);

        public static event Action<float> RegisterDestroyEvent;
        public static void DestroyConsole(float time) => RegisterDestroyEvent?.Invoke(time);

        public static event Action<string> RegisterInputfieldTextEvent;
        public static void SetInputfieldText(string text) => RegisterInputfieldTextEvent?.Invoke(text);

        public static event Action RegisterConsoleColorsChangedEvent;
        public static void ConsoleColorsChanged() => RegisterConsoleColorsChangedEvent?.Invoke();

        public static event Action<ConsoleLogOptions> RegisterConsoleLogOptionsChanged;
        public static void ChangeConsoleLogOptions(ConsoleLogOptions logOption) => RegisterConsoleLogOptionsChanged?.Invoke(logOption);

        public static event Action<ConsoleLogOptions> RegisterConsoleThreadedLogOptionsChanged;
        public static void ChangeConsoleThreadedLogOptions(ConsoleLogOptions logOption) => RegisterConsoleThreadedLogOptionsChanged?.Invoke(logOption);

        public static event Action<bool> RegisterInputPredctionChanged;
        public static void ShowInputPredictions(bool showPredictions) => RegisterInputPredctionChanged?.Invoke(showPredictions);

        public static event Action<GameObject, PointerEventData, string> RegisterOnContextMenuShow;
        public static void ShowContextMenu(GameObject go, PointerEventData data, string msg) => RegisterOnContextMenuShow?.Invoke(go, data, msg);

        public static event Action RegisterOnMessageDelete;
        public static void MessageDeleted() => RegisterOnMessageDelete?.Invoke();

        public static event Action<bool> RegisterOnCommandExecuted;
        public static void CommandExecuted(bool success) => RegisterOnCommandExecuted?.Invoke(success);

    }
}