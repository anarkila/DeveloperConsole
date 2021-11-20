using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class CloseButton : MonoBehaviour {

        private Button button;

        private void Start() {
            if (TryGetComponent(out Button btn)) {
                button = btn;
                button.onClick.AddListener(CloseButtonClicked);
            }
#if UNITY_EDITOR
            else {
                Debug.Log(string.Format("Gameobject: {0} doesn't have Button component!", gameObject.name));
            }
#endif
        }

        private void OnDestroy() {
            if (button == null) return;

            button.onClick.RemoveAllListeners();
        }

        private void CloseButtonClicked() {
            ConsoleEvents.CloseConsole();
        }
    }
}