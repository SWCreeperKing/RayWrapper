using ZimonIsHimUtils.ExtensionMethods;
using static RayWrapper.Base.GameBox.AttributeManager;
using static RayWrapper.Base.GameBox.AttributeManager.PlacerType;

namespace RayWrapper.LegacyUI.Animation;

public static class Animator
{
    private static Queue<Animation> _animationQueue = new();
    private static List<Animation> _animationList = new();

    public static void AddToAnimationQueue(Animation ani) => _animationQueue.Enqueue(ani);

    public static void AddAnimation(Animation ani)
    {
        _animationList.Add(ani);
        ani.InitAnimation();
    }

    private static Animation _queuedAnimation;

    [GameBoxWedge(BeforeUpdate)]
    public static void Update(float dt)
    {
        var remove = _animationList.Where(a => a.UpdateAnimation());
        if (remove.Any())
        {
            foreach (var animation in remove) animation.onEnd?.Invoke();
            _animationList.RemoveAll(a => remove.Contains(a));
        }

        if (_queuedAnimation is null)
        {
            if (_animationQueue.Count == 0) return;
            _queuedAnimation = _animationQueue.Dequeue();
            _queuedAnimation.InitAnimation();
        }

        if (!_queuedAnimation.UpdateAnimation()) return;
        _queuedAnimation.onEnd?.Invoke();
        _queuedAnimation = null;
    }

    [GameBoxWedge(AfterRender, 99)]
    public static void Render()
    {
        _queuedAnimation?.Render();
        _animationList.Each(animation => animation.Render());
    }
}