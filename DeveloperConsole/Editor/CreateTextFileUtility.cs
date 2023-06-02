#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

namespace Anarkila.DeveloperConsole {

    public static class CreateTextFileUtility {

        public static bool GenerateCommandList() {
            if (Application.isPlaying) {
                Debug.Log("Exit play mode to generate command list.");
                return false;
            }

            var path = GetScriptRootPath("CreateTextFileUtility", true);
            path += "ConsoleCommands.txt";
            DeleteFile(path);
            MakeTextFile(path);

            return true;
        }

        private static void DeleteFile(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        private static void MakeTextFile(string path) {

            // Get all console commands
            var commands = CommandDatabase.GetConsoleCommandAttributes(Debug.isDebugBuild, false);

            // make new txt file and write to it
            StreamWriter writer = new StreamWriter(path, true);
            string separator = "------ \n";
            writer.WriteLine("All commands found with [ConsoleCommand] attribute in the current project.");
            writer.WriteLine("Commands found: " + commands.Count);

            for (int i = 0; i < commands.Count; i++) {
                writer.WriteLine(separator);

                writer.WriteLine("Command name: " + commands[i].commandName);

                if (commands[i].parameters == null || commands[i].parameters.Length == 0) {
                    writer.WriteLine("Parameter: None");
                }
                else {
                    for (int k = 0; k < commands[i].parameters.Length; k++) {
                        var x = string.Format("Parameter {0}: {1}", k + 1, commands[i].parameters[k].FullName);
                        writer.WriteLine(x);
                    }
                }
                string defaultValue = commands[i].defaultValue;
                if (!string.IsNullOrEmpty(defaultValue)) {
                    writer.WriteLine("Default value: " + defaultValue);
                }

                writer.WriteLine("Script name: " + commands[i].scriptNameString + ".cs");

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

                writer.WriteLine("Method name: " + methodType + commands[i].methodName);


                if (commands[i].hiddenCommand) {
                    writer.WriteLine("is hidden command: " + commands[i].hiddenCommand);
                }

                if (commands[i].hiddenCommandMinimalGUI) {
                    writer.WriteLine("is hidden minimal GUI: " + commands[i].hiddenCommandMinimalGUI);
                }
            }

            writer.Close();

            // refresh Unity assets and select/highlight new txt file
            AssetDatabase.ImportAsset(path);
            EditorUtility.FocusProjectWindow();
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            Selection.activeObject = obj;

            Debug.Log("Command list generated to path: " + path);
        }

        private static string GetScriptRootPath(string scriptname, bool parse = false) {

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

#endif