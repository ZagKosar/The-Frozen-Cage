using UnityEngine;

namespace Scripts.Game.Items
{
    public abstract class UsableItem
    {
        [SerializeField] protected  int _id;
        [SerializeField] protected string _name;
        [SerializeField] protected string _description;
        [SerializeField] protected GameObject _model;
        
        public abstract void Pickup();
        public abstract void Use();
        public abstract void AltUse();
    }
}