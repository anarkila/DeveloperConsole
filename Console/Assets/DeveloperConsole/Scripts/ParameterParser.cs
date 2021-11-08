﻿using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

namespace DeveloperConsole {

    public static class ParameterParser {

        private static List<float> floats = new List<float>();

        /// <summary>
        ///  Check if parameter type is supported
        /// </summary>
        /// <returns></returns>
        public static bool IsSupportedType(MethodInfo method, string methodName, string commandName, Type className) {
            var parameters = method.GetParameters();

            // Early return if method doesn't take in any parameters.
            if (parameters == null || parameters.Length == 0) return true;

            if (parameters.Length > 1) {
#if UNITY_EDITOR
                Debug.Log(string.Format(ConsoleConstants.WARNING + "Command '{0}' in class '{1}' with method name {2} takes in two or more parameters.\n " +
                    "Multiple parameters are not supported!", commandName, className, methodName));
#endif
                return false;
            }

            var paramType = parameters[0].ParameterType;
            if (ConsoleConstants.SupportedTypes.Contains(paramType)) {
                return true;
            }
            else {
#if UNITY_EDITOR
                // See SupportedTypes array for all supported types
                Debug.Log(string.Format(ConsoleConstants.WARNING + "Parameter typeof {0} is not supported! \n" +
                    "Command '{1}' in class '{2}' with method name '{3}' will be ignored!", paramType, commandName, className, methodName));
#endif
                return false;
            }
        }

        /// <summary>
        /// Try to parse parameter from string
        /// </summary>
        public static object ParseParameterFromString(string input, Type type) {
            // UnityEngine types such as Vector2, Vector3 etc are not part of C# TypeCode
            // so they must be checked with other way
            if (ConsoleConstants.UnityTypes.Contains(type)) {
                return ParseUnityTypes(input, type);
            }
            else {
                return ParseBuiltInTypes(input, type);
            }
        }

        private static object ParseUnityTypes(string input, Type unityTypes) {

            // Delete all f's from strings
            input = ConsoleUtils.DeleteCharF(input);

            string[] paramArr;
            if (input.Contains(ConsoleConstants.COMMA)) {
                paramArr = input.Split(ConsoleConstants.CHARCOMMA);
            }
            else {
                paramArr = input.Split();
            }

            float f;
            floats.Clear(); // Creating new list would generate garbage.

            for (int i = 0; i < paramArr.Length; i++) {
                if (float.TryParse(paramArr[i], out f)) {
                    floats.Add(f);
                }
            }

            if (unityTypes == typeof(UnityEngine.Vector2) && floats.Count == 2) {
                return new Vector2(floats[0], floats[1]);
            }
            else if (unityTypes == typeof(UnityEngine.Vector3) && floats.Count == 3) {
                return new Vector3(floats[0], floats[1], floats[2]);
            }
            else if (unityTypes == typeof(UnityEngine.Vector4) && floats.Count == 4) {
                return new Vector4(floats[0], floats[1], floats[2], floats[3]);
            }
            else if (unityTypes == typeof(UnityEngine.Quaternion) && floats.Count == 4) {
                return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
            }
            else {
                return null;
            }
        }

        private static object ParseBuiltInTypes(string input, Type type) {

            if (type == typeof(string[])) {
                return ParseStringArray(input);
            }

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Int32:
                    return ParseInt(input);
                case TypeCode.Boolean:
                    return ParseBoolean(input);
                case TypeCode.Decimal:
                    return ParseDecimal(input);
                case TypeCode.Double:
                    return ParseDouble(input);
                case TypeCode.Single:
                    return ParseFloat(input);
                case TypeCode.String:
                    return input;
                case TypeCode.Char:
                    return ParseChar(input);
                default:
                    return null;
            }
        }

        private static object ParseBoolean(string input) {
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);
            bool value;
            if (bool.TryParse(input, out value)) {
                return value;
            }
            else {
                return null;
            }
        }

        private static object ParseStringArray(string input) {
            string[] words = input.Split(ConsoleConstants.SEPARATORS);

            // Remove all whitespaces from start and end of the string
            for (int i = 0; i < words.Length; i++) {
                words[i] = words[i].Trim();
            }
            return words as object;
        }

        private static object ParseInt(string input) {
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);

            int number;
            bool success = int.TryParse(input, out number);
            if (success) {
                return number;
            }
            else {
                return null;
            }
        }

        private static object ParseFloat(string input) {
            input = ConsoleUtils.DeleteCharF(input);
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);

            float number;
            bool success = float.TryParse(input, out number);
            if (success) {
                return number;
            }
            else {
                return null;
            }
        }

        private static object ParseDouble(string input) {
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);
            double number;
            bool success = double.TryParse(input, out number);
            if (success) {
                return number;
            }
            else {
                return null;
            }
        }

        private static object ParseDecimal(string input) {
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);
            decimal number;
            bool success = decimal.TryParse(input, out number);
            if (success) {
                return number;
            }
            else {
                return null;
            }
        }

        private static object ParseChar(string input) {
            input = ConsoleUtils.DeleteWhiteSpacesFromString(input);
            Char value;
            bool result = Char.TryParse(input, out value);
            if (result) {
                return value;
            }
            else {
                return null;
            }
        }
    }
}