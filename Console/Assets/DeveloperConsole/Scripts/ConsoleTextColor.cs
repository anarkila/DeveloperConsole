using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    public class ConsoleTextColor : MonoBehaviour {

        [SerializeField] private ConsoleGUIStyle style = ConsoleGUIStyle.Large;

        private void Start() {

            var settings = ConsoleManager.GetSettings();
            if (settings == null) return;

            if (TryGetComponent(out TMP_Text textComponent)) {
                var color = style == ConsoleGUIStyle.Large ? settings.largeGUITextColor : settings.MinimalGUITextColor;
                textComponent.color = color;
            }
        }
    }
}