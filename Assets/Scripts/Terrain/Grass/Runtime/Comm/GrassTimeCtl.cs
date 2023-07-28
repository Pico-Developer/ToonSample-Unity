using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comm
{
    public class GrassTimeCtl
    {
        private float _time;

        private float _prevTime;

        public GrassTimeCtl()
        {
            float now = GetNowTime();
            _prevTime = now;
            _time = 0;
        }

        public float Update(float speed)
        {
            float now = GetNowTime();
            float dt = now - _prevTime;
            _prevTime = now;
            _time += dt * speed;            
            _time %= 2.0f * 3.1415926f;
            return _time;
        }

        public float GetTime()
        {
            return _time;
        }

        private float GetNowTime()
        {
#if UNITY_EDITOR
            float time = UnityEngine.Device.Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
            float time = Time.time;
#endif
            return time;
        }
    }
}