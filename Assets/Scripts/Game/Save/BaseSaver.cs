using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Save
{
    public abstract class BaseSaver : MonoBehaviour
    {
        public abstract string Key { get; }

        public abstract JObject Save();

        public abstract bool Load(JObject data);
    }
}
