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
        [ConsoleCommand("help", hiddenCommandMinimalGUI:true)] // hide 'help' command when console GUI style is Minimal
        private static void Help() {
            CommandDatabase.PrintAllCommands();
        }

        /// <summary>
        /// Quit Application
        /// </summary>
        [ConsoleCommand("quit")]
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
        [ConsoleCommand("close")]
        private static void CloseDeveloperConsole() {
            ConsoleEvents.CloseConsole();
        }

        /// <summary>
        /// Clear all console messages
        /// </summary>
        [ConsoleCommand("clear")]
        private static void ClearAllDeveloperConsoleMessages() {
            ConsoleEvents.ClearConsoleMessages();
        }

        /// <summary>
        /// Resets Console window size and position
        /// </summary>
        [ConsoleCommand("reset", hiddenCommandMinimalGUI: true)]
        private static void ResetDeveloperConsole() {
            ConsoleEvents.ResetConsole();
        }

        /// <summary>
        /// Set application target frame rate
        /// </summary>
        [ConsoleCommand("max_fps")]
        private static void SetTargetFrameRate(int fps) {
            Application.targetFrameRate = fps;
        }



        // Below commands are set to be debug only commands,
        // meaning they only register in Editor and Development Builds.
        // For final build, these commands are ignored,
        // remove 'debugOnlyCommand:true' if you wish to include these in final build for some reason.

        /// <summary>
        /// Toggle console style between Large and Minimal
        /// </summary>
        [ConsoleCommand("console.style", debugOnlyCommand:true)]
        private static void ChangeConsoleGUI() {
            ConsoleEvents.SwitchGUIStyle();
        }

        /// <summary>
        /// Load scene by Index
        /// Scenes must be included in 'Scenes in build' in the Build settings!
        /// </summary>
        [ConsoleCommand("scene.loadbyindex", "1", debugOnlyCommand: true)]
        private static void LoadSceneByIndex(int index) {
            ConsoleEvents.LoadSceneByIndex(index);
        }

        /// <summary>
        /// Load scene by name
        /// Scenes must be included in 'Scenes in build' in the Build settings!
        /// </summary>
        [ConsoleCommand("scene.loadbyname", debugOnlyCommand: true)]
        private static void LoadSceneByName(string sceneName) {
            ConsoleEvents.LoadSceneByName(sceneName);
        }
    }
}