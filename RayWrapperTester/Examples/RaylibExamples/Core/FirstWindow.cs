using RayWrapper;
using RayWrapper.Objs;
using RayWrapperTester.Example_Setup;

namespace RayWrapperTester.Examples.RaylibExamples.Core;

[Example("Raylib.Core.FirstWindow")]
public class FirstWindow : Example
{
    public FirstWindow(string tabName) : base(tabName)
    {
        var text = new Text("Congrats! You created your first window!", GameBox.WindowSize/2)
        {
            style =
            {
                // this tells the text to draw at the center of the screen
                drawMode = Text.Style.DrawMode.Center
            }
        };
        RegisterGameObj(text);
    }
}

// this example was made to run in the example project, however here is what the actual first window looks like:
/*
new GameBox(new Program(), new Vector2(window width, window height), "title");

public partial class Program : GameLoop 
{
    public override void Init() 
    {
        // game object init
        // if you have objects that just need to render and update
        // then use RegisterGameObj()
        
        var text = new Text("Congrats! You created your first window!", GameBox.WindowSize/2)
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
    public override void UpdateLoop()
    {
        // put update loop stuff here
    }
    
    public override void RenderLoop()
    {
        // put render loop stuff here
    }
}
*/