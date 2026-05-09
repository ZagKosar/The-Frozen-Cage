using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Windows.Dialog
{
    public class DialogWindowChoice : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _textTMP;

        public Action Choiced;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void SetInfo(string text)
        {
            _textTMP.text = text;
        }

        private void OnClick()
        {
            Choiced?.Invoke();
        }
    }
}
