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

            settings.SetColors();
            ConsoleManager.InitializeDeveloperConsole(settings, System.Threading.Thread.CurrentThread);
            ConsoleEvents.RegisterDestroyEvent += DestroyConsole;

#if UNITY_EDITOR
            if (settings.interfaceStyle == ConsoleGUIStyle.Minimal && settings.allowGUIChangeRuntime) {
                if (settings.UnityLogOption != ConsoleLogOptions.DontPrintLogs || settings.unityThreadedLogOption != ConsoleLogOptions.DontPrintLogs) {
                    // if you are only using Minimal GUI style, Consider changing settings UnityLogOption and unityThreadedLogOption to ConsoleLogOptions.DontPrintLogs
                    // to reduce garbage collection. By default messages are still printed to Large GUI even when Minimal GUI is selected!
                    // Why? So you can toggle between them during runtime with command 'console.style'
                    Debug.Log("If you are only using Minimal console style, consider changing options UnityLogOption and unityThreadedLogOption to DontPrintLogs " +
                        "or set option 'Allow GUI Change Runtime' to false.");
                }
            }
#endif
            if (settings.allowGUIChangeRuntime) {
                Console.RegisterCommand(this, "ChangeConsoleGUI", "console_style", info: "Change console style between Large and Minimal", hiddenCommandMinimalGUI:false);
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
            Destroy(this.gameObject, time);
        }

        // for no domain/scene reload purposes
        private void OnApplicationQuit() {
            Instance = null;
        }
    }
}