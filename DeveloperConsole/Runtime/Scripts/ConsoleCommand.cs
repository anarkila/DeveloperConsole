public class ConsoleCommand : System.Attribute {

    private string commandName;                     // command name
    private string value;                           // Optional parameter: command default value (as string)
    private string info;                            // Optional parameter: Command info
    private bool debugOnlyCommand = false;          // Optional parameter: if set to true, command will only be registered in debug builds (Editor and Development build)
    private bool hiddenCommandMinimalGUI = false;   // Optional parameter: if set to true, command won't show up in predictions when using Minimal GUI
    private bool fullyHiddenCommand = false;        // Optional parameter: if set to true, command won't show up in predictions
    private bool autoClear = true;                 // Optional parameter: if set to false, command won't clear when load scene

    public ConsoleCommand(string commandName, string value = "", string info = "",
        bool debugOnlyCommand = false, bool hiddenCommandMinimalGUI = false, bool hiddenCommand = false, bool autoClear = true) {

        this.commandName = commandName;
        this.value = value;
        this.info = info;
        this.debugOnlyCommand = debugOnlyCommand;
        this.hiddenCommandMinimalGUI = hiddenCommandMinimalGUI;
        this.fullyHiddenCommand = hiddenCommand;
        this.autoClear = autoClear;
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

    public bool AutoClear()
    {
        return autoClear;
    }
}