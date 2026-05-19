using UnityEngine;

namespace Scripts.Game.Items
{
    public class PhotoCamera : UsableItem
    {
        private GameObject _camera;
        
        public override void Pickup()
        {
            var camera = GetOrCreateCamera();
            camera.SetActive(true);
        }

        public override void Use()
        {
            var gallery = DependencyContainer.PhotoGallery;
            var camera = GetOrCreateCamera().GetComponentInChildren<Camera>();
            var renderTexture = new RenderTexture(512, 512, 24);
            
            camera.targetTexture = renderTexture;
            
            var photo = new Texture2D(512,512, TextureFormat.RGB24, false);
            
            camera.Render();
            
            RenderTexture.active = renderTexture;
            
            photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            photo.Apply();
            
            camera.targetTexture = null;
            RenderTexture.active = null;
            
            Object.DestroyImmediate(renderTexture);
            
            gallery.Add(photo);
        }

        public override void AltUse()
        {
            
        }

        private GameObject GetOrCreateCamera()
        {
            if (_camera == null)
            {
                var screenPosition = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
                var position = Camera.main.ScreenToWorldPoint(screenPosition);
                var player = DependencyContainer.Player;
                _camera = GameObject.Instantiate(_model, position, Quaternion.identity);
                _camera.transform.SetParent(player.transform);
            }
            
            return _camera;
        }
    }
}