using Scripts.App.Constants;
using Scripts.Events.Game;
using UnityEngine;

namespace Scripts.Game.Items
{
    public class PhotoCamera : UsableItem
    {
        private GameObject _camera;
        private bool _isEquiped = false;

        public override bool IsEquiped
        {
            get => _isEquiped;
            set { _isEquiped = value; }
        }

        public override void Pickup()
        {
            var camera = GetOrCreateCamera();
            camera.SetActive(true);

            _isEquiped = true;
        }

        public override void Unequipe()
        {
            var camera = GetOrCreateCamera();
            camera.SetActive(false);

            _isEquiped = false;
        }

        public override void Use()
        {
            var gallery = DependencyContainer.PhotoGallery;
            var camera = GetOrCreateCamera().GetComponentInChildren<Camera>();
            var renderTexture = new RenderTexture(512, 512, 24);

            camera.targetTexture = renderTexture;

            var photo = new Texture2D(512, 512, TextureFormat.RGB24, false);

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
            EventManager.Instance.Invoke(new GameEvent.OnGallery());
        }

        private GameObject GetOrCreateCamera()
        {
            if (_camera == null)
            {
                var screenPosition = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
                var mainCameraTransform = Camera.main.transform;
                var position = Camera.main.ScreenToWorldPoint(screenPosition) + mainCameraTransform.forward * 0.52f + mainCameraTransform.right * 0.3f + mainCameraTransform.up * -0.3f;

                _camera = GameObject.Instantiate(_model.gameObject, position, Quaternion.identity);
                _camera.transform.SetParent(mainCameraTransform, true);
                _camera.transform.localEulerAngles = Vector3.right * 90;
            }

            return _camera;
        }
    }
}