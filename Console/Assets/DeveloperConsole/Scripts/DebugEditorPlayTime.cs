#if UNITY_EDITOR

using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using System;

namespace DeveloperConsole {

    /// <summary>
    /// This class measures Editor play button click to playable scene time
    /// if consoleSettings printEditorDebugInfo is true.
    /// </summary>
    [ExecuteInEditMode]
    public class DebugEditorPlayTime : MonoBehaviour {

        public long playButtonTime;
        public long startTime;
        public bool called;

        private void OnEnable() {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnPlayModeChanged(PlayModeStateChange stateChange) {
            switch (stateChange) {
                case PlayModeStateChange.ExitingEditMode:
                    playButtonTime = DateTime.Now.Ticks;

                    // this needs to be called because this script is attached to Prefab
                    // Otherwise playButtonTime will reset on Play
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    if (!ConsoleManager.GetSettings().printPlayButtonToSceneTime || called) return;

                    startTime = DateTime.Now.Ticks;
                    var difference = (startTime - playButtonTime) / TimeSpan.TicksPerMillisecond;
                    var sceneName = SceneManager.GetActiveScene().name;

                    // Uncomment below line if you wish to log this into Developer Console instead
                    //Console.Log(string.Format("Loading scene [{0}] took {1} ms", sceneName, difference));
                    Debug.Log(string.Format("Loading scene [{0}] took {1} ms", sceneName, difference));

                    called = true;
                    break;
            }
        }
    }
}

#endif