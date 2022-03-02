<h3 align="center">RayWrapper</h3>

<div align="center">

  [![Stars](https://img.shields.io/github/stars/SWCreeperKing/RayWrapper)](https://github.com/SWCreeperKing/RayWrapper/stargazers)
  [![NuGet](https://img.shields.io/nuget/dt/RayWrapper)](https://www.nuget.org/packages/RayWrapper/)
  [![Last Commit](https://img.shields.io/github/last-commit/SWCreeperKing/RayWrapper)](https://github.com/SWCreeperKing/RayWrapper/commits/development)
  [![License](https://img.shields.io/github/license/SWCreeperKing/RayWrapper)](/LICENSE)

</div>

---

<p align="center"> This is a Wrapper for the C# bindings (<a href="https://github.com/ChrisDill/Raylib-cs">Raylib-cs</a>) of <a href="https://github.com/raysan5/raylib">Raylib</a>, a 'bare-bones' graphics library.
    <br> 
</p>

## 📝 Table of Contents
- [About](#about)
- [Features](#features)
- [Usage](#usage)
- [Example Template](#example_template)
- [Built Using](#built_using)
- [Authors](#authors)

## 🧐 About <a name = "about"></a>
This is a Wrapper for the C# port (Raylib-cs) of Raylib, a 'bare-bones' graphics library.
Made with .Net5 on nuget.

For credit, please link back to this repo.

paths:
- RayWrapper (wrapper code)
- RayWrapperTester (testing program)
- RayWrapperTesterCollision (collision testing program).

## 🎁 Features <a name="features"></a>
- In game console (can add custom commands, use ` to access console)
- A built in save system using Newtonsoft.Json
- ~~NumberClass is included (A big number lib)~~
  - no longer included, but it still is on Nuget
- Many premade gameobjects
- Discord rich presence support
- a WIP collision system
- an animation system
- Internal logger and crash log system

## 🎈 Usage <a name="usage"></a>
TODO: Insert usage instructions here. Say some stuff about adding the NuGet package and .Net version.


## 🏷️ Example Template <a name="example_template"></a>
```C#
new GameBox(new Program(), new Vector2(window width, window height, "title"));

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


## ⛏️ Built Using <a name = "built_using"></a>
- [Raylib-cs (4.0.0.1)](https://www.nuget.org/packages/Raylib-cs/) - C# bindings for the raylib framework
- [DiscordRichPresence (1.0.175)](https://www.nuget.org/packages/DiscordRichPresence/) - A .NET implementation of Discord's Rich Presence functionality
- [Newtonsoft.Json (13.0.1)](https://www.nuget.org/packages/Newtonsoft.Json/) - Popular high-performance JSON framework for .NET

## ✍️ Authors <a name = "authors"></a>
- [@SWCreeperKing](https://github.com/SWCreeperKing) - Idea, Initial work, general development & management
- [@ZimonIsHim](https://github.com/ZimonIsHim) - Major/Main contributor
