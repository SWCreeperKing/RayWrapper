using System;
using System.Collections.Generic;
using System.Linq;
using RayWrapper.Animation.Transitions;
using RayWrapper.Var_Interfaces;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Animation
{
    public abstract class Animation
    {
        public int animationStep;
        public long stepTime;
        public long animationTime;
        public IDictionary<int, Func<bool>> stepConditions = new Dictionary<int, Func<bool>>();
        public Action onInit = null;
        public Action onEnd = null;

        private long _lastTime = -1;
        private List<IGameObject> _register = new();
        private IDictionary<int, List<Transition>> _transitions = new Dictionary<int, List<Transition>>();

        public virtual void Animate(long deltaTime)
        {
        }

        public virtual void Draw()
        {
        }

        public void InitAnimation()
        {
            if (!_transitions.ContainsKey(0)) return;
            _transitions[animationStep].Each(transition => transition.InitTransition());
            onInit?.Invoke();
        }
        
        /// <returns>if the animation has completed</returns>
        public bool UpdateAnimation()
        {
            if (!stepConditions.ContainsKey(animationStep) && !_transitions.ContainsKey(animationStep)) return true;
            if (_lastTime == -1) _lastTime = GameBox.GetTimeMs();
            var deltaTime = GameBox.GetTimeMs() - _lastTime;
            _lastTime = GameBox.GetTimeMs();
            stepTime += deltaTime;
            animationTime += deltaTime;
            Animate(deltaTime);
            if (_transitions.ContainsKey(animationStep) && !_transitions[animationStep].All(t => t.overFlag))
            {
                _transitions[animationStep].Each(transition => transition.Update(deltaTime));
                return false;
            }

            if (stepConditions.ContainsKey(animationStep) && !stepConditions[animationStep].Invoke()) return false;
            animationStep++;
            stepTime = 0;
            if (!_transitions.ContainsKey(animationStep)) return false;
            _transitions[animationStep].Each(transition => transition.InitTransition());
            return false;
        }

        public void Render()
        {
            Draw();
            _register.Each(go => go.Render());
        }

        public void AddToRegister(params IGameObject[] go) => _register.AddRange(go);

        public void AddTransition(int stage, params Transition[] tran)
        {
            if (!_transitions.ContainsKey(stage)) _transitions.Add(stage, new List<Transition>());
            _transitions[stage].AddRange(tran);
        }
    }
}