using UnityEngine;

[System.Serializable]
public class ConsoleColors {

    [Header("Minimal GUI Colors")]
    [Tooltip("Minimal GUI background color")]
    public Color minimalGUIBackgroundColor = new Color(0.16f, 0.16f, 0.16f, 1f);

    [Tooltip("Minimal GUI text color")]
    public Color minimalGUITextColor = new Color(1f, 0f, 0f, 1f);


    [Header("Large GUI Colors")]
    [Tooltip("Large GUI background color")]
    public Color largeGUIBackgroundColor = new Color(0f, 0f, 0f, 0.97f);

    [Tooltip("Large GUI background color")]
    public Color largeGUIBorderColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1f);

    [Tooltip("Large GUI highlight color for mouse hover and click")]
    public Color largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f);

    [Tooltip("Large GUI inputfield, scrollrect, button color")]
    public Color largeGUIControlsColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f);

    [Tooltip("Large GUI scrollbar background color")]
    public Color largeGUIScrollbarBackgroundColor = new Color(0.1686275f, 0.1686275f, 0.1686275f, 0.9f);

    [Tooltip("Large GUI scrollbar handle color")]
    public Color largeGUIScrollbarHandleColor = new Color(0.2588235f, 0.2470588f, 0.2431373f, 0.9f);

    [Tooltip("Large GUI text color")]
    public Color largeGUITextColor = new Color(1f, 1f, 1f, 1f);
}