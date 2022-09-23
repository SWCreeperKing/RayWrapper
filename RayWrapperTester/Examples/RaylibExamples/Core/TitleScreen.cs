using System.Numerics;
using RayWrapper.Base.Gameobject;
using RayWrapper.Objs;
using RayWrapperTester.Example_Setup;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapperTester.Examples.RaylibExamples.Core;

// todo: this might need to be its own thing due to SceneManager

[Example("Raylib|Core|TitleScreen")]
public class TitleScreen : Example
{
    public GameObject[] objs;
    
    public TitleScreen(string tabName) : base(tabName)
    {
        
    }
}

// these classes can be in their own files
class Logo : GameObject
{
    public Logo()
    {
        Text logo = new("LOGO SCREEN", new Vector2(120), LIGHTGRAY, 40);
        Text infoText = new("WAIT for 2 SECONDS...", WindowSize, LIGHTGRAY, 20)
        {
            style =
            {
                drawMode = Text.Style.DrawMode.Center
            }
        };
        
        RegisterGameObj(logo, infoText);
    }
}

class Title : GameObject
{
    public Title()
    {
        Text logo = new("TITLE SCREEN", new Vector2(120), DARKGREEN, 40);
        Text infoText = new("PRESS ENTER or TAP or JUMP to GAMPLE SCREEN", WindowSize, DARKGREEN, 20)
        {
            style =
            {
                drawMode = Text.Style.DrawMode.Center
            }
        };
    }
}