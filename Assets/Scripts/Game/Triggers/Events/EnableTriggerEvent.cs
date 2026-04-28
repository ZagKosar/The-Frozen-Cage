using Newtonsoft.Json;
using Sirenix.OdinInspector;
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
        [SerializeReference, ValueDropdown("GetAllTriggers"), JsonIgnore] private string _target;
        [SerializeField, JsonIgnore] private SceneTriggers _sceneTriggers;

        public void Run()
        {
            var currentTrigger = _sceneTriggers.Triggers.FirstOrDefault(t => t.GUID == _target);

            if (currentTrigger == null)
                return;

            currentTrigger.Enable();
        }

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem<string>> GetAllTriggers()
        {
            if (_sceneTriggers == null || _sceneTriggers.Triggers == null)
                return new List<ValueDropdownItem<string>>();
            
            var list = new List<ValueDropdownItem<string>>();

            for (int i = 0; i < _sceneTriggers.Triggers.Count; i++)
            {
                var item = _sceneTriggers.Triggers[i];
                var autoPrefix = item.EnableOnStart ? "(auto)" : "";
                var name = item != null ? $"Trigger {item.GUID} {autoPrefix}":$"Trigger {i} (null)";
                
                list.Add(new ValueDropdownItem<string>(name, item.GUID));
            }

            return list;
        }
#endif

    }
}
