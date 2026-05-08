using Scripts.Events.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Game.Triggers.Events
{
    [Serializable]
    public class LoadNextSceneTriggerEvent : ITriggerEvent
    {
        public void Run()
        {
            EventManager.Instance.Invoke(new GameEvent.LoadNextScene());
        }
    }
}
