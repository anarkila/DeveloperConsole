using System.Reflection;
using UnityEngine;
using System;

namespace DeveloperConsole {

    public class ConsoleCommandData {

        public MonoBehaviour scriptName;
        public string methodname;
        public string commandName;
        public string defaultValue = "";
        public Type parameterType;
        public bool isStaticMethod = false;
        public MethodInfo methodInfo;
        public bool isCoroutine= false;
        public bool hiddenCommand = false;
        public bool hiddenCommandMinimalGUI = false;
        public string scriptNameString;

        public void SetValues(MonoBehaviour scriptName, string methodname, string commandName, string commandExplanation, Type parameterType,
            bool isStaticMethod = false, MethodInfo methodInfo = null, bool isCoroutine = false, bool hiddenCommand = false,
            bool hiddenCommandMinimalGUI = false,string scriptNameString = "") {

            this.scriptName = scriptName;
            this.methodname = methodname;
            this.commandName = commandName;
            this.defaultValue = commandExplanation;
            this.parameterType = parameterType;
            this.isStaticMethod = isStaticMethod;
            this.methodInfo = methodInfo;
            this.isCoroutine = isCoroutine;
            this.hiddenCommand = hiddenCommand;
            this.hiddenCommandMinimalGUI = hiddenCommandMinimalGUI;
            this.scriptNameString = scriptNameString;
        }
    }
}