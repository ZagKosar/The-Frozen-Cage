using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.NPC
{
    public partial class PatrolNPC
    {
        [SerializeField, FoldoutGroup("Поиск")] private float _searchBufferTime = 2f;
        
        private float _searchEndTime;
        private bool _lookingBehindTriggered;
        
        private void SearchBehavior()
        {
            if (CanSeePlayer())
            {
                EnterChase();
                return;
            }

            if (Time.time >= _searchEndTime)
            {
                EnterPatrol();
                return;
            }

            if (!_lookingBehindTriggered)
            {
                _agent.SetDestination(_lastPlayerPosition);
                _animator?.SetFloat(SpeedParam, 0.3f);

                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _lookingBehindTriggered = true;
                    _agent.isStopped = true;
                    _animator?.SetFloat(SpeedParam, 0f);
                    _animator?.SetTrigger(LookingBehindParam);
                }
            }
        }
        
        private void EnterSearch()
        {
            _currentState = State.Search;
            _lookingBehindTriggered = false;
            _agent.speed = _patrolSpeed;
            _agent.isStopped = false;
            var distance = Vector3.Distance(transform.position, _lastPlayerPosition);
            _searchEndTime = Time.time + distance / Mathf.Max(_agent.speed, 0.1f) + _searchBufferTime;
            _animator?.SetFloat(SpeedParam, 0.3f);
        }
    }
}