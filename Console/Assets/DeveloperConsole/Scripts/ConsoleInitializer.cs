using UnityEngine;

namespace DeveloperConsole {

    [DefaultExecutionOrder(-10000)]
    public class ConsoleInitializer : MonoBehaviour {

        public ConsoleSettings settings;

        private void Awake() {

            if (!settings.includeConsoleInFinalBuild && !Debug.isDebugBuild) {
                this.gameObject.SetActive(false);
                Console.DestroyConsole(1f);
                return;
            }

            transform.SetParent(null);
            ConsoleManager.InitilizeDeveloperConsole(settings, System.Threading.Thread.CurrentThread);
            ConsoleEvents.RegisterDestroyEvent += DestroyConsole;
        }

        private void DestroyConsole(float time) {
            ConsoleEvents.RegisterDestroyEvent -= DestroyConsole;
            Destroy(this.gameObject, time);
        }
    }
}