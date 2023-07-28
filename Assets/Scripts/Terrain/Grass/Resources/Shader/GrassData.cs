using UnityEngine;
using UnityEngine.Rendering;

namespace GamePlus.Urp.Terrain.Grass.Resources.Shader
{
    [GenerateHLSL(needAccessors = false)]
    public struct GrassData
    {
        public static int Size = sizeof(float) * 15;
        
        // 位置
        public Vector3 Position;
        // 朝向
        public Vector2 Facing;

        public float Lambert;

        // 风向
        public Vector2 Wind;
        
        // fake Shadow
        public float FakeShadow;

        public float ShadowMask;

        public float Wave;

        // hash
        public float PerBladeHash;

        // 高度
        public float Height;
        // 宽度
        public float Width;

        // 倾斜
        public float Tilt;
    }
}