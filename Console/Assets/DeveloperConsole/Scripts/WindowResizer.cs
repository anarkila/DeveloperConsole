using UnityEngine.EventSystems;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class handles resizing Developer Console Window on mouse drag
    /// This is disabled for WebGL builds
    /// </summary>
    #pragma warning disable 0162
    public class WindowResizer : MonoBehaviour, IPointerDownHandler, IDragHandler {

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 minSize = new Vector2(480, 480);
        [SerializeField] private Vector2 maxSize = new Vector2(1920, 1080);

        private Vector2 previousPointerPosition;
        private Vector2 currentPointerPosition;
        private Vector2 newSizeDelta;
        private Vector3 defaultSize;
        private float panelHeight;
        private float localX = 1f;
        private float localY = 1f;
        private float panelWidth;

        private const float minScaleX = 0.5f;
        private const float minScaleY = 0.5f;
        private const float maxScaleX = 1.1f;
        private const float maxScaleY = 1f;

        private bool forceInsideScreenBounds = true;
        private bool resetWindowSizeOnEnable = false;

        private void Start() {
#if UNITY_WEBGL
            // Resizing currently works very oddly in WebGL so it's disabled.
            enabled = false;
            return;
#endif
            maxSize = new Vector2(Screen.width - 50, Screen.height - 50);

            if (rectTransform != null) {
                defaultSize = rectTransform.localScale;
                panelHeight = rectTransform.sizeDelta.x;
                panelWidth = rectTransform.sizeDelta.y;
                localX = rectTransform.localScale.x;
                localY = rectTransform.localScale.y;
                newSizeDelta = rectTransform.sizeDelta;
                ConsoleEvents.RegisterConsoleResetEvent += ResetConsole;
            }
#if UNITY_EDITOR
            else {
                Debug.LogError("rectTransform is null! Resizing console window will not work!");
                enabled = false;
                return;
            }
#endif

            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                forceInsideScreenBounds = settings.forceConsoleInsideScreenBounds;
                resetWindowSizeOnEnable = settings.resetWindowPositionOnEnable;
                var size = settings.consoleWindowDefaultSize;

                if (rectTransform != null) {
                    rectTransform.localScale = new Vector3(rectTransform.localScale.x * size, rectTransform.localScale.y * size, rectTransform.localScale.z);
                    defaultSize = rectTransform.localScale;
                }

                if (!settings.allowConsoleResize) {
                    enabled = false;
                    return;
                }
            }
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleResetEvent -= ResetConsole;
        }

        private void OnEnable() {
            if (resetWindowSizeOnEnable) {
                ResetConsole();
            }
        }

        private void ResetConsole() {
            if (rectTransform == null) return;

            rectTransform.localScale = defaultSize;
        }

        public void OnPointerDown(PointerEventData data) {
            if (rectTransform == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }

        public void OnDrag(PointerEventData data) {
            if (rectTransform == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
            Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

            newSizeDelta += new Vector2(resizeValue.x, -resizeValue.y);
            newSizeDelta = new Vector2(Mathf.Clamp(newSizeDelta.x, minSize.x, maxSize.x), Mathf.Clamp(newSizeDelta.y, minSize.y, maxSize.y));

            float previousX = localX;
            float previosY = localY;

            localX *= (newSizeDelta.x / panelHeight);
            localY *= (newSizeDelta.y / panelWidth);

            previousPointerPosition = currentPointerPosition;

            if (localX >= maxScaleX) localX = maxScaleX;
            if (localX <= minScaleX) localX = minScaleX;
            if (localY >= maxScaleY) localY = maxScaleY;
            if (localY <= minScaleY) localY = minScaleY;

            // check that window is still inside screen boounds if forceInsideScreenBounds is set to true
            if (forceInsideScreenBounds && !ConsoleUtils.IsRectTransformInsideSreen(rectTransform, 15f)) {
                localX = previousX;
                localY = previosY;
            }
            else {
                rectTransform.localScale = new Vector3(localX, localY, rectTransform.localScale.z);
            }
        }
    }
}