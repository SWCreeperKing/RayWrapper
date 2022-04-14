using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapperTester.Example_Setup;

public class Example : GhostObject
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