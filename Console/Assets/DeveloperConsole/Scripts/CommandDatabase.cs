﻿using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

namespace DeveloperConsole {

    public static class CommandDatabase {

        private static List<ConsoleCommandData> consoleCommandsRegisteredBeforeInit = new List<ConsoleCommandData>();
        private static List<ConsoleCommandData> consoleCommands = new List<ConsoleCommandData>();
        private static List<ConsoleCommandData> staticCommands = new List<ConsoleCommandData>();
        private static List<string> commandStringsWithDefaultValues = new List<string>();
        private static List<string> consoleCommandStrings = new List<string>();
        private static List<string> executedCommands = new List<string>();
        private static bool staticCommandsCached = false;
        private static int executedCommandCount;
        private static int failedCommandCount;

        /// <summary>
        /// Try to execute command from string.
        /// </summary>
        public static bool TryExecuteCommand(string input) {

            bool success = false;
            string parameterAsString = null;
            char empty = ' ';
            if (input.Contains(empty)) {
                int index = input.IndexOf(empty);
                index = input.IndexOf(empty, index);
                parameterAsString = input.Substring(index, input.Length - index);
                input = input.Substring(0, index);
            }
            //Debug.Log(parameterAsString);

            // Loop through all console commands and try to find matching command
            for (int i = 0; i < consoleCommands.Count; i++) {
                if (input == consoleCommands[i].commandName) {

                    // If command does not take parameter..
                    if (consoleCommands[i].parameterType == null && parameterAsString != null) {

                        // if parameter is not null or just white spaces continue..
                        if (!string.IsNullOrWhiteSpace(parameterAsString)) {
                            continue;
                        }
                    }

                    object[] parameter = null;
                    if (consoleCommands[i].parameterType != null) {
                        parameter = new object[1];

                        parameter[0] = ParameterParser.ParseParameterFromString(parameterAsString, consoleCommands[i].parameterType);

                        // if parsed parameter is null, continue loop
                        if (parameter[0] == null) continue;
                    }

                    try {
                        if (consoleCommands[i].isCoroutine) {
                            var param = parameter == null ? null : parameter[0];
                            consoleCommands[i].scriptName.StartCoroutine(consoleCommands[i].methodname, param);
                            executedCommands.Add(input);
                            success = true;
                            continue;
                        }

                        if (consoleCommands[i].methodInfo == null) continue;

                        consoleCommands[i].methodInfo.Invoke(consoleCommands[i].scriptName, parameter);
                        executedCommands.Add(input);
                        success = true;
                    }
                    catch (ArgumentException e) {

                    }
                    finally {
                        ++executedCommandCount;
                    }
                }
            }

            if (!success) {
                ++failedCommandCount;
                Debug.Log(string.Format("Command '{0}' was not recognized.", input));
            }

            return success;
        }

        /// <summary>
        /// Register new Console command
        /// </summary>
        public static void RegisterCommand(MonoBehaviour script, string methodName, string command, string defaultValue = "", 
            bool isHiddenCommand = false, bool hiddenCommandMinimalGUI = false) {
            if (script != null && ConsoleManager.GetSettings().registerStaticCommandsOnly) return;

            if (defaultValue == null) defaultValue = "";

            MethodInfo methodInfo = null;
            methodInfo = script.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (methodInfo == null) {
                return;
            }

            bool isStatic = methodInfo.IsStatic;

            if (!isStatic && script == null) {
                return;
            }

            bool isCoroutine = false;
            isCoroutine = methodInfo.ToString().Contains(ConsoleConstants.IENUMERATOR);

            Type paraType = null;
            var methodParams = methodInfo.GetParameters();
            if (methodParams.Length != 0) {
                paraType = methodParams[0].ParameterType;
            }

            var data = new ConsoleCommandData();
            data.SetValues(script, methodName, command, defaultValue, paraType, isStatic, methodInfo, isCoroutine, isHiddenCommand, hiddenCommandMinimalGUI);
            consoleCommands.Add(data);
            if (isStatic) staticCommands.Add(data);

            if (ConsoleManager.IsConsoleInitialized()) {
                UpdateLists();
                ConsoleEvents.ConsoleRefresh();
            }
            else {
                // new command registered before console was itialized
                consoleCommandsRegisteredBeforeInit.Add(data);
            }
        }


