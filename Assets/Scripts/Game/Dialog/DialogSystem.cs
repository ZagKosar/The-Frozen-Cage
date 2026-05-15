using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Dialog
{
    [Serializable]
    public class DialogSystem
    {
        [SerializeField] private List<DialogNode> _nodes;

        private Dictionary<string, DialogNode> _nodesDict = new();
        private Dictionary<string, DialogСhoice> _choiceDict = new();

        public void Initialize()
        {
            foreach (var node in _nodes)
            {
                _nodesDict[node.ID] = node;

                foreach (var choice in node.DialogСhoice)
                    _choiceDict[choice.ID] = choice;
            }
        }

        public bool TryGetNode(string id, out DialogNode node)
        {
            return _nodesDict.TryGetValue(id, out node);
        }

        public bool TryGetChoice(string id, out DialogСhoice choice)
        {
            return _choiceDict.TryGetValue(id, out choice);
        }
    }
}
