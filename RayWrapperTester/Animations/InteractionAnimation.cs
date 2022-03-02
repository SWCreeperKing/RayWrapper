using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Animation;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Animation.Transitions;
using RayWrapper.Objs;
using static RayWrapper.GameBox;

namespace RayWrapperTester.Animations
{
    public class InteractionAnimation : Animation
    {
        public Rect rect = new Rectangle(25, WindowSize.Y / 2 - 40, 20, 20);

        public InteractionAnimation()
        {
            stepConditions.Add(() => rect.rect.IsMouseIn());
            AddTransition(1, new MoveTransition(new Vector2(WindowSize.X - 55, 0), rect, .5f));
            stepConditions.Add(() => rect.rect.IsMouseIn());
            AddTransition(2, new MoveTransition(new Vector2(-WindowSize.X / 2 - 10, 0), rect, .5f));
            stepConditions.Add(
                () => rect.rect.IsMouseIn() && Raylib.IsMouseButtonPressed(Raylib.MOUSE_LEFT_BUTTON));
            AddTransition(3, new MoveTransition(new Vector2(0, WindowSize.Y / 2 + 50), rect, .5f));
            AddToRegister(rect);
        }
    }
}