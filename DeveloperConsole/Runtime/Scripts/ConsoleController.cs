using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class handles enabling/disabling Developer Console
    /// </summary>
    [DefaultExecutionOrder(-9997)]
    public class ConsoleController : MonoBehaviour {

        private ConsoleSettings settings;
        private GameObject minimalConsole;
        private GameObject largeConsole;
        private bool canOpenConsole = true;

        private void Awake() {
            settings = ConsoleManager.GetSettings();

            minimalConsole = transform.GetChild(0).gameObject;
            largeConsole = transform.GetChild(1).gameObject;

#if UNITY_EDITOR
            if (minimalConsole == null || largeConsole == null || settings == null) {
                Debug.LogError("one or more references is null!");
                return;
            }
#endif
            minimalConsole.SetActive(false);
            largeConsole.SetActive(false);
            
            ConsoleEvents.RegisterConsoleStateChangeEvent += SetDeveloperConsoleState;
            ConsoleEvents.RegisterConsoleEnabledEvent += ConsoleEnabledChanged;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleStateChangeEvent -= SetDeveloperConsoleState;
            ConsoleEvents.RegisterConsoleEnabledEvent -= ConsoleEnabledChanged;
        }

        private void ConsoleEnabledChanged(bool canOpen) {
            canOpenConsole = canOpen;
        }

        private void SetDeveloperConsoleState(bool enable) {
            if (!canOpenConsole) return;

            switch (settings.interfaceStyle) {
                case ConsoleGUIStyle.Minimal:
                    minimalConsole.SetActive(enable);
                    break;
                case ConsoleGUIStyle.Large:
                    largeConsole.SetActive(enable);
                    break;
            }
        }
    }
}