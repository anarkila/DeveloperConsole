#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DeveloperConsole {

    /// <summary>
    /// This script collects rendering information in Unity Editor
    /// if collectRenderInfoEditor setting is set to true.
    /// To print rendering information to console
    /// call 'debug.renderinfo'
    /// </summary>
    public class DebugRenderInfo : MonoBehaviour {

        private int HighestTrianglessCount = 0;
        private int HighestDrawCallsCount = 0;
        private int HighestVerticesCount = 0;
        private int HighestBatchesCount = 0;

        //private int lowestFPS = 10000;
        private int highestFPS = 0;
        private float avgFPS = 0f;

        private void Awake() {
            var settings = ConsoleManager.GetSettings();

            if (settings.collectRenderInfoEditor) {
                ConsoleAPI.RegisterCommand(this, "PrintRenderInfo", "debug.renderinfo");
            }
            else {
                this.enabled = false;
            }
        }

        private void Update() {

            var deltaTime = Time.deltaTime;

            // calculate low and high FPS
            var fps = 1.0 / deltaTime;
            //if (fps < lowestFPS) lowestFPS = (int)fps;
            if (fps > highestFPS) highestFPS = (int)fps;
            
            // calculate average FPS
            avgFPS += ((deltaTime / Time.timeScale) - avgFPS) * 0.03f;

            if (HighestDrawCallsCount < UnityStats.drawCalls) HighestDrawCallsCount = UnityStats.drawCalls;
            if (HighestBatchesCount < UnityStats.batches) HighestBatchesCount = UnityStats.batches;

            if (HighestTrianglessCount < UnityStats.triangles) HighestTrianglessCount = UnityStats.triangles;
            if (HighestVerticesCount < UnityStats.vertices) HighestVerticesCount = UnityStats.vertices;
        }

        private void PrintRenderInfo() {
            // this might always show really low fps as this checked right after scene load.
            //Debug.Log("Low FPS: " + lowestFPS);      

            Debug.Log("Avg FPS: " + (int)(1F / avgFPS));
            Debug.Log("High FPS: " + highestFPS);

            Debug.Log("Highest batches count: " + HighestBatchesCount);
            Debug.Log("Highest draw call count: " + HighestDrawCallsCount);

            Debug.Log("Highest vertices count: " + HighestVerticesCount);
            Debug.Log("Highest triagnles count: " + HighestTrianglessCount);
        }
    }
}

#endif