using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TextFileWriter {

    public static void WriteToFile(string[] messages) {
        if (messages == null || messages.Length == 0) {
#if UNITY_EDITOR
            Console.Log("No messages to write.");
#endif
            return;
        }

#if UNITY_EDITOR
        WriteToFileEditorWindows(messages);
        return;
#endif
    }

#if UNITY_EDITOR
    private static void WriteToFileEditorWindows(string[] messages) {
        if (Application.platform != RuntimePlatform.WindowsEditor) {
            return;
        }

        string assetsFolderPath = Application.dataPath;

        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        string fileName = "ConsoleMessages_" + Timestamp;
        string fileNameEnding = fileName + ".txt";
        string fullPath = assetsFolderPath + "/" + fileNameEnding;

        // Create a new .txt file and write messages to it
        using (StreamWriter sw = File.CreateText(fullPath)) {
            for (int i = 0; i < messages.Length; i++) {
                sw.WriteLine(messages[i]);
            }
        }

        HighlightAssetByName(fileName);
    }
#endif


#if UNITY_EDITOR
    private static void HighlightAssetByName(string fileName) {
        UnityEditor.AssetDatabase.Refresh();
        var asset = AssetDatabase.FindAssets(fileName);
        var path = AssetDatabase.GUIDToAssetPath(asset[0]);
        AssetDatabase.ImportAsset(path);
        EditorUtility.FocusProjectWindow();
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        Selection.activeObject = obj;
    }
#endif

}