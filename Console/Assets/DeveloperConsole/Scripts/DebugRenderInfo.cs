#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace DeveloperConsole {

    /// <summary>
    /// This script collects rendering information in Unity Editor
    /// if collectRenderInfoEditor setting is true
    /// To print rendering information to console 
    /// call command: debug.print.renderinfo
    /// </summary>
    public class DebugRenderInfo : MonoBehaviour {

        private int HighestTrianglessCount = 0;
        private int HighestDrawCallsCount = 0;
        private int HighestVerticesCount = 0;
        private int HighestBatchesCount = 0;

        private void Start() {
            var settings = ConsoleManager.GetSettings();

            if (settings.collectRenderInfoEditor) {
                ConsoleAPI.RegisterCommand(this, "PrintRenderInfo", "debug.print.renderinfo");
            }
            else {
                this.enabled = false;
            }
        }

        private void Update() {

            var thisFrameDrawCallCount = UnityStats.drawCalls;
            var thisFrameBatchesCount = UnityStats.batches;

            var thisFrameTrianglesCount = UnityStats.triangles;
            var thisFrameVerticesCount = UnityStats.vertices;

            if (HighestDrawCallsCount < thisFrameDrawCallCount) HighestDrawCallsCount = thisFrameDrawCallCount;
            if (HighestBatchesCount < thisFrameBatchesCount)  HighestBatchesCount = thisFrameBatchesCount;

            if (HighestTrianglessCount < thisFrameTrianglesCount)  HighestTrianglessCount = thisFrameTrianglesCount;
            if (HighestVerticesCount < thisFrameVerticesCount) HighestVerticesCount = thisFrameVerticesCount;
        }

        private void PrintRenderInfo() {
            Debug.Log("Highest batches count: " + HighestBatchesCount);
            Debug.Log("Highest draw call count: " + HighestDrawCallsCount);

            Debug.Log("Highest vertices count: " + HighestVerticesCount);
            Debug.Log("Highest triagnles count: " + HighestTrianglessCount);
        }
    }
}

#endif