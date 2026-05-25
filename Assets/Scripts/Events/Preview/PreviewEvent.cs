using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Events.Preview
{
    public class PreviewEvent
    {
        public struct Drag
        {
            public Vector2 Delta;
        }
        public struct ShowNext
        {
            public Transform NextModel;
            public Vector3 Scale;
        }
        public struct ShowPrevious
        {
            public Transform PreviousModel;
            public Vector3 Scale;
        }
        public struct Show
        {
            public Transform Model;
            public Vector3 Scale;
        }
    }
}
