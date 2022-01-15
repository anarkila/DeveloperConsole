using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ConsoleColorSetter: MonoBehaviour {

        private enum ColorTarget {
            LargeGUIBorder,
            largeGUIBackground,
            minimalGUIBackground,
            controlColor,
            LargeGUIScrollbarHandle,
            LargeGUIScrollbarBackground
        }

        [SerializeField] private ColorTarget style = ColorTarget.largeGUIBackground;

        private void Start() {

            Image image = null;

            if (TryGetComponent(out Image img)) {
                image = img;
            }
#if UNITY_EDITOR
            else {
                Debug.Log(string.Format("Gameobject {0} doesn't have Image component!", gameObject.name));
                enabled = false;
                return;
            }
#endif

            var settings = ConsoleManager.GetSettings();
            if (settings == null || image == null) return;

            switch (style) {
                case ColorTarget.LargeGUIBorder:
                    image.color = settings.largeGUIBorderColor;
                    break;

                case ColorTarget.largeGUIBackground:
                    image.color = settings.largeGUIBackgroundColor;
                    break;

                case ColorTarget.minimalGUIBackground:
                    image.color = settings.minimalGUIBackgroundColor;
                    break;

                case ColorTarget.controlColor:
                    image.color = settings.largeGUIControlsColor;
                    break;

                case ColorTarget.LargeGUIScrollbarHandle:
                    image.color = settings.largeGUIScrollbarHandleColor;
                    break;

                case ColorTarget.LargeGUIScrollbarBackground:
                    image.color = settings.largeGUIScrollbarBackgroundColor;
                    break;
            }
        }
    }
}