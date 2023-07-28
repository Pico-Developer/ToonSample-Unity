using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Data
{
    public class GrassParam
    {
        public Matrix4x4 LocalToWorld;
        public Matrix4x4 WorldToLocal;

        public Color Top;
        public Color Bottom;

        public Color ShadowTop;
        public Color ShadowBottom;

        public Color WaveTop;
        public Color WaveBottom;
        
        // x: ColorBias, y: LightIntensityLerp, zw: unused
        public Vector4 ShaderParam0 = new Vector4();


        public Texture TerrainHeightNormalTex;
        public Texture FakeCloudShadowTex;
        public Texture GrassWaveTex;
        public Texture GrassDataTex;
        public float GrassNullParam;


        // x: MaxGrassSize, y: TerrainHeightMax, z: MaskClip, w: SlopeClip
        public Vector4 GrassParam1 = new Vector4();
        // xy: TerrainHeightMap Size, zw: unused
        public Vector4 GrassParam2 = new Vector4();
        
        // x: MaxDisplayRange, y: GrassHeight, z: GrassWidth, w: unused
        public Vector4 GrassParam3 = new Vector4();
        // x: FakeCloudPower, y: GrassWavePower, z: LambertStrength, w: shadowmask strength
        public Vector4 GrassParam4 = new Vector4();
        
        // x: scaleX, y: scaleY, z: speed, w: strength
        public Vector4 FakeCloudParameter = new Vector4();
        // x: scaleX, y: scaleY, z: speed, w: strength
        public Vector4 WaveParameter = new Vector4();
        
        // x: wind rotation, y: wind split, z: wind strength, w: unused
        public Vector4 WindParameter = new Vector4();

        public int MaxGrassCount = 40_000;

        // public GraphicsBuffer IndexBuffer;
    }
}