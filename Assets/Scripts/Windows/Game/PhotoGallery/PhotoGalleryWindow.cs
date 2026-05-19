using System.Collections.Generic;
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
            _closeButton.onClick.AddListener(Close);
        }

        public override void Destroy()
        {
            _closeButton.onClick.RemoveListener(Close);
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
            
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private Photo GetOrCreatePhoto(int index)
        {
            Photo photo;
            
            if (_photos.Count <= index)
            {
                photo = Instantiate(_photoPrefab, _container);
            }
            else
            {
                photo = _photos[index];
            }
            
            return photo;
        }
    }
}