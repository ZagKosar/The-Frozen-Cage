using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Save.Items
{
    public class InteractableItemSaver : BaseSaver
    {
        [SerializeField] private Interactable _item;
        [SerializeField] private string _uniqId = Guid.NewGuid().ToString();

        public override string Key => "InteractableItem_" + _uniqId;

        public override JObject Save()
        {
            var jobject = new JObject();
            return jobject;
        }

        public override bool Load(JObject data)
        {
            if (data == null)
            {
                Destroy(_item.gameObject);
            }

            return true;
        }
    }
}
