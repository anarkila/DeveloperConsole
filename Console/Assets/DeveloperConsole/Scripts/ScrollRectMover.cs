﻿using UnityEngine.UI;
using UnityEngine;

namespace DeveloperConsole {

    public class ScrollRectMover : MonoBehaviour {

        private Vector2 cachedVector = Vector2.zero;
        private ScrollRect scrollRect;
        private bool scrollToBottom;

        private void Awake() {
            scrollRect = GetComponent<ScrollRect>();

            if (scrollRect == null) {
#if UNITY_EDITOR
                Debug.Log(string.Format("Gameobject {0} doesn't have ScrollRect component!", gameObject.name));
#endif
                this.enabled = false;
                return;
            }

            ConsoleEvents.RegisterConsoleScrollMoveEvent += ScrollToBottom;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleScrollMoveEvent -= ScrollToBottom;
        }

        private void Start() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null && scrollRect != null) {
                scrollToBottom = settings.scrollToBottomOnEnable;
                scrollRect.scrollSensitivity = settings.scrollSensitivity;
                ScrollToBottom();
            }
        }

        private void OnDisable() {
            // whether to scroll to bottom when Developer console is disabled
            // So when console is opened again, scroll will be at bottom
            if (scrollToBottom) {
                ScrollToBottom();
            }
        }

        private void ScrollToBottom() {
            if (scrollRect == null) return;

            scrollRect.normalizedPosition = cachedVector;
        }
    }
}