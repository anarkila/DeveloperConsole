using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script is attached to every Console message
    /// </summary>
    [DefaultExecutionOrder(-9990)]
    public class ConsoleMessage : MonoBehaviour {

        private Vector3 cachedVector = Vector3.one;
        private Color defaultColor = Color.white;
        private Transform cachedTransform;
        private TMP_Text textComponent;

        private void Awake() {
            if (TryGetComponent(out TMP_Text text)) {
                textComponent = text;
            }
#if UNITY_EDITOR
            else {
                Debug.Log(string.Format("Gameobject {0} doesn't have TMP_Text component!", gameObject.name));
            }
#endif
            cachedTransform = this.transform;

            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                defaultColor = settings.defaultMessageTextColor;
            }
        }

        public void SetMessage(string text, Color? textColor = null) {
            if (textComponent == null) return;

            textComponent.text = text;

            if (textColor != null) {
                textComponent.faceColor = (Color)textColor;
            }
            else {
                textComponent.faceColor = defaultColor;
            }
            cachedTransform.SetAsLastSibling();
            cachedTransform.localScale = cachedVector;
        }
    }
}