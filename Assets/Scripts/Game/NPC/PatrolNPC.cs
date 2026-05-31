using System;
using System.Collections.Generic;
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
        [SerializeField, FoldoutGroup("Видимость")] private float _viewDistance = 10f;
        [SerializeField, FoldoutGroup("Видимость")] private float _viewAngle = 90f;
        [SerializeField, FoldoutGroup("Видимость")] private LayerMask _obstacleLayerMask;
        [SerializeField, FoldoutGroup("Видимость")] private Transform _eyeTransform;

        private Transform _player;
        
        private State _currentState;
        
        private enum State {Patrol,Chase,Search}

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _currentState = State.Patrol;
            _agent.speed = _patrolSpeed;
            _agent.autoBraking = true;
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
            var dir = (_player.position - _eyeTransform.position).normalized;
            var distance = Vector3.Distance(_eyeTransform.position, _player.position);
            
            if (distance >  _viewDistance)
                return false;
            
            if (Vector3.Angle(transform.forward, dir) > _viewAngle * 0.5f)
                return false;
            
            if (Physics.Raycast(_eyeTransform.position, dir, distance,  _obstacleLayerMask))
                return false;
            
            return true;
        }
    }
}