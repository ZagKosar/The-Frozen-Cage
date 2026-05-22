using Scripts.Events.Preview;
using Scripts.UI;
using Scripts.WindowSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Game.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Windows.Inventory
{
    public class InventoryWindow : WindowPanel
    {
        [SerializeField] private DragbleUIElement _previewImage;
        [SerializeField] private Transform _tempModel;
        [SerializeField] private TMP_Text _nameTMP;
        [SerializeField] private TMP_Text _descriptionTMP;
        [SerializeField] private TMP_Text _useTMP;

        private List<InventoryItem> _items;
        private int _currentIndex = 0;
        private bool _isPlaying = false;

        public override int Priority => 2;

        public override void Load()
        {
        }

        public override void Destroy()
        {
        }

        public override void Open(object context = null)
        {
            _items = DependencyContainer.Inventory.Items.ToList();
            _currentIndex = 0;

            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnNext += OnNext;
            inputHandler.OnPrevious += OnPrevious;
            inputHandler.OnCancel += Close;
            inputHandler.EnableGame = false;
            inputHandler.EnablePlayer = true;
            inputHandler.EnableUI = true;
            
            _previewImage.Drag += OnDrag;

            ShowCurrentItem();

            gameObject.SetActive(true);
        }

        public override void Close()
        {
            var inputHandler = DependencyContainer.InputHandler;

            if (inputHandler != null)
            {
                inputHandler.OnNext -= OnNext;
                inputHandler.OnPrevious -= OnPrevious;
                inputHandler.OnCancel -= Close;
                inputHandler.EnableGame = true;
                inputHandler.EnablePlayer = true;
                inputHandler.EnableUI = false;
            }

            _previewImage.Drag -= OnDrag;

            gameObject.SetActive(false);
        }

        private void ShowCurrentItem()
        {
            if (_items.Count == 0)
            {
                _nameTMP.text = "";
                _descriptionTMP.text = "";
                return;
            }

            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (!itemsLibrary.TryGetItem(currentItem.Id, out var item))
                return;

            var isUsable = item is UsableItem;
            var inputHandler = DependencyContainer.InputHandler;

            _nameTMP.text = item.Name;
            _descriptionTMP.text = item.Description;

            EventManager.Instance.Invoke(new PreviewEvent.Show() { Model = item.Model });

            _useTMP.gameObject.SetActive(isUsable);

            inputHandler.OnSubmit -= EquipItem;

            if (!isUsable)
                return;

            if (((UsableItem)item).IsEquiped)
            {
                inputHandler.OnSubmit += UnequipItem;
                inputHandler.OnSubmit -= EquipItem;
                
                _useTMP.text = "[F] Снять предмет";
            }
            else
            {
                inputHandler.OnSubmit += EquipItem;
                inputHandler.OnSubmit -= UnequipItem;
                
                _useTMP.text = "[F] Взять предмет";
            }
        }

        private void EquipItem()
        {
            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (!itemsLibrary.TryGetItem(currentItem.Id, out var item) || item is not UsableItem usableItem)
                return;

            usableItem.Pickup();
        }

        private void UnequipItem()
        {
            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (!itemsLibrary.TryGetItem(currentItem.Id, out var item) || item is not UsableItem usableItem)
                return;
            
            usableItem.Unequipe();
        }

        private void OnNext()
        {
            if (_isPlaying)
                return;

            if (_items.Count <= 1)
                return;

            _currentIndex = (_currentIndex + 1) % _items.Count;

            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (itemsLibrary.TryGetItem(currentItem.Id, out var item))
            {
                _nameTMP.text = item.Name;
                _descriptionTMP.text = item.Description;

                EventManager.Instance.Invoke(new PreviewEvent.ShowNext() { NextModel = item.Model });
            }

            _isPlaying = true;
            Invoke(nameof(StopPlaying), 0.5f);
        }

        private void OnPrevious()
        {
            if (_isPlaying)
                return;

            if (_items.Count <= 1)
                return;

            _currentIndex--;

            if (_currentIndex < 0)
                _currentIndex = _items.Count - 1;

            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (itemsLibrary.TryGetItem(currentItem.Id, out var item))
            {
                _nameTMP.text = item.Name;
                _descriptionTMP.text = item.Description;

                EventManager.Instance.Invoke(new PreviewEvent.ShowPrevious() { PreviousModel = item.Model });
            }

            _isPlaying = true;
            Invoke(nameof(StopPlaying), 0.5f);
        }

        private void OnDrag(Vector2 delta)
        {
            EventManager.Instance.Invoke(new PreviewEvent.Drag() { Delta = delta });
        }

        private void StopPlaying()
        {
            _isPlaying = false;
        }
    }
}