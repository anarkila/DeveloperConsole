using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

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
        private static bool allowMultipleCommands = true;
        private static bool staticCommandsCached = false;
        private static bool trackDuplicates = false;
        private static bool trackFailedCommands = true;
        private static int executedCommandCount;
        private static int failedCommandCount;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Clear() {
            // for domain reload purposes
            consoleCommandsRegisteredBeforeInit.Clear();
            commandRemovedBeforeInit.Clear();
            consoleCommands.Clear();
            staticCommands.Clear();
            commandStringsWithDefaultValues.Clear();
            commandStringsWithInfos.Clear();
            consoleCommandList.Clear();
            executedCommands.Clear();
            parseList.Clear();
            staticCommandsCached = false;
            allowMultipleCommands = true;
            trackDuplicates = false;
            executedCommandCount = 0;
            failedCommandCount = 0;
        }
#endif

        /// <summary>
        /// Try to execute console command
        /// </summary>
        public static bool TryExecuteCommand(string input) {
            if (!ConsoleManager.IsConsoleInitialized()) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "Unable to execute command. Developer Console does not exist in the scene or has been destroyed.");
#endif
                return false;
            }

            bool success = false;

            // Does input contains character "&"
            bool constainsAnd = input.Contains(ConsoleConstants.AND);

            // Execute single command
            if (!constainsAnd || !allowMultipleCommands) {
                success = ExecuteCommand(input, constainsAnd);
            }

            // If single command failed then test multi but only if input contains character "&"
            if (!success && constainsAnd && allowMultipleCommands) {
                var commandList = ParseMultipleCommands(input);
                if (commandList == null || commandList.Count == 0) return success;

                for (int i = 0; i < commandList.Count; i++) {
                    success = ExecuteCommand(commandList[i]);
                    // uncomment this to return after command have failed.
                    //if (!success) return success;
                }
            }

            return success;
        }

        private static bool ExecuteCommand(string input, bool silent = false) {
            bool caseSensetive = ConsoleManager.IsCaseSensetive();
            string[] parametersAsString = null;
            string remaining = string.Empty;
            bool parametersParsed = false;
            object[] parameters = null;
            string rawInput = input;
            bool success = false;

            if (!caseSensetive) {
                input = input.ToLower();
            }

            // Parse command and parameter(s) from input
            if (input.Contains(ConsoleConstants.SPACE)) {
                int index = input.IndexOf(ConsoleConstants.EMPTYCHAR);
                index = input.IndexOf(ConsoleConstants.EMPTYCHAR, index);
                remaining = input.Substring(index + 1);
                parametersAsString = remaining.Split(ConsoleConstants.CHARCOMMA);
                input = input.Substring(0, index);
                if (!caseSensetive) {
                    input = input.ToLower();
                }
            }

            // Loop through all console commands and try to find matching command
            for (int i = 0; i < consoleCommands.Count; i++) {

                string command = caseSensetive ? consoleCommands[i].commandName : consoleCommands[i].commandNameLower;
                if (command != input) continue;

                // If command does not take parameter and user passed in parameter --> continue
                if (consoleCommands[i].parameters == null && parametersAsString != null) {
                    continue;
                }

                if (parametersAsString != null || consoleCommands[i].parameters.Length != 0) {

                    // We only need parse this once
                    if (!parametersParsed) {
                        parameters = ParameterParser.ParseParametersFromString(parametersAsString, remaining, consoleCommands[i].parameters, consoleCommands[i]);
                        parametersParsed = true;
                    }

                    // Do final checks before trying to call command
                    bool wrongParameter = false;
                    if (parameters != null) {
                        for (int j = 0; j < parameters.Length; j++) {
                            if (parameters[j] == null && !consoleCommands[i].optionalParameter[j]) {
                                wrongParameter = true;
                                break;
                            }
                        }
                        if (parameters.Length != consoleCommands[i].parameters.Length) {
                            wrongParameter = true;
                        }
                    }

                    if (parameters == null && consoleCommands[i].parameters != null) {
                        wrongParameter = true;
                    }

                    if (wrongParameter) {
                        continue;
                    }
                }

                try {

                    if (consoleCommands[i].monoScript == null && !consoleCommands[i].isStaticMethod) {
                        // This can happen when GameObject with [ConsoleCommand()] attribute is destroyed runtime.
                        consoleCommands.Remove(consoleCommands[i]);
                        UpdateLists();
                        ConsoleEvents.ListsChanged();
                        continue;
                    }

                    // if Command doesn't take in any parameters, use parsed input instead
                    if (consoleCommands[i].parameters.Length == 0) {
                        rawInput = input;
                    }

                    if (consoleCommands[i].isCoroutine) {
                        var param = consoleCommands[i].parameters.Length == 0 ? null : parameters[0];

                        // Starting coroutine by string limits argument count to one.
                        // https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html
                        // If you need to start coroutine with multiple parameters
                        // make a normal method that starts the coroutine instead.
                        consoleCommands[i].monoScript.StartCoroutine(consoleCommands[i].methodName, param);
                        success = true;
                        continue;
                    }

                    if (consoleCommands[i].methodInfo == null) continue;

                    // MethodInfo.Invoke is quite slow but it should be okay for this use case.
                    // Commands are not called, or at least should not be called that often it to matter.
                    consoleCommands[i].methodInfo.Invoke(consoleCommands[i].monoScript, parameters);
                    success = true;
                }
                catch (ArgumentException e) {
                    // Allow expection to be thrown so it can be printed to console (depending on the print setting)
                }
                finally {
                    ++executedCommandCount;
                }
            }

            if (success || trackFailedCommands) {
                bool contains = executedCommands.Contains(rawInput);
                if (!contains || trackDuplicates) {
                    executedCommands.Add(rawInput);
                }
                // Not pretty, but this keeps the list ordered correctly
                else if (contains) {
                    executedCommands.Remove(rawInput);
                    executedCommands.Add(rawInput);
                }
            }

            if (!success) {
                // TODO: perhaps there should be log if command was right but parameter was wrong?
                if (!silent && ConsoleManager.PrintUnrecognizedCommandInfo()) {
                    Console.Log(string.Format("Command '{0}' was not recognized.", rawInput));
                    ++failedCommandCount;
                }
            }

            ConsoleEvents.CommandExecuted(success);

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
                    if (arr[j] != ConsoleConstants.EMPTYCHAR) {
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

            if (!ConsoleManager.IsRunningOnMainThread(System.Threading.Thread.CurrentThread)) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "Console.RegisterCommand cannot be called from another thread.");
#endif
                return;
            }

            if (command == null || command.Length == 0 || methodName == null || methodName.Length == 0) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "command or methodname is null or empty!");
