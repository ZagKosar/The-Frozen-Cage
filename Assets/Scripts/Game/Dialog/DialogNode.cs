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
        public List<DialogСhoice> DialogСhoice = new();

        private void OnValidate()
        {
            for (int i = 0; i < DialogСhoice.Count; i++)
            {
                var choice = DialogСhoice[i];
                
                if (choice is null)
                    continue;

                var split = ID.Split('_');

                if (split.Length < 2)
                    continue;

                choice.ID = $"{split[0]}Choice_{split[1]}.{i}";
            }
        }
    }
}
