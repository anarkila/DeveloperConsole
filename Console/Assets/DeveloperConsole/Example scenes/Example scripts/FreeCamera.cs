using UnityEngine;

namespace DeveloperConsole {

    /// <summary>
    /// Simple free camera script for moving transform with mouse and keyboard input
    /// This script is enabled/disabled when Developer Console state changes
    /// </summary>
    public class FreeCamera : MonoBehaviour {

        [SerializeField] private float lookSpeed = 3f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float sprintSpeed = 40f;

        private float pitch;
        private float yaw;

        private void Awake() {
            ConsoleAPI.RegisterConsoleStateChangeEvent += ConsoleEvent;    // Register console state change event
            CursorState(false);
        }

        private void OnDestroy() {                              // UnRegister console state change event
            ConsoleAPI.RegisterConsoleStateChangeEvent -= ConsoleEvent;    // Be sure to unregister from event either on OnDestroy() or OnDisable() 
        }                                                       // Otherwise you may get error                    

        /// <summary>
        /// Get callback from Developer Console when 
        /// its state is changed (opened/closed)
        /// and enable/disable this script based on console state
        /// </summary>
        private void ConsoleEvent(bool consoleIsEnabled) {
            this.enabled = !consoleIsEnabled;

            if (this.enabled) {
                CursorState(false);
            }
        }

        private void OnEnable() {
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }

        private void Update() {
            // Other way to check if Developer Console is open
            //if (ConsoleAPI.IsConsoleOpen()) return;

            var rotStrafe = Input.GetAxis("Mouse X");
            var rotFwd = Input.GetAxis("Mouse Y");

            yaw = (yaw + lookSpeed * rotStrafe) % 360f;
            pitch = (pitch - lookSpeed * rotFwd) % 360f;
            transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);

            var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
            var forward = speed * Input.GetAxis("Vertical");
            var right = speed * Input.GetAxis("Horizontal");
            var up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
            transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
        }

        private static void CursorState(bool show) {
            Cursor.visible = show;
            if (show) {
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}