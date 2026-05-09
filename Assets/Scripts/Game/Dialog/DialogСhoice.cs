using Scripts.Game.Dialog.DialogActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Game.Dialog
{
    [Serializable]
    public class DialogСhoice
    {
        public string ID;
        public string Text;
        public string NextNodeID;
        public List<IDialogAction> Actions;
    }
}
