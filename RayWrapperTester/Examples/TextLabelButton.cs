using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTester.Examples;

[Example("Text, Label & Button")]
public class TextLabelButtonExample : Example
{
    private int _buttonInc;
    private Label _testLabel;
    private RichText richTextTest;

    public TextLabelButtonExample(string tabName) : base(tabName)
    {
        var testButton = new Button(new Rectangle(12, 130, 200, 200), "Just a Name")
        {
            isDisabled = new Actionable<bool>(false, () => _buttonInc > 10)
        };
        testButton.Clicked += () =>
        {
            _buttonInc++;
            testButton.baseL.style.drawMode = (Label.Style.DrawMode) (_buttonInc % 5);
        };

        _testLabel = new Label(new Rectangle(475, 80, 200, 200),
            "Look! I can move with the arrow keys!")
        {
            style =
            {
                drawMode = Label.Style.DrawMode.WrapText
            }
        };

        Text testSpin = new("WWEEEEEE!!", GameBox.WindowSize / 2);
        testSpin.style.SetRotationOriginToCenter("WWEEEEEE!!");
        SchedulerSetup.AddScheduler(
            new Scheduler(50, () => { testSpin.style.rotation = (testSpin.style.rotation + 3) % 360; }));

        Text textTest = new(new Actionable<string>(() => $"Hello, world! [i] is {_buttonInc}"), new Vector2(12, 100));
        // RichText richTextTest = new("""
        // Testing [#fF0a0a]rich
        //  [!aqua]text [!gold]test
        // """, new Vector2(12, 500));
        richTextTest = new("""
        [!green]Goblin Titan[!white] (Elite)[!white]
        Spawn Weight: [1]

        Tags: [[!darkgray]goblin[!white], [!darkgray]humanoid[!white], [!darkgray]titan[!white]][!white]
        Max Hp: [!red]100[!white] | defense: [!gray]10[!white] | Attack: [!blue]10[!white]
        Speed: 0.5 (+0.25/other turn)

        Description: 
        A rare powerful goblin]
        """, new Vector2(12, 450));

        KeyButton kb = new(new Vector2(12, 400), KeyboardKey.KEY_C)
        {
            keyChange = key => Logger.Log($"New Key from Key Button: {key}")
        };
        ImageButton imageButton = new("Assets/Images/Untitled.png", new Vector2(900, 100));
        imageButton.Clicked += () => Logger.Log("Clicked Image");

        ImageButton imageButtonDisabled = new("Assets/Images/Untitled.png", new Rectangle(900, 300, 32, 32))
        {
            isDisabled = true
        };

        RegisterGameObj(testButton, _testLabel, testSpin, textTest, richTextTest, kb, imageButton, imageButtonDisabled);
    }

    protected override void UpdateCall(float dt)
    {
        if (IsKeyDown(KeyboardKey.KEY_LEFT)) _testLabel.Position += new Vector2(-3, 0);
        else if (IsKeyDown(KeyboardKey.KEY_RIGHT)) _testLabel.Position += new Vector2(3, 0);
        if (IsKeyDown(KeyboardKey.KEY_UP)) _testLabel.Position += new Vector2(0, -3);
        else if (IsKeyDown(KeyboardKey.KEY_DOWN)) _testLabel.Position += new Vector2(0, 3);
    }
}