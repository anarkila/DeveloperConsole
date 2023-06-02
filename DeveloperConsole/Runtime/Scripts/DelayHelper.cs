using System.Collections;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    [DefaultExecutionOrder(-10000)]
    public class DelayHelper : MonoBehaviour {

        public static DelayHelper Instance;

        public static DelayHelper GetInstance() {
            return Instance;
        }

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(this);
            }
        }

        /// <summary>
        /// Invoke callback after given frames
        /// </summary>
        public void DelayedCallFrames(Action callback, int frames) {
            if (!Application.isPlaying || Instance == null) return;

            StartCoroutine(DelayedCallFramesCoroutine(callback, frames));
        }

        /// <summary>
        /// Invoke callback after n frames
        /// </summary>
        private IEnumerator DelayedCallFramesCoroutine(Action callback, int frames) {
            for (int i = 0; i < frames; i++) {
                yield return null;
            }
            callback.Invoke();
        }

#if UNITY_EDITOR
        // for no domain/scene reload purposes
        private void OnApplicationQuit() {
            Instance = null;
        }
#endif
    }
}