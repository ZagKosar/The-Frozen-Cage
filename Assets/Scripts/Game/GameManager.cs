using Scripts.App;
using Scripts.App.Constants;
using Scripts.Events.Game;
using Scripts.Windows.Dialog;
using System;
using UnityEngine;
using static Scripts.Events.Game.DialogEvent;

namespace Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Player _player;

        private bool _isPaused;
        private GameTime _gameTime;
        private Interactable _currentInteractable;

        private void Start()
        {
            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnPause += OnPause;
            inputHandler.OnInventory += OnInventory;
            inputHandler.OnInteract += OnInteract;

            EventManager.Instance.Subscribe<GameEvent.Pause>(OnPauseEvent);
            EventManager.Instance.Subscribe<GameEvent.InteractHover>(OnInteractHover);
            EventManager.Instance.Subscribe<GameEvent.InteractHoverEnd>(OnInteractHoverEnd);
            EventManager.Instance.Subscribe<GameEvent.AddItem>(OnAddItem);
            EventManager.Instance.Subscribe<DialogEvent.OpenDialog>(OnOpenDialog);
            EventManager.Instance.Subscribe<DialogEvent.CloseDialog>(OnCloseDialog);

            _player.SetInventory(DependencyContainer.Inventory);

            _gameTime = DependencyContainer.GameTime;

            _cameraController.SetMouseLock(true);

            EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.PlayerGUI });
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
                _gameTime.Update(0f);

            _cameraController.SetMouseLock(!_isPaused);

            if (_isPaused)
                EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.PauseWindow });
            else
                EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.PauseWindow });
        }

        private void OnInventory()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
                _gameTime.Update(0f);

            _cameraController.SetMouseLock(!_isPaused);

            if (_isPaused)
                EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.InventoryWindow });
            else
                EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.InventoryWindow });
        }

        private void OnInteract()
        {
            if (_currentInteractable == null)
                return;

            _currentInteractable.Interact();
        }

        private void OnPauseEvent(GameEvent.Pause data)
        {
            OnPause();
        }

        private void OnInteractHover(GameEvent.InteractHover data)
        {
            _currentInteractable = data.Interact;
        }

        private void OnInteractHoverEnd(GameEvent.InteractHoverEnd data)
        {
            _currentInteractable = null;
        }

        private void OnAddItem(GameEvent.AddItem data)
        {
            _player.Inventory.AddItem(data.Id, data.Amount);
        }

        private void OnOpenDialog(DialogEvent.OpenDialog data)
        {
            _isPaused = true;

            _gameTime.Update(0f);

            _cameraController.SetMouseLock(false);

            EventManager.Instance.Invoke(new UIEvents.CloseWindow { Name = Constants.PlayerGUI });
            EventManager.Instance.Invoke(new UIEvents.OpenWindowWithContext { Name = Constants.DialogWindow, Context = new DialogWindowContext() { NodeID = data.NodeID } });
        }

        private void OnCloseDialog(DialogEvent.CloseDialog data)
        {
            _isPaused = false;

            _cameraController.SetMouseLock(true);

            EventManager.Instance.Invoke(new UIEvents.OpenWindow { Name = Constants.PlayerGUI });
            EventManager.Instance.Invoke(new UIEvents.CloseWindow { Name = Constants.DialogWindow });
        }

        private void OnDestroy()
        {
            var inputHandler = DependencyContainer.InputHandler;
            if (inputHandler != null)
            {
                inputHandler.OnPause -= OnPause;
                inputHandler.OnInventory -= OnInventory;
                inputHandler.OnInteract -= OnInteract;
            }

            EventManager.Instance.Unsubscribe<GameEvent.Pause>(OnPauseEvent);
            EventManager.Instance.Unsubscribe<GameEvent.InteractHover>(OnInteractHover);
            EventManager.Instance.Unsubscribe<GameEvent.InteractHoverEnd>(OnInteractHoverEnd);
            EventManager.Instance.Unsubscribe<GameEvent.AddItem>(OnAddItem);
            EventManager.Instance.Unsubscribe<DialogEvent.OpenDialog>(OnOpenDialog);
            EventManager.Instance.Unsubscribe<DialogEvent.CloseDialog>(OnCloseDialog);
        }
    }
}