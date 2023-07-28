using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comm
{
    public static class GrassShaderId
    {
        public static readonly int InstanceData = Shader.PropertyToID("instanceData");
        public static readonly int IndirectArgsBuffer = Shader.PropertyToID("indirectArgsBuffer");
        
        public static readonly int Position = Shader.PropertyToID("_Position");
        public static readonly int ShaderParam0 = Shader.PropertyToID("_ShaderParam0");

        
        public static readonly int WorldToLocalMatrix = Shader.PropertyToID("worldToLocalMatrix");
        public static readonly int LocalToWorldMatrix = Shader.PropertyToID("localToWorldMatrix");
        public static readonly int GrassColorTop = Shader.PropertyToID("grassColorTop");
        public static readonly int GrassColorBottom = Shader.PropertyToID("grassColorBottom");
        public static readonly int ShadowColorTop = Shader.PropertyToID("shadowColorTop");
        public static readonly int ShadowColorBottom = Shader.PropertyToID("shadowColorBottom");
        public static readonly int WaveColorTop = Shader.PropertyToID("waveColorTop");
        public static readonly int WaveColorBottom = Shader.PropertyToID("waveColorBottom");
        public static readonly int WindParameter = Shader.PropertyToID("windParameter");
        public static readonly int FakeCloudParameter = Shader.PropertyToID("fakeCloudParameter");
        public static readonly int GrassWaveParameter = Shader.PropertyToID("grassWaveParameter");
        public static readonly int VpMatrix = Shader.PropertyToID("vpMatrix");
        public static readonly int GrassParam0 = Shader.PropertyToID("grassParam0");
        public static readonly int GrassParam1 = Shader.PropertyToID("grassParam1");
        public static readonly int GrassParam2 = Shader.PropertyToID("grassParam2");
        public static readonly int GrassParam3 = Shader.PropertyToID("grassParam3");
        public static readonly int GrassParam4 = Shader.PropertyToID("grassParam4");
        public static readonly int TerrainHeightNormalMap = Shader.PropertyToID("TerrainHeightNormalMap");
        public static readonly int GrassDataMap = Shader.PropertyToID("GrassDataMap");
        public static readonly int GrassNullParam = Shader.PropertyToID("grassNullParam");
        public static readonly int CameraPositionWs = Shader.PropertyToID("cameraPositionWS");
        public static readonly int FakeShadowMap = Shader.PropertyToID("FakeCloudShadowMap");
        public static readonly int GrassWaveMap = Shader.PropertyToID("GrassWaveMap");
    }
}