        /// <summary>
        /// UnRegister console command
        /// </summary>
        public static void RemoveCommand(string command, bool log = false) {
            bool foundAny = false;
            var toBeRemoved = new List<ConsoleCommandData>();
            for (int i = 0; i < consoleCommands.Count; i++) {
                if (command == consoleCommands[i].commandName) {
                    toBeRemoved.Add(consoleCommands[i]);
                    foundAny = true;
                }
            }

            for (int i = 0; i < toBeRemoved.Count; i++) {
                consoleCommands.Remove(toBeRemoved[i]);

                if (toBeRemoved[i].isStaticMethod) {
                    staticCommands.Remove(toBeRemoved[i]);
                }
            }

            if (log) {
                if (foundAny) {
                    Debug.Log(string.Format("Removed command [{0}]", command));
                }
                else {
                    Debug.Log(string.Format("Did not find any Command with name [{0}]", command));
                }
            }
            UpdateLists();
            ConsoleEvents.ConsoleRefresh();
        }

        public static int GetExcecutedCommandCount() {
            return executedCommandCount;
        }

        public static int GetFailedCommandCount() {
            return failedCommandCount;
        }

        public static List<ConsoleCommandData> GetConsoleCommands() {
            return consoleCommands;
        }

        public static List<string> GeCommandStringsWithDefaultValues() {
            return commandStringsWithDefaultValues;
        }

        public static List<string> GetCommandStrings() {
            return consoleCommandStrings;
        }

        public static bool StaticCommandsRegistered() {
            return staticCommandsCached;
        }

        public static void PrintAllCommands() {
            var consoleCommands = GetCommandStrings();
            Console.Log("All available commands:");
            for (int i = 0; i < consoleCommands.Count; i++) {
                Console.Log(consoleCommands[i]);
            }
        }

        /// <summary>
        /// Get previously successfully executed commands
        /// </summary>
        public static List<string> GetPreviouslyExecutedCommands() {
            return executedCommands.Distinct().ToList();
        }

        public static void ClearLists() {
            consoleCommands.Clear();
        }

