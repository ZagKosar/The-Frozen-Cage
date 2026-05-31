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
        [SerializeField, FoldoutGroup("Патруль")] private float _patrolSpeed;
        [SerializeField, FoldoutGroup("Патруль")] private float _minWaitTime;
        [SerializeField, FoldoutGroup("Патруль")] private float _maxWaitTime;
        [SerializeField, FoldoutGroup("Патруль")] private List<int> _waitPoints = new List<int>();
        
        private float _waitEndTime;
        private float _t;
        private float _multiplier = 1f;
        private int _currentKnotPosition = 0;

        private void PatrolBehavior()
        {
            if (Time.time >  _waitEndTime)
                return;
            
            _t += (Time.deltaTime * _patrolSpeed * _multiplier) / _splineContainer.Spline.GetLength();
            
            if (_t >= 1)
                _multiplier = -1f;
            if (_t <= 0)
                _multiplier = 1f;

            Vector3 position = _splineContainer.Spline.EvaluatePosition(_t);
            Vector3 dir = _splineContainer.Spline.EvaluateTangent(_t);
            
            _agent.SetDestination(position);
            
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5);
        }

        private void CheckForWait()
        {
            if (_waitPoints.Count == 0)
                return;

            Vector3 position = _splineContainer.Spline.EvaluatePosition(_t);

            for (var i = 0; i < _waitPoints.Count; i++)
            {
                var knots = _splineContainer.Spline.Knots;
                var point = _waitPoints[i];
                
                if (point < 0 || point > knots.Count())
                    continue;
                
                Vector3 knotPosition = _splineContainer.Spline.EvaluatePosition((float)point / (knots.Count() -1 ));
                
                var distance = Vector3.Distance(knotPosition, transform.position);

                if (distance < 1.8f)
                {
                    StartCoroutine(WaitAtPoint());
                    
                    _currentKnotPosition = point;
                    
                    return;
                }
            }
        }

        private IEnumerator WaitAtPoint()
        {
            _agent.isStopped = true;
            
            var waitDuration = Random.Range(_minWaitTime, _maxWaitTime);
            
            waitDuration = Time.time + waitDuration;
            
            yield return new WaitForSeconds(waitDuration);
            
            _agent.isStopped = false;
        }
        
        private void EnterPatrol()
        {
            
        }
    }
}