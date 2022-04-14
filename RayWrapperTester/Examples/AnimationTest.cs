using System.Numerics;
using RayWrapper.Animation;
using RayWrapper.Objs;
using RayWrapper.Vars;
using RayWrapperTester.Animations;
using RayWrapperTester.Example_Setup;

namespace RayWrapperTester.Examples;

[Example("Animation")]
public class AnimationTest : Example
{
    public AnimationTest(string tabName) : base(tabName)
    {
        Button aniB = new(new Vector2(20, 100), "Queue Animation");
        aniB.Clicked += () => Animator.AddToAnimationQueue(new TestAnimation1());

        Button aniBC = new(new Vector2(20, 140), "Add Animation");
        aniBC.Clicked += () => Animator.AddAnimation(new Mover());

        Button aniBT = new(new Vector2(20, 180), "Queue Trigger Animation");
        aniBT.Clicked += () => Animator.AddToAnimationQueue(new InteractionAnimation());

        RegisterGameObj(aniB, aniBC, aniBT);
    }
}