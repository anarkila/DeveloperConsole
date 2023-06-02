#if UNITY_EDITOR

using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class measures Unity Editor play button click to playable scene time,
    /// if consoleSettings printEditorDebugInfo is set to true.
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

                    Console.Log(string.Format("Scene [{0}] loaded in {1} ms.", sceneName, difference));

                    called = true;
                    break;
            }
        }
    }
}

#endif