using Scripts.App.ValueProvider;
using Scripts.Game.Dialog.DialogActions;
using Sirenix.OdinInspector;
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
        [ValueDropdown(nameof(GetIds))] public string NextNodeID;
        public bool NextIsStart;
        public List<IDialogAction> Actions;

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem<string>> GetIds()
        {
            return DialogIDProvider.GetAllNodeIds();
        }
#endif
    }
}
