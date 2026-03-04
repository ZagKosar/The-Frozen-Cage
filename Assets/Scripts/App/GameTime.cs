using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.App
{
    [Serializable]
    public class GameTime
    {
        private float _time = 0f;
        private float _deltaTime = 0f;

        public float Time => _time;
        public float DeltaTime => _deltaTime;

        public void Update(float deltaTime)
        {
            _time += deltaTime;
            _deltaTime = deltaTime;
        }
    }
}
