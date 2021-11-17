using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anarkila.DeveloperConsole {

    [CustomEditor(typeof(DeveloperConsole))]
    public class ConsoleInitializerEditor : Editor {

        private string[] tabs = new string[] { "All Settings", "GUI Settings", "General Settings", "Keybindings", "Debug Settings" };
        private SerializedObject sTarget;
        private int toolbarTab;

        private List<SerializedProperty> guiSettings = new List<SerializedProperty>();
        private List<SerializedProperty> generalSettings = new List<SerializedProperty>();
        private List<SerializedProperty> keybindings = new List<SerializedProperty>();
        private List<SerializedProperty> debugSettings = new List<SerializedProperty>();

        private DeveloperConsole myTarget;
        private bool renderCustomGUI = true;

        private void OnEnable() {
            myTarget = (DeveloperConsole)target;
            sTarget = new SerializedObject(target);

            RegisterSerializedProperties();
        }

        private void RegisterSerializedProperties() {
            guiSettings.Clear();
            generalSettings.Clear();
            keybindings.Clear();
            debugSettings.Clear();

            var fieldValues = myTarget.settings.GetType().GetFields().Select(f => f.Name).ToList();
            string setting = "settings.";
            for (int i = 0; i < fieldValues.Count; i++) {
                var name = setting + fieldValues[i];

                if (0 <= i && i <= 6) {
                    AppendToList(guiSettings, name);
                }
                else if (7 <= i && i <= 22) {
                    AppendToList(generalSettings, name);
                }
                else if (23 <= i && i <= 27) {
                    AppendToList(keybindings, name);
                }
                else {
                    AppendToList(debugSettings, name);
                }
            }
        }

        private void AppendToList(List<SerializedProperty> list, string name) {
            list.Add(sTarget.FindProperty(name));
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Developer Console 0.8.1", EditorStyles.boldLabel);

            renderCustomGUI = EditorGUILayout.Toggle("Custom GUI", renderCustomGUI);
            GUILayout.Space(20);

            if (!renderCustomGUI) {
                DrawDefaultInspector();
                return;
            }

            sTarget.Update();
            EditorGUI.BeginChangeCheck();

            toolbarTab = GUILayout.Toolbar(toolbarTab, tabs);
            GUILayout.Space(10);

            RenderTab();

            if (EditorGUI.EndChangeCheck()) {
                sTarget.ApplyModifiedProperties();
            }

            RenderBottomButtons(); 
        }

        private void RenderTab() {
            switch (toolbarTab) {
                case 0: RenderAll(); break;
                case 1: RenderGUISettings(); break;
                case 2: RenderGeneralSettings(); break;
                case 3: RenderKeybindingSettings(); break;
                case 4: RenderDebugSettings(); break;
            }
        }

        private void RenderBottomButtons() {
            GUILayout.Space(20);
            if (GUILayout.Button("Reset Settings", GUILayout.Height(30))) {
                myTarget.settings = new ConsoleSettings();
                EditorUtility.SetDirty(myTarget.gameObject);
                Debug.Log("Settings reset to default.");
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Generate Command List", GUILayout.Height(30))) {
                CreateTextFileUtility.GenerateCommandList();
            }
        }

        private void RenderAll() {
            RenderGUISettings();
            RenderGeneralSettings();
            RenderKeybindingSettings();
            RenderDebugSettings();
        }

        private void RenderGUISettings() {
            Render(guiSettings);
        }

        private void RenderGeneralSettings() {
            Render(generalSettings);
        }

        private void RenderKeybindingSettings() {
            Render(keybindings);
        }

        private void RenderDebugSettings() {
            Render(debugSettings);
        }

        private void Render(List<SerializedProperty> list) {
            for (int i = 0; i < list.Count; i++) {
                if (list[i] == null) continue;
                EditorGUILayout.PropertyField(list[i]);
            }
        }
    }
}