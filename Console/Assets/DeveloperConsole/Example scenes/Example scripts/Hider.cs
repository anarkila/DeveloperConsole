using UnityEngine;

namespace DeveloperConsole {

    public class Hider : MonoBehaviour {

        private void Awake() {
            gameObject.SetActive(false);
        }
    }
}