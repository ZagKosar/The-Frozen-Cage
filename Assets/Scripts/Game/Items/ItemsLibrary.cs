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
        [SerializeField] private List<Item> _items;

        public IReadOnlyList<Item> Items => _items;

        private Dictionary<int, Item> _itemsDictionary = new();

        public void Initialize()
        {
            foreach (var item in _items)
                _itemsDictionary[item.Id] = item;
        }

        public bool TryGetItem(int id, out Item item)
        {
            return _itemsDictionary.TryGetValue(id, out item);
        }
    }

    [Serializable]
    public class Item
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Transform _model;

        public int Id => _id; 
        public string Name => _name;
        public string Description => _description;
        public Transform Model => _model;

    }
}
