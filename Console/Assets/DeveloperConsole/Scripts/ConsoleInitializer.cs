using UnityEngine;

namespace DeveloperConsole {

    [DefaultExecutionOrder(-10000)]
    public class ConsoleInitializer : MonoBehaviour {

        public ConsoleSettings settings;

        private void Awake() {
            if (!settings.íncludeConsoleInBuild && !Application.isEditor) {
                this.gameObject.SetActive(false);
                return;
            }

            transform.parent = null;
            ConsoleManager.InitilizeDeveloperConsole(settings, System.Threading.Thread.CurrentThread);
            ConsoleEvents.RegisterDestroyEvent += DestroyConsole;
        }

        private void DestroyConsole(float time) {
            ConsoleEvents.RegisterDestroyEvent -= DestroyConsole;
            Destroy(this.gameObject, time);
        }


#if UNITY_EDITOR
        private void OnValidate() {
            if (settings == null) return;

            string previous = gameObject.tag;

            string tag = settings.íncludeConsoleInBuild ? "Untagged" : "EditorOnly";
            gameObject.tag = tag;

            // If tag changed, mark scene as dirty.
            if (previous != gameObject.tag) {
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
        }
#endif

    }
}