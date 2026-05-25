using DG.Tweening;
using Scripts.Events.Preview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Scripts.Events.Preview.PreviewEvent;

namespace Scripts.Game.Preview
{
    public class PreviewShower : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _swipeOffset = 100f;
        [SerializeField] private float _swipeDuration = 1f;

        private Transform _currentModel;
        private Transform _tempModel;
        private bool _isPlaying = false;

        private void Start()
        {
            EventManager.Instance.Subscribe<PreviewEvent.Drag>(OnDrag);
            EventManager.Instance.Subscribe<PreviewEvent.ShowNext>(OnShowNext);
            EventManager.Instance.Subscribe<PreviewEvent.ShowPrevious>(OnShowPrevious);
            EventManager.Instance.Subscribe<PreviewEvent.Show>(OnShow);
        }

        private void OnDrag(PreviewEvent.Drag data)
        {
            _currentModel.Rotate(Vector3.up, -data.Delta.x * _rotationSpeed, Space.World);
            _currentModel.Rotate(Vector3.right, -data.Delta.y * _rotationSpeed, Space.World);
        }

        private void OnShowNext(PreviewEvent.ShowNext data)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            var parent = _currentModel.parent;

            _tempModel = _currentModel;

            _currentModel = Instantiate(data.NextModel, parent);
            _currentModel.localScale = data.Scale;
            _currentModel.localPosition = Vector3.right * -_swipeOffset;
            _currentModel.DOMoveX(0, _swipeDuration);

            _tempModel.localPosition = Vector3.zero;
            _tempModel.DOMoveX(_swipeOffset, _swipeDuration).onComplete += () =>
            {
                Destroy(_tempModel.gameObject);
                _isPlaying = false;
            };
        }

        private void OnShowPrevious(PreviewEvent.ShowPrevious data)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            var parent = _currentModel.parent;

            _tempModel = _currentModel;

            _currentModel = Instantiate(data.PreviousModel, parent);
            _currentModel.localScale = data.Scale;
            _currentModel.localPosition = Vector3.right * _swipeOffset;
            _currentModel.DOMoveX(0, _swipeDuration);

            _tempModel.localPosition = Vector3.zero;
            _tempModel.DOMoveX(-_swipeOffset, _swipeDuration).onComplete += () =>
            {
                Destroy(_tempModel.gameObject);
                _isPlaying = false;
            };
        }

        private void OnShow(PreviewEvent.Show data)
        {
            if (_currentModel != null)
            {
                Destroy(_currentModel.gameObject);
            }
            _currentModel = Instantiate(data.Model, _container);
            _currentModel.localScale = data.Scale;
            _currentModel.localPosition = Vector3.zero;
        }
    }
}
