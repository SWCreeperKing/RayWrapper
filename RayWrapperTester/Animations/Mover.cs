using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.Animation;
using RayWrapper.Animation.Transitions;
using RayWrapper.Objs;

namespace RayWrapperTester.Animations
{
    public class Mover : Animation
    {
        public Rect square = new Rectangle(0, GameBox.WindowSize.Y / 2 - 25, 50, 50);

        public Mover()
        {
            square.color = Color.RED;
            AddTransition(0, new MoveTransition(new Vector2(GameBox.WindowSize.X, 0), square, 1));
            AddToRegister(square);
        }
    }
}