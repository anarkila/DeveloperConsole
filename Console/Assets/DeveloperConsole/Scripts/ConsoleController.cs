using UnityEngine;

namespace DeveloperConsole {

    /// <summary>
    /// This class handles enabling/disabling Developer Console
    /// </summary>
    public class ConsoleController : MonoBehaviour {

        private static ConsoleController Instance;

        private ConsoleSettings settings;
        private GameObject minimalConsole;
        private GameObject largeConsole;

        private void Awake() {
            // Allow only one DeveloperConsole Instance
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Start() {
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
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleStateChangeEvent -= SetDeveloperConsoleState;
            Instance = null;
        }

        private void SetDeveloperConsoleState(bool enable) {
            switch (settings.InterfaceStyle) {
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