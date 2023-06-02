using UnityEngine.EventSystems;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class handles moving Developer Console Window on mouse drag (Large GUI only)
    /// </summary>
    [DefaultExecutionOrder(-9990)]
    public class ConsoleWindowDragger : MonoBehaviour, IDragHandler {

        [Tooltip("Drag and drop main panel here")]
        [SerializeField] private RectTransform rectTransform;

        private bool resetWindowPositionOnEnable = false;
        private bool forceInsideScreenBounds = true;
        private bool allowWindowDragging = true;
        private Vector3 defaultPosition;

        private void Awake() {
            if (rectTransform == null) {
#if UNITY_EDITOR
                Debug.Log("RectTransform is null!");
#endif
                enabled = false;
                return;
            }

            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                resetWindowPositionOnEnable = settings.resetWindowPositionOnEnable;
                forceInsideScreenBounds = settings.forceConsoleInsideScreenBounds;
                allowWindowDragging = settings.allowConsoleWindowDrag;
            }

            defaultPosition = rectTransform.position;
            ConsoleEvents.RegisterConsoleResetEvent += ResetWindowPosition;
        }

        private void OnEnable() {
            if (rectTransform == null) return;

            // Check that at least two corners are inside screen bounds,
            // if not, reset console.
            if (!ConsoleUtils.IsRectTransformInsideSreen(rectTransform, 2) || resetWindowPositionOnEnable) {
                ResetWindowPosition();
            }
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleResetEvent -= ResetWindowPosition;
        }

        public void OnDrag(PointerEventData eventData) {
            if (!allowWindowDragging) return;

            Vector3 oldPos = rectTransform.position;

            //rectTransform.anchoredPosition += eventData.delta; // works as well but feels a bit off
            rectTransform.position += (Vector3)eventData.delta;

            if (forceInsideScreenBounds && !ConsoleUtils.IsRectTransformInsideSreen(rectTransform)) {
                rectTransform.position = oldPos;
            }
        }

        private void ResetWindowPosition() {
            if (rectTransform == null) return;

            rectTransform.position = defaultPosition;
        }
    }
}