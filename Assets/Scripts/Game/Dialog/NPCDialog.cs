using Scripts.App.ValueProvider;
using Scripts.Events.Game;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Scripts.Events.Game.DialogEvent;

namespace Scripts.Game.Dialog
{
    public class NPCDialog : Interactable
    {
        [SerializeField] private string _interactDescription;
        [SerializeField, ValueDropdown(nameof(GetIds))] private string _startNodeID;
        [SerializeField] private Animator _animator;

        public override string InteractDescription => _interactDescription;
        public string StartNodeID => _startNodeID;

        private void Start()
        {
            EventManager.Instance.Subscribe<DialogEvent.OpenDialog>(OnOpenDialog);
            EventManager.Instance.Subscribe<DialogEvent.OnChoice>(OnChoice);
            EventManager.Instance.Subscribe<DialogEvent.CloseDialog>(OnCloseDialog);
        }

        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe<DialogEvent.OpenDialog>(OnOpenDialog);
            EventManager.Instance.Unsubscribe<DialogEvent.OnChoice>(OnChoice);
            EventManager.Instance.Unsubscribe<DialogEvent.CloseDialog>(OnCloseDialog);
        }

        public override void Interact()
        {
            EventManager.Instance.Invoke(new DialogEvent.OpenDialog() { NodeID = _startNodeID });
        }

        public void SetStartNode(string nodeID)
        {
            this.enabled = !String.IsNullOrEmpty(nodeID);
            _startNodeID = nodeID;
        }

        private void OnOpenDialog(DialogEvent.OpenDialog data)
        {
            if (data.NodeID != _startNodeID)
                return;
            
            _animator.SetBool("IsTalking", true);
        }

        private void OnChoice(DialogEvent.OnChoice data)
        {
            var dialogSystem = DependencyContainer.DialogSystem;
            if (!dialogSystem.TryGetNode(_startNodeID, out var startNode))
                return;

            var choice = startNode.DialogСhoice.FirstOrDefault(c => c.ID == data.ChoiceID);

            if (choice is null)
                return;

            if (!choice.NextIsStart)
                return;

            SetStartNode(choice.NextNodeID);
        }

        private void OnCloseDialog(DialogEvent.CloseDialog data)
        {
            if (_animator.GetBool("IsTalking"))
                _animator.SetBool("IsTalking", false);
        }

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem<string>> GetIds()
        {
            return DialogIDProvider.GetAllNodeIds();
        }
#endif
    }
}
