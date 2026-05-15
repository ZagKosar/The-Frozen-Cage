using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract string InteractDescription { get; }

        public abstract void Interact();
    }
}
