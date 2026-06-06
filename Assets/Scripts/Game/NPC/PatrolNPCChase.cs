using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.NPC
{
    public partial class PatrolNPC
    {
        [SerializeField, FoldoutGroup("Погоня")] private float _chaseSpeed = 5f;
        
        private Vector3 _lastPlayerPosition;
        
        private void ChaseBehavior()
        {
            _agent.SetDestination(_player.position);
            _lastPlayerPosition = _player.position;
        }
        
        private void EnterChase()
        {
            _currentState = State.Chase;
            _agent.speed = _chaseSpeed;
            _agent.isStopped = false;
        }
    }
}