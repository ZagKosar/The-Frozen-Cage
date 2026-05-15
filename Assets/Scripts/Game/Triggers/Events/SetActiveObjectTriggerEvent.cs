using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Events
{
    [Serializable]
    public class SetActiveObjectTriggerEvent : ITriggerEvent
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _isActive;

        public void Run()
        {
            _gameObject.SetActive(_isActive);
        }
    }
}
