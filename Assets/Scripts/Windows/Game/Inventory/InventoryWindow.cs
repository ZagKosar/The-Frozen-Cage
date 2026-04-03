using Scripts.Events.Preview;
using Scripts.UI;
using Scripts.WindowSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Scripts.Windows.Inventory
{
    public class InventoryWindow : WindowPanel
    {
        [SerializeField] private DragbleUIElement _previewImage;
        [SerializeField] private Transform _tempModel;
        [SerializeField] private TMP_Text _nameTMP;
        [SerializeField] private TMP_Text _descriptionTMP;

        private List<InventoryItem> _items;
        private int _currentIndex = 0;

        public override void Load()
        {

        }

        public override void Destroy()
        {
            
        }

        public override void Open()
        {
            _items = DependencyContainer.Inventory.Items.ToList();

            _currentIndex = 0;

            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnNext += OnNext;
            inputHandler.OnPrevious += OnPrevious;

            _previewImage.Drag += OnDrag;

            if (_items.Count > 0)
            {
                var itemsLibrary = DependencyContainer.ItemsLibrary;
                var currentItem = _items[_currentIndex];
                
                if (itemsLibrary.TryGetItem(currentItem.Id, out var item))
                {
                    _nameTMP.text = item.Name;
                    _descriptionTMP.text = item.Description;

                    EventManager.Instance.Invoke(new PreviewEvent.Show() { Model = item.Model });
                }
            }

            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnNext()
        {
            if (_items.Count == 0 || _items.Count == 1)
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
        }

        private void OnPrevious()
        {
            if (_items.Count == 0 || _items.Count == 1)
                return;
            _currentIndex--;

            if (_currentIndex == -1)
                _currentIndex = _items.Count - 1;

            var itemsLibrary = DependencyContainer.ItemsLibrary;
            var currentItem = _items[_currentIndex];

            if (itemsLibrary.TryGetItem(currentItem.Id, out var item))
            {
                _nameTMP.text = item.Name;
                _descriptionTMP.text = item.Description;

                EventManager.Instance.Invoke(new PreviewEvent.ShowPrevious() { PreviousModel = item.Model });
            }
        }

        private void OnDrag(Vector2 delta)
        {
            EventManager.Instance.Invoke(new PreviewEvent.Drag() { Delta = delta });
        }
    }
}