        public static List<ConsoleCommandData> GetConsoleCommandAttributes(bool staticOnly = false) {
            IEnumerable<MethodInfo> methods = null;
            BindingFlags flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (staticCommandsCached) {
                if (staticOnly) {
                    return staticCommands;
                }
                flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

#if UNITY_WEBGL || ENABLE_IL2CPP
            // Get all methods with the execute attribute
            methods = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(x => x.GetTypes())
                                   .Where(x => x.IsClass)
                                   .SelectMany(x => x.GetMethods(flags))
                                   .Where(x => x.GetCustomAttributes(typeof(ConsoleCommand), false).FirstOrDefault() != null);

#else
            // For Mono backend 
            methods = AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                                               .SelectMany(x => x.GetTypes())
                                               .Where(x => x.IsClass)
                                               .SelectMany(x => x.GetMethods(flags))
                                               .Where(x => x.GetCustomAttributes(typeof(ConsoleCommand), false).FirstOrDefault() != null);
#endif

            var commandList = new List<ConsoleCommandData>();

            string coroutine = "IEnumerator";
            string bracket = "(";
            string empty = " ";

            foreach (var method in methods) {
                if (method.IsStatic && staticCommandsCached) continue;

                ConsoleCommand attribute = (ConsoleCommand)method.GetCustomAttributes(typeof(ConsoleCommand), false).First();

                var commandName = attribute.GetCommandName();
                var defaultValue = attribute.GetValue();
                var hiddenCommand = attribute.IsHiddenCommand();
                var hiddenMinimalGUI = attribute.IsHiddenMinimalGUI();

                var className = method.DeclaringType;
                var methodName = method.ToString();

                if (string.IsNullOrEmpty(commandName)) {
#if UNITY_EDITOR
                    // this warning means you have method with [ConsoleCommand(null)] or [ConsoleCommand("")] somewhere
                    // below message should print the script and method where this is located.
                    // This message won't show up in Console window because this Debug.Log is called from another thread.
                    Debug.Log(string.Format("{0}.{1} [ConsoleCommand] name is empty or null! Please assign different command name.", className, methodName));
#endif
                    continue;
                }

                var newData = new ConsoleCommandData();

                // Get ConsoleCommand method parameters
                ParameterInfo[] parameters = method.GetParameters();

                // if method doesn't take parameter and it was given some default value, don't show it.
                if (parameters.Length == 0 && !string.IsNullOrWhiteSpace(defaultValue) || defaultValue == null) {
                    defaultValue = "";
                }

                // check for duplicates.
                bool foundDuplicate = false;
                if (commandList.Count != 0) {
                    for (int i = 0; i < commandList.Count; i++) {
                        if (commandName == commandList[i].commandName) {
                            if (parameters.Length == 0 && commandList[i].parameterType != null || parameters.Length != 0 && parameters[0].ParameterType != commandList[i].parameterType) {
                                // If you see this this debug.log in console then you have
                                // same console command that take in different parameters
                                // try to avoid using same command in two places
#if UNITY_EDITOR
                                Debug.Log(string.Format(ConsoleConstants.WARNING + "Command '{0}' has already been registered with different parameter. " +
                                    "Command '{0}' in class '{1}' with method name '{2}' will be ignored. \n " +
                                    "Give this method another ConsoleCommand name!", commandName, className, methodName));
#endif

                                foundDuplicate = true;
                            }
                        }
                    }
                }

                if (foundDuplicate) {
                    continue;
                }

                bool isCoroutine = false;
                isCoroutine = methodName.Contains(coroutine);

                methodName = methodName.Substring(methodName.IndexOf(empty) + 1);
                methodName = methodName.Substring(0, methodName.IndexOf(bracket));

                if (!ParameterParser.IsSupportedType(method, methodName, commandName, className)) {
                    continue;
                }

                if (method.IsStatic) {
                    var staticParameter = method.GetParameters();
                    var data = new ConsoleCommandData();
                    if (staticParameter.Length == 0) {
                        data.SetValues(null, methodName, commandName, defaultValue, null, true, method, false, hiddenCommand, hiddenMinimalGUI);
                    }
                    else {
                        data.SetValues(null, methodName, commandName, defaultValue, staticParameter[0].ParameterType, true, method, false, hiddenCommand, hiddenMinimalGUI);
                    }
                    //consoleCommands.Add(data);
                    staticCommands.Add(data);
                    continue;
                }

                Type type = parameters.Length == 0 ? null : parameters[0].ParameterType;

                newData.SetValues(null, methodName, commandName, defaultValue, type, false, method, isCoroutine, hiddenCommand, hiddenMinimalGUI, className.ToString());
                commandList.Add(newData);
            }
            staticCommandsCached = true;

            return commandList;
        }

        public static void RegisterCommandsPartTwo(List<ConsoleCommandData> commands) {
            consoleCommands.AddRange(staticCommands);

            GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            // loop through all gameobjects in scene and try to find MonoBehaviour
            // scripts with [ConsoleCommand] attribute, if found, add new console command
            // This way we can get Monobehaviour class instances
            // Note. This can be slow for big scenes!
            for (int i = 0; i < allGameObjects.Length; i++) {
                if (allGameObjects[i] == null) continue;

                for (int j = 0; j < commands.Count; j++) {

                    if (commands[j].isStaticMethod) continue;

                    var script = allGameObjects[i].GetComponent(commands[j].scriptNameString) as MonoBehaviour;

                    // script.gameObject.scene.name != null check is for prefabs
                    if (script != null && script.gameObject.scene.name != null) {
                        var data = new ConsoleCommandData();

                        data.SetValues(script, commands[j].methodname, commands[j].commandName, commands[j].defaultValue, commands[j].parameterType, false, commands[j].methodInfo, commands[j].isCoroutine, commands[j].hiddenCommand);
                        consoleCommands.Add(data);
                    }
                }
            }

            if (consoleCommandsRegisteredBeforeInit.Count != 0) {
                consoleCommands.AddRange(consoleCommandsRegisteredBeforeInit);
                consoleCommandsRegisteredBeforeInit.Clear();
            }

            UpdateLists();
        }

        public static void UpdateLists() {

            consoleCommandStrings.Clear();
            for (int i = 0; i < consoleCommands.Count; i++) {
                if (consoleCommands[i].hiddenCommand) continue;
                consoleCommandStrings.Add(consoleCommands[i].commandName);
            }
            consoleCommandStrings = consoleCommandStrings.Distinct().ToList(); // Delete duplicates



            // create consoleCommandStringsWithSuggestions list
            var style = ConsoleManager.GetGUIStyle();
            commandStringsWithDefaultValues.Clear();
            for (int i = 0; i < consoleCommands.Count; i++) {

                if (consoleCommands[i].hiddenCommand) continue;
                if (consoleCommands[i].hiddenCommandMinimalGUI && style == ConsoleGUIStyle.Minimal) continue;

                // Ensure first character in a string is space
                var defaultValue = consoleCommands[i].defaultValue;
                char first = defaultValue.FirstOrDefault();
                char empty = ' ';
                if (first != empty) {
                    defaultValue = empty + defaultValue;
                }
                commandStringsWithDefaultValues.Add(consoleCommands[i].commandName + defaultValue);
            }
            commandStringsWithDefaultValues = commandStringsWithDefaultValues.Distinct().ToList(); // Delete duplicates
        }
    }
}