using System;
using RayWrapper.Vars;

namespace RayWrapper.Animation.Transitions
{
    public abstract class Transition
    {
        public GameObject obj;
        public bool overFlag;
        public float duration;

        private float _timeTaken;

        public Transition(GameObject obj, float duration = 5) => (this.obj, this.duration) = (obj, duration * 1000f);

        public abstract void InitTransition();
        public abstract void UpdateTransition(float deltaTime);
        public abstract void SnapTransition();

        public void Update(float deltaTime)
        {
            if (overFlag) return;
            _timeTaken += deltaTime;
            if (_timeTaken >= duration)
            {
                SnapTransition();
                overFlag = true;
            }
            else UpdateTransition(deltaTime);
        }
    }
}