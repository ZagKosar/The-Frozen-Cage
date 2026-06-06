using Scripts.App.ValueProvider;
using Scripts.Events.Game;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Conditions.Dialog
{
    [Serializable]
    public class DialogChoiceCondition : ICondition
    {
        
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetIds))]
#endif
        [SerializeField] private string _choiceID;

        public event Action Complete;

        public void Initialize()
        {
            EventManager.Instance.Subscribe<DialogEvent.OnChoice>(OnChoice);
        }

        private void OnChoice(DialogEvent.OnChoice data)
        {
            if (data.ChoiceID != _choiceID)
                return;

            Complete?.Invoke();
        }

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem<string>> GetIds()
        {
            return DialogIDProvider.GetAllChoiceIds();
        }
#endif
    }
}
