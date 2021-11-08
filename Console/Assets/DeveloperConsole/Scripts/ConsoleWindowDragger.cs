using UnityEngine.EventSystems;
using UnityEngine;

namespace DeveloperConsole {

    /// <summary>
    /// This class handles moving Developer Console Window on mouse drag (Large GUI only)
    /// </summary>
    public class ConsoleWindowDragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

        private bool forceInsideScreenBounds = true;
        private RectTransform rectTransform;
        private Vector3 positionOnBeginDrag;
        private Vector3 defaultPosition;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null) {
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
            if (!ConsoleUtils.IsRectTransformInsideSreen(rectTransform, 2)) {
                ConsoleEvents.ResetConsole();
            }
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                forceInsideScreenBounds = settings.ForceConsoleInsideScreenBounds;
            }

            defaultPosition = rectTransform.position;
            ConsoleEvents.RegisterConsoleResetEvent += ResetWindowPosition;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleResetEvent -= ResetWindowPosition;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            positionOnBeginDrag = rectTransform.position;
        }

        public void OnDrag(PointerEventData eventData) {
            Vector3 oldPos = rectTransform.position;
            rectTransform.anchoredPosition += eventData.delta;

            if (forceInsideScreenBounds && !ConsoleUtils.IsRectTransformInsideSreen(rectTransform)) {
                rectTransform.position = oldPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (forceInsideScreenBounds && !ConsoleUtils.IsRectTransformInsideSreen(rectTransform)) {
                rectTransform.position = positionOnBeginDrag;
            }
        }

        private void ResetWindowPosition() {
            if (rectTransform == null) return;

            rectTransform.position = defaultPosition;
        }
    }
}