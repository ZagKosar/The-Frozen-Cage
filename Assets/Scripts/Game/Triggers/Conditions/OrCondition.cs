using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Conditions
{
    public class OrCondition : ICondition
    {
        [SerializeReference] private List<ICondition> _conditions = new();

        public event Action Complete;

        public void Initialize()
        {
            foreach (var condition in _conditions)
            {
                if (condition == null)
                {
                    continue;
                }

                condition.Initialize();
                condition.Complete += OnComplete;
            }
        }

        private void OnComplete()
        {
            foreach (var condition in _conditions)
            {
                if (condition == null)
                {
                    continue;
                }

                condition.Complete -= OnComplete;
            }

            Complete?.Invoke();
        }
    }
}
