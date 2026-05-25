using Scripts.Events.Game;
using Scripts.Game.Items;
using UnityEngine;

namespace Scripts.Game
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        
        private UsableItem _currentItem;

        public Inventory Inventory => _inventory;
        
        public void OnEnable()
        {
            var inputHandler = DependencyContainer.InputHandler;
            inputHandler.OnAction += OnAction;
            inputHandler.OnExtraAction += OnExtraAction;
            
            EventManager.Instance.Subscribe<GameEvent.OnPlayerItemEquip>(OnPlayerItemEquip);
            EventManager.Instance.Subscribe<GameEvent.OnPlayerItemUnEquip>(OnPlayerItemUnEquip);
        }

        private void OnDisable()
        {
            var inputHandler = DependencyContainer.InputHandler;
            inputHandler.OnAction -= OnAction;
            inputHandler.OnExtraAction -= OnExtraAction;
            
            EventManager.Instance.Unsubscribe<GameEvent.OnPlayerItemEquip>(OnPlayerItemEquip);
            EventManager.Instance.Unsubscribe<GameEvent.OnPlayerItemUnEquip>(OnPlayerItemUnEquip);
        }

        private void OnAction()
        {
            _currentItem?.Use();
        }

        private void OnExtraAction()
        {
            _currentItem?.AltUse();
        }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        private void OnPlayerItemEquip(GameEvent.OnPlayerItemEquip data)
        {
            _currentItem = data.UsableItem;
        }
        
        private void OnPlayerItemUnEquip(GameEvent.OnPlayerItemUnEquip data)
        {
            _currentItem = null;
        }
    }
}