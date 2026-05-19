using UnityEngine;
using UnityEngine.UI;

namespace Windows.Game.PhotoGallery
{
    public class Photo : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        public void SetImage(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}