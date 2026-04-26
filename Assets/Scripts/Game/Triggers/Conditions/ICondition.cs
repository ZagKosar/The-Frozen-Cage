using System;
using UnityEngine;

namespace Scripts.Game.Triggers
{
    public interface ICondition
    {
        public event Action Complete;

        public void Initialize();
    }
}