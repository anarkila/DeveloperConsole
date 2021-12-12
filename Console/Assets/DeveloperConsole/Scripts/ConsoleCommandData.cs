using System.Reflection;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    public class ConsoleCommandData {

        public MonoBehaviour monoScript { get; private set; }
        public string methodName { get; private set; }
        public string commandName { get; private set; }
        public string commandNameLower { get; private set; }
        public string defaultValue { get; private set; }
        public string info { get; private set; }
        public Type[] parameters { get; private set; }
        public bool isStaticMethod { get; private set; }
        public MethodInfo methodInfo { get; private set; }
        public bool isCoroutine { get; private set; }
        public bool[] optionalParameter { get; private set; }
        public bool hiddenCommand { get; private set; }
        public bool hiddenCommandMinimalGUI { get; private set; }
        public string scriptNameString { get; private set; }
        public char[] commandNameCharArray { get; private set; }
        public char[] commandNameLowerCharArray { get; private set; }

        public ConsoleCommandData(MonoBehaviour scriptName, string methodName, string commandName, string commandExplanation, string info, Type[] parameters,
            bool isStaticMethod, MethodInfo methodInfo, bool isCoroutine, bool[] optionalParameter, bool hiddenCommand = false,
            bool hiddenCommandMinimalGUI = false,string scriptNameString = "") {

            this.monoScript = scriptName;
            this.methodName = methodName;
            this.commandName = commandName;
            this.defaultValue = commandExplanation;
            this.info = info;
            this.parameters = parameters;
            this.isStaticMethod = isStaticMethod;
            this.methodInfo = methodInfo;
            this.isCoroutine = isCoroutine;
            this.optionalParameter = optionalParameter;
            this.hiddenCommand = hiddenCommand;
            this.hiddenCommandMinimalGUI = hiddenCommandMinimalGUI;
            this.scriptNameString = scriptNameString;
            this.commandNameLower = commandName.ToLower();
            this.commandNameCharArray = commandName.ToCharArray();
            this.commandNameLowerCharArray = this.commandNameLower.ToCharArray();
        }
    }
}