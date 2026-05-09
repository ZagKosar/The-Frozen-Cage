using Scripts.Events.Game;
using Scripts.Game.Dialog;
using Scripts.WindowSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Scripts.Windows.Dialog
{
    public class DialogWindow : WindowPanel
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Transform _container;
        [SerializeField] private DialogWindowChoice _choicePrefab;
        
        private List<DialogWindowChoice> _choices = new();

        public override int Priority => 2;

        public override void Open(object context = null)
        {
            if (context == null || context is not DialogWindowContext dialogWindowContext)
                throw new ArgumentNullException("Контекст не может быть null.");

            var dialogSystem = DependencyContainer.DialogSystem;

            if (!dialogSystem.TryGetNode(dialogWindowContext.NodeID, out var node))
                throw new Exception($"Нода с id-{dialogWindowContext.NodeID} не найдена");

            UpdateWindow(node);
        }

        public override void Close()
        {
            
        }

        public override void Load()
        {
            
        }

        public override void Destroy()
        {
            
        }

        private void UpdateWindow(DialogNode node)
        {
            for (var i = 0; i < node.DialogСhoice.Count; i++)
            {
                var choice = GetChoice(i);
                var dialogChoice = node.DialogСhoice[i];

                choice.SetInfo(dialogChoice.Text);
                choice.Choiced = null;
                choice.Choiced += () => OnChoice(dialogChoice);
                choice.gameObject.SetActive(true);
            }

            for (var i = node.DialogСhoice.Count; i < _choices.Count; i++)
            {
                var choice = _choices[i];
                choice.gameObject.SetActive(false);
            }
        }

        private void OnChoice(DialogСhoice choice)
        {
            EventManager.Instance.Invoke(new DialogEvent.OnChoice { ChoiceID = choice.ID });
            
            var dialogSystem = DependencyContainer.DialogSystem;

            if (String.IsNullOrEmpty(choice.NextNodeID) || !dialogSystem.TryGetNode(choice.NextNodeID, out var node))
            {
                Close();
                return;
            }

            UpdateWindow(node);
        }

        private DialogWindowChoice GetChoice(int index)
        {
            DialogWindowChoice choice;

            if (index >= _choices.Count)
            {
                choice = Instantiate(_choicePrefab, _container);
            }
            else
            {
                choice = _choices[index];
            }

            return choice;
        }
    }
}
