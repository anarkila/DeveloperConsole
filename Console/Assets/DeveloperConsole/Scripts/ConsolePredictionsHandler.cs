using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace DeveloperConsole {

    public class ConsolePredictionsHandler : MonoBehaviour {

        [SerializeField] private TMP_Text[] suggestions;            // must be setup in inspector!
        [SerializeField] private Image[] images;                    // must be setup in inspector!
        private int imageIndex = 0;

        private void Start() {

#if UNITY_EDITOR
            if (suggestions.Length == 0 || images.Length == 0 || images.Length != suggestions.Length) {
                Debug.Log("Suggestions are not setup correctly!");
            }
#endif

            ShowSuggestionPanel(false);
            ConsoleEvents.RegisterConsoleSuggestions += SuggestionsEvent;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleSuggestions -= SuggestionsEvent;
        }

        private void SuggestionsEvent(List<string> newSuggestions) {
            if (newSuggestions == null || newSuggestions.Count == 0) {
                ShowSuggestionPanel(false);
                ShowSuggestionPanelBackGroundImage(0);
                HideAllSuggestionPanelImages();
                return;
            }

            ShowSuggestionPanelBackGroundImage(newSuggestions.Count);
            ShowSuggestionPanel(true);
            var count = newSuggestions.Count;
            for (int i = 0; i < suggestions.Length; i++) {
                if (suggestions[i] == null) continue;

                if (i < count) {
                    suggestions[i].text = newSuggestions[i];
                }
                else {
                    suggestions[i].text = string.Empty;
                }
            }
        }

        private void ShowSuggestionPanel(bool show) {
            gameObject.SetActive(show);
        }

        private void ShowSuggestionPanelBackGroundImage(int suggestionCount) {
            if (images[imageIndex] != null) images[imageIndex].enabled = false;     // disable previous image

            int count = suggestionCount - 1;
            if (count >= 0 && count < suggestions.Length) {
                if (images[imageIndex] != null) images[count].enabled = true;
                imageIndex = count;
            }
        }

        private void HideAllSuggestionPanelImages() {
            for (int i = 0; i < images.Length; i++) {
                if (images[i] == null) continue;

                images[i].enabled = false;
            }
        }
    }
}