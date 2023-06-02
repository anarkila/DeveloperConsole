# Developer Console

### Use cases 
- Development cheats
- Debug assistance
- In game cheat codes, change settings easily etc

![thumbnail](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_dark.PNG)

## Installation

### From a git url

You can install this plugin via assets manager from a git url

```
https://github.com/canoide/DeveloperConsole.git?path=DeveloperConsole
```

## Getting Started
1. Download and import [DeveloperConsole package](https://github.com/anarkila/DeveloperConsole/releases/download/v1.0.2/DeveloperConsole_1.0.2.unitypackage) into your project
2. Drag & drop DeveloperConsole prefab into your scene
3. Add ``[ConsoleCommand()]`` attribute to your methods like below. See [ConsoleExamples.cs](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Example%20scenes/Example%20scripts/ConsoleExamples.cs) for all examples. 
4. Play your scene and press ``§`` to toggle Developer Console

```C#
using UnityEngine;

public class ExampleScript : MonoBehaviour {

    [ConsoleCommand("test")]
    private void Test() {
        Debug.Log("Called 'test' from Console!");
    }
    
    [ConsoleCommand("test_int")]
    private void TestInt(int i) {
       // single parameter allowed types: 
       // int, float, string, bool, double, char, string[], Vector2, Vector3, Vector4, Quaternion
       Debug.Log(string.Format("Called 'test_int' with value: {0} from Console!", i));
    }

    [ConsoleCommand("test_multi")]
    private void TestMulti(int i, float f) {
       // multi parameter allowed types:
       // int, float, string, bool, double, char
       Debug.Log(string.Format("Called 'test_multi' with value: {0} and {1} from Console!", i, f));
    }
}
```

## Features

- Call static, non-static and Unity Coroutines methods (both public and private)
- No parameter and optional parameter(s) support
- Single parameter support with following types:
    - int, float, string, bool, double, char, string[], Vector2, Vector3, Vector4, Quaternion
- Multi parameter support with following types:
    - int, float, string, bool, double, char
- Easy drag & drop setup
- Mono and IL2CPP support
- Desktop and WebGL support
- Domain/Scene reload support ([Enter Play Mode](https://docs.unity3d.com/Manual/ConfigurableEnterPlayMode.html))
- Simple static [runtime API](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Scripts/Console.cs)
- 2 GUI styles: [Large](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_dark.PNG) and [Minimal](https://github.com/anarkila/DeveloperConsole/blob/main/Images/minimal.png)
- Draggable & resizable window (Large GUI only)
- Log messages into Console Window (``Console.Log``, ``Debug.Log`` and ``Debug.LogError``)
- Input predictions
- [Settings](https://github.com/anarkila/DeveloperConsole/blob/main/Images/settings.PNG) to tweak
- GUI themes ([Dark](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_dark.PNG), [Darker](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_darker.png), [Red](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_red.PNG) or Custom)
- Documentation and example scenes

## Default Commands
Developer Console comes with few commands by default.

* ``help`` - Print list of available commands
* ``quit`` - Quit the application
* ``close`` - Close Console
* ``clear`` - Clear all Console messages
* ``reset`` - Reset Console window to default size and position (Large GUI only)
* ``max_fps (int)`` - Set [Application.TargetFrameRate](https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html)
* ``console_style`` - Toggle GUI style between Large and Minimal

Editor and Development build only:

* ``scene_loadindex (int)`` - Load scene asynchronously by build index
* ``scene_addloadindex (int)`` - Load scene asynchronously additively by build index
* ``scene_loadname (string)`` - Load scene asynchronously by scene name
* ``scene_unloadindex (int)`` - Unload scene asynchronously by build index
* ``scene_unloadname (string)`` - Unload scene asynchronously by scene name
* ``scene_information`` - Print Scene count and names
* ``empty`` - Log empty line to console
* ``debug_renderinfo`` - Print rendering information: High and Avg FPS, highest draw call, batches, triangle and vertices count. This command is editor only.
* ``log_to_file`` - Log all current messages to .txt file. This command is Editor only.

## Logging
``Console.Log("hello")`` to output directly into Developer Console window. 

``Console.Log("hello", Color.red)`` with color.

``Console.Log("<color=red>hello</color>")`` with Rich Text color.

By default Unity ``Debug.Log()`` or ``Debug.LogError()`` messages will also output to Developer Console.

## Notes
- Requires Unity 2019 or later
- Uses old Unity input system
- Uses Gameobject based UI
- Uses TextMeshPro