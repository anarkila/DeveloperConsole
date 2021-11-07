using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;

namespace DeveloperConsole {

    public class ConsoleInputfield : MonoBehaviour {

        private List<string> commandsWithValues = new List<string>();
        private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.050f);
        private List<string> closestMatches = new List<string>();
        private List<string> predictions = new List<string>();
        private List<string> allConsoleCommands = new List<string>();
        private bool allowHintChecking = true;
        private int previousCommandIndex = 0;
        private bool allowEnterClick = true;
        private TMP_InputField inputField;
        private string currentSuggestion;
        private int suggestionIndex = 0;

        private void Awake() {
            inputField = GetComponent<TMP_InputField>();

            ConsoleEvents.RegisterConsoleRefreshEvent += ConsoleRefreshedCallback;
            ConsoleEvents.RegisterPreviousCommandEvent += SearchPreviousCommand;
            ConsoleEvents.RegisterFillCommandEvent += FillCommandFromSuggestion;
            ConsoleEvents.RegisterInputFieldSubmit += InputFieldEnter;

#if UNITY_EDITOR
            if (inputField == null) {
                Debug.LogError("InputField is null!");
                this.enabled = false;
            }
#endif
        }

        private void Start() {
            if (inputField == null) return;

            commandsWithValues = CommandDatabase.GeCommandStringsWithDefaultValues();
            allConsoleCommands = CommandDatabase.GetCommandStrings();
            inputField.onValueChanged.AddListener(FindClosestsPredictions);
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterConsoleRefreshEvent -= ConsoleRefreshedCallback;
            ConsoleEvents.RegisterPreviousCommandEvent -= SearchPreviousCommand;
            ConsoleEvents.RegisterFillCommandEvent -= FillCommandFromSuggestion;
            ConsoleEvents.RegisterInputFieldSubmit -= InputFieldEnter;
        }

        private void ConsoleRefreshedCallback() {
            commandsWithValues = CommandDatabase.GeCommandStringsWithDefaultValues();
            allConsoleCommands = CommandDatabase.GetCommandStrings();
        }

        private void OnEnable() {
            allowEnterClick = true;
            FocusInputField();
        }

        private void OnDisable() {
            ClearInputField();
            ResetParameters();
            ClearSuggestion();
            previousCommandIndex = 0;
            allowEnterClick = true;
        }

        private void SearchPreviousCommand() {
            var previouslyExecutedCommands = CommandDatabase.GetPreviouslyExecutedCommands();

            if (inputField == null || previouslyExecutedCommands.Count == 0) return;

            ++previousCommandIndex;
            if (previousCommandIndex > previouslyExecutedCommands.Count || previousCommandIndex == previouslyExecutedCommands.Count) {
                previousCommandIndex = 0;
            }
            inputField.text = string.Empty;
            inputField.text = previouslyExecutedCommands[previousCommandIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        private void FillCommandFromSuggestion() {
            if (inputField == null || currentSuggestion == null) return;


            if (suggestionIndex > closestMatches.Count || suggestionIndex == closestMatches.Count) {
                suggestionIndex = 0;
            }
            if (closestMatches == null || closestMatches.Count == 0) return;

            allowHintChecking = false;
            inputField.text = closestMatches[suggestionIndex];
            inputField.caretPosition = inputField.text.Length;

            ++suggestionIndex;

            StartCoroutine(AllowEnterClickDelay());
        }

        private void InputFieldEnter() {
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

            StartCoroutine(AllowEnterClickDelay());
            CommandDatabase.TryExecuteCommand(text);
        }

        private IEnumerator AllowEnterClickDelay() {
            yield return delay;
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
            yield return delay;
            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();
            allowHintChecking = true;
        }

        /// <summary>
        /// Try to find predictions from current inputfield text
        /// This is pretty messy..
        /// </summary>
        private void FindClosestsPredictions(string text) {
            if (inputField == null || !allowHintChecking || !ConsoleManager.ShowConsolePredictions()) return;

            if (string.IsNullOrEmpty(text)) {
                closestMatches.Clear();
                ConsoleEvents.Predictions(null);
                return;
            }

            int index = 10000;
            bool valid = false;
            bool closeMatch = false;
            int smallestDistance = 10000;
            closestMatches.Clear();
            predictions.Clear();

            // loop through all console commands strings and try to find closest matching command
            // TODO: If this list becomes huge, then this check might be better to move to background thread?
            for (int i = 0; i < commandsWithValues.Count; i++) {

                // check if first letter is the same
                char inputfieldFirstChar = text[0];
                char commandFirstChar = allConsoleCommands[i][0];
                if (inputfieldFirstChar != commandFirstChar) continue;

                // if text contains command name
                if (text.Contains(allConsoleCommands[i])) {
                    closeMatch = true;
                    index = i;
                    closestMatches.Add(commandsWithValues[i]);
                }

                int distance = ConsoleUtils.CalcLevenshteinDistance(text, allConsoleCommands[i]);

                if (smallestDistance >= distance) {
                    smallestDistance = distance;
                    index = i;

                    // Validate that all characters in a string exist in current command name string
                    char[] charArr = text.ToCharArray();
                    bool validCharacters = true;
                    for (int j = 0; j < charArr.Length; j++) {
                        valid = allConsoleCommands[i].Contains(charArr[j]);
                        if (!valid) {
                            validCharacters = false;
                        }
                        else {
                            if (text.Length < allConsoleCommands[i].Length + 1 && !closestMatches.Contains(commandsWithValues[i])) {
                                closestMatches.Add(commandsWithValues[i]);
                            }
                        }
                    }
                    if (!validCharacters) {
                        valid = false;
                    }
                }
            }

            if (closestMatches.Count != 0) {

                // Reverse list
                closestMatches.Reverse();

                // add first 5 items from list to final list.
                for (int i = 0; i < closestMatches.Count; i++) {
                    predictions.Add(closestMatches[i]);
                    if (i == 4) break;
                }
            }

            ConsoleEvents.Predictions(predictions);
            if (closeMatch || smallestDistance < commandsWithValues.Count && valid) {
                currentSuggestion = commandsWithValues[index];
            }
            else {
                ClearSuggestion();
                ResetParameters();
            }
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
    }
}