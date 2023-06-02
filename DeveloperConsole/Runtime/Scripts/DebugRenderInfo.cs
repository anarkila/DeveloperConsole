#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// This script collects rendering information in Unity Editor
    /// if 'collectRenderInfoEditor' option is set to true.
    /// To print rendering information to console call 'debug.renderinfo'
    /// </summary>
    public class DebugRenderInfo : MonoBehaviour {

        private int HighestTrianglessCount = 0;
        private int HighestDrawCallsCount = 0;
        private int HighestVerticesCount = 0;
        private int HighestBatchesCount = 0;

        private int highestFPS = 0;
        private float avgFPS = 0f;

        private void Awake() {
            var settings = ConsoleManager.GetSettings();
            if (!settings.collectRenderInfoEditor) {
                Console.RemoveCommand("debug.renderinfo");
                this.enabled = false;
            }
        }

        private void Update() {
            var deltaTime = Time.deltaTime;

            // calculate low and high FPS
            var fps = 1.0f / deltaTime;
            if (fps > highestFPS) highestFPS = (int)fps;
    
            // calculate average FPS
            avgFPS += ((deltaTime / Time.timeScale) - avgFPS) * 0.03f;

            if (HighestDrawCallsCount < UnityStats.drawCalls) HighestDrawCallsCount = UnityStats.drawCalls;
            if (HighestBatchesCount < UnityStats.batches) HighestBatchesCount = UnityStats.batches;

            if (HighestTrianglessCount < UnityStats.triangles) HighestTrianglessCount = UnityStats.triangles;
            if (HighestVerticesCount < UnityStats.vertices) HighestVerticesCount = UnityStats.vertices;
        }

        [ConsoleCommand("debug_renderinfo", info: "Print rendering information (Editor only)")]
        private void PrintRenderInfo() {

            var currentTargetFPS = Application.targetFrameRate;
            var target = string.Empty;
            if (currentTargetFPS <= 0) {
                target = ConsoleConstants.UNLIMITED;
            } 
            else {
                target = currentTargetFPS.ToString();
            }

            Console.LogEmpty();
            Debug.Log(string.Format("Current resolution is: {0} x {1}", Screen.width, Screen.height));
            Debug.Log("Application target frame rate is set to: " + target);
            Debug.Log(string.Format("Highest FPS: {0} --- Avg FPS: {1}", highestFPS, (int)(1f / avgFPS)));
            Debug.Log("Highest batches count: " + HighestBatchesCount);
            Debug.Log("Highest draw call count: " + HighestDrawCallsCount);
            Debug.Log("Highest vertices count: " + HighestVerticesCount);
            Debug.Log("Highest triangles count: " + HighestTrianglessCount);
            Console.LogEmpty();
        }
    }
}

#endif