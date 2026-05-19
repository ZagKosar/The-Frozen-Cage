using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Scripts.Game
{
    [Serializable]
    public class PhotoGallery
    {
        [SerializeField] private List<string> _photosBase64 = new();
        [SerializeField] private List<Sprite> _photos = new();
        
        public IReadOnlyList<Sprite> Photos => _photos;

        public void SetPhotos(List<string> photos)
        {
            _photosBase64.Clear();
            _photos.Clear();
            _photosBase64 = photos;

            foreach (var photo in photos)
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(Convert.FromBase64String(photo));
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
                _photos.Add(sprite);
            }
        }
        
        public void Add(Texture2D photo)
        {
            _photosBase64.Add(Convert.ToBase64String(photo.EncodeToPNG()));

            var sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
            
            _photos.Add(sprite);
        }
    }
}