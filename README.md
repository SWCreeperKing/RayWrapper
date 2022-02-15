This is a Wrapper for the C# port (Raylib-cs) of RayLib, a 'bare-bones' graphics library

Made with .Net5

on nuget

[![NuGet](https://img.shields.io/nuget/dt/RayWrapper)](https://www.nuget.org/packages/RayWrapper/)

it has 3 dependencies:
- Raylib-cs (3.7.0)
- DiscordRichPresence (1.0.175)
- Newtonsoft.Json (13.0.1)

For credit, link back to this repo

paths:
- RayWrapper (wrapper code)
- RayWrapperTester (testing program)
- RayWrapperTesterCollision (collision testing program)

## some notable features

---
- in game console (can add custom commands, use ` to access console)
- a built in save system using Newtonsoft.Json
- bad code
- ~~NumberClass is included (A big number lib)~~
  - no longer included, but it still is on Nuget
- many premade gameobjects
- discord rich presence support
- a very bad collision system
- a not as bad animation system
- internal logger and crash log system

## raw template example

---

```C#
new GameBox(new Program(), new Vector2(window width, window height, "title"))

public class Program : GameLoop 
{
    public override void Init() 
    {
        // game object init
        // if you have objects that just need to render and update
        // then use RegisterGameObj()
    }
    
    // tip: DO NOT init new objects everytime in Update/Render Loops
    public override void UpdateLoop()
    {
        // put update loop stuff here
    }
    
    public override void RenderLoop()
    {
        // put render loop stuff here
    }
}
```