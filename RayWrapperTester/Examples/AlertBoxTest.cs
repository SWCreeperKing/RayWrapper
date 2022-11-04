using System.Numerics;
using RayWrapper.Base;
using RayWrapper.Objs;
using RayWrapper.Objs.AlertBoxes;
using RayWrapperTester.Example_Setup;

namespace RayWrapperTester.Examples;

[Example("AlertBoxes & InputBox")]
public class AlertBoxTest : Example
{
    public AlertBoxTest(string tabName) : base(tabName)
    {
        Vector2 pos = new(75, 100);

        var b = new Button(pos, "Test Close");
        var bb = new Button(pos + new Vector2(0, 60), "Test Confirm");
        var bbb = new Button(pos + new Vector2(0, 120), "Test Input");
        b.Clicked += () => new AlertClose("Testing", "Just testing alert boxes").Show();
        bb.Clicked += () => new AlertConfirm("Testing", "Just testing alert\nboxes").Show();
        bbb.Clicked += () => new AlertInput("Testing", "Hello?") { onResult = Logger.Log }.Show();

        var inputBox = new InputBox(pos + new Vector2(300, 0));
        var inputBoxSmall = new InputBox(pos + new Vector2(300, 50), maxCharacters: 7) { numbersOnly = true };
        var inputBoxPass = new InputBox(pos + new Vector2(300, 100), maxCharacters: 7) { password = true };

        RegisterGameObj(b, bb, bbb, inputBox, inputBoxSmall, inputBoxPass);
    }
}