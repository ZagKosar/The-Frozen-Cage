using UnityEngine;

namespace Scripts.Game.Items
{
    public abstract class UsableItem : Item
    {
        public abstract bool IsEquiped
        {
            get;
            set;
        }
        
        public virtual void Initialize(){}
        public abstract void Pickup();
        public abstract void Unequipe();
        public abstract void Use();
        public abstract void AltUse();
    }
}