using DG.Tweening;
using Scripts.App;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Scripts.Game
{
    public class PlayerMoveSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Transform _model;

        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;
        [SerializeField] private float _crouchSpeed = 2f;

        [Header("Crouch")]
        [SerializeField] private float _standHeight = 2f;
        [SerializeField] private float _crouchHeight = 1.2f;
        [SerializeField] private float _crouchDuration = 0.6f;
        [SerializeField] private LayerMask _ceilingMask;

        private GameTime _gameTime;
        private Vector2 _moveInput;
        private bool _isSprinting;
        private bool _isCrouching;
        private bool _isCrouchingPress;
 

        private Tween _heightTween;

        public bool IsSprinting => _isSprinting && _moveInput.magnitude > 0.1f && !_isCrouching;
        public bool IsCrouching => _isCrouching;
        public bool IsMoving => _moveInput.magnitude > 0.1f;

        public event Action<float> OnStep;
        private float _stepTimer;

        private void Start()
        {
#if UNITY_EDITOR
            _sprintSpeed = 25;
#endif

            _gameTime = DependencyContainer.GameTime;

            var inputHandler = DependencyContainer.InputHandler;

            inputHandler.OnMove += OnMove;
            inputHandler.OnSprintStart += OnSprintStart;
            inputHandler.OnSprintStop += OnSprintStop;
            inputHandler.OnCrouchStart += OnCrouchStart;
            inputHandler.OnCrouchStop += OnCrouchStop;
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void Update()
        {
            HandleSteps();

            if (!_isCrouching || _isCrouchingPress || CeilingAbove())
                return;

            _isCrouching = false;
            ApplyCrouch();
        }

        private void ApplyMovement()
        {
            float speed = GetCurrentSpeed();

            var forward = _rigidbody.transform.forward * _moveInput.y;
            var right = _rigidbody.transform.right * _moveInput.x;
            var moveDir = (forward + right).normalized;

            var horizontal = moveDir * speed;

            _rigidbody.linearVelocity = new Vector3(
                horizontal.x,
                _rigidbody.linearVelocity.y,
                horizontal.z
            );
        }

        private void ApplyCrouch()
        {
            if (!_isCrouching && CeilingAbove())
                return;

            _isCrouching = !_isCrouching;

            float targetHeight = _isCrouching ? _crouchHeight : _standHeight;

            _heightTween?.Kill();

            _heightTween = DOTween.To(
                () => _collider.height,
                h =>
                {
                    _collider.height = h;

                    var center = _collider.center;

                    center.y = h * 0.5f - 1f;
                    _collider.center = center;

                    _model.localScale = new Vector3(1, h/_standHeight, 1);
                },
                targetHeight,
                _crouchDuration
            ).SetEase(Ease.OutQuad);
        }

        private bool CeilingAbove()
        {
            float checkDistance = _standHeight - _collider.height + 0.1f;
            var origin = transform.position + Vector3.up * _collider.height;

            return Physics.Raycast(origin, Vector3.up, checkDistance, _ceilingMask);
        }

        private float GetCurrentSpeed()
        {
            if (_gameTime.DeltaTime == 0) return 0;
            if (_isCrouching) return _crouchSpeed;
            if (IsSprinting) return _sprintSpeed;
            return _walkSpeed;
        }

        private void HandleSteps()
        {
            if (!IsMoving)
            {
                _stepTimer = 0f;
                return;
            }

            float interval = _isCrouching ? 0.7f : (IsSprinting ? 0.35f : 0.5f);

            _stepTimer += _gameTime.DeltaTime;

            if (_stepTimer >= interval)
            {
                _stepTimer = 0f;
                OnStep?.Invoke(GetCurrentSpeed());
            }
        }

        // Input

        private void OnMove(Vector2 vector) => _moveInput = vector;
        private void OnSprintStart() => _isSprinting = true;
        private void OnSprintStop() => _isSprinting = false;
        private void OnCrouchStart()
        {
            _isCrouchingPress = true;
            ApplyCrouch();
        }

        private void OnCrouchStop()
        {
            _isCrouchingPress = false;
            ApplyCrouch();
        }

        private void OnDestroy()
        {
            var inputHandler = DependencyContainer.InputHandler;
            if (inputHandler == null) return;

            inputHandler.OnMove -= OnMove;
            inputHandler.OnSprintStart -= OnSprintStart;
            inputHandler.OnSprintStop -= OnSprintStop;
            inputHandler.OnCrouchStart -= OnCrouchStart;
            inputHandler.OnCrouchStop -= OnCrouchStop;
        }
    }
}