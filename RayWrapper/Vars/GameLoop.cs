using System;
using System.Collections.Generic;
using System.Linq;
using RayWrapper.Objs;

namespace RayWrapper.Vars
{
    public abstract class GameLoop
    {
        public bool focusOnScenes;
        private List<GameObject> _registry = new();
        private List<Scene> _scenes = new();
        private int _currentScene = -1;

        public void Update()
        {
            if (_currentScene >= _scenes.Count) _currentScene = _scenes.Count - 1;
            if (_currentScene > -1) _scenes[_currentScene].UpdateCall();
            if (focusOnScenes && _scenes.Any() && _currentScene != -1) return;
            UpdateLoop();
            foreach (var a in _registry) a.Update();
        }

        public void Render()
        {
            if (_currentScene >= _scenes.Count) _currentScene = _scenes.Count - 1;
            if (_currentScene > -1) _scenes[_currentScene].RenderCall();
            if (focusOnScenes && _scenes.Any() && _currentScene != -1) return;
            RenderLoop();
            foreach (var a in _registry) a.Render();
        }

        public void AddScene(Scene scene) => _scenes.Add(scene);

        public void SetScene(int scene)
        {
            if (!_scenes.Any() || scene == _currentScene) return;
            _currentScene = Math.Clamp(scene, 0, _scenes.Count - 1);
            _scenes[_currentScene].Init();
        }

        public void RegisterGameObj(params GameObject[] igo) => _registry.AddRange(igo);
        public void DeregisterGameObj(GameObject igo) => _registry.Remove(igo);
        public abstract void Init();
        public abstract void UpdateLoop();
        public abstract void RenderLoop();

        public virtual void Dispose()
        {
        }
    }
}