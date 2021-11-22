using UnityEngine.EventSystems;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class handles moving Developer Console Window on mouse drag (Large GUI only)
    /// </summary>
    public class ConsoleWindowDragger : MonoBehaviour, IDragHandler {

        private bool resetWindowPositionOnEnable = false;
        private bool forceInsideScreenBounds = true;
        private bool allowWindowDragging = true;
        private RectTransform rectTransform;
        private Vector3 defaultPosition;

        private void Awake() {
            if (TryGetComponent(out RectTransform rect)) {
                rectTransform = rect;
            }
            else {
#if UNITY_EDITOR
                Debug.Log(string.Format("Gameobject: {0} doesn't have RectTransform component!", gameObject.name));
#endif
                enabled = false;
                return;
            }
        }

        private void OnEnable() {
            if (rectTransform == null) return;

            // Check that at least two corners are inside screen bounds,
            // if not, reset console.
            if (!ConsoleUtils.IsRectTransformInsideSreen(rectTransform, 2) || resetWindowPositionOnEnable) {
                ResetWindowPosition();
            }
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                forceInsideScreenBounds = settings.ForceConsoleInsideScreenBounds;
                allowWindowDragging = settings.allowConsoleWindowDrag;
                resetWindowPositionOnEnable = settings.resetWindowPositionOnEnable;
            }

            defaultPosition = rectTransform.position;
            ConsoleEvents.RegisterConsoleResetEvent += ResetWindowPosition;
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