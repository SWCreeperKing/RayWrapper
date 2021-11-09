the animation system is one of the more complex systems RayWrapper has

but it isn't has difficult as it used to be, you don't want to use the old revisions of this system x.x

to sum it up, the animation system is a keyframe system

to start you need a class that extends from the animation abstract class and make a constructor for it

```c#
public class ExampleAnimation : Animation 
{
    // objects go here

    public ExampleAnimation() 
    {
        // animation steps go here, don't 
        // forget to init the objects before starting the animation steps 
    }

```

the most difficult part of this system is how to animate

there are 2 layers to the system, `stepConditions` and `Transitions`

the `stepCondidtions` allow you to test for things before the next step can proceed, if you need a stepCondition anywhere, you need to add them for previous steps (you can just make it return true)

now for `Transitions`

inorder to add a transition to an animation, you can do `AddTransition(step, transition)` (animation steps start at 0 btw)

there are a few different types of transitions
- Move
- Size
- Slide

it shouldn't be too difficult to fill out the requirements for the transitions, if you need help you can go to the Animations example (https://github.com/SWCreeperKing/RayWrapper/tree/master/RayWrapperTester/Animations)

the Animation also has its own gameobject register to add to

once you made your animation you need to add it to the `Animator`

the `Animator` has 2 options for animations:
- normal (animations can stack)
- queue (animations wait for one to pass before the next one can)