using System;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Game.Triggers
{
    [Serializable]
    public class Trigger
    {
        [SerializeReference] private List<ICondition> _conditions = new();
        [SerializeReference] private List<ITriggerEvent> _triggerEvents = new();

        [SerializeField] private bool _enableOnStart = false;

        public bool EnableOnStart => _enableOnStart;

        private int _conditionCompleteCount = 0;

        public void Enable()
        {
            foreach (var condition in _conditions)
            {
                condition.Initialize();
                condition.Complete += OnConditionComplete;
            }
        }

        private void OnConditionComplete()
        {
            _conditionCompleteCount++;

            if (_conditionCompleteCount != _conditions.Count)
                return;

            foreach (var triggerEvent in _triggerEvents)
            {
                triggerEvent.Run();
            }
        }
    }
}
