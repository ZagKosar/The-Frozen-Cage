using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.WindowSwitcher
{
    public abstract class WindowPanel: MonoBehaviour
    {
        public abstract int Priority { get; }

        public abstract void Load();
        public abstract void Destroy();
        public abstract void Open(object context = null);
        public abstract void Close();

    }
}
