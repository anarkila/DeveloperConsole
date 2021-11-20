using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ConsoleBackgroundOpacity : MonoBehaviour {

        private Image image;

        private void Start() {
            if (TryGetComponent(out Image img)) {
                image = img;
            }

            var settings = ConsoleManager.GetSettings();
            float percentage = settings.consoleBackgroundOpacity / 100;
            SetBackgroundAlpha(percentage);
        }

        // Uncomment below line if you wish to change large GUI background opacity through command
        //[ConsoleCommand("console.alpha", "0.5")]
        private void SetBackgroundAlpha(float alpha) {
            if (image == null) return;

            if (alpha <= 0.0f || alpha >= 1.0f) {
                alpha = 1f;
            }

            var tempColor = image.color;
            tempColor.a = alpha;
            image.color = tempColor;
        }
    }
}