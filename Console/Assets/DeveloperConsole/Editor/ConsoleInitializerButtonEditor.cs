using UnityEngine;
using UnityEditor;

namespace DeveloperConsole {

    [CustomEditor(typeof(ConsoleInitializer))]
    public class ConsoleInitializerButtonEditor : Editor {

        ConsoleInitializer myTarget;

        private void OnEnable() {
            myTarget = (ConsoleInitializer)target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            GUILayout.Space(10);
            if (GUILayout.Button("Reset Settings", GUILayout.Height(30))) {
                myTarget.settings = new ConsoleSettings();
                EditorUtility.SetDirty(myTarget.gameObject);
                Debug.Log("Settings reset to default.");
            }
        }
    }
}