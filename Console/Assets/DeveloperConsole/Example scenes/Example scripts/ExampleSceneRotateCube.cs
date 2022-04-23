using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ExampleSceneRotateCube : MonoBehaviour {

        private float degreesPerSecond = 29.0f;
        private float amplitude = 0.5f;
        private float frequency = 1f;

        private Transform cachedTransform;
        private Vector3 startPosition;
        private Vector3 tempPos;

        private void Awake() {
            cachedTransform = this.transform;
        }

        private void OnEnable() {
            startPosition = cachedTransform.position;
        }

        private void Update() {
            // Spin object around Y-Axis
            var rotation = new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f);
            cachedTransform.Rotate(rotation, Space.World);

            // Float up/down
            tempPos = startPosition;
            tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
            cachedTransform.position = tempPos;
        }


        // Multiple instances of same command is allowed.
        // In the example scenes, all 3 cubes get disabled/enabled when this is called.
        [ConsoleCommand("cube_enabled", "false")]
        private void EnableCubeRotation(bool enabled) {
            this.enabled = enabled;
        }
    }
}