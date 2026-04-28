using Newtonsoft.Json;
using Scripts.Events.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Triggers.Events
{
    [Serializable]
    public class InnerDialogueTriggerEvent : ITriggerEvent
    {
        [SerializeField, JsonIgnore] private string _text;

        public void Run()
        {
            EventManager.Instance.Invoke(new GameEvent.InnerDialogue() { Text = _text });
        }
    }
}
