using System.Reflection;
using UnityEngine;
using System;

namespace Anarkila.DeveloperConsole {

    public class ConsoleCommandData {

        public MonoBehaviour monoScript;
        public string methodName;
        public string commandName;
        public string commandNameLower;
        public string defaultValue = "";
        public string info = "";
        public Type parameterType;
        public bool isStaticMethod = false;
        public MethodInfo methodInfo;
        public bool isCoroutine= false;
        public bool optionalParameter = false;
        public bool hiddenCommand = false;
        public bool hiddenCommandMinimalGUI = false;
        public string scriptNameString;
        public char[] commandNameCharArray;
        public char[] commandNameLowerCharArray;

        public ConsoleCommandData(MonoBehaviour scriptName, string methodName, string commandName, string commandExplanation, string info, Type parameterType,
            bool isStaticMethod, MethodInfo methodInfo, bool isCoroutine, bool optionalParameter, bool hiddenCommand = false,
            bool hiddenCommandMinimalGUI = false,string scriptNameString = "") {

            this.monoScript = scriptName;
            this.methodName = methodName;
            this.commandName = commandName;
            this.defaultValue = commandExplanation;
            this.info = info;
            this.parameterType = parameterType;
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