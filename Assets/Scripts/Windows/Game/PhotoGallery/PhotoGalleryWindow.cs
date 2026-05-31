using System.Collections.Generic;
using Scripts.Events.Game;
using Scripts.WindowSwitcher;
using UnityEngine;
using UnityEngine.UI;

namespace Windows.Game.PhotoGallery
{
    public class PhotoGalleryWindow : WindowPanel
    {
        [SerializeField] private Photo _photoPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private Button _closeButton;
        
        public override int Priority => 2;
        
        private List<Photo> _photos = new();
        
        public override void Load()
        {
            _closeButton.onClick.AddListener(OnCloseClick);
        }
        
        public override void Destroy()
        {
            _closeButton.onClick.RemoveListener(OnCloseClick);
        }

        public override void Open(object context = null)
        {
            var gallery = DependencyContainer.PhotoGallery;

            for (var index = 0; index < gallery.Photos.Count; index++)
            {
                var sprite = gallery.Photos[index];
                var photo = GetOrCreatePhoto(index);
                photo.SetImage(sprite);
                photo.gameObject.SetActive(true);
            }

            for (var index = gallery.Photos.Count; index < _photos.Count; index++)
            {
                _photos[index].gameObject.SetActive(false);
            }
            
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseClick()
        {
            EventManager.Instance.Invoke(new GameEvent.OnGallery());
        }

        private Photo GetOrCreatePhoto(int index)
        {
            Photo photo;
            
            if (_photos.Count <= index)
            {
                photo = Instantiate(_photoPrefab, _container);
                _photos.Add(photo);
            }
            else
            {
                photo = _photos[index];
            }
            
            return photo;
        }
    }
}