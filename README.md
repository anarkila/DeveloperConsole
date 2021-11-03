# Developer Console

Developer Console for Unity with easy integration to existing projects.

### Use cases 
- Development cheats
- Debug assistance
- In game cheat codes
- Change settings easily in game

![thumbnail](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large.png)

## Getting Started
1. Import the DeveloperConsole package into your project
2. Drag & drop DeveloperConsole prefab into your scene (root hierarchy)
3. Add [[ConsoleCommand]](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Scripts/ConsoleCommand.cs) attribute to your methods like below. See [ConsoleExamples.cs](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Example%20scenes/Example%20scripts/ConsoleExamples.cs) for all examples. 
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

- Call static, instance methods and Unity Coroutines (both public and private)
- Single parameter support with following types:
    - int, float, string, string[], bool, double, byte, char, Vector2, Vector3, Vector4, Quaternion
- Support for both Unity backends: Mono and IL2CPP
- WebGL support
- Simple [static API](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Scripts/ConsoleAPI.cs) to interact Console programmatically.
- 2 GUI styles: [Large](https://github.com/anarkila/DeveloperConsole/blob/main/Images/large.png) and [Minimal](https://github.com/anarkila/DeveloperConsole/blob/main/Images/minimal.png)
- Draggable & resizable window (Large GUI only)
- Log directly into Developer Console window
- Log Unity ``Debug.Log`` and ``Debug.LogError`` messages into Developer Console window
- Input predictions
- Documentation and Example scenes
- Editor debug helpers:
    - Print rendering info
    - Print Play button click to playable Scene time

## Default Commands
Developer Console comes with few commands by default. If you wish to modify or delete them, simply modify [DefaultCommands.cs.](https://github.com/anarkila/DeveloperConsole/blob/main/Console/Assets/DeveloperConsole/Scripts/DefaultCommands.cs)

* ``help`` - Print list of available commands
* ``quit`` - Quit the application
* ``close`` - Close Developer Console
* ``clear`` - Clear all Developer Console messages
* ``console.style`` - Toggle GUI style between Large / Minimal
* ``console.reset`` - Reset Developer Console window to default size and position. (Large GUI style only)
* ``scene.loadbyindex (int)`` - Load new scene asynchronously by scene build index
* ``scene.loadbyname (string)`` - Load new scene asynchronously by string
* ``debug.print.renderinfo`` - Print important rendering information: highest draw and batches count, highest triangle/vertices count. This command only works in Editor.

## Logging
``Console.Log()`` or ``ConsoleAPI.Log()``to output directly into Developer Console window.

By default Unity ``Debug.Log()`` or ``Debug.LogError()`` will also output to Developer Console.

By default Developer Console pools 150 messages and once 150 messages has been reached, messages will start to recycle from the beginning.

## TODO
- Explore ways to have multiple parameters
- Explore ways to register Monobehaviour commands faster
- Improve GUI (I'm not GUI designer)
- Improve Garbage Collection
- Ability to generate grid or list of buttons that can fire commands