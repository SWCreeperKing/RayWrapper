using System.Collections.Generic;

namespace RayWrapper.Vars
{
    public abstract class GameLoop : IInitializable, IRayObject
    {
        private List<GameObject> _registryBefore = new();
        private List<GameObject> _registryAfter = new();

        public void Update()
        {
            _registryBefore.ForEach(a => a.Update());
            UpdateLoop();
            _registryAfter.ForEach(a => a.Update());
        }

        public void Render()
        {
            _registryBefore.ForEach(a => a.Render());
            RenderLoop();
            _registryAfter.ForEach(a => a.Render());
        }

        public void RegisterGameObj(bool isBefore, params GameObject[] igo) =>
            (isBefore ? _registryBefore : _registryAfter).AddRange(igo);

        public void DeregisterGameObj(bool isBefore, GameObject igo) =>
            (isBefore ? _registryBefore : _registryAfter).Remove(igo);

        public abstract void Init();
        public abstract void UpdateLoop();
        public abstract void RenderLoop();
    }
}