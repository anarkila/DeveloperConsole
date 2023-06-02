using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ScrollRectMover : MonoBehaviour {

        private Vector2 cachedVector = Vector2.zero;
        private ScrollRect scrollRect;
        private bool scrollToBottom;

        private void Awake() {
            if (TryGetComponent(out ScrollRect rect)) {
                scrollRect = rect;

                var settings = ConsoleManager.GetSettings();
                if (settings != null) {
                    scrollRect.verticalScrollbarVisibility = settings.ScrollRectVisibility;
                }
                ConsoleEvents.RegisterConsoleScrollMoveEvent += ScrollToBottom;
            }
            else {
#if UNITY_EDITOR
                Debug.Log(string.Format("Gameobject {0} doesn't have ScrollRect component!", gameObject.name));
#endif
                ConsoleEvents.RegisterConsoleScrollMoveEvent -= ScrollToBottom;
                this.enabled = false;
                return;
            }
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleScrollMoveEvent -= ScrollToBottom;
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null) {
                scrollToBottom = settings.scrollToBottomOnEnable;
                scrollRect.scrollSensitivity = settings.scrollSensitivity;
                ScrollToBottom();
            }
        }

        private void OnDisable() {
            if (scrollToBottom) {
                ScrollToBottom();
            }
        }

        private void ScrollToBottom() {
            scrollRect.normalizedPosition = cachedVector;
        }
    }
}