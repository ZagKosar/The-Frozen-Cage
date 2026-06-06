using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace Scripts.Game.NPC
{
    public partial class PatrolNPC
    {
        [SerializeField, FoldoutGroup("Патруль")]
        private float _patrolSpeed;

        [SerializeField, FoldoutGroup("Патруль")]
        private float _minWaitTime;

        [SerializeField, FoldoutGroup("Патруль")]
        private float _maxWaitTime;

        [SerializeField, FoldoutGroup("Патруль")]
        private List<int> _waitPoints = new List<int>();

        private static Vector3 _backAngle = new Vector3(0, 180, 0);
        private float _waitEndTime;
        private float _t;
        private float _multiplier = 1f;
        private int _nextWaitIndex = 0;

        private void PatrolBehavior()
        {
            var patrolPos = _splineContainer.EvaluatePosition(_t);

            if (Time.time < _waitEndTime)
            {
                _agent.SetDestination(patrolPos);
                _animator?.SetFloat(SpeedParam, _agent.isStopped ? 0f : 0.3f);
                return;
            }

            _t += (Time.deltaTime * _patrolSpeed * _multiplier) / _splineContainer.Spline.GetLength();

            if (_t >= 1)
                _multiplier = -1f;
            if (_t <= 0)
                _multiplier = 1f;

            _agent.SetDestination(_splineContainer.EvaluatePosition(_t));

            _animator?.SetFloat(SpeedParam, _agent.isStopped ? 0f : 0.3f);
            CheckForWait();
        }

        private void CheckForWait()
        {
            if (_waitPoints.Count == 0)
                return;
            
            var pointIndex = _waitPoints[_nextWaitIndex];

            if (pointIndex < 0 || pointIndex > _patrolPoints.Count)
                return;

            var absolutePosition = _patrolPoints[pointIndex];
            var distance = Mathf.Abs(_t - absolutePosition);

            if (distance < 0.02f)
            {
                StartCoroutine(WaitAtPoint());

                if (Mathf.Approximately(_multiplier, 1f))
                {
                    if (_nextWaitIndex == _waitPoints.Count - 1)
                        _nextWaitIndex = Mathf.Max(_nextWaitIndex - 1, 0);
                    else
                        _nextWaitIndex = Mathf.Min(_nextWaitIndex + 1, _waitPoints.Count - 1);
                }
                else
                {
                    if (_nextWaitIndex == 0)
                        _nextWaitIndex = Mathf.Min(_nextWaitIndex + 1, _waitPoints.Count - 1);
                    else
                        _nextWaitIndex = Mathf.Max(_nextWaitIndex - 1, 0);
                }
            }
        }

        private IEnumerator WaitAtPoint()
        {
            _agent.isStopped = true;

            var waitDuration = Random.Range(_minWaitTime, _maxWaitTime);

            _waitEndTime = Time.time + waitDuration;

            yield return new WaitForSeconds(waitDuration);

            _agent.isStopped = false;
        }

        private void EnterPatrol()
        {
            _currentState = State.Patrol;
            _agent.speed = _patrolSpeed;
            _agent.isStopped = false;
            _animator?.SetFloat(SpeedParam, 0.3f);
            var distance = Vector3.Distance(transform.position, _splineContainer.EvaluatePosition(_t));
            _waitEndTime = Time.time + distance / Mathf.Max(_patrolSpeed, 0.1f);
        }
    }
}