# endif
                return;
            }

            if (script == null) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "MonoBehaviour reference is null! If you are registering non-Monobehaviour commands, Use [ConsoleCommand()] attribute instead.");
#endif
                return;
            }

            if (ConsoleManager.GetSettings().registerStaticCommandsOnly) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "Trying to register new MonoBehaviour command while option RegisterStaticCommandsOnly is enabled.");
#endif
                return;
            }

            if (debugCommandOnly && !Debug.isDebugBuild) return;

            MethodInfo methodInfo = null;
            methodInfo = script.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null) return;

            var data = CreateCommandData(methodInfo, script, methodName, command, defaultValue, info, isHiddenCommand, hiddenCommandMinimalGUI);
            if (data == null) return;

            if (ConsoleManager.IsConsoleInitialized()) {
                consoleCommands.Add(data);
                UpdateLists();
                ConsoleEvents.ListsChanged();
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
            if (!ConsoleManager.IsRunningOnMainThread(System.Threading.Thread.CurrentThread)) {
#if UNITY_EDITOR
                Debug.Log(ConsoleConstants.EDITORWARNING + "Console.RemoveCommand cannot be called from another thread.");
#endif
                return;
            }

            if (command == null || command.Length == 0 || !Application.isPlaying) return;

            if (!ConsoleManager.IsConsoleInitialized() && !forceDelete) {
                if (!commandRemovedBeforeInit.ContainsKey(command)) {
                    commandRemovedBeforeInit.Add(command, log);
                }
                return;
            }

            bool foundAny = false;
            var toRemove = new List<ConsoleCommandData>();
            for (int i = 0; i < consoleCommands.Count; i++) {
                if (command == consoleCommands[i].commandName) {
                    toRemove.Add(consoleCommands[i]);
                    foundAny = true;
                }
            }

            for (int i = 0; i < toRemove.Count; i++) {
                consoleCommands.Remove(toRemove[i]);
                if (toRemove[i].isStaticMethod) {
                    staticCommands.Remove(toRemove[i]);
                }
            }

#if UNITY_EDITOR
            if (log) {
                if (foundAny) {
                    Console.Log(string.Format("Removed command [{0}]", command));
                }
                else {
                    Console.Log(string.Format("Didn't find command with name [{0}]", command));
                }
            }
#endif
            UpdateLists();
            ConsoleEvents.ListsChanged();
        }

        /// <summary>
        /// Get all [ConsoleCommand()] attributes
        /// </summary>
        public static List<ConsoleCommandData> GetConsoleCommandAttributes(bool isDebugBuild, bool staticOnly, bool scanAllAssemblies = false) {
            consoleCommands.Clear();

            BindingFlags flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (staticCommandsCached) {
                if (staticOnly) {
                    return staticCommands;
                }
                flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            var commandList = new List<ConsoleCommandData>(64);
            var methods = GetAllAttributesFromAssembly(flags, scanAllAssemblies);

            // Loop through all methods with [ConsoleCommand()] attributes
            foreach (var method in methods) {
                if (method.IsStatic && staticCommandsCached) continue;

                ConsoleCommand attribute = (ConsoleCommand)method.GetCustomAttributes(typeof(ConsoleCommand), false)[0];

                if (attribute == null || attribute.IsDebugOnlyCommand() && !isDebugBuild) continue;

                var data = CreateCommandData(method, null, method.Name,
                    attribute.GetCommandName(), attribute.GetValue(),
                    attribute.GetInfo(), attribute.IsHiddenCommand(),
                    attribute.IsHiddenMinimalGUI());

                if (data == null) continue;

                if (method.IsStatic) {
                    staticCommands.Add(data);
                }
                else {
                    commandList.Add(data);
                }
            }

            if (!staticCommandsCached && staticOnly) {
                consoleCommands.AddRange(staticCommands);
            }

            staticCommandsCached = true;

            return commandList;
        }

        /// <summary>
        /// Create ConsoleCommandData data.
        /// </summary>
        private static ConsoleCommandData CreateCommandData(MethodInfo methodInfo, MonoBehaviour script, string methodName, string command, string defaultValue, string info, bool isHiddenCommand, bool hiddenCommandMinimalGUI) {
            if (methodInfo == null) return null;

            Type className = methodInfo.DeclaringType;
            string classNameString = className.ToString();

            if (string.IsNullOrEmpty(command)) {
#if UNITY_EDITOR
                // this warning means you have method with [ConsoleCommand(null)] or [ConsoleCommand("")] somewhere.
                // Below message should print the script and method where this is located.
                Debug.Log(string.Format("{0}{1}.{2} [ConsoleCommand] name is empty or null! Please assign different command name.", ConsoleConstants.EDITORWARNING, classNameString, methodName));
#endif
                return null;
            }

            if (command.Contains(ConsoleConstants.AND) || command.Contains(ConsoleConstants.COMMA) || command.Contains(ConsoleConstants.SPACE)) {
#if UNITY_EDITOR
                // [ConsoleCommand()] cannot contain characters '&' or ',' (comma) because
                // character '&' is used to parse multiple commands
                // and character ',' (comma) is used to parse multiple parameters
                Debug.Log(string.Format("{0}[ConsoleCommand] cannot contain whitespace, '&' or comma. Rename command [{1}] in {2}{3}", ConsoleConstants.EDITORWARNING, command, classNameString, methodName));
#endif
                return null;
            }

            bool isStatic = methodInfo.IsStatic;
            bool isCoroutine = methodInfo.ToString().Contains(ConsoleConstants.IENUMERATOR);
            ParameterInfo[] methodParams = methodInfo.GetParameters();
            Type[] paraType = new Type[methodParams.Length];
            bool[] optionalParameters = new bool[methodParams.Length];

            for (int i = 0; i < methodParams.Length; i++) {
                paraType[i] = methodParams[i].ParameterType;
                optionalParameters[i] = methodParams[i].IsOptional;
            }

            if (!ParameterParser.IsSupportedType(methodParams, isCoroutine, methodName, command, className)) {
                return null;
            }

            if (CheckForDuplicates(consoleCommands, paraType, command, classNameString, methodName)) {
                return null;
            }

            if (defaultValue == null) defaultValue = "";

            ConsoleCommandData data = new ConsoleCommandData(script, methodName, command, defaultValue, info, paraType, isStatic, methodInfo, isCoroutine, optionalParameters, isHiddenCommand, hiddenCommandMinimalGUI, classNameString);

            return data;
        }

        /// <summary>
        /// Get all ConsoleCommand attributes from assembly
        /// </summary>
        private static List<MethodInfo> GetAllAttributesFromAssembly(BindingFlags flags, bool scanAllAssemblies = false) {
            List<MethodInfo> attributeMethodInfos = new List<MethodInfo>(64);
            ConcurrentBag<MethodInfo> cb = new ConcurrentBag<MethodInfo>();
            bool parallel = true;

#if UNITY_WEBGL
            // WebGL doesn't support Parallel.For
            parallel = false;
#endif
            // Looping through all assemblies is slow
            if (scanAllAssemblies) {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                if (parallel) {
                    Parallel.For(0, assemblies.Length, i =>
                    {
                        var type = assemblies[i].GetTypes();
                        for (int j = 0; j < type.Length; j++) {
                            FindAttributeAndAdd(flags, j, type, cb);
                        }
                    });
                }
                else {
                    for (int i = 0; i < assemblies.Length; i++) {
                        var type = assemblies[i].GetTypes();
                        for (int j = 0; j < type.Length; j++) {
                            FindAttributeAndAdd(flags, j, type, cb);
                        }
                    }
                }
            }

            // else loop through current assembly which should be Unity assembly
            else {
                var unityAssembly = Assembly.GetExecutingAssembly();
                var types = unityAssembly.GetTypes();

                // For small projects, it's faster to just use single threaded loop
                if (types.Length <= 100) parallel = false;

                if (parallel) {
                    Parallel.For(0, types.Length, i =>
                    {
                        FindAttributeAndAdd(flags, i, types, cb);
                    });
                }
                else {
                    for (int i = 0; i < types.Length; i++) {
                        FindAttributeAndAdd(flags, i, types, cb);
                    }
                }
            }

            attributeMethodInfos = cb.ToList();

            return attributeMethodInfos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FindAttributeAndAdd(BindingFlags flags, int j, Type[] type, ConcurrentBag<MethodInfo> cb) {
            if (!type[j].IsClass) return;

            var methodInfos = type[j].GetMethods(flags);
            if (methodInfos.Length == 0) return;

            for (int i = 0; i < methodInfos.Length; i++) {
                if (methodInfos[i].GetCustomAttributes(typeof(ConsoleCommand), false).Length > 0) {
                    cb.Add(methodInfos[i]);
                }
            }
        }

        /// <summary>
        /// Register MonoBehaviour commands
        /// </summary>
        public static void RegisterMonoBehaviourCommands(List<ConsoleCommandData> commands) {

            // Find all MonoBehaviour class names
            var scriptNames = new List<string>();
            for (int i = 0; i < commands.Count; i++) {
                if (commands[i].isStaticMethod) continue;

                if (!scriptNames.Contains(commands[i].scriptNameString)) {
                    scriptNames.Add(commands[i].scriptNameString);
                }
            }

            // Loop through all MonoBehaviour classes added above.
            // This uses GameObject.FindObjectsOfType to find all those scripts in the current scene and
            // loops through them to find MonoBehaviour references.
            // these loops look scary but this is reasonable fast
            for (int i = 0; i < scriptNames.Count; i++) {
                Type type = Type.GetType(scriptNames[i]);
                if (type == null) continue;

                MonoBehaviour[] monoScripts = GameObject.FindObjectsOfType(type) as MonoBehaviour[];

                for (int j = 0; j < monoScripts.Length; j++) {
                    if (monoScripts[j] == null) continue;

                    string scriptName = monoScripts[j].GetType().ToString();

                    for (int k = 0; k < commands.Count; k++) {
                        if (commands[k].isStaticMethod) continue;

                        MonoBehaviour script = null;
                        if (scriptName == commands[k].scriptNameString) {
                            script = monoScripts[j];
                        }

                        if (script != null) {
                            var data = new ConsoleCommandData(script, commands[k].methodName, commands[k].commandName,
                                commands[k].defaultValue, commands[k].info, commands[k].parameters, false,
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
                    if (CheckForDuplicates(consoleCommands, command.parameters, command.commandName, command.scriptNameString, command.methodName)) {
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
            trackFailedCommands = ConsoleManager.TrackFailedCommands();
            allowMultipleCommands = ConsoleManager.AllowMultipleCommands();
            trackDuplicates = ConsoleManager.TrackDuplicates();

            commandStringsWithDefaultValues.Clear();
            commandStringsWithInfos.Clear();
            consoleCommandList.Clear();

            var style = ConsoleManager.GetGUIStyle();
            char space = ' ';

            for (int i = 0; i < consoleCommands.Count; i++) {
                if (consoleCommands[i].hiddenCommand) continue;

                if (consoleCommands[i].hiddenCommandMinimalGUI && style == ConsoleGUIStyle.Minimal) {
                    continue;
                }

                if (!consoleCommandList.Contains(consoleCommands[i].commandName)) {
                    consoleCommandList.Add(consoleCommands[i].commandName);
                }

                if (!string.IsNullOrWhiteSpace(consoleCommands[i].info)) {
                    var fullText = consoleCommands[i].commandName + ConsoleConstants.LINE + consoleCommands[i].info;
                    if (!commandStringsWithInfos.Contains(fullText)) {
                        commandStringsWithInfos.Add(fullText);
                    }
                }
                else {
                    if (!commandStringsWithInfos.Contains(consoleCommands[i].commandName)) {
                        commandStringsWithInfos.Add(consoleCommands[i].commandName);
                    }
                }

                // Ensure first character in a string is space
                var defaultValue = consoleCommands[i].defaultValue;
                char first = defaultValue.FirstOrDefault();
                if (first != space) {
                    defaultValue = space + defaultValue;
                }

                var full = consoleCommands[i].commandName + defaultValue;

                if (!commandStringsWithDefaultValues.Contains(full)) {
                    commandStringsWithDefaultValues.Add(full);
                }
            }
        }

        public static void PrintAllCommands() {
            var settings = ConsoleManager.GetSettings();

            if (settings != null) {
                var commands = settings.printCommandInfoTexts ? GetConsoleCommandsWithInfos() : GetConsoleCommandList();

                if (settings.printCommandsAlphabeticalOrder) {
                    commands = commands.OrderBy(x => x).ToList();
                }

                Console.LogEmpty();
                ConsoleEvents.Log(ConsoleConstants.COMMANDMESSAGE);
                for (int i = 0; i < commands.Count; i++) {
                    ConsoleEvents.Log(commands[i]);
                }
            }
        }

        /// <summary>
        /// Check that list doesn't already contain command that we are trying to register.
        /// </summary>
        private static bool CheckForDuplicates(List<ConsoleCommandData> commands, Type[] parameters, string commandName, string className, string methodName) {
            if (commands.Count == 0) return false;

            bool found = false;

            for (int i = 0; i < commands.Count; i++) {
                if (commandName == commands[i].commandName) {
                    if (className != commands[i].scriptNameString || methodName != commands[i].methodName || parameters != commands[i].parameters) {
#if UNITY_EDITOR
                        Debug.Log(string.Format(ConsoleConstants.EDITORWARNING + "Command '{0}' has already been registered. " +
                              "Command '{0}' in class '{1}' with method name '{2}' will be ignored. " +
                              "Give this attribute other command name!", commandName, className, methodName));
#endif
                        found = true;
                    }
                }
            }

            return found;
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

        public static int GetConsoleCommandsCount() {
            return consoleCommands.Count;
        }

        public static int GetStaticConsoleCommandsCount() {
            return staticCommands.Count;
        }

        public static List<string> GeCommandStringsWithDefaultValues() {
            return commandStringsWithDefaultValues;
        }

        public static List<string> GetConsoleCommandList() {
            return consoleCommandList;
        }

        public static List<string> GetConsoleCommandsWithInfos() {
            return commandStringsWithInfos;
        }

        public static bool StaticCommandsRegistered() {
            return staticCommandsCached;
        }

        public static List<string> GetPreviouslyExecutedCommands() {
            return executedCommands;
        }
    }
}