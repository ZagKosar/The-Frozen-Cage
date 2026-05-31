using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Items
{
    [Serializable, CreateAssetMenu(fileName = "ItemsLibrary", menuName = "Libraries/ItemsLibrary")]
    public class ItemsLibrary : ScriptableObject
    {
        [SerializeReference] private List<Item> _items = new();

        public IReadOnlyList<Item> Items => _items;

        private Dictionary<int, Item> _itemsDictionary = new();

        public void Initialize()
        {
            foreach (var item in _items)
            {
                _itemsDictionary[item.Id] = item;
                
                if (item is not UsableItem usableItem)
                    continue;
                
                usableItem.Initialize();
            }
        }

        public bool TryGetItem(int id, out Item item)
        {
            return _itemsDictionary.TryGetValue(id, out item);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var index = 0; index < _items.Count; index++)
            {
                var item = _items[index];
                
                item.SetID(index);
            }
        }
#endif
    }
}
