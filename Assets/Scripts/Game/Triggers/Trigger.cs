using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Game.Triggers
{
    [Serializable]
    [Sirenix.OdinInspector.InlineProperty]
    [Sirenix.OdinInspector.HideLabel]
    public class Trigger
    {
        [SerializeField, JsonProperty] private string _guid;
        [SerializeReference, JsonIgnore] private List<ICondition> _conditions = new();
        [SerializeReference, JsonIgnore] private List<ITriggerEvent> _triggerEvents = new();

        [SerializeField, JsonProperty] private bool _enableOnStart = false;
        [SerializeField, JsonProperty] private bool _playOnce = true;

        [JsonIgnore] public string GUID => _guid;
        [JsonIgnore] public bool EnableOnStart => _enableOnStart;
        [JsonIgnore] public bool PlayOnce => _playOnce;
        [JsonIgnore] public bool Enabled => _enabled;

        public Action<Trigger> CanRun;

        [JsonProperty] private bool _enabled = false;
        [JsonProperty] private int _conditionCompleteCount = 0;

        public void UpdateData(Trigger trigger)
        {
            _conditionCompleteCount = trigger._conditionCompleteCount;
        }

        public void Enable()
        {
            foreach (var condition in _conditions)
            {
                condition.Initialize();
                condition.Complete += OnConditionComplete;
            }

            _enabled = true;
        }

        public void Disable()
        {
            foreach (var condition in _conditions)
            {
                condition.Complete -= OnConditionComplete;
            }

            _enabled = false;
        }

        public void Run()
        {
            foreach (var triggerEvent in _triggerEvents)
            {
                triggerEvent.Run();
            }
        }

        private void OnConditionComplete()
        {
            _conditionCompleteCount++;

            if (_conditionCompleteCount != _conditions.Count)
                return;

            CanRun?.Invoke(this);
        }

#if UNITY_EDITOR
        [Button]
        private void GenerateGUID()
        {
            _guid = Guid.NewGuid().ToString();
        }
#endif
    }
}
