using UnityEngine;
using UnityEditor;

namespace Anarkila.DeveloperConsole {

    public class ConsoleEditorWindow : EditorWindow {

        private bool writingTextFile = false;

        [MenuItem("Tools/DeveloperConsole")]
        public static void Open() {
            var window = GetWindow<ConsoleEditorWindow>();
            var titleContent = new GUIContent("Developer Console");
            window.titleContent = titleContent;
        }

        private void OnGUI() {
            DrawLayout();
        }

        private void DrawLayout() {
            GUILayout.Space(20);
            if (GUILayout.Button("Generate Command List", GUILayout.Height(30))) {

                if (writingTextFile) return;
                writingTextFile = CreateTextFileUtility.GenerateCommandList();
            }
        }
    }
}