public class ConsoleCommand : System.Attribute {

    private string commandName;
    private string value;
    private string info;
    private bool editorOnlyCommand = false;
    private bool hiddenCommand = false;

    /// <summary>
    /// Overloads: 
    /// value overload: default value
    /// info overload: Info text
    /// editorOnlyCommand overload: if true command will only work in Editor.
    /// hiddenCommand overload: commandwon't show in predictions or command list.
    /// </summary>
    public ConsoleCommand(string commandName, string value = "", string info = "", bool editorOnlyCommand = false, bool hiddenCommand = false) {
        this.commandName = commandName;
        this.value = value;
        this.info = info;
        this.editorOnlyCommand = editorOnlyCommand;
        this.hiddenCommand = hiddenCommand;
    }

    public string GetCommandName() {
        return commandName;
    }

    public string GetValue() {
        return value;
    }

    public string GetInfo() {
        return info;
    }

    public bool IsEditorOnlyCommand() {
        return editorOnlyCommand;
    }

    public bool IsHiddenCommand() {
        return hiddenCommand;
    }
}