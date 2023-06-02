using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script is attached to every Console message
    /// </summary>
    [DefaultExecutionOrder(-9990)]
    public class ConsoleMessage : MonoBehaviour, IPointerClickHandler {

        private Vector3 cachedVector = Vector3.one;
        private Color textColor = Color.white;
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
                textColor = settings.interfaceStyle == ConsoleGUIStyle.Large ? settings.consoleColors.largeGUITextColor : settings.consoleColors.minimalGUITextColor;
            }
        }

        public void SetMessage(string text, Color? textColor = null) {
            if (textComponent == null) return;

            textComponent.text = text;

            if (textColor != null) {
                textComponent.faceColor = (Color)textColor;
            }
            else {
                textComponent.faceColor = this.textColor;
            }
            cachedTransform.SetAsLastSibling();
            cachedTransform.localScale = cachedVector;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (textComponent == null) return;

            if (eventData.button == PointerEventData.InputButton.Right) {
                ConsoleEvents.ShowContextMenu(gameObject, eventData, textComponent.text);
            }
        }

    }
}