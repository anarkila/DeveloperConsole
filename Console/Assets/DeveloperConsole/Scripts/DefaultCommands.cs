using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This class implements default Developer Console commands
    /// Feel free to modify or delete these as you wish
    /// </summary>
    public static class DefaultCommands {

        /// <summary>
        /// Print all available console commands
        /// </summary>
        [ConsoleCommand("help", info: "Print all available commands", hiddenCommandMinimalGUI: true)]
        private static void Help() {
            CommandDatabase.PrintAllCommands();
        }

        /// <summary>
        /// Quit Application
        /// </summary>
        [ConsoleCommand("quit", info: "Quit application")]
        private static void QuitApplication() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Close console
        /// </summary>
        [ConsoleCommand("close", info: "Close Console")]
        private static void CloseDeveloperConsole() {
            ConsoleEvents.CloseConsole();
        }

        /// <summary>
        /// Open Console. This is hidden command.
        /// This only exists so you can call: Console.ExecuteCommand("open");
        /// Though it's faster to just call: Console.OpenConsole();
        /// </summary>
        [ConsoleCommand("open", hiddenCommand:true)]
        private static void OpenConsole() {
            ConsoleEvents.OpenConsole();
        }

        /// <summary>
        /// Clear all console messages
        /// </summary>
        [ConsoleCommand("clear", info: "Clear all messages")]
        private static void ClearAllDeveloperConsoleMessages() {
            ConsoleEvents.ClearConsoleMessages();
        }

        /// <summary>
        /// Reset Console window size and position
        /// </summary>
        [ConsoleCommand("reset", hiddenCommandMinimalGUI: true, info: "Reset Console window size and position")]
        private static void ResetDeveloperConsole() {
            ConsoleEvents.ResetConsole();
        }

#if !UNITY_WEBGL
        /// <summary>
        /// Set application target frame rate
        /// </summary>
        [ConsoleCommand("max_fps", info: "Set application target frame rate")]
        private static void SetTargetFrameRate(int fps) {
            Application.targetFrameRate = fps;
        }
#endif



        // --- All commands below are marked as Debug only which only work in Unity Editor and Development build(s) ---
        // to allow below commands in final build, remove debugOnlyCommand overloads

        /// <summary>
        /// Load scene by build index
        /// Scenes must be included in 'Scenes in build' in the Build settings!
        /// </summary>
        [ConsoleCommand("scene_loadindex", "1", debugOnlyCommand: true, info: "Load scene by index")]
        private static void LoadSceneByIndexSingle(int index) {
            ConsoleEvents.LoadSceneByIndexSingle(index);
        }

        /// <summary>
        /// Load scene by build name
        /// </summary>
        [ConsoleCommand("scene_loadname", debugOnlyCommand: true, info: "Load scene by name")]
        private static void LoadSceneByName(string sceneName) {
            ConsoleEvents.LoadSceneByName(sceneName);
        }

        /// <summary>
        /// Load scene additively by build index
        /// </summary>
        [ConsoleCommand("scene_addloadindex", "2", debugOnlyCommand: true, info: "Load scene by index additive")]
        private static void LoadSceneByIndexAdditive(int index) {
            ConsoleEvents.LoadSceneByIndexAdditive(index);
        }

        /// <summary>
        /// Load scene by build name
        /// </summary>
        [ConsoleCommand("scene_unloadname", debugOnlyCommand: true, info: "Unload scene by name")]
        private static void UnLoadSceneByName(string sceneName) {
            ConsoleEvents.UnLoadSceneByName(sceneName);
        }

        /// <summary>
        /// Unload scene by build index
        /// </summary>
        [ConsoleCommand("scene_unloadindex", debugOnlyCommand: true, info: "Unload scene by index")]
        private static void UnLoadSceneByIndex(int index) {
            ConsoleEvents.UnloadSceneByIndex(index);
        }

        /// <summary>
        /// Print scene information
        /// </summary>
        [ConsoleCommand("scene_information", info: "Print Scene information", debugOnlyCommand: true)]
        private static void PrintSceneInformation() {
            Console.LogEmpty();
            Console.Log("Total Scene Count: " + UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings);
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++) {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
                string s = "Scene " + i + " Name: ";
                Console.Log(s + sceneName);
            }
        }

        /// <summary>
        /// Log empty line to console (visibility help)
        /// You can call this programmatically from anywhere with:
        /// Console.LogEmpty();
        /// </summary>
        [ConsoleCommand("empty", info: "Print empty line", debugOnlyCommand: true)]
        private static void LogEmpty() {
            Console.LogEmpty();
        }

        [ConsoleCommand("log_to_file", info: "Log all current messages to file", debugOnlyCommand: true)]
        private static void LogMessagesToFile() {
            Console.WriteMessagesToFile();
        }
    }
}