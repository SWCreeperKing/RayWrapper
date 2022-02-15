using System;
using System.Collections.Generic;
using System.Linq;
using RayWrapper.Animation.Transitions;
using RayWrapper.Vars;

namespace RayWrapper.Animation
{
    public abstract class Animation
    {
        public int animationStep;
        public long stepTime;
        public long animationTime;
        public IList<Func<bool>> stepConditions = new List<Func<bool>>();
        public Action onInit = null;
        public Action onEnd = null;

        private long _lastTime = -1;
        private List<GameObject> _register = new();
        private Dictionary<int, List<Transition>> _transitions = new();

        public virtual void Animate(long deltaTime)
        {
        }

        public virtual void Draw()
        {
        }

        public void InitAnimation()
        {
            if (!_transitions.ContainsKey(0)) return;
            foreach (var transition in _transitions[animationStep]) transition.InitTransition();
            onInit?.Invoke();
        }

        public bool UpdateAnimation()
        {
            if (stepConditions.Count <= animationStep && !_transitions.ContainsKey(animationStep)) return true;
            if (_lastTime == -1) _lastTime = GameBox.GetTimeMs();
            var deltaTime = GameBox.GetTimeMs() - _lastTime;
            _lastTime = GameBox.GetTimeMs();
            stepTime += deltaTime;
            animationTime += deltaTime;
            Animate(deltaTime);
            if (_transitions.ContainsKey(animationStep) && !_transitions[animationStep].All(t => t.overFlag))
            {
                foreach (var transition in _transitions[animationStep]) transition.Update(deltaTime);
                return false;
            }

            if (stepConditions.Count > animationStep && !stepConditions[animationStep].Invoke()) return false;
            animationStep++;
            stepTime = 0;
            if (!_transitions.ContainsKey(animationStep)) return false;
            foreach (var transition in _transitions[animationStep])
                transition.InitTransition();
            return false;
        }

        public void Render()
        {
            Draw();
            foreach (var go in _register) go.Render();
        }

        public void AddToRegister(params GameObject[] go) => _register.AddRange(go);

        public void AddTransition(int stage, params Transition[] tran)
        {
            if (!_transitions.ContainsKey(stage)) _transitions.Add(stage, new List<Transition>());
            _transitions[stage].AddRange(tran);
        }
    }
}