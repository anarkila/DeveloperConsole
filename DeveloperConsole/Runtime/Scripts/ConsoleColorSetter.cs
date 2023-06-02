using UnityEngine.UI;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ConsoleColorSetter : MonoBehaviour {

        private enum ColorTarget {
            LargeGUIBorder,
            largeGUIBackground,
            minimalGUIBackground,
            controlColor,
            LargeGUIScrollbarHandle,
            LargeGUIScrollbarBackground
        }

        [SerializeField] private ColorTarget style = ColorTarget.largeGUIBackground;

        private void Awake() {
            ConsoleEvents.RegisterConsoleColorsChangedEvent += SetColors;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleColorsChangedEvent -= SetColors;
        }

        private void Start() {
            SetColors();
        }

        private void SetColors() {
            var settings = ConsoleManager.GetSettings();

            if (settings == null) return;

            if (TryGetComponent(out Image image)) {
                switch (style) {
                    case ColorTarget.LargeGUIBorder:
                        image.color = settings.consoleColors.largeGUIBorderColor;
                        break;

                    case ColorTarget.largeGUIBackground:
                        image.color = settings.consoleColors.largeGUIBackgroundColor;
                        break;

                    case ColorTarget.minimalGUIBackground:
                        image.color = settings.consoleColors.minimalGUIBackgroundColor;
                        break;

                    case ColorTarget.controlColor:
                        image.color = settings.consoleColors.largeGUIControlsColor;
                        break;

                    case ColorTarget.LargeGUIScrollbarHandle:
                        image.color = settings.consoleColors.largeGUIScrollbarHandleColor;
                        break;

                    case ColorTarget.LargeGUIScrollbarBackground:
                        image.color = settings.consoleColors.largeGUIScrollbarBackgroundColor;
                        break;
                }
            }
#if UNITY_EDITOR
            else {
                Debug.Log(string.Format("Gameobject {0} doesn't have Image component!", gameObject.name));
                ConsoleEvents.RegisterConsoleColorsChangedEvent -= SetColors;
                enabled = false;
                return;
            }
#endif
        }
    }
}