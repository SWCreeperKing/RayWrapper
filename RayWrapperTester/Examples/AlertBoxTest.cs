using System.Numerics;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.AlertBoxes;
using RayWrapper.LegacyUI.UI;
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

        var inputBox = new InputBox(pos + new Vector2(300, 0), 200);
        var inputBoxSmall = new InputBox(pos + new Vector2(300, 50), 200, maxCharacters: 7) { numbersOnly = true };
        var inputBoxPass = new InputBox(pos + new Vector2(300, 100), 200, maxCharacters: 7) { password = true };

        RegisterGameObj(b, bb, bbb, inputBox, inputBoxSmall, inputBoxPass);
    }
}