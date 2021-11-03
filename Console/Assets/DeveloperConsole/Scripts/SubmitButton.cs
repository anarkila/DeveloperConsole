using UnityEngine.UI;
using UnityEngine;

namespace DeveloperConsole {

    public class SubmitButton : MonoBehaviour {

        private Button button;

        private void Start() {
            button = GetComponent<Button>();

            if (button != null) {
                button.onClick.AddListener(SubmitButtonClick);
            }
#if UNITY_EDITOR
            else {
                Debug.Log("button is null! Submitting will not work!");
            }
#endif
        }

        private void SubmitButtonClick() {
            ConsoleEvents.InputFieldSubmit();
        }
    }
}