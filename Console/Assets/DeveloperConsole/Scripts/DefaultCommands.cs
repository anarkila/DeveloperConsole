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
        [ConsoleCommand("help", hiddenCommandMinimalGUI:true)]
        private static void Help() {
            CommandDatabase.PrintAllCommands();
        }

        /// <summary>
        /// Quit Application
        /// </summary>
        [ConsoleCommand("quit", info:"Quit application")]
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
        /// Clear all console messages
        /// </summary>
        [ConsoleCommand("clear", info: "Clear all messages")]
        private static void ClearAllDeveloperConsoleMessages() {
            ConsoleEvents.ClearConsoleMessages();
        }

        /// <summary>
        /// Resets Console window size and position
        /// </summary>
        [ConsoleCommand("reset", hiddenCommandMinimalGUI: true, info: "Reset Console window size and position")]
        private static void ResetDeveloperConsole() {
            ConsoleEvents.ResetConsole();
        }

        /// <summary>
        /// Set application target frame rate
        /// </summary>
        [ConsoleCommand("max_fps", info:"Set application target frame rate")]
        private static void SetTargetFrameRate(int fps) {
            Application.targetFrameRate = fps;
        }

        /// <summary>
        /// Toggle console style between Large and Minimal
        /// </summary>
        [ConsoleCommand("console.style", debugOnlyCommand: true, info: "Change console style between Large and Minimal")]
        private static void ChangeConsoleGUI() {
            ConsoleEvents.SwitchGUIStyle();
        }

        /// <summary>
        /// Load scene by build index
        /// Scenes must be included in 'Scenes in build' in the Build settings!
        /// </summary>
        [ConsoleCommand("scene.loadindex", "1", debugOnlyCommand: true, info: "Load scene by index")]
        private static void LoadSceneByIndexSingle(int index) {
            ConsoleEvents.LoadSceneByIndexSingle(index);
        }

        /// <summary>
        /// Load scene by build name
        /// </summary>
        [ConsoleCommand("scene.loadname", debugOnlyCommand: true, info: "Load scene by name")]
        private static void LoadSceneByName(string sceneName) {
            ConsoleEvents.LoadSceneByName(sceneName);
        }

        /// <summary>
        /// Load scene additively by build index
        /// </summary>
        [ConsoleCommand("scene.loadindexadd", "2", debugOnlyCommand: true, info: "Load scene by index additively")]
        private static void LoadSceneByIndexAdditive(int index) {
            ConsoleEvents.LoadSceneByIndexAdditive(index);
        }

        /// <summary>
        /// Load scene by build name
        /// </summary>
        [ConsoleCommand("scene.unloadname", debugOnlyCommand: true, info: "Unload scene by name")]
        private static void UnLoadSceneByName(string sceneName) {
            ConsoleEvents.UnLoadSceneByName(sceneName);
        }

        /// <summary>
        /// Unload scene by build index
        /// </summary>
        [ConsoleCommand("scene.unloadindex", debugOnlyCommand: true, info: "Unload scene by index")]
        private static void UnLoadSceneByIndex(int index) {
            ConsoleEvents.UnloadSceneByIndex(index);
        }
    }
}