using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    public class ConsoleMessage : MonoBehaviour {

        private Vector3 cachedVector = Vector3.one;
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
        }

        public void SetMessage(string text) {
            if (textComponent == null) return;

            textComponent.text = text;
            cachedTransform.SetAsLastSibling();
            cachedTransform.localScale = cachedVector;
        }
    }
}