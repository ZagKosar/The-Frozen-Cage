using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.Save.Items
{
    public class DisappearObjectSaver : BaseSaver
    {
        [SerializeField] private GameObject _object;
        [SerializeField] private string _uniqId;

        public override string Key => "DisappearObject_" + _uniqId;

        public override JObject Save()
        {
            var jobject = new JObject();
            return jobject;
        }

        public override bool Load(JObject data)
        {
            if (data == null)
            {
                Destroy(_object.gameObject);
            }

            return true;
        }

#if UNITY_EDITOR
        [Button]
        private void GenerationUID()
        {
            _uniqId = Guid.NewGuid().ToString();
        }
#endif
    }
}
