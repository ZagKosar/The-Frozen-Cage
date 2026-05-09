using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Dialog
{
    [CreateAssetMenu(fileName = "DialogNode", menuName = "Dialog/Node/DialogNode")]
    public class DialogNode : ScriptableObject
    {
        public string ID;
        public string Text;
        public List<DialogСhoice> DialogСhoice;
    }
}
