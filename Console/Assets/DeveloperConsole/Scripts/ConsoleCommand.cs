public class ConsoleCommand : System.Attribute {

    private string commandName;
    private string value;
    private string info;
    private bool debugOnlyCommand = false;          // if set to true, command will only be registered in debug builds (Editor and Development build)
    private bool hiddenCommandMinimalGUI = false;   // if set to true, command won't show up in predictions when using Minimal GUI
    private bool fullyHiddenCommand = false;        // if set to true, command won't show up in predictions

    /// <summary>
    /// Overloads: 
    /// value overload: default value
    /// info overload: Info text
    /// debugOnlyCommand overload: if true command will only work in Debug build (Editor and Development build).
    /// hiddenCommandMinimalGUI overload: command won't show in predictions with Minimal GUI.
    /// hiddenCommand overload: command won't show in predictions.
    /// </summary>
    public ConsoleCommand(string commandName, string value = "", string info = "",
        bool debugOnlyCommand = false, bool hiddenCommandMinimalGUI = false, bool hiddenCommand = false) {
        this.commandName = commandName;
        this.value = value;
        this.info = info;
        this.debugOnlyCommand = debugOnlyCommand;
        this.hiddenCommandMinimalGUI = hiddenCommandMinimalGUI;
        this.fullyHiddenCommand = hiddenCommand;
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

    public bool IsDebugOnlyCommand() {
        return debugOnlyCommand;
    }

    public bool IsHiddenCommand() {
        return fullyHiddenCommand;
    }

    public bool IsHiddenMinimalGUI() {
        return hiddenCommandMinimalGUI;
    }
}