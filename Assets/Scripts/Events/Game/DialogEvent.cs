using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Events.Game
{
    public static class DialogEvent
    {
        public struct OpenDialog
        {
            public string NodeID;
        }

        public struct OnChoice
        {
            public string ChoiceID;
        }

        public struct CloseDialog
        {

        }
    }
}
