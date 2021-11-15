using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script handles listening key inputs:
    /// - enable/disable Developer Console  (default: § or ½ key below ESC)
    /// - submit current inputfield text    (default: enter)
    /// - fill from prediction              (default: tab and down arrow)
    /// - fill inputfield with previous     (default: up arrow)
    /// </summary>
    public class ConsoleKeyInputs : MonoBehaviour {

        private KeyCode consoleToggleKey = KeyCode.Backslash;
        private KeyCode submitKey = KeyCode.Return;
        private KeyCode searchPreviousCommand = KeyCode.UpArrow;
        private KeyCode fillCommand = KeyCode.DownArrow;
        private KeyCode fillCommandAlt = KeyCode.Tab;
        private bool checkActivatorKey = true;
        private bool consoleIsOpen = false;
       
        private void Start() {
            GetSettings();
            ConsoleEvents.RegisterListenActivatStateEvent += ActivatorStateChangeEvent;
            ConsoleEvents.RegisterSettingsChangedEvent += GetSettings;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterListenActivatStateEvent -= ActivatorStateChangeEvent;
            ConsoleEvents.RegisterSettingsChangedEvent -= GetSettings;
        }

        private void Update() {
            ListenPlayerInputs();
        }

        private void ListenPlayerInputs() {
            if (Input.GetKeyDown(consoleToggleKey) && checkActivatorKey) {
                consoleIsOpen ^= true;
                ConsoleEvents.SetConsoleState(consoleIsOpen);
            }

            if (!checkActivatorKey) {
                consoleIsOpen = ConsoleManager.IsConsoleOpen();
            }

            if (!consoleIsOpen) return;   // If console is not open then don't check other input keys

            if (Input.GetKeyDown(submitKey)) {
                ConsoleEvents.InputFieldSubmit();
            }

            if (Input.GetKeyDown(searchPreviousCommand)) {
                ConsoleEvents.SearchPreviousCommand();
            }

            if (Input.GetKeyDown(fillCommand) || Input.GetKeyDown(fillCommandAlt)) {
                ConsoleEvents.FillCommand();
            }
        }

        private void ActivatorStateChangeEvent(bool enabled) {
            checkActivatorKey = enabled;
        }

        private void GetSettings() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null) {
                searchPreviousCommand = settings.consoleSearchCommandKey;
                fillCommandAlt = settings.ConsoleFillCommandKeyAlt;
                fillCommand = settings.consoleFillCommandKey;
                consoleToggleKey = settings.consoleToggleKey;
                submitKey = settings.consoleSubmitKey;
            }
        }
    }
}