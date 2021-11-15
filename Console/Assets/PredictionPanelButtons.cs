using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    public class PredictionPanelButtons : MonoBehaviour {

        [SerializeField] private GameObject[] panelObjects; // must be setup in inspector!
        private Button[] buttons;
        private TMP_Text[] texts;

        private void Start() {
            ShowPredictionPanel(false);

            buttons = new Button[panelObjects.Length];
            texts = new TMP_Text[panelObjects.Length];

            for (int i = 0; i < panelObjects.Length; i++) {
                if (panelObjects[i] == null) {
#if UNITY_EDITOR
                    Debug.Log("null GameObject!");
#endif
                    continue;
                }

                buttons[i] = panelObjects[i].GetComponent<Button>();
                texts[i] = panelObjects[i].transform.GetChild(0).GetComponent<TMP_Text>();
            }

            ConsoleEvents.RegisterConsolePredictionEvent += PredictionEvent;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsolePredictionEvent -= PredictionEvent;
        }

        private void PredictionEvent(List<string> newSuggestions) {
            if (newSuggestions == null || newSuggestions.Count == 0) {
                ShowPredictionPanel(false);
                return;
            }

            ShowPredictionPanel(true);

            var count = newSuggestions.Count;
            for (int i = 0; i < texts.Length; i++) {
                if (texts[i] == null) continue;

                if (i < count) {
                    var temp = newSuggestions[i];
                    texts[i].text = temp;
                    panelObjects[i].SetActive(true);
                    buttons[i].onClick.AddListener(() => SetInputfieldText(temp));

                }
                else {
                    texts[i].text = string.Empty;
                    panelObjects[i].SetActive(false);
                    buttons[i].onClick.RemoveAllListeners();
                }
            }
        }

        private void ShowPredictionPanel(bool show) {
            gameObject.SetActive(show);
        }

        private void SetInputfieldText(string input) {
            ConsoleEvents.SetInputfieldText(input);
            ShowPredictionPanel(false);
        }
    }
}