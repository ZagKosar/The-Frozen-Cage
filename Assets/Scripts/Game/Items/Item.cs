using System;
using UnityEngine;

namespace Scripts.Game.Items
{
    [Serializable]
    public class Item
    {
        [SerializeField] protected int _id;
        [SerializeField] protected string _name;
        [SerializeField] protected string _description;
        [SerializeField] protected Transform _model;

        public int Id => _id; 
        public string Name => _name;
        public string Description => _description;
        public Transform Model => _model;
        
#if UNITY_EDITOR
        public void SetID(int id)
        {
            _id = id;
        }
#endif
    }
}