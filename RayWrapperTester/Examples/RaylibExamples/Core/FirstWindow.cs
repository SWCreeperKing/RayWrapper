using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.UI;

namespace RayWrapperTester.Examples.RaylibExamples.Core;

// commented out due to this 
// new GameBox(new Program(), new Vector2(window width, window height), "title");

public partial class Program : GameLoop 
{
    public override void Init() 
    {
        // game object init
        // if you have objects that just need to render and update
        // then use RegisterGameObj()
        
        var text = new Text("Congrats! You created your first window!", GameBox.WindowSize/2, Raylib.LIGHTGRAY, 20)
        {
            style =
            {
                // this tells the text to draw at the center of the screen
                drawMode = Text.Style.DrawMode.Center
            }
        };
        RegisterGameObj(text);
    }
    
    // tip: DO NOT init new objects everytime in Update/Render Loops
    public override void UpdateLoop(float dt)
    {
        // put update loop stuff here
    }
    
    public override void RenderLoop()
    {
        // put render loop stuff here
    }
}