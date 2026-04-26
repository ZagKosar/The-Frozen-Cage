using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Conditions
{
    [Serializable]
    public class ItemPickedUp : ICondition
    {
        [SerializeField] private PickableItem _target;

        public event Action Complete;

        public void Initialize()
        {
            _target.PickedUp += OnTargetPickedUp; 
        }

        private void OnTargetPickedUp()
        {
            Complete?.Invoke();

            _target.PickedUp -= OnTargetPickedUp;
        }
    }
}
