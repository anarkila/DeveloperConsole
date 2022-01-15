using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Anarkila.DeveloperConsole {

    public class ConsoleInputField : MonoBehaviour {

        private WaitForSecondsRealtime cachedDelay = new WaitForSecondsRealtime(0.050f);
        private List<string> commandsWithValues = new List<string>();
        private List<string> allConsoleCommands = new List<string>();
        private List<string> closestMatches = new List<string>();
        private List<string> predictions = new List<string>(); 
        private bool allowPredictionCheck = true;
        private int previousCommandIndex = 0;
        private bool allowPredictions = true;
        private bool allowEnterClick = true;
        private TMP_InputField inputField;
        private string currentSuggestion;
        private int suggestionIndex = 0;
        private string previousText;

        private void Awake() {
            if (TryGetComponent(out TMP_InputField inputfield)) {
                inputField = inputfield;
            }

#if UNITY_EDITOR
            else {
                Debug.Log(string.Format("Gameobject {0} doesn't have TMP_InputField component!", gameObject.name));
                enabled = false;
                return;
            }
#endif
            ConsoleEvents.RegisterPreviousCommandEvent += SearchPreviousCommand;
            ConsoleEvents.RegisterFillCommandEvent += FillCommandFromSuggestion;
            ConsoleEvents.RegisterInputfieldTextEvent += SetInputfieldText;
            ConsoleEvents.RegisterInputFieldSubmit += InputFieldSubmit;
            ConsoleEvents.RegisterListsChangedEvent += UpdateLists;
        }

        private void Start() {
            if (inputField == null) return;

            UpdateLists();

            // Add onvaluechanged listener to Console inputfield
            inputField.onValueChanged.AddListener(PredictInput);
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterFillCommandEvent -= FillCommandFromSuggestion;
            ConsoleEvents.RegisterPreviousCommandEvent -= SearchPreviousCommand;
            ConsoleEvents.RegisterInputfieldTextEvent -= SetInputfieldText;
            ConsoleEvents.RegisterInputFieldSubmit -= InputFieldSubmit;
            ConsoleEvents.RegisterListsChangedEvent -= UpdateLists;
        }

        private void SetInputfieldText(string input) {
            inputField.text = input;
            previousText = inputField.text;
            inputField.caretPosition = inputField.text.Length;

            if (gameObject.activeInHierarchy) {
                FocusInputField();
            }
        }

        private void UpdateLists() {
            commandsWithValues = CommandDatabase.GeCommandStringsWithDefaultValues();
            allConsoleCommands = CommandDatabase.GetConsoleCommandList();
            allowPredictions = ConsoleManager.ShowConsolePredictions();
        }

        private void OnEnable() {
            allowEnterClick = true;
            FocusInputField();
        }

        private void OnDisable() {
            StopAllCoroutines();
            ClearInputField();
            ResetParameters();
            ClearSuggestion();
            previousCommandIndex = 0;
            allowEnterClick = true;
        }

        private void SearchPreviousCommand() {
            var prevExecutedCommands = CommandDatabase.GetPreviouslyExecutedCommands();

            if (inputField == null || prevExecutedCommands.Count == 0) return;

            ++previousCommandIndex;
            if (previousCommandIndex > prevExecutedCommands.Count || previousCommandIndex == prevExecutedCommands.Count) {
                previousCommandIndex = 0;
            }
            inputField.text = string.Empty;
            inputField.text = prevExecutedCommands[previousCommandIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        private void FillCommandFromSuggestion() {
            if (inputField == null || currentSuggestion == null) return;

            if (suggestionIndex > closestMatches.Count || suggestionIndex == closestMatches.Count) {
                suggestionIndex = 0;
            }

            if (closestMatches == null || closestMatches.Count == 0) return;

            allowPredictionCheck = false;
            inputField.text = closestMatches[suggestionIndex];
            previousText = inputField.text;
            inputField.caretPosition = inputField.text.Length;

            ++suggestionIndex;

            StartCoroutine(AllowEnterClickDelay());
        }

        private void InputFieldSubmit() {
            if (inputField == null || !allowEnterClick) return;

            var text = inputField.text;

            if (string.IsNullOrWhiteSpace(text)) {
                ClearSuggestion();
                return;
            }

            allowEnterClick = false;
            ClearInputField();
            FocusInputField();
            ClearSuggestion();

            if (gameObject.activeInHierarchy) {
                StartCoroutine(AllowEnterClickDelay());
            }

            // Try to execute command
            CommandDatabase.TryExecuteCommand(text);
        }

        private IEnumerator AllowEnterClickDelay() {
            yield return cachedDelay;
            allowEnterClick = true;
        }

        private void ClearInputField() {
            if (inputField == null) return;

            inputField.Select();
            inputField.text = string.Empty;
        }

        private void FocusInputField() {
            if (inputField == null) return;

            // For some reason TMP_InputField doesn't work in OnEnable without delay
            StartCoroutine(DelayEnable());
        }

        private IEnumerator DelayEnable() {
            yield return cachedDelay;
            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();
            allowPredictionCheck = true;
        }

        private void ClearSuggestion() {
            closestMatches.Clear();
            predictions.Clear();
            ConsoleEvents.Predictions(closestMatches);
        }

        private void ResetParameters() {
            currentSuggestion = string.Empty;
            suggestionIndex = 0;
        }


        /// <summary>
        /// Try to find predictions from current inputfield text
        /// This is messy and needs cleanup
        /// </summary>
        private void PredictInput(string input) {
            if (inputField == null || !allowPredictions) return;

            // if input is null, empty or contains '&', then don't show any predictions.
            if (string.IsNullOrEmpty(input) || input.Length == 0 || input.Contains(ConsoleConstants.AND)) {
                closestMatches.Clear();
                ConsoleEvents.Predictions(null);
                return;
            }

            // if allowPredictionCheck is false (command filled with Tab delay)
            // check if user deleted one char (default key: backspace)
            // and allow prediction checking again
            if (!allowPredictionCheck) {
                int len = previousText.Length - input.Length;
                if (len != 1) {
                    return;
                }
                allowPredictionCheck = true;
            }

            int smallestDistance = 10000;
            bool closeMatch = false;
            closestMatches.Clear();
            previousText = input;
            predictions.Clear();
            bool valid = false;
            int index = 10000;

            // loop through all console commands strings and try to find closest matching command
            for (int i = 0; i < commandsWithValues.Count; i++) {

                // check if first letter is the same
                char inputfieldFirstChar = input[0];
                char commandFirstChar = allConsoleCommands[i][0];
                if (inputfieldFirstChar != commandFirstChar) continue;

                // if text contains command name
                if (input.Contains(allConsoleCommands[i])) {
                    closeMatch = true;
                    index = i;
                    closestMatches.Add(commandsWithValues[i]);
                }

                int distance = ConsoleUtils.CalcLevenshteinDistance(input, allConsoleCommands[i]);

                if (smallestDistance >= distance) {
                    smallestDistance = distance;
                    index = i;

                    // Validate that all characters in a string exist in current command name string
                    char[] charArr = input.ToCharArray();
                    for (int j = 0; j < charArr.Length; j++) {
                        valid = allConsoleCommands[i].Contains(charArr[j].ToString());
                        if (valid && input.Length < allConsoleCommands[i].Length + 1 && !closestMatches.Contains(commandsWithValues[i])) {
                            closestMatches.Add(commandsWithValues[i]);
                        }
                    }
                }
            }

            if (closestMatches.Count != 0) {
                closestMatches.Reverse();

                // add first 5 items from list to final list.
                for (int i = 0; i < closestMatches.Count; i++) {
                    if (string.IsNullOrEmpty(closestMatches[i])) continue;
  
                    predictions.Add(closestMatches[i]);

                    if (i == 4) break;
                }
            }

            // Send prediction event
            ConsoleEvents.Predictions(predictions);

            if (closeMatch || smallestDistance < commandsWithValues.Count && valid) {
                currentSuggestion = commandsWithValues[index];
            }
            else {
                ClearSuggestion();
                ResetParameters();
            }
        }
    }
}