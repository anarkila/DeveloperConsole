using UnityEngine.UI;
using UnityEngine;

namespace DeveloperConsole {

    public class CloseButton : MonoBehaviour {

        private Button button;

        private void Start() {
            button = GetComponent<Button>();

            if (button != null) {
                button.onClick.AddListener(CloseButtonClicked);
            }
#if UNITY_EDITOR
            else {
                Debug.Log("button is null! Closing will not work!");
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