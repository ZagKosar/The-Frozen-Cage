using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class YesNoPopup : MonoBehaviour
    {
        [SerializeField] private Button _yesBtn;
        [SerializeField] private Button _noBtn;

        public event Action<bool> Result;

        private void OnEnable()
        {
            _yesBtn.onClick.AddListener(OnYes);
            _noBtn.onClick.AddListener(OnNo);
        }

        private void OnDisable()
        {
            _yesBtn.onClick.RemoveListener(OnYes);
            _noBtn.onClick.RemoveListener(OnNo);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnYes()
        {
            Result?.Invoke(true);
        }

        private void OnNo()
        {
            Result?.Invoke(false);
        }
    }
}
