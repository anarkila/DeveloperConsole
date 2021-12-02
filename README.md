# Developer Console

Developer Console for Unity with easy integration to existing projects.

[WebGL demo](https://anarkila.github.io/DeveloperConsole/Demo)

### Use cases 
- Development cheats
- Debug assistance
- In game cheat codes, change settings easily etc

![thumbnail](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_dark.PNG)

## Getting Started
1. Download and import [DeveloperConsole package](https://github.com/anarkila/DeveloperConsole/releases/download/v0.8.2/DeveloperConsole_0.8.2.unitypackage) into your project
2. Drag & drop DeveloperConsole prefab into your scene
3. Add ``[ConsoleCommand()]`` attribute to your methods like below. See [ConsoleExamples.cs](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Example%20scenes/Example%20scripts/ConsoleExamples.cs) for all examples. 
4. Play your scene and press ``§`` to toggle Developer Console

```C#
using UnityEngine;

public class ExampleScript : MonoBehaviour {

    [ConsoleCommand("test")]
    private void Test() {
        Debug.Log("Called 'Test' from Developer Console!");
    }
    
    [ConsoleCommand("test.int")]
    private void TestInt(int i) {
       Debug.Log(string.Format("Called 'TestInt' with value: {0} from Developer Console!", i));
    }
}
```

## Features

- Call static, non-static and Unity Coroutines methods (both public and private)
- Single parameter support:
    - Allowed types: int, float, string, string[], bool, double, char, Vector2, Vector3, Vector4, Quaternion
    - Optional parameter support
- Easy drag & drop setup
- Mono and IL2CPP support
- WebGL support
- 2 GUI styles: [Large](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large_dark.png) and [Minimal](https://github.com/anarkila/DeveloperConsole/blob/main/Images/minimal.png)
- Draggable & resizable window (Large GUI only)
- Log directly into Developer Console window
- Log Unity ``Debug.Log`` and ``Debug.LogError`` messages into Console window
- Input predictions
- [Settings](https://github.com/anarkila/DeveloperConsole/blob/main/Images/settings.PNG) to tweak
- GUI themes (Dark, Red or Custom)
- Documentation and example scenes
- Generate command list into text file
- Editor debug information:
    - Print rendering info
    - Print Play button click to playable Scene time

## Default Commands
Developer Console comes with few commands by default.

* ``help`` - Print list of available commands
* ``quit`` - Quit the application
* ``close`` - Close Console
* ``clear`` - Clear all Console messages
* ``reset`` - Reset Console window to default size and position (Large GUI only)
* ``max_fps (int)`` - Set [Application.TargetFrameRate](https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html)
* ``console_style`` - Toggle GUI style between Large and Minimal
* ``scene_loadindex (int)`` - Load scene asynchronously by build index
* ``scene_addloadindex (int)`` - Load scene asynchronously additively by build index
* ``scene_loadname (string)`` - Load scene asynchronously by scene name
* ``scene_unloadindex (int)`` - UnLoad scene asynchronously by build index
* ``scene_unloadname (string)`` - UnLoad scene asynchronously by scene name
* ``debug_renderinfo`` - Print rendering information: High and Avg FPS, highest draw call, batches, triangle and vertices count. This command is editor only.

## Logging
``Console.Log()`` to output directly into Developer Console window.

By default Unity ``Debug.Log()`` or ``Debug.LogError()`` messages will also output to Developer Console.

By default Developer Console pools 150 messages and once 150 messages has been reached, messages will start to recycle from the beginning.

## Notes
- Requires Unity 2019 or later
- Uses old Unity input system
- Uses Gameobject based UI
- Uses TextMeshPro

## TODO
- Explore way to have multiple parameters
- Ability to generate grid or list of buttons that can fire commands
- Support for no Domain/Scene reload
- Input prediction improvements