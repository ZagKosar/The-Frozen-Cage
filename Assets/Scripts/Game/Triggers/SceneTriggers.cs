using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers
{
    public class SceneTriggers : MonoBehaviour
    {
        [SerializeField] private List<Trigger> _triggers;

        public IReadOnlyList<Trigger> Triggers => _triggers;

        private void Start()
        {
            foreach (var trigger in _triggers)
                if (trigger.EnableOnStart)
                    trigger.Enable();
        }
    }
}
