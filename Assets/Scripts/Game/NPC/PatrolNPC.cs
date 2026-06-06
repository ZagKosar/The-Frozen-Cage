using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace Scripts.Game.NPC
{
    public partial class PatrolNPC : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Компоненты")] private NavMeshAgent _agent;
        [SerializeField, FoldoutGroup("Компоненты")] private SplineContainer _splineContainer;
        [SerializeField, FoldoutGroup("Компоненты")] private Animator _animator;
        [SerializeField, FoldoutGroup("Видимость")] private float _viewDistance = 10f;
        [SerializeField, FoldoutGroup("Видимость")] private float _viewAngle = 90f;
        [SerializeField, FoldoutGroup("Видимость")] private LayerMask _obstacleLayerMask;
        [SerializeField, FoldoutGroup("Видимость")] private Transform _eyeTransform;

        private Transform _player;
        
        private List<float> _patrolPoints = new List<float>();

        private State _currentState;
        
        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int LookingBehindParam = Animator.StringToHash("LookingBehind");

        private enum State {Patrol,Chase,Search}

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _agent.autoBraking = true;
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            
            var knots = _splineContainer.Spline.Knots.ToList();
            var length = _splineContainer.CalculateLength();
            
            _patrolPoints.Add(0);
            
            for (int i = 0; i < knots.Count - 1; i++)
            {
                Vector3 knot = knots[i].Position;
                Vector3 nextKnot = knots[i + 1].Position;
                
                var distance = Vector3.Distance(knot, nextKnot);
                
                _patrolPoints.Add(_patrolPoints[i] + distance/length);
            }

            EnterPatrol();
        }

        private void Update()
        {
            switch (_currentState)
            {
                case State.Patrol:
                    PatrolBehavior();
                    if (CanSeePlayer())
                        EnterChase();
                    break;
                case State.Chase:
                    ChaseBehavior();
                    if (!CanSeePlayer())
                        EnterSearch();
                    break;
                case State.Search:
                    SearchBehavior();
                    break;
            }
        }

        private bool CanSeePlayer()
        {
            var distance = Vector3.Distance(_eyeTransform.position, _player.position);
            
            if (distance >  _viewDistance)
                return false;
            
            var dir = (_player.position - _eyeTransform.position).normalized;
            var angle = Vector3.Angle(_eyeTransform.forward, dir);
            
            if (angle > _viewAngle * 0.5f)
                return false;
            
            if (Physics.Raycast(_eyeTransform.position, dir, distance,  _obstacleLayerMask))
                return false;
            
            return true;
        }

        [Button]
        private void TestFunc(Vector3 pos)
        {
            _agent.SetDestination(pos);
        }
    }
}