using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Animation;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Animation.Transitions;
using Rectangle = RayWrapper.Base.Rectangle;

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