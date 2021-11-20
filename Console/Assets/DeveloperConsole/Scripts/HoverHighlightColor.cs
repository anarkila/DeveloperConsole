using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class HoverHighlightColor : MonoBehaviour {

        private void Start() {

            var settings = ConsoleManager.GetSettings();
            if (settings == null) return;

            if (TryGetComponent(out Button button)) {
                var highlightColor = settings.highlightColor;
                ColorBlock colorVar = button.colors;
                colorVar.highlightedColor = highlightColor;
                colorVar.pressedColor = highlightColor;
                button.colors = colorVar;
            }
        }
    }
}