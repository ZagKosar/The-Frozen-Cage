using Scripts.Events.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Scripts.Game.Dialog
{
    public class NPCDialog : Interactable
    {
        [SerializeField] private string _interactDescription;
        [SerializeField] private string _startNodeID;

        public override string interactDescription => _interactDescription;

        public override void Interact()
        {
            EventManager.Instance.Invoke(new DialogEvent.OpenDialog() { NodeID = _startNodeID });
        }
    }
}
