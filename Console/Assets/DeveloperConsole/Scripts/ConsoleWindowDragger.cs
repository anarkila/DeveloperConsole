using UnityEngine.EventSystems;
using UnityEngine;

namespace DeveloperConsole {

    /// <summary>
    /// This class handles moving Developer Console Window on mouse drag (Large GUI only)
    /// </summary>
    public class ConsoleWindowDragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

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

            if (!ConsoleUtils.IsRectTransformInsideSreen(rectTransform)) {
                rectTransform.position = oldPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (!ConsoleUtils.IsRectTransformInsideSreen(rectTransform)) {
                rectTransform.position = positionOnBeginDrag;
            }
        }

        private void ResetWindowPosition() {
            if (rectTransform == null) return;

            rectTransform.position = defaultPosition;
        }
    }
}