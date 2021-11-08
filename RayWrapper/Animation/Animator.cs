using System.Collections.Generic;
using System.Linq;

namespace RayWrapper.Animation
{
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

        public static void Update()
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

        public static void Render()
        {
            _queuedAnimation?.Render();
            foreach (var animation in _animationList) animation.Render();
        }
    }
}