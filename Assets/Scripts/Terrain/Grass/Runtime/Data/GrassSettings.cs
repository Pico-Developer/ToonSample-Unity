using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Data
{
    [System.Serializable]
    public class GrassSettings
    {
        [Header("Fake Cloud")]
        // 假云影
        public Texture2D FakeCloudShadowTex;
        [Range(0, 5)] public float FakeCloudSpeed = 1;
        [Range(0.001f, 20f)] public float FakeCloudScaleX = 1;
        [Range(0.001f, 20f)] public float FakeCloudScaleY = 1;
        [Range(0, 1)] public float FakeCloudStrength = 1;
        [Range(1, 32)] public float FakeCloudPower = 1;
        public Color ShadowColorTop = Color.white;
        public Color ShadowColorBottom = Color.black;
        
        [Header("Grass Wave")]
        // 草浪
        public Texture2D GrassWaveTex;
        [Range(0, 5)] public float GrassWaveSpeed = 1;
        [Range(0.001f, 20f)] public float GrassWaveScaleX = 1;
        [Range(0.001f, 20f)] public float GrassWaveScaleY = 1;
        [Range(0, 1)] public float GrassWaveStrength = 1;
        [Range(1, 32)] public float GrassWavePower = 1;
        public Color GrassWaveColorTop = Color.white;
        public Color GrassWaveColorBottom = Color.black;
        
        
        [Header("Color")]
        public Color ColorTop = Color.white;
        public Color ColorBottom = Color.black;
        [Range(0, 1)] public float LambertStrength = 1;

        [Range(-1, 1)] public float ColorBias;
        // [Range(0, 1)] public float EnvironmentLerp = 0;
        [Range(0, 1)] public float LightIntensityLerp = 0;
        
        [Header("Wind")]
        [Range(0, 2)] public float WindRotation = 0;
        [Range(0, 1)] public float WindSplit = 0;
        [Range(1, 10)] public float WindStrength = 0;
        [Range(0, 5)] public float WindOfTilt = 1;
        
        [Header("Grass Shape")]
        [Range(0.1f, 3f)] public float GrassHeight = 1;
        [Range(0.1f, 3f)] public float GrassWidth = 1;
        
        [Header("Advanced")]
        [Range(5, 1000)] public float MaxDisplayRange = 80;

        [Range(1, 1000000)] public int MaxGrassCount = 4_0000;

        [Range(1, 7)] public int GrassBladeLevel = 3;
        
        public bool UseGizmos = false;
    }
}