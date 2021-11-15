using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    public class ConsoleMessage : MonoBehaviour {

        private Vector3 cachedVector = Vector3.one;
        private Transform cachedTransform;
        private TMP_Text textComponent;

        private void Awake() {
            textComponent = GetComponent<TMP_Text>();
            cachedTransform = this.transform;
        }

        public void SetMessage(string text) {
            if (textComponent == null) return;

            textComponent.text = text;
            cachedTransform.SetAsLastSibling();
            cachedTransform.localScale = cachedVector;
        }
    }
}