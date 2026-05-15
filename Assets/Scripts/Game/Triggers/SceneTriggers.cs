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
        [SerializeField, Sirenix.OdinInspector.ListDrawerSettings(
        DraggableItems = true,
        ShowIndexLabels = true,
        NumberOfItemsPerPage = 5
        )] private List<Trigger> _triggers;

        public IReadOnlyList<Trigger> Triggers => _triggers;

        private void Start()
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.EnableOnStart)
                {
                    trigger.Enable();
                }
                
                if (trigger.CanRun is null)
                    trigger.CanRun += RunTrigger;
            }
        }

        public void SetTriggers(List<Trigger> triggers)
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.Enabled)
                {
                    trigger.Disable();
                }

                trigger.CanRun -= RunTrigger;
            }

            for (int i = 0; i < _triggers.Count; i++)
            {
                var trigger = _triggers[i];
                var currentTrigger = triggers.FirstOrDefault(t => t.GUID == trigger.GUID);

                if (currentTrigger is null)
                {
                    _triggers.Remove(trigger);
                    
                    i--;

                    continue;
                }

                trigger.UpdateData(currentTrigger);

                if (currentTrigger.Enabled)
                {
                    trigger.Enable();
                }

                trigger.CanRun += RunTrigger;
            }
        }

        private void RunTrigger(Trigger trigger)
        {
            trigger?.Run();

            if (!trigger.PlayOnce)
                return;

            trigger.CanRun -= RunTrigger;
            
            _triggers.Remove(trigger);
        }
    }
}
