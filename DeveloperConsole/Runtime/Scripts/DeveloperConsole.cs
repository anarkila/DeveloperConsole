using UnityEngine.EventSystems;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    [DefaultExecutionOrder(-9999)]
    public class DeveloperConsole : MonoBehaviour {

        public ConsoleSettings settings;

        private static DeveloperConsole Instance;

        private void Awake() {
            transform.SetParent(null);

            // Allow only one instance of Developer Console.
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
                return;
            }

            if (!settings.includeConsoleInFinalBuild && !Debug.isDebugBuild) {
                this.gameObject.SetActive(false);
                Console.DestroyConsole(1f);
                return;
            }

            // this should never happen, but just in case.
            if (settings == null) {
                settings = new ConsoleSettings();
            }

            settings.ApplyColors();

            ConsoleManager.SetSettings(settings);

            // Initalize developer console here or when the console is first opened
            if (!settings.initializeConsoleOnFirstOpen) {
                ConsoleManager.InitializeDeveloperConsole(settings);
            }
            
            ConsoleEvents.RegisterDestroyEvent += DestroyConsole;

#if UNITY_EDITOR
            if (settings.interfaceStyle == ConsoleGUIStyle.Minimal && settings.allowGUIStyleChangeRuntime) {
                if (settings.UnityLogOption != ConsoleLogOptions.DontPrintLogs || settings.unityThreadedLogOption != ConsoleLogOptions.DontPrintLogs) {
                    // if you are only using Minimal GUI style, Consider changing settings UnityLogOption and unityThreadedLogOption to ConsoleLogOptions.DontPrintLogs
                    // to reduce garbage collection. By default messages are still printed to Large GUI even when Minimal GUI is selected!
                    // Why? Incase you toggle to Large GUI during runtime (command: 'console.style'), you will see previous messages.
                    Debug.Log("If you are only using Minimal console style, consider changing options UnityLogOption and unityThreadedLogOption to DontPrintLogs " +
                        "or set option 'Allow GUI Change Runtime' to false.");
                }
            }
#endif
            if (settings.allowGUIStyleChangeRuntime) {
                Console.RegisterCommand(this, "ChangeConsoleGUI", "console_style", info: "Change Console style between Large and Minimal", hiddenCommandMinimalGUI:false);
            }
        }

        /// <summary>
        /// Toggle console style between Large and Minimal
        /// </summary>
        private void ChangeConsoleGUI() {
            ConsoleEvents.SwitchGUIStyle();
        }

        private void DestroyConsole(float time) {
            ConsoleEvents.RegisterDestroyEvent -= DestroyConsole;
#if UNITY_EDITOR
            Debug.Log("Destroying DeveloperConsole..");
#endif
            Destroy(this.gameObject, time);
        }

#if UNITY_EDITOR
        // for no domain/scene reload purposes
        private void OnApplicationQuit() {
            Instance = null;
        }

        private void OnValidate() {
            // In the editor, check EventSystem component exists in the scene.
            // Otherwise UI inputs cannot be received.
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null) {
                Debug.Log("Did not find EventSystem in the current scene. EventSystem has been added to current scene.");

                // Create new GameObject and add EventSystem and StandaloneInputModule components to it.
                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.transform.SetParent(null);

                eventSystemGo.AddComponent<EventSystem>();
                eventSystemGo.AddComponent<StandaloneInputModule>();
            }
        }
#endif

    }
}