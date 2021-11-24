using UnityEngine;

namespace Anarkila.DeveloperConsole {

    [DefaultExecutionOrder(-10000)]
    public class DeveloperConsole : MonoBehaviour {

        public ConsoleSettings settings;

        private static DeveloperConsole Instance;

        private void Awake() {

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

            transform.SetParent(null);

            // this should never happen, but just in case.
            if (settings == null) {
                settings = new ConsoleSettings();
            }

            settings.SetColors();
            ConsoleManager.InitilizeDeveloperConsole(settings, System.Threading.Thread.CurrentThread);
            ConsoleEvents.RegisterDestroyEvent += DestroyConsole;
        }

        private void DestroyConsole(float time) {
            ConsoleEvents.RegisterDestroyEvent -= DestroyConsole;
            Destroy(this.gameObject, time);
        }
    }
}