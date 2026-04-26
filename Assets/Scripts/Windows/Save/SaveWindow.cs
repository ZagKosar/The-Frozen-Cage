using Scripts.App.Constants;
using Scripts.Events.App;
using Scripts.WindowSwitcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Windows.Save
{
    public class SaveWindow : WindowPanel
    {
        [SerializeField] private SaveSlot _saveSlotPrefab;
        [SerializeField] private Transform _slotsContainer;
        [SerializeField] private Button _closeButton;

        private bool _isSaving;
        private List<SaveSlot> _saveSlots = new List<SaveSlot>();

        public override int Priority => 3;

        public override void Open(object context = null)
        {
            _closeButton.onClick.AddListener(OnClose);

            if (context != null && context is SaveWindowContext saveContext)
            {
                _isSaving = saveContext.IsSaving;
            }

            gameObject.SetActive(true);
            AddSlots();
        }

        public override void Close()
        {
            _closeButton.onClick.RemoveListener(OnClose);

            gameObject.SetActive(false);
            HideSlots();
        }

        public override void Load()
        {
            
        }

        public override void Destroy()
        {
            
        }

        private void AddSlots()
        {
            var savePath = Path.Combine(Application.persistentDataPath, "Save");
            var slotsCount = 5;

            if (Directory.Exists(savePath))
            {
                slotsCount -= Directory.GetFiles(savePath).Length;
            }

            for (int i = 0; i < 5 - slotsCount; i++)
            {
                int index = i;

                SaveSlot slot;

                if (_saveSlots.Count - 1 < i)
                {
                    slot = Instantiate(_saveSlotPrefab, _slotsContainer);
                    
                    _saveSlots.Add(slot);
                }
                else
                {
                    slot = _saveSlots[i];
                    slot.gameObject.SetActive(true);
                }

                slot.UpdateSlot(i + 1, false);
                slot.Click = null;

                if (_isSaving)
                    slot.Click += () => SaveSlot(index);
                else
                    slot.Click += () => LoadSlot(index);
            }

            if (slotsCount > 0 && _isSaving)
            {
                var nextSlot = 5 - slotsCount;

                SaveSlot slot;

                if (_saveSlots.Count - 1 < nextSlot)
                {
                    slot = Instantiate(_saveSlotPrefab, _slotsContainer);

                    _saveSlots.Add(slot);
                }
                else
                {
                    slot = _saveSlots[nextSlot];
                    slot.gameObject.SetActive(true);
                }

                slot.UpdateSlot(nextSlot + 1, true);
                slot.Click = null;
                slot.Click += () => SaveSlot(nextSlot);
            }
        }

        private void SaveSlot(int slot)
        {
            EventManager.Instance.Invoke(new AppEvents.Save() { Slot = slot });

            AddSlots();
        }

        private void LoadSlot(int slot)
        {
            EventManager.Instance.Invoke(new AppEvents.Load() { Slot = slot });
        }

        private void OnClose()
        {
            EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.SaveWindow });
        }

        private void HideSlots()
        {
            foreach (var slot in _saveSlots)
                slot.gameObject.SetActive(false);
        }
    }
}
