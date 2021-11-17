using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script handles Developer Console messages
    /// this is implemented by having basic object pool system
    /// By default Developer Console pools 150 messages and once 150 messages has been reached, 
    /// it will start to recycle messages from the beginning.
    /// Increase maxMessageCount in inspector if you want to increase this value
    /// </summary>
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
        [SerializeField] private Image[] ScrollBarImages;
        [SerializeField] private GameObject[] Ghosts;

        private Dictionary<GameObject, ConsoleMessage> messages = new Dictionary<GameObject, ConsoleMessage>();
        private List<GameObject> currentMessages = new List<GameObject>();
        private Dictionary<PoolTag, Queue<GameObject>> poolDictionary;
        private ConsoleGUIStyle currentGUIStyle;
        private RectTransform rectTransform;
        private int maxMessageCount = 150;
        private Transform cachedTransform;
        private bool setupDone = false;
        private int messageCount = 0;
        private bool allGhostsHidden;
        private Vector2 defaultSize;
        private bool consoleIsOpen;

        private void Awake() {
            cachedTransform = this.transform;
            if (content != null) {
                rectTransform = content.GetComponent<RectTransform>();
                defaultSize = rectTransform.offsetMax;
            }
            PoolObjects();
            ConsoleEvents.RegisterConsoleStateChangeEvent += ConsoleStateChange;
            ConsoleEvents.RegisterConsoleClearEvent += ClearConsoleMessages;
            ConsoleEvents.RegisterGUIStyleChangeEvent += ConsoleGUIChanged;
            ConsoleEvents.RegisterDeveloperConsoleLogEvent += LogMessage;
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
                currentGUIStyle = settings.InterfaceStyle;
            }
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
                currentMessages[i].transform.SetParent(cachedTransform);
            }
            currentMessages.Clear();
            messageCount = 0;
            rectTransform.offsetMax = defaultSize;
            HandleGhostMessages();
        }

        private void LogMessage(string text) {
            if (messageParent == null) return;

            var messageObj = SpawnMessageFromPool(text);
            if (messageObj == null) return;

            ++messageCount;
            HandleGhostMessages();

            if (consoleIsOpen && currentGUIStyle == ConsoleGUIStyle.Large) {
                StartCoroutine(DelayScroll());  // Add frame delay before moving scroll bar to bottom
            }
        }

        private IEnumerator DelayScroll() {
            yield return null;
            ConsoleEvents.ScrollToBottom();
        }

        private void PoolObjects() {
            if (setupDone || pool.prefab == null) return;

            poolDictionary = new Dictionary<PoolTag, Queue<GameObject>>();
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < maxMessageCount; ++i) {
                GameObject obj = Instantiate(pool.prefab);

                var msg = obj.GetComponent<ConsoleMessage>();
                if (msg != null) messages.Add(obj, msg);

                obj.SetActive(false);
                obj.transform.SetParent(cachedTransform);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            setupDone = true;
        }

        private GameObject SpawnMessageFromPool(string message) {
            if (!setupDone || messageParent == null) return null;

            GameObject objectToSpawn = poolDictionary[PoolTag.Message].Dequeue();

            if (objectToSpawn != null) {
                objectToSpawn.transform.SetParent(messageParent);
                objectToSpawn.SetActive(true);

                var msg = messages[objectToSpawn];
                if (msg != null) {
                    msg.SetMessage(message);
                }
            }

            poolDictionary[PoolTag.Message].Enqueue(objectToSpawn);
            currentMessages.Add(objectToSpawn);
            return objectToSpawn;
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