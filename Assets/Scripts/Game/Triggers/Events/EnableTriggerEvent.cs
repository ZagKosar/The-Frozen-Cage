using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Events
{
    [Serializable]
    public class EnableTriggerEvent : ITriggerEvent
    {
        //[SerializeReference] private Trigger _target;
        [SerializeField] private SceneTriggers _sceneTriggers;
        [SerializeField] private int _id;

        public void Run()
        {
            var trigger = _sceneTriggers.Triggers[_id];

            trigger.Enable();
        }
    }
}
