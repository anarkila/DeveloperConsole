using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script handles Developer Console messages
    /// by default Developer Console pools 150 messages and once 150 messages has been reached, 
    /// it will start to recycle messages from the beginning.
    /// Increase maxMessageCount setting in inspector if you want to increase this value
    /// </summary>
    [DefaultExecutionOrder(-9998)]
    public class ConsoleMessageHandler : MonoBehaviour {

        public enum PoolTag {
            Message
        }

        [System.Serializable]
        public class Pool {
            public PoolTag tag;
            public GameObject prefab;
        }

        [SerializeField] private Transform content;
        [SerializeField] private Transform messageParent;
        [SerializeField] private Pool pool;
        [SerializeField] private GameObject[] Ghosts;

        private Dictionary<GameObject, ConsoleMessage> messages = new Dictionary<GameObject, ConsoleMessage>(256);
        private List<TempMessage> messagesBeforeInitDone = new List<TempMessage>(32);
        private List<GameObject> currentMessages = new List<GameObject>(64);
        private Dictionary<PoolTag, Queue<GameObject>> poolDictionary;
        private ConsoleGUIStyle currentGUIStyle;
        private RectTransform rectTransform;
        private int maxMessageCount = 150;
        private bool coroutineIsRunning;
        private bool setupDone = false;
        private int messageCount = 0;
        private bool allGhostsHidden;
        private Vector2 defaultSize;
        private bool consoleIsOpen;

        private void Awake() {
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChange;
            ConsoleEvents.RegisterConsoleClearEvent += ClearConsoleMessages;
            ConsoleEvents.RegisterGUIStyleChangeEvent += ConsoleGUIChanged;
            ConsoleEvents.RegisterDeveloperConsoleLogEvent += LogMessage;

            if (content != null) {
                if (content.TryGetComponent(out RectTransform rect)) {
                    rectTransform = rect;
                }
#if UNITY_EDITOR
                else {
                    Debug.Log(string.Format("Gameobject: {0} doesn't have RectTransform component!", content.name));
                }
#endif
                defaultSize = rectTransform.offsetMax;
            }
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleStateChangeEvent -= ConsoleStateChange;
            ConsoleEvents.RegisterConsoleClearEvent -= ClearConsoleMessages;
            ConsoleEvents.RegisterGUIStyleChangeEvent -= ConsoleGUIChanged;
            ConsoleEvents.RegisterDeveloperConsoleLogEvent -= LogMessage;
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();
            if (settings != null) {
                maxMessageCount = settings.maxMessageCount;
                currentGUIStyle = settings.interfaceStyle;

                // No need to pool messages if only Minimal GUI is used.
                if (settings.interfaceStyle == ConsoleGUIStyle.Minimal
                    && settings.UnityLogOption == ConsoleLogOptions.DontPrintLogs
                    && settings.unityThreadedLogOption == ConsoleLogOptions.DontPrintLogs
                    || settings.interfaceStyle == ConsoleGUIStyle.Minimal && !settings.allowGUIStyleChangeRuntime) {

                    enabled = false;
                    return;
                }
            }
            PoolMessages();
            HandleGhostMessages();
        }

        private void ConsoleStateChange(bool state) {
            consoleIsOpen = state;
        }

        private void ConsoleGUIChanged(ConsoleGUIStyle guiStyle) {
            currentGUIStyle = guiStyle;
        }

        private void ClearConsoleMessages() {
            for (int i = 0; i < currentMessages.Count; i++) {
                currentMessages[i].SetActive(false);
            }
            currentMessages.Clear();
            messageCount = 0;
            rectTransform.offsetMax = defaultSize;
            HandleGhostMessages();
        }

        private void LogMessage(string text, Color? textColor) {
            if (!Application.isPlaying) return;

            var success = SpawnMessageFromPool(text, textColor);
            if (!success) return;

            ++messageCount;
            HandleGhostMessages();

            if (consoleIsOpen && !coroutineIsRunning && currentGUIStyle == ConsoleGUIStyle.Large) {
                // Add frame delay before moving scroll bar to bottom
                // Only allow one Coroutine to run at once.
                StartCoroutine(DelayScroll());
            }
        }

        private IEnumerator DelayScroll() {
            coroutineIsRunning = true;
            yield return null;
            ConsoleEvents.ScrollToBottom();
            coroutineIsRunning = false;
        }

        private void PoolMessages() {
            if (setupDone || pool.prefab == null || messageParent == null) return;

            poolDictionary = new Dictionary<PoolTag, Queue<GameObject>>(maxMessageCount);
            Queue<GameObject> objectPool = new Queue<GameObject>(maxMessageCount);

            for (int i = 0; i < maxMessageCount; ++i) {
                GameObject obj = Instantiate(pool.prefab);

                if (obj.TryGetComponent(out ConsoleMessage msg)) {
                    messages.Add(obj, msg);
                }

                obj.SetActive(false);
                obj.transform.SetParent(messageParent);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            setupDone = true;

            for (int i = 0; i < messagesBeforeInitDone.Count; i++) {
                LogMessage(messagesBeforeInitDone[i].message, messagesBeforeInitDone[i].messageColor);
            }
            messagesBeforeInitDone.Clear();
        }

        private bool SpawnMessageFromPool(string message, Color? textColor) {
            bool success = false;

            if (!setupDone || messageParent == null) {
                var temp = new TempMessage();
                temp.message = message;
                temp.messageColor = textColor;
                messagesBeforeInitDone.Add(temp);
                return success;
            }

            GameObject objectToSpawn = poolDictionary[PoolTag.Message].Dequeue();

            if (objectToSpawn != null) {
                objectToSpawn.SetActive(true);

                var msg = messages[objectToSpawn];
                if (msg != null) {
                    msg.SetMessage(message, textColor);
                    success = true;
                }
            }

            poolDictionary[PoolTag.Message].Enqueue(objectToSpawn);
            currentMessages.Add(objectToSpawn);

            return success;
        }

        // HACK: In order to have console messages from bottom to top
        // There's invisible text components with text color alpha set to zero
        // So for example if there's only one console message
        // there's actually 19 invisible messages on top of it.
        // it just works!
        private void HandleGhostMessages() {
            if (messageCount >= 20 && !allGhostsHidden) return;

            var visible = 19 - messageCount;
            allGhostsHidden = visible >= 0;

            for (int i = 0; i < Ghosts.Length; i++) {
                if (Ghosts[i] == null) continue;

                bool active = visible >= i;
                Ghosts[i].SetActive(active);
            }
        }
    }
}