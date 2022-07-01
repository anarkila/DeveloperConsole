using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ContextMenuHandler : MonoBehaviour, IPointerExitHandler {

        // Must be setup in the inspector!
        [SerializeField] private GameObject contextMenuBase;
        [SerializeField] private Button copyButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button clearButton;

        private Vector3 cachedOffset = new Vector3(35f, -35f, 0f);
        private RectTransform rectTransform;
        private GameObject lastGo;
        private string lastMsg;

        private void Start() {
            if (contextMenuBase == null || copyButton == null || deleteButton == null || clearButton == null) {
#if UNITY_EDITOR
                Debug.LogError("ContextMenu setup is incorrect.");
#endif
                gameObject.SetActive(false);
                return;
            }

            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                if (!settings.showContextMenuOnMessageRightClick) {
                    gameObject.SetActive(false);
                    return;
                }
            }

            if (TryGetComponent(out RectTransform rect)) {
                rectTransform = rect;
            }

            copyButton.onClick.AddListener(CopyButtonClicked);
            deleteButton.onClick.AddListener(DeleteButtonClicked);
            clearButton.onClick.AddListener(ClearButtonClicked);
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChanged;
            ConsoleEvents.RegisterOnContextMenuShow += OnContextMenuRequest;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleStateChanged;
            ConsoleEvents.RegisterOnContextMenuShow -= OnContextMenuRequest;
            if (copyButton != null) copyButton.onClick.RemoveAllListeners();
            if (deleteButton != null) deleteButton.onClick.RemoveAllListeners();
            if (clearButton != null) clearButton.onClick.RemoveAllListeners();
        }

        private void ConsoleStateChanged(bool consoleIsOpen) {
            if (!consoleIsOpen) {
                SetContextMenuEnabled(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            SetContextMenuEnabled(false);
        }

        private void OnContextMenuRequest(GameObject go, PointerEventData eventData, string msg) {
            SetContextMenuEnabled(true);
            MoveContextMenu((Vector3)eventData.position);
            lastMsg = msg;
            lastGo = go;
        }

        private void CopyButtonClicked() {
            if (string.IsNullOrEmpty(lastMsg)) return;

            GUIUtility.systemCopyBuffer = lastMsg;
            SetContextMenuEnabled(false);
        }

        private void DeleteButtonClicked() {
            if (lastGo == null) return;

            lastGo.SetActive(false);
            lastGo = null;
            SetContextMenuEnabled(false);
            ConsoleEvents.MessageDeleted();
        }

        private void ClearButtonClicked() {
            Console.ClearConsoleMessages();
            SetContextMenuEnabled(false);
        }

        private void SetContextMenuEnabled(bool enable) {
            if (contextMenuBase == null) return;

            contextMenuBase.SetActive(enable);
        }

        private void MoveContextMenu(Vector3 position) {
            Vector3 newPos = position + cachedOffset;
            rectTransform.position = newPos;
        }
    }
}