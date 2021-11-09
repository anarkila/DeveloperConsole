using System;
using UnityEngine;

public static class ConsoleConstants {

    // Constant strings, don't modify.
#if UNITY_EDITOR
    public const string EDITORWARNING = "<color=red>Developer Console Editor Warning: </color>";
#endif

    public const string CONSOLEINIT = "Console Initialized. ";
    public const string COLOR_RED_START = "<color=red>";
    public const string DATETIMEFORMAT = "HH:mm:ss";
    public const string IENUMERATOR = "IEnumerator";
    public const string COLOR_END = "</color>";
    public const string CLOSEDBRACKET = "] ";
    public const string OPENBRACKET = "[";
    public const char CHARCOMMA = ',';
    public const string SPACE = " ";
    public const string COMMA = ",";
    public const string EMPTY = "";
    public const string T = "\t";
    public const string F = "f";

  
    // Char array of all allowed supported array separators
    public static char[] SEPARATORS = { ',', '.', ':', ';' };

    // Array of all supported parameter types
    // If you want to add types to this list,
    // you need to modify ParameterParser.ParseBuiltInTypes() function.
    public static Type[] SupportedTypes = {
            typeof(int),      typeof(float),   
            typeof(decimal),  typeof(double),  
            typeof(bool),     typeof(string),  
            typeof(string[]), typeof(char), 
            typeof(Vector2),  typeof(Vector3), 
            typeof(Vector4),  typeof(Quaternion)
    };

    // Array of supported Unity types
    // If you want to add types to this list,
    // you need to modify ParameterParser.ParseUnityTypes() function.
    public static Type[] UnityTypes = {
            typeof(Vector2), typeof(Vector3),
            typeof(Vector4), typeof(Quaternion)
    };
}