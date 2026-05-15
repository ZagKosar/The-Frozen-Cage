using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scripts.Game.Dialog;
using Scripts.Game.Save;
using Scripts.Game.Triggers;
using UnityEngine;

namespace Game.Save.Dialog
{
    public class NPCDialogSaver : BaseSaver
    {
        [SerializeField] private NPCDialog _npcDialog;
        public override string Key => "NPCDialog" + _npcDialog.gameObject.name;
        public override JObject Save()
        {
            var jobject = new JObject();
            var startNodeJson = JsonConvert.SerializeObject(_npcDialog.StartNodeID);

            jobject.Add("startNode", startNodeJson);

            return jobject;
        }

        public override bool Load(JObject data)
        {
            if (data is null)
            {
                return true;
            }

            if (data.TryGetValue("startNode", out var startNodeJson))
            {
                var startNode = JsonConvert.DeserializeObject<string>(startNodeJson.ToString());

                _npcDialog.SetStartNode(startNode);
            }

            return true;
        }
    }
}