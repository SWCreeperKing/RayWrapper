using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public abstract class GameLoop
    {
        private List<GameObject> _registryBefore = new();
        private List<GameObject> _registryAfter = new();

        public void Update()
        {
            foreach (var a in _registryBefore) a.Update();
            UpdateLoop();
            foreach (var a in _registryAfter) a.Update();
        }

        public void Render()
        {
            foreach (var a in _registryBefore) a.Render();
            GameBox.RenderColliders();
            RenderLoop();
            foreach (var a in _registryAfter) a.Render();
        }

        public void RegisterGameObj(bool isBefore, params GameObject[] igo) =>
            (isBefore ? _registryBefore : _registryAfter).AddRange(igo);

        public void DeregisterGameObj(bool isBefore, GameObject igo) =>
            (isBefore ? _registryBefore : _registryAfter).Remove(igo);

        public abstract void Init();
        public abstract void UpdateLoop();
        public abstract void RenderLoop();

        public void Text(string text, Vector2 pos, Color color, int fontSize = 24, float spacing = 1.5f) =>
            GameBox.Font.DrawText(text, pos, color, fontSize, spacing);
    }
}