using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.UI
{
    public class DragbleUIElement : MonoBehaviour, IDragHandler
    {
        public event Action<Vector2> Drag;
        public void OnDrag(PointerEventData eventData)
        {
            Drag?.Invoke(eventData.delta);
        }
    }
}
