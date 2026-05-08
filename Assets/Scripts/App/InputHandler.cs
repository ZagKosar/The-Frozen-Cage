using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.App
{
    public class InputHandler : MonoBehaviour
    {
        private InputSystemActions _actionAsset;

        // Player
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;

        public event Action OnInteract;
        public event Action OnCrouchStart;
        public event Action OnCrouchStop;
        public event Action OnSprintStart;
        public event Action OnSprintStop;
        public event Action OnFlashlight;

        // UI
        public event Action OnSubmit;
        public event Action OnCancel;
        public event Action OnNext;
        public event Action OnPrevious;

        // Game
        public event Action OnPause;
        public event Action OnInventory;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Initialize()
        {
            _actionAsset = new();

#if UNITY_EDITOR
            _actionAsset.Game.Pause.ApplyBindingOverride("<Keyboard>/v", path: "<Keyboard>/escape");
            //_actionAsset.Player.Interact.ApplyBindingOverride("<Keyboard>/c", path: "<Keyboard>/f");
#endif

            _actionAsset.Enable();

            // Player
            _actionAsset.Player.Move.performed += callback => OnMove?.Invoke(callback.ReadValue<Vector2>());
            _actionAsset.Player.Move.canceled += callback => OnMove?.Invoke(Vector2.zero);

            _actionAsset.Player.Look.performed += callback => OnLook?.Invoke(callback.ReadValue<Vector2>());

            _actionAsset.Player.Interact.performed += _ => OnInteract?.Invoke();

            _actionAsset.Player.Flashlight.performed += _ => OnFlashlight?.Invoke();

            _actionAsset.Player.Crouch.started += _ => OnCrouchStart?.Invoke();
            _actionAsset.Player.Crouch.canceled += _ => OnCrouchStop?.Invoke();

            _actionAsset.Player.Sprint.started += _ => OnSprintStart?.Invoke();
            _actionAsset.Player.Sprint.canceled += _ => OnSprintStop?.Invoke();

            // UI
            _actionAsset.UI.Submit.performed += _ => OnSubmit?.Invoke();

            _actionAsset.UI.Cancel.performed += _ => OnCancel?.Invoke();

            _actionAsset.UI.Next.performed += _ => OnNext?.Invoke();

            _actionAsset.UI.Previous.performed += _ => OnPrevious?.Invoke();

            // Game

            _actionAsset.Game.Pause.performed += _ => OnPause?.Invoke();
            _actionAsset.Game.Inventory.performed += _ => OnInventory?.Invoke();
        }
    }
}
