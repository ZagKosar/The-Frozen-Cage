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
        
        public abstract void Pickup();
        public abstract void Unequipe();
        public abstract void Use();
        public abstract void AltUse();
    }
}