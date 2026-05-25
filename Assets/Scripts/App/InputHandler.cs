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
        private bool _enablePlayer = true;
        private bool _enableUI = true;
        private bool _enableGame = true;

        public bool EnablePlayer
        {
            set
            {
                _enablePlayer = value;
                UpdateEnables();
            }

            get { return _enablePlayer; }
        }

        public bool EnableUI
        {
            set
            {
                _enableUI = value;
                UpdateEnables();
            }

            get { return _enableUI; }
        }

        public bool EnableGame
        {
            set
            {
                _enableGame = value;
                UpdateEnables();
            }
            
            get { return _enableGame; }
        }

        // Player
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action OnInteract;
        public event Action OnCrouchStart;
        public event Action OnCrouchStop;
        public event Action OnSprintStart;
        public event Action OnSprintStop;
        public event Action OnFlashlight;
        public event Action OnAction;
        public event Action OnExtraAction;

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

            UpdateEnables();
        }

        private void UpdateEnables()
        {
            // Player
            if (EnablePlayer)
                SubscribePlayer();
            else
                UnsubscribePlayer();

            // UI
            if (EnableUI)
                SubscribeUI();
            else
                UnsubscribeUI();

            // Game
            if (EnableGame)
                SubscribeGame();
            else
                UnsubscribeGame();
        }

        private void SubscribePlayer()
        {
            _actionAsset.Player.Move.performed += OnMovePerformed;
            _actionAsset.Player.Move.canceled += OnMoveCanceled;

            _actionAsset.Player.Look.performed += OnLookPerformed;

            _actionAsset.Player.Interact.performed += OnInteractPerformed;
            
            _actionAsset.Player.Inventory.performed += OnInventoryPerformed;

            _actionAsset.Player.Flashlight.performed += OnFlashlightPerformed;
            
            _actionAsset.Player.Action.started += OnActionPerformed;
            _actionAsset.Player.ExtraAction.started += OnExtraActionPerformed;

            _actionAsset.Player.Crouch.started += OnCrouchStarted;
            _actionAsset.Player.Crouch.canceled += OnCrouchCanceled;

            _actionAsset.Player.Sprint.started += OnSprintStarted;
            _actionAsset.Player.Sprint.canceled += OnSprintCanceled;
        }

        private void UnsubscribePlayer()
        {
            _actionAsset.Player.Move.performed -= OnMovePerformed;
            _actionAsset.Player.Move.canceled -= OnMoveCanceled;

            _actionAsset.Player.Look.performed -= OnLookPerformed;

            _actionAsset.Player.Interact.performed -= OnInteractPerformed;
            
            _actionAsset.Player.Inventory.performed -= OnInventoryPerformed;

            _actionAsset.Player.Flashlight.performed -= OnFlashlightPerformed;
            
            _actionAsset.Player.Action.started -= OnActionPerformed;
            _actionAsset.Player.ExtraAction.started -= OnExtraActionPerformed;

            _actionAsset.Player.Crouch.started -= OnCrouchStarted;
            _actionAsset.Player.Crouch.canceled -= OnCrouchCanceled;

            _actionAsset.Player.Sprint.started -= OnSprintStarted;
            _actionAsset.Player.Sprint.canceled -= OnSprintCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext callback)
        {
            OnMove?.Invoke(callback.ReadValue<Vector2>());
        }

        private void OnMoveCanceled(InputAction.CallbackContext callback)
        {
            OnMove?.Invoke(Vector2.zero);
        }

        private void OnLookPerformed(InputAction.CallbackContext callback)
        {
            OnLook?.Invoke(callback.ReadValue<Vector2>());
        }

        private void OnInteractPerformed(InputAction.CallbackContext callback)
        {
            OnInteract?.Invoke();
        }

        private void OnFlashlightPerformed(InputAction.CallbackContext callback)
        {
            OnFlashlight?.Invoke();
        }
        
        private void OnActionPerformed(InputAction.CallbackContext callback)
        {
            OnAction?.Invoke();
        }
        
        private void OnExtraActionPerformed(InputAction.CallbackContext callback)
        {
            OnExtraAction?.Invoke();
        }

        private void OnCrouchStarted(InputAction.CallbackContext callback)
        {
            OnCrouchStart?.Invoke();
        }

        private void OnCrouchCanceled(InputAction.CallbackContext callback)
        {
            OnCrouchStop?.Invoke();
        }

        private void OnSprintStarted(InputAction.CallbackContext callback)
        {
            OnSprintStart?.Invoke();
        }

        private void OnSprintCanceled(InputAction.CallbackContext callback)
        {
            OnSprintStop?.Invoke();
        }

        // UI подписка и отписка
        private void SubscribeUI()
        {
            _actionAsset.UI.Submit.performed += OnSubmitPerformed;
            _actionAsset.UI.Cancel.performed += OnCancelPerformed;
            _actionAsset.UI.Next.performed += OnNextPerformed;
            _actionAsset.UI.Previous.performed += OnPreviousPerformed;
        }

        private void UnsubscribeUI()
        {
            _actionAsset.UI.Submit.performed -= OnSubmitPerformed;
            _actionAsset.UI.Cancel.performed -= OnCancelPerformed;
            _actionAsset.UI.Next.performed -= OnNextPerformed;
            _actionAsset.UI.Previous.performed -= OnPreviousPerformed;
        }

        // Game подписка и отписка
        private void SubscribeGame()
        {
            _actionAsset.Game.Pause.performed += OnPausePerformed;
        }

        private void UnsubscribeGame()
        {
            _actionAsset.Game.Pause.performed -= OnPausePerformed;
        }

        // Обработчики UI
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            OnSubmit?.Invoke();
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            OnCancel?.Invoke();
        }

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            OnNext?.Invoke();
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            OnPrevious?.Invoke();
        }

        // Обработчики Game
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            OnPause?.Invoke();
        }

        private void OnInventoryPerformed(InputAction.CallbackContext context)
        {
            OnInventory?.Invoke();
        }
    }
}