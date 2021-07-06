using System;
using System.Collections.Generic;
using System.Linq;

namespace RayWrapper.Animation
{
    public class Animator
    {
        // main control
        private Queue<Animation> _animationQueue = new();
        private List<Animation> _animations = new();
        private Animation _activeAnimation;
        
        public void Update()
        {
            try
            {
                List<Animation> remove = new();
                foreach (var animation in _animations)
                {
                    animation.Update();
                    if (animation.HasEnded()) remove.Add(animation);
                }

                _animations = _animations.Except(remove).ToList();
            }
            catch (ArgumentException)
            {
            }

            if (_animationQueue.Count < 1 && _activeAnimation is null) return;
            _activeAnimation ??= _animationQueue.Dequeue();
            _activeAnimation.Update();
            if (_activeAnimation.HasEnded()) _activeAnimation = null;
        }
        
        public void Render()
        {
            foreach (var animation in _animations) animation.Render();
            _activeAnimation?.Render();
        }

        public void AddAnimation(Animation anim) => _animations.Add(anim.CopyAnimation());
        public void QueueAnimation(Animation anim) => _animationQueue.Enqueue(anim.CopyAnimation());
    }
}