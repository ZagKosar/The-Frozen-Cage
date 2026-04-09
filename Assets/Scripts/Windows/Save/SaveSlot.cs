using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Windows.Save
{
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _textTMP;

        public Action Click;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button?.onClick.RemoveListener(OnClick);
        }

        public void UpdateSlot(int slot, bool isEmpty)
        {
            if (isEmpty)
                _textTMP.text = $"Новое сохранение {slot}";
            else
                _textTMP.text = $"Слот {slot}";
        }

        private void OnClick()
        {
            Click?.Invoke();
        }
    }
}
