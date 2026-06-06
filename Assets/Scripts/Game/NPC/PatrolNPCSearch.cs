using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.NPC
{
    public partial class PatrolNPC
    {
        [SerializeField, FoldoutGroup("Поиск")] private float _loseTime = 5f;
        
        private float _searchEndTime;
        
        private void SearchBehavior()
        {
            if (Time.time >= _searchEndTime)
            {
                EnterPatrol();
                return;
            }

            if (CanSeePlayer())
            {
                EnterChase();
                return;
            }
            
            _agent.SetDestination(_lastPlayerPosition);
        }
        
        private void EnterSearch()
        {
            _currentState = State.Search;
            _searchEndTime = Time.time + _loseTime;
        }
    }
}