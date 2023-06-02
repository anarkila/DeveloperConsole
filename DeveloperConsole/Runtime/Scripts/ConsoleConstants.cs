using UnityEngine;
using System;

public static class ConsoleConstants {

    // Constant strings, don't modify.
#if UNITY_EDITOR
    public const string EDITORWARNING = "<color=red>Developer Console Editor Warning: </color>";
    public const string UNLIMITED = "Unlimited";
#endif
    public const string HELPTEXT = "Type 'help' and press Enter to print all available commands.";
    public const string REGISTEREDSTATIC = " (static commands only).";
    public const string COMMANDMESSAGE = "All available commands:";
    public const string CONSOLEINIT = "Console Initialized. ";
    public const string COLOR_RED_START = "<color=red>";
    public const string DATETIMEFORMAT = "HH:mm:ss";
    public const string IENUMERATOR = "IEnumerator";
    public const string COLOR_END = "</color>";
    public const string OPENPARENTHESIS = "(";
    public const string CLOSEDBRACKET = "] ";
    public const string OPENBRACKET = "[";
    public const char EMPTYCHAR = ' ';
    public const char CHARCOMMA = ',';
    public const string LINE = " - ";
    public const char ANDCHAR = '&';
    public const string SPACE = " ";
    public const string COMMA = ",";
    public const string EMPTY = "";
    public const string AND = "&";
    public const string T = "\t";
    public const string F = "f";

    // Array of all supported parameter types
    // If you want to add types to this list,
    // you need to modify ParameterParser.ParseBuiltInTypes() function.
    public static Type[] SupportedTypes = {
            typeof(int),      typeof(float),
            typeof(decimal),  typeof(double),
            typeof(bool),     typeof(string),
            typeof(char),     typeof(string[]),
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