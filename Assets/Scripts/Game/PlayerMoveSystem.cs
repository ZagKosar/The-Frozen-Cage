using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game
{
    public class PlayerMoveSystem : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private Vector3 _lastMoveVector;

        private float moveSpeed = 10f;

        private void Start()
        {
            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnMove += OnMove;
        }

        private void Update()
        {
            var moveDir = _rigidbody.transform.forward * _lastMoveVector.y + _rigidbody.transform.right * _lastMoveVector.x;

            _rigidbody.linearVelocity = new Vector3(moveDir.x * moveSpeed, 0, moveDir.z * moveSpeed);
        }

        private void OnMove(Vector2 vector)
        {
            _lastMoveVector = vector;
        }
    }
}
