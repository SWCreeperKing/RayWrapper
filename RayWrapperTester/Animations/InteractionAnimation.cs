using System.Numerics;
using RayWrapper.LegacyUI.Animation;
using RayWrapper.LegacyUI.Animation.SinglePurposeObjects;
using RayWrapper.LegacyUI.Animation.Transitions;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.GameBox;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTester.Animations;

public class InteractionAnimation : Animation
{
    public Rect rect = new Rectangle(25, WindowSize.Y / 2 - 40, 20, 20);

    public InteractionAnimation()
    {
        stepConditions.Add(0, () => rect.rect.IsMouseIn());

        AddTransition(1, new MoveTransition(new Vector2(WindowSize.X - 55, 0), rect, .5f));
        stepConditions.Add(1, () => rect.rect.IsMouseIn());

        AddTransition(2, new MoveTransition(new Vector2(-WindowSize.X / 2 - 10, 0), rect, .5f));
        stepConditions.Add(2, () => rect.rect.IsMouseIn() && IsMouseButtonPressed(MOUSE_LEFT_BUTTON));

        AddTransition(3, new MoveTransition(new Vector2(0, WindowSize.Y / 2 + 50), rect, .5f));

        AddToRegister(rect);
    }
}