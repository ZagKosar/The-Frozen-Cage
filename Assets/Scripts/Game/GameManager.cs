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
            _gameTime = DependencyContainer.GameTime;

            _cameraController.SetMouseLock(true);
        }

        private void Update()
        {
            if (_isPaused) return;

            _gameTime.Update(Time.deltaTime);
        }
    }
}
