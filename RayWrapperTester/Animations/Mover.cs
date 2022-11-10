using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.Animation;
using RayWrapper.LegacyUI.Animation.SinglePurposeObjects;
using RayWrapper.LegacyUI.Animation.Transitions;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTester.Animations;

public class Mover : Animation
{
    public Rect square = new Rectangle(0, GameBox.WindowSize.Y / 2 - 25, 50, 50);

    public Mover()
    {
        square.ColorMod = Raylib.RED;
        AddTransition(0, new MoveTransition(new Vector2(GameBox.WindowSize.X, 0), square, 1));
        AddTransition(0, new AlphaTransition<Rect>(0, square, 1));
        AddToRegister(square);
    }
}