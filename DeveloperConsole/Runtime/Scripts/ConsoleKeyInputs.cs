using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script handles listening key inputs (old Unity input system)
    /// - enable/disable Developer Console  (default: § or ½ key below ESC)
    /// - submit current inputfield text    (default: enter)
    /// - fill from prediction              (default: tab and down arrow)
    /// - fill inputfield with previous     (default: up arrow)
    /// </summary>
    public class ConsoleKeyInputs : MonoBehaviour {

        private KeyCode consoleToggleKey = KeyCode.Backslash;
        private KeyCode submitKey = KeyCode.Return;
        private KeyCode searchPreviousCommand = KeyCode.UpArrow;
        private KeyCode nextSuggestedCommandKey = KeyCode.DownArrow;
        private KeyCode nextSuggestedCommandKeyAlt = KeyCode.Tab;

        private bool listenActivateKey = true;
        private bool consoleIsOpen = false;
       
        private void Start() {
            GetSettings();
            ConsoleEvents.RegisterListenActivatStateEvent += ActivatorStateChangeEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChanged;
            ConsoleEvents.RegisterSettingsChangedEvent += GetSettings;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterListenActivatStateEvent -= ActivatorStateChangeEvent;
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleStateChanged;
            ConsoleEvents.RegisterSettingsChangedEvent -= GetSettings;
        }

        private void Update() {
            ListenPlayerInputs();
        }

        private void ListenPlayerInputs() {
            // If you wish to move into the new Unity Input system, modify this.

            if (Input.GetKeyDown(consoleToggleKey) && listenActivateKey) {
                ConsoleEvents.SetConsoleState(!consoleIsOpen);
            }

            if (!listenActivateKey) {
                consoleIsOpen = ConsoleManager.IsConsoleOpen();
            }

            // If console is not open then don't check other input keys
            if (!consoleIsOpen) return;

            if (Input.GetKeyDown(submitKey)) {
                ConsoleEvents.InputFieldSubmit();
            }

            if (Input.GetKeyDown(searchPreviousCommand)) {
                ConsoleEvents.SearchPreviousCommand();
            }

            if (Input.GetKeyDown(nextSuggestedCommandKey) || Input.GetKeyDown(nextSuggestedCommandKeyAlt)) {
                ConsoleEvents.FillCommand();
            }
        }

        private void ActivatorStateChangeEvent(bool enabled) {
            listenActivateKey = enabled;
        }

        private void ConsoleStateChanged(bool state) {
            consoleIsOpen = state;
        }

        private void GetSettings() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null) {
                searchPreviousCommand = settings.consoleSearchCommandKey;
                nextSuggestedCommandKeyAlt = settings.NextSuggestedCommandKeyAlt;
                nextSuggestedCommandKey = settings.NextSuggestedCommandKey;
                consoleToggleKey = settings.consoleToggleKey;
                submitKey = settings.consoleSubmitKey;
            }
        }
    }
}