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

        public void Initialize()
        {
            foreach (var node in _nodes)
            {
                _nodesDict[node.ID] = node;
            }
        }

        public bool TryGetNode(string id, out DialogNode node)
        {
            return _nodesDict.TryGetValue(id, out node);
        }
    }
}
