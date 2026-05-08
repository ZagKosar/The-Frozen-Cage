using Scripts.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Game
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private Light _flashlight;

        private bool _isFlashlightOn = false;

        private void OnEnable()
        {
            var inputhandler = DependencyContainer.InputHandler;

            inputhandler.OnFlashlight += OnFlashlight;
        }

        private void OnDisable()
        {
            var inputhandler = DependencyContainer.InputHandler;

            inputhandler.OnFlashlight -= OnFlashlight;

            _flashlight.transform.parent = null;
        }

        private void Start()
        {
            var cameraTransform = Camera.main.transform;

            _flashlight.transform.SetParent(cameraTransform, false);
            _flashlight.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        private void OnFlashlight()
        {
            if (_flashlight == null)
                return;

            _isFlashlightOn = !_isFlashlightOn;
            _flashlight.enabled = _isFlashlightOn;
        }
    }
}
