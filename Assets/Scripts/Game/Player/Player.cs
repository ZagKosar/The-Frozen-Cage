using UnityEngine;

namespace Scripts.Game
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;

        public Inventory Inventory => _inventory;

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }
    }
}