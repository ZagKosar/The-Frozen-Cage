using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Conditions
{
    [Serializable]
    public class OnTriggerEnterCondition : ICondition
    {
        [SerializeField, JsonIgnore] private ColliderDetector _detector;
        [SerializeField, JsonIgnore] private string _tag;
        
        public event Action Complete;

        public void Initialize()
        {
            _detector.TriggerEnter += OnTriggerEnter;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != _tag)
                return;

            Complete?.Invoke();
        }
    }
}
