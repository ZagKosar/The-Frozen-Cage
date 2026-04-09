using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Events.App
{
    public class AppEvents
    {
        public struct Save
        {
            public int Slot;
        }

        public struct Load
        {
            public int Slot;
        }
    }
}
