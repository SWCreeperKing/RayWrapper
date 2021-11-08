This is a Wrapper for Raylib-cs

Made with .Net5

it has 3 dependencies:
- Raylib-cs (3.7.0)
- DiscordRichPresence (1.0.175)
- Newtonsoft.Json (13.0.1)

For credit, link back to this repo

paths:
- RayWrapper (wrapper code)
- RayWrapperTester (testing program)
- RayWrapperTesterCollision (collision testing program)

some notable features
---
- in game console (can add custom commands)
- a built in save system using Newtonsoft.Json
- bad code
- NumberClass is included (A big number lib)
- many premade gameobjects
- discord rich presence support
- a very bad collision system
- a not as bad animation system

small example
---

```C#
new GameBox(new Program(), new Vector2(window width, window height, "title"))

public class Program : GameLoop 
{
    public override void Init() 
    {
        // game object init
        // if you have objects that just need to render and update
        // then use RegisterGameObject()
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