using System.Numerics;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.Animation;
using RayWrapper.LegacyUI.Animation.SinglePurposeObjects;
using RayWrapper.LegacyUI.Animation.Transitions;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTester.Animations;

public class TestAnimation1 : Animation
{
    public Rect square = new Rectangle(GameBox.WindowSize.X / 2, GameBox.WindowSize.Y / 2, 100, 100);

    public TestAnimation1()
    {
        AddTransition(0, new SlideTransition<Rect>(new Vector2(-50), square, .25f));
        AddTransition(1, new SizeTransition<Rect>(new Vector2(-60), square, .25f));
        AddTransition(2, new SlideTransition<Rect>(new Vector2(-100, 20), square, 1));
        AddTransition(3, new MoveTransition(new Vector2(-200, -20), square, .75f),
            new SizeTransition<Rect>(new Vector2(200, 20), square, .75f));
        AddToRegister(square);
    }
}