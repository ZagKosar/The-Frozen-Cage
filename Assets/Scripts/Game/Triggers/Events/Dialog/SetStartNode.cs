using Scripts.App.ValueProvider;
using Scripts.Game.Dialog;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Events.Dialog
{
    [Serializable]
    public class SetStartNode : ITriggerEvent
    {
        [SerializeField] private NPCDialog _npc;
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetIds))]
#endif
        [SerializeField] private string _nodeID;

        public void Run()
        {
            _npc.SetStartNode(_nodeID);
        }

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem<string>> GetIds()
        {
            return DialogIDProvider.GetAllNodeIds();
        }
#endif
    }
}
