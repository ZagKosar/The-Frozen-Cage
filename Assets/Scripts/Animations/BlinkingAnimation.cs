using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Animations
{
    public class BlinkingAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _object;
        [SerializeField] private float _onDelay;
        [SerializeField] private float _offDelay;

        [SerializeField] private bool _playOnStart;
        [SerializeField] private bool _startOn;

        private void Start()
        {
            if (!_playOnStart)
                return;

            if (_startOn)
                OnObject();
            else
                OffObject();
        }

        private void OnObject()
        {
            _object.SetActive(true);

            Invoke(nameof(OffObject), _onDelay);
        }

        private void OffObject()
        {
            _object.SetActive(false);

            Invoke(nameof(OnObject), _offDelay);
        }
    }
}
