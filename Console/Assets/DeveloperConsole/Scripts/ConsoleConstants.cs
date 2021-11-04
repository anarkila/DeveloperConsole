using UnityEngine;
using System;

public static class ConsoleConstants {

    // Constant strings, do not modify.
    public const string WARNING = "<color=Yellow>Developer Console Warning: </color>";
    public const string DATETIMEFORMAT = "HH:mm:ss";
    public const string IENUMERATOR = "IEnumerator";
    public const string CLOSEDBRACKET = "] ";
    public const string OPENBRACKET = "[";
    public const string COMMA = ",";
    public const char CHARCOMMA = ',';
    public const string EMPTY = "";
    public const string SPACE = " ";
    public const string T = "\t";
    public const string F = "f";
  
    // Array of all allowed array separators
    public static char[] SEPARATORS = { ',', '.', ':', ';' };

    // Array of all supported parameter types
    // If you want to add types to this list,
    // you need to modify ParameterParser.ParseBuiltInTypes() function.
    public static Type[] SupportedTypes = {
            typeof(int),     typeof(byte),
            typeof(float),   typeof(decimal),
            typeof(double),  typeof(bool),
            typeof(string),  typeof(string[]),
            typeof(Vector2), typeof(Vector3),
            typeof(Vector4), typeof(Quaternion),
            typeof(char),    // lonely char
    };

    // Array of supported Unity types
    // If you want to add types to this list,
    // you need to modify ParameterParser.ParseUnityTypes() function.
    public static Type[] UnityTypes = {
            typeof(Vector2), typeof(Vector3),
            typeof(Vector4), typeof(Quaternion)
    };
}