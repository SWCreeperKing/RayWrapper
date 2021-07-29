using System;
using System.Collections.Generic;
using RayWrapper.Animation.AnimationShapes;

namespace RayWrapper.Animation
{
    public class Animation
    {
        // contains and controls animation steps
        private AnimationBuilder _builder;
        private AnimationStep _currentStep;

        public Animation(AnimationBuilder builder) => _builder = builder;

        public void Update()
        {
            _currentStep ??= _builder.GetNextStep();
            if (HasEnded()) return;
            var done = _currentStep.Execute(_builder);
            if (done) _currentStep = null;
        }

        public void Render()
        {
            if (HasEnded()) return;
            _builder.RenderShapes();
            _currentStep?.runningTrigger?.Invoke(_builder);
        }

        public bool HasEnded() => _currentStep is null && _builder.IsOver();

        public Animation CopyAnimation() => new(_builder.CopyBuilder());
    }
}