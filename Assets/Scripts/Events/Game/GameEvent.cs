using Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scripts.Game.Items;

namespace Scripts.Events.Game
{
    public class GameEvent
    {
        public struct Pause
        {

        }

        public struct InteractHover
        {
            public Interactable Interact;
        }

        public struct InteractHoverEnd
        {
            public Interactable Interact;
        }

        public struct AddItem
        {
            public int Id;
            public int Amount;
        }

        public struct InnerDialogue
        {
            public string Text;
        }

        public struct LoadNextScene
        {
            
        }

        public struct OnPlayerItemEquip
        {
            public UsableItem UsableItem;
        }
        
        public struct OnPlayerItemUnEquip
        {
            
        }

        public struct OnGallery
        {
            
        }
    }
}
