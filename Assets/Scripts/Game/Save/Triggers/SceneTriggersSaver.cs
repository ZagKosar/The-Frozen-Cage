using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scripts.Game.Save.Utils;
using Scripts.Game.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Scripts.Game.Save.Triggers
{
    public class SceneTriggersSaver : BaseSaver
    {
        [SerializeField] private SceneTriggers _target;
        [SerializeField] private string _type;
        public override string Key => "SceneTriggers" + _type;

        public override JObject Save()
        {
            var jobject = new JObject();
            var triggersJson = JsonConvert.SerializeObject(_target.Triggers);

            jobject.Add("triggers", triggersJson);

            return jobject;
        }

        public override bool Load(JObject data)
        {
            if (data is null)
            {
                return true;
            }

            if (data.TryGetValue("triggers", out var triggersJson))
            {
                var triggers = JsonConvert.DeserializeObject<List<Trigger>>(triggersJson.ToString());

                _target.SetTriggers(triggers);
            }

            return true;
        }
    }
}
