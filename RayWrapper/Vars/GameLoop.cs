using System;
using System.Collections.Generic;
using System.Linq;

namespace RayWrapper.Vars
{
    public abstract class GameLoop : GameObjReg
    {
        public bool focusOnScenes;
        private List<Scene> _scenes = new();
        private int _currentScene = -1;

        public void Update()
        {
            if (_currentScene >= _scenes.Count) _currentScene = _scenes.Count - 1;
            if (_currentScene > -1) _scenes[_currentScene].Update();
            if (focusOnScenes && _scenes.Any() && _currentScene != -1) return;
            UpdateLoop();
            UpdateReg();
        }

        public void Render()
        {
            if (_currentScene >= _scenes.Count) _currentScene = _scenes.Count - 1;
            if (_currentScene > -1) _scenes[_currentScene].Render();
            if (focusOnScenes && _scenes.Any() && _currentScene != -1) return;
            RenderLoop();
            RenderReg();
        }

        public void AddScene(Scene scene) => _scenes.Add(scene);

        public void InsertScene(Scene scene, int index)
        {
            if (_currentScene >= index) _currentScene++;
            _scenes.Insert(index , scene);
        }

        public void ChangeScene(int scene)
        {
            if (!_scenes.Any() || scene == _currentScene) return;
            _currentScene = Math.Clamp(scene, 0, _scenes.Count - 1);
        }

        public abstract void Init();
        public abstract void UpdateLoop();
        public abstract void RenderLoop();

        public virtual void Dispose()
        {
        }
    }
}