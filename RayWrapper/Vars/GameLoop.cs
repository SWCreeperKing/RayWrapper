using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

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
            GameBox.RenderColliders();
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

        public void Text(string text, Vector2 pos, Color color, int fontSize = 24, float spacing = 1.5f) =>
            GameBox.font.DrawText(text, pos, color, fontSize, spacing);
    }
}