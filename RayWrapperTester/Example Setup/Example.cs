using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Gameobject;
using RayWrapper.Objs;
using RayWrapper.Objs.AlertBoxes;

namespace RayWrapperTester.Example_Setup;

public abstract class Example : GameObject
{
    public Example(string tabName)
    {
        Button closeTab = new(new Vector2(50, 60), " X ")
        {
            style =
            {
                BackColor = Raylib.RED
            }
        };

        closeTab.Clicked += () =>
        {

            new AlertConfirm("Close Tab", "Are you sure you want to close this tab?")
            {
                onResult = r =>
                {
                    if (r is "yes") Program.exampleView.RemoveTab(tabName);
                }
            }.Show();
        };
        
        RegisterGameObj(closeTab);
    }
}