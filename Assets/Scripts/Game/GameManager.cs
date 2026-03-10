using Scripts.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;

        private bool _isPaused;
        private GameTime _gameTime;

        private void Start()
        {
            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnPause += OnPause;
            
            _gameTime = DependencyContainer.GameTime;

            _cameraController.SetMouseLock(true);
        }

        private void Update()
        {
            if (_isPaused) return;

            _gameTime.Update(Time.deltaTime);
        }

        private void OnPause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
                EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = "PauseMenu" });
            else
                EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = "PauseMenu" });
        }
    }
}
