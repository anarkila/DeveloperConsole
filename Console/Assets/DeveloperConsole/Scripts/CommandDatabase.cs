using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

#pragma warning disable 0168
#pragma warning disable 0219
    public static class CommandDatabase {

        private static List<ConsoleCommandData> consoleCommandsRegisteredBeforeInit = new List<ConsoleCommandData>();
        private static Dictionary<string, bool> commandRemovedBeforeInit = new Dictionary<string, bool>();
        private static List<ConsoleCommandData> consoleCommands = new List<ConsoleCommandData>(32);
        private static List<ConsoleCommandData> staticCommands = new List<ConsoleCommandData>(32);
        private static List<string> commandStringsWithDefaultValues = new List<string>(32);
        private static List<string> commandStringsWithInfos = new List<string>(32);
        private static List<string> consoleCommandList = new List<string>(32);
        private static List<string> executedCommands = new List<string>(32);
        private static List<string> parseList = new List<string>();
        private static bool staticCommandsCached = false;
        private static int executedCommandCount;
        private static int failedCommandCount;

        /// <summary>
        /// Try to execute command
        /// </summary>
        public static bool TryExecuteCommand(string input) {

            bool success = false;

            // Does input constain "&"
            var constainsAnd = input.Contains(ConsoleConstants.AND);

            // Test single command first
            success = ExecuteCommand(input, constainsAnd);

            // If single command failed then test multi but only if input contains "&"
            if (!success && constainsAnd && ConsoleManager.AllowMultipleCommands()) {
                var commandList = ParseMultipleCommands(input);

                if (commandList == null || commandList.Count == 0) return success;

                for (int i = 0; i < commandList.Count; i++) {
                    success = ExecuteCommand(commandList[i]);

                    // uncomment this if you wish to return after command have failed.
                    //if (!success) return success;
                }
            }

            return success;
        }


        private static bool ExecuteCommand(string input, bool silent = false) {

            string parameterAsString = null;
            bool commandFound = false;
            bool success = false;

            // Parse input for empty characters
            if (input.Contains(ConsoleConstants.EMPTYCHAR)) {
                int index = input.IndexOf(ConsoleConstants.EMPTYCHAR);
                index = input.IndexOf(ConsoleConstants.EMPTYCHAR, index);
                parameterAsString = input.Substring(index, input.Length - index);
                input = input.Substring(0, index);
            }

            bool caseSensetive = ConsoleManager.IsCaseSensetive();

            // Loop through all console commands and try to find matching command
            for (int i = 0; i < consoleCommands.Count; i++) {

                if (caseSensetive && input != consoleCommands[i].commandName) continue;
                else if (input.ToLower() != consoleCommands[i].commandNameLower) continue;
                // else command matches..

                // If command does not take parameter and user passed in parameter.
                if (consoleCommands[i].parameterType == null && parameterAsString != null) {
                    if (!string.IsNullOrWhiteSpace(parameterAsString)) {
                        continue;
                    }
                }

                commandFound = true;

                object[] parameter = null;
                if (consoleCommands[i].parameterType != null) {
                    parameter = new object[1];

                    parameter[0] = ParameterParser.ParseParameterFromString(parameterAsString, consoleCommands[i].parameterType);

                    // if parsed parameter is null, continue loop
                    if (parameter[0] == null && !consoleCommands[i].optionalParameter) {
                        continue;
                    }

                }

                try {

                    if (consoleCommands[i].monoScript == null && !consoleCommands[i].isStaticMethod) {
                        // This can happen when GameObject with [ConsoleCommand()] attribute is destroyed runtime.
                        consoleCommands.Remove(consoleCommands[i]);
                        UpdateLists();
                        ConsoleEvents.RefreshConsole();
                        continue;
                    }

                    if (consoleCommands[i].isCoroutine) {
                        var param = parameter == null ? null : parameter[0];
                        consoleCommands[i].monoScript.StartCoroutine(consoleCommands[i].methodName, param);

                        if (!executedCommands.Contains(input)) {
                            executedCommands.Add(input);
                        }
                        success = true;
                        continue;
                    }

                    if (consoleCommands[i].methodInfo == null) continue;

                    consoleCommands[i].methodInfo.Invoke(consoleCommands[i].monoScript, parameter);
                    if (!executedCommands.Contains(input)) {
                        executedCommands.Add(input);
                    }
                    success = true;
                }
                catch (ArgumentException e) {
                    // Allow expection to be thrown so it can be printed to console (depending on the print setting).
                }
                finally {
                    ++executedCommandCount;
                }
            }

            if (!success && !commandFound && !silent && ConsoleManager.PrintUnrecognizedCommandInfo()) {
                Debug.Log(string.Format("Command '{0}' was not recognized.", input));
                ++failedCommandCount;
            }

            return success;
        }

        /// <summary>
        /// Parse multiple commands separated by "&" or "&&"
        /// </summary>
        private static List<string> ParseMultipleCommands(string input) {
            if (input == null || input.Length == 0) return null;

            parseList.Clear();

            string[] commandArray = input.Split(ConsoleConstants.ANDCHAR);

            for (int i = 0; i < commandArray.Length; i++) {

                // if commandArray length is 0, skip it
                // this likely happens because "&&" was typed instead of "&"
                if (commandArray[i].Length == 0) continue;

                char[] arr = commandArray[i].ToCharArray();
                for (int j = 0; j < arr.Length; j++) {
                    if (arr[j] != ' ') {
                        break;
                    }
                    else {
                        commandArray[i] = commandArray[i].Substring(1);
                    }
                }
                parseList.Add(commandArray[i]);
            }

            return parseList;
        }

        /// <summary>
        /// Register new Console command
        /// </summary>
        public static void RegisterCommand(MonoBehaviour script, string methodName, string command, string defaultValue = "", string info = "",
            bool debugCommandOnly = false, bool isHiddenCommand = false, bool hiddenCommandMinimalGUI = false) {

            if (command == null || command.Length == 0 || methodName == null || methodName.Length == 0) {
#if UNITY_EDITOR
                Debug.Log("command or methodname is null or empty!");
# endif
                return;
            }

            if (script == null) {
#if UNITY_EDITOR
                Debug.Log("MonoBehaviour reference is null! If you are registering non-Monobehaviour commands, Use [ConsoleCommand()] attribute instead.");
#endif
                return;
            }

            if (script != null && ConsoleManager.GetSettings().registerStaticCommandAttributesOnly) return;
            if (debugCommandOnly && !Debug.isDebugBuild) return;

            if (defaultValue == null) defaultValue = "";

            MethodInfo methodInfo = null;
            methodInfo = script.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null) return;

            bool isStatic = methodInfo.IsStatic;

            bool isCoroutine = false;
            isCoroutine = methodInfo.ToString().Contains(ConsoleConstants.IENUMERATOR);

            bool optionalParameter = false;
            Type paraType = null;
            var methodParams = methodInfo.GetParameters();
            if (methodParams.Length != 0) {
                paraType = methodParams[0].ParameterType;
                optionalParameter = methodParams[0].IsOptional;
            }

            if (CheckForDuplicates(consoleCommands, paraType, command, methodInfo.DeclaringType.ToString(), methodName)) {
                return;
            }

            var data = new ConsoleCommandData(script, methodName, command, defaultValue, info, paraType, isStatic, methodInfo, isCoroutine, optionalParameter, isHiddenCommand, hiddenCommandMinimalGUI);

            if (ConsoleManager.IsConsoleInitialized()) {
                consoleCommands.Add(data);
                UpdateLists();
                ConsoleEvents.RefreshConsole();
            }
            else {
                // new command registered before console was initialized
                consoleCommandsRegisteredBeforeInit.Add(data);
            }
        }


        /// <summary>
        /// Remove command
        /// </summary>
        public static void RemoveCommand(string command, bool log = false, bool forceDelete = false) {
            if (command == null || command.Length == 0) return;

            if (!ConsoleManager.IsConsoleInitialized() && !forceDelete) {
                if (!commandRemovedBeforeInit.ContainsKey(command)) {
                    commandRemovedBeforeInit.Add(command, log);
                }
                return;
            }

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

#if UNITY_EDITOR
            if (log) {
                if (foundAny) {
                    Debug.Log(string.Format("Removed command [{0}]", command));
                }
                else {
                    Debug.Log(string.Format("Didn't find command with name [{0}]", command));
                }
            }
#endif
            UpdateLists();
            ConsoleEvents.RefreshConsole();
        }

        /// <summary>
        /// Get all [ConsoleCommand()] attributes
        /// </summary>
        public static List<ConsoleCommandData> GetConsoleCommandAttributes(bool isDebugBuild, bool staticOnly) {
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

            foreach (var method in methods) {
                if (method.IsStatic && staticCommandsCached) continue;

                ConsoleCommand attribute = (ConsoleCommand)method.GetCustomAttributes(typeof(ConsoleCommand), false).First();

                if (attribute == null) continue; // this should never happen, but just in case.

                if (attribute.IsDebugOnlyCommand() && !isDebugBuild) continue;

                var commandName = attribute.GetCommandName();
                var defaultValue = attribute.GetValue();
                var hiddenCommand = attribute.IsHiddenCommand();
                var hiddenMinimalGUI = attribute.IsHiddenMinimalGUI();
                var info = attribute.GetInfo();

                var className = method.DeclaringType;
                var methodName = method.ToString();


                if (commandName.Contains(ConsoleConstants.AND)) {
#if UNITY_EDITOR
                    // [ConsoleCommand()] cannot contain character & because it's used to parse multiple commands.
                    // like: 'test.int 1 & test.int 2' or 'test.int 3 && test.int 4'
                    Debug.Log(string.Format("{0}[ConsoleCommand] cannot contain character '&'. Please rename command {1} in {2}{3}", ConsoleConstants.EDITORWARNING, commandName, className, methodName));
#endif
                    continue;
                }

                if (string.IsNullOrEmpty(commandName)) {
#if UNITY_EDITOR
                    // this warning means you have method with [ConsoleCommand(null)] or [ConsoleCommand("")] somewhere.
                    // Below message should print the script and method where this is located.
                    // This message won't show up in Console window because this Debug.Log is called from another thread (expect in WebGL)
                    Debug.Log(string.Format("{0}.{1} [ConsoleCommand] name is empty or null! Please assign different command name.", className, methodName));
#endif
                    continue;
                }

                // Get ConsoleCommand method parameters
                ParameterInfo[] parameters = method.GetParameters();

                // if method doesn't take parameter and it was given some default value, don't show it.
                if (parameters.Length == 0 && !string.IsNullOrWhiteSpace(defaultValue) || defaultValue == null) {
                    defaultValue = "";
                }

                bool isCoroutine = false;
                isCoroutine = methodName.Contains(ConsoleConstants.IENUMERATOR);

                methodName = methodName.Substring(methodName.IndexOf(ConsoleConstants.SPACE) + 1);
                methodName = methodName.Substring(0, methodName.IndexOf(ConsoleConstants.OPENPARENTHESIS));

                if (!ParameterParser.IsSupportedType(method, methodName, commandName, className)) {
                    continue;
                }

                bool isStatic = method.IsStatic;
                Type type = parameters.Length == 0 ? null : parameters[0].ParameterType;

                bool optionalParameter = false;
                if (type != null) {
                    optionalParameter = parameters[0].IsOptional;
                }

                string classNameString = className.ToString();
                if (CheckForDuplicates(commandList, type, commandName, classNameString, methodName)) {
                    continue;
                }

                var newData = new ConsoleCommandData(null, methodName, commandName, defaultValue, info, type, isStatic, method, isCoroutine, optionalParameter, hiddenCommand, hiddenMinimalGUI, classNameString);

                if (isStatic) {
                    staticCommands.Add(newData);
                }
                else {
                    commandList.Add(newData);
                }
            }

            if (!staticCommandsCached && staticOnly) {
                consoleCommands.AddRange(staticCommands);
            }

            staticCommandsCached = true;

            return commandList;
        }


        /// <summary>
        /// Register MonoBehaviour commands
        /// </summary>
        /// <param name="commands"></param>
        public static void RegisterMonoBehaviourCommands(List<ConsoleCommandData> commands) {

            // Find all different script names
            var scriptnames = new List<string>();
            for (int i = 0; i < commands.Count; i++) {
                if (!scriptnames.Contains(commands[i].scriptNameString)) {
                    scriptnames.Add(commands[i].scriptNameString);
                }
            }

            // Loop through all different script names
            // Use GameObject.FindObjectsOfType to find all those scripts in the current scene
            // loop though those scripts and all commands to find MonoBehaviour references.
            // these loops look scary but this is reasonable fast (approx. ~1.2 ms for example scenes)
            for (int i = 0; i < scriptnames.Count; i++) {
                Type type = Type.GetType(scriptnames[i]);
                MonoBehaviour[] objects = GameObject.FindObjectsOfType(type) as MonoBehaviour[];

                for (int j = 0; j < objects.Length; j++) {
                    string scriptName = objects[j].GetType().ToString();
                    MonoBehaviour monoScript = objects[j];

                    for (int k = 0; k < commands.Count; k++) {
                        if (commands[k].isStaticMethod) continue;

                        MonoBehaviour script = null;
                        if (scriptName == commands[k].scriptNameString) {
                            script = monoScript;
                        }

                        if (script != null) {
                            var data = new ConsoleCommandData(script, commands[k].methodName, commands[k].commandName,
                                commands[k].defaultValue, commands[k].info, commands[k].parameterType, false,
                                commands[k].methodInfo, commands[k].isCoroutine, commands[k].optionalParameter, commands[k].hiddenCommand);
                            consoleCommands.Add(data);
                        }
                    }
                }
            }


            // Add static commands to final console command list
            consoleCommands.AddRange(staticCommands);

            // If user called Console.RegisterCommand before console was fully initilized
            // Add those commands now.
            if (consoleCommandsRegisteredBeforeInit.Count != 0) {
                for (int i = 0; i < consoleCommandsRegisteredBeforeInit.Count; i++) {
                    var command = consoleCommandsRegisteredBeforeInit[i];
                    if (CheckForDuplicates(consoleCommands, command.parameterType, command.commandName, command.scriptNameString, command.methodName)) {
                        consoleCommandsRegisteredBeforeInit.Remove(command);
                    }
                    else {
                        consoleCommands.Add(command);
                    }
                }
                consoleCommandsRegisteredBeforeInit.Clear();
            }

            // If user called Console.RemoveCommand before console was fully initilized
            // Remove those commands now.
            if (commandRemovedBeforeInit.Count != 0) {
                foreach (var dict in commandRemovedBeforeInit) {
                    RemoveCommand(dict.Key, dict.Value, true);
                }
            }

            UpdateLists();
        }

        /// <summary>
        /// Generate needed console lists
        /// </summary>
        public static void UpdateLists() {

            commandStringsWithInfos.Clear();
            consoleCommandList.Clear();
            commandStringsWithDefaultValues.Clear();

            var style = ConsoleManager.GetGUIStyle();

            string line = " - ";

            for (int i = 0; i < consoleCommands.Count; i++) {
                if (consoleCommands[i].hiddenCommand) continue;

                if (!consoleCommandList.Contains(consoleCommands[i].commandName)) {
                    consoleCommandList.Add(consoleCommands[i].commandName);
                }

                if (!string.IsNullOrWhiteSpace(consoleCommands[i].info)) {
                    var fullText = consoleCommands[i].commandName + line + consoleCommands[i].info;
                    if (!commandStringsWithInfos.Contains(fullText)) {
                        commandStringsWithInfos.Add(fullText);
                    }
                }
                else {
                    if (!commandStringsWithInfos.Contains(consoleCommands[i].commandName)) {
                        commandStringsWithInfos.Add(consoleCommands[i].commandName);
                    }
                }

                if (consoleCommands[i].hiddenCommandMinimalGUI && style == ConsoleGUIStyle.Minimal) continue;

                // Ensure first character in a string is space
                var defaultValue = consoleCommands[i].defaultValue;
                char first = defaultValue.FirstOrDefault();
                char space = ' ';
                if (first != space) {
                    defaultValue = space + defaultValue;
                }

                var full = consoleCommands[i].commandName + defaultValue;

                if (!commandStringsWithDefaultValues.Contains(full)) {
                    commandStringsWithDefaultValues.Add(full);
                }
            }
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

        public static List<string> GetConsoleCommandList() {
            return consoleCommandList;
        }

        public static List<string> GetCommandsWithInfos() {
            return commandStringsWithInfos;
        }

        public static bool StaticCommandsRegistered() {
            return staticCommandsCached;
        }

        public static void PrintAllCommands() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null) {
                var commands = settings.printCommandInfoTexts ? GetCommandsWithInfos() : GetConsoleCommandList();

                if (settings.printCommandsAlphabeticalOrder) {
                    commands = commands.OrderBy(x => x).ToList();
                }

                Console.Log(ConsoleConstants.COMMANDMESSAGE);
                for (int i = 0; i < commands.Count; i++) {
                    Console.Log(commands[i]);
                }
            }
        }

        /// <summary>
        /// Get previously successfully executed commands
        /// </summary>
        public static List<string> GetPreviouslyExecutedCommands() {
            return executedCommands;
        }

        public static void ClearConsoleCommands() {
            consoleCommands.Clear();
        }

        /// <summary>
        /// Check that list doesn't already contain command that we are trying to register.
        /// </summary>
        private static bool CheckForDuplicates(List<ConsoleCommandData> commandList, Type parameter, string commandName, string className, string methodName) {
            if (commandList.Count == 0) return false;

            bool foundDuplicate = false;

            for (int i = 0; i < commandList.Count; i++) {
                if (commandName == commandList[i].commandName) {

                    if (className != commandList[i].scriptNameString
                        || methodName != commandList[i].methodName
                        || parameter != commandList[i].parameterType) {
#if UNITY_EDITOR
                        Debug.Log(string.Format(ConsoleConstants.EDITORWARNING + "Command '{0}' has already been registered. " +
                              "Command '{0}' in class '{1}' with method name '{2}' will be ignored. " +
                              "Give this attribute other command name!", commandName, className, methodName));
#endif
                        foundDuplicate = true;
                    }
                }
            }

            return foundDuplicate;
        }
    }
}