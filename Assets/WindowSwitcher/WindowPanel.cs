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
        public abstract void Open();
        public abstract void Close();

    }
}
