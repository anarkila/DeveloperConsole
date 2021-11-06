using UnityEngine;
using UnityEditor;
using System.IO;

namespace DeveloperConsole {

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
            if (GUILayout.Button("Generate Command list to txt file", GUILayout.Height(30))) {
                if (!writingTextFile) GenerateCommandList();
            }
        }


        private void GenerateCommandList() {
            if (Application.isPlaying) {
                Debug.Log("Exit play mode to generate command txt list.");
                return;
            }

            var path = GetScriptRootPath("ConsoleEditorWindow", true);
            path += "ConsoleCommands.txt";
            DeleteFile(path);
            MakeTextFile(path);
        }

        private void DeleteFile(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        private void MakeTextFile(string path) {
            writingTextFile = true;

            // Get all console commands
            var commands = CommandDatabase.GetConsoleCommandAttributes(Debug.isDebugBuild, false);

            // make new txt file and write to it
            StreamWriter writer = new StreamWriter(path, true);
            string sep = "------ \n";
            writer.WriteLine("All commands found with [ConsoleCommand] attribute in the current project. \n" +
                "Note that manually added commands won't show up here! \n");

            for (int i = 0; i < commands.Count; i++) {
                writer.WriteLine(sep);

                writer.WriteLine("Command name: " + commands[i].commandName);

                string paramInfo;
                if (commands[i].parameterType == null) {
                    paramInfo = "none";
                }
                else {
                    paramInfo = commands[i].parameterType.ToString();
                }
                writer.WriteLine("Parameter: " + paramInfo);

                string defaultValue = commands[i].defaultValue;
                if (!string.IsNullOrEmpty(defaultValue)) {
                    writer.WriteLine("Default value: " + defaultValue);
                }

                writer.WriteLine("Script name: " + commands[i].scriptNameString);

                string methodType;
                if (commands[i].isStaticMethod) {
                    methodType = "static void ";
                }
                else if (commands[i].isCoroutine) {
                    methodType = "IEnumerator ";
                }
                else {
                    methodType = "void ";
                }

                writer.WriteLine("Method name: " + methodType + commands[i].methodname);
            }

            writer.Close();

            // refresh Unity assets and select/highlight new txt file
            AssetDatabase.ImportAsset(path);
            EditorUtility.FocusProjectWindow();
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            Selection.activeObject = obj;

            Debug.Log("Command list generated to path: " + path);
            writingTextFile = false;
        }

        private string GetScriptRootPath(string scriptname, bool parse = false) {

            if (string.IsNullOrEmpty(scriptname)) {
                return "";
            }

            var filter = string.Format("t:Script {0}", scriptname);
            var asset = AssetDatabase.FindAssets(filter);
            var path = AssetDatabase.GUIDToAssetPath(asset[0]);

            if (parse) {
                int index = path.LastIndexOf("/");
                if (index >= 0) {
                    path = path.Substring(0, index + 1);
                }
            }

            return path;
        }
    }
}