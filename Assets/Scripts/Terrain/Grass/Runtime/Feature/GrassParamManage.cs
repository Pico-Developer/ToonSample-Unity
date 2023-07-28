using GamePlus.Urp.Terrain.Grass.Runtime.Comp;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Feature
{
    public class GrassParamManage
    {
        public static float MAX_DISPLAY_RANGE = 100;
        public static bool USE_GIZMOS = false;
        
        
        private IGrassBladeLevel _glassBlade;

        private readonly GrassParam _param = new GrassParam();
        
        public GrassParam GetParam()
        {
            return _param;
        }
        
        public void SetGlassBlade(IGrassBladeLevel glassBlade)
        {
            _glassBlade = glassBlade;
        }
        public IGrassBladeLevel GetGlassBlade()
        {
            return _glassBlade;
        }

        public void SetTime(float time)
        {
            _param.ShaderParam0.z = time;
        }

        public void SetupWithRoot(GrassRoot root)
        {
            var transform = root.transform;
            _param.LocalToWorld = transform.localToWorldMatrix;
            _param.WorldToLocal = transform.worldToLocalMatrix;
            
            _param.GrassDataTex = root.GrassDataTex;
            _param.GrassNullParam = _param.GrassDataTex == null ? 0f: 1f;
            
            _param.GrassParam1.Set(
                root.MaxGrassSize,
                _param.TerrainHeightNormalTex == null ? 0f : root.TerrainHeightMax,
                _param.GrassDataTex == null ? 0f : root.MaskClip,
                _param.GrassDataTex == null ? 0f : root.SlopeClip
            );

            _param.TerrainHeightNormalTex = root.TerrainHeightNormalTex;
            if (root.TerrainHeightNormalTex)
            {
                _param.GrassParam2.x = root.TerrainHeightNormalTex.width;
                _param.GrassParam2.y = root.TerrainHeightNormalTex.height;
            }
            
            // 如果没有terrain normal 贴图, 则不进行计算lambert
            _param.GrassParam4.z = _param.TerrainHeightNormalTex == null ? 0f : _param.GrassParam4.z;
            // 如果没有data贴图, 则shadow mask不进行计算, 对应信息保存在b通道中
            _param.GrassParam4.w = _param.GrassDataTex == null ? 0f : root.ShadowMaskStrength;
            
        }
        
        public void SetupGlobal(GrassSettings settings)
        {
            USE_GIZMOS = settings.UseGizmos;
            MAX_DISPLAY_RANGE = settings.MaxDisplayRange;

            _param.MaxGrassCount = settings.MaxGrassCount;
            
            _param.FakeCloudShadowTex = settings.FakeCloudShadowTex;
            _param.GrassWaveTex = settings.GrassWaveTex;
            
            _param.Top = settings.ColorTop;
            _param.Bottom = settings.ColorBottom;
            _param.ShadowTop = settings.ShadowColorTop;
            _param.ShadowBottom = settings.ShadowColorBottom;
            _param.WaveTop = settings.GrassWaveColorTop;
            _param.WaveBottom = settings.GrassWaveColorBottom;
            
            _param.ShaderParam0.Set(
                settings.ColorBias,
                settings.LightIntensityLerp,
                0f, 
                0f);
            
            _param.WindParameter.Set(
                settings.WindRotation,
                settings.WindSplit * 0.5f,
                settings.WindStrength,
                settings.WindOfTilt
                );
            
            _param.FakeCloudParameter.Set(
                settings.FakeCloudScaleX,
                settings.FakeCloudScaleY,
                settings.FakeCloudSpeed,
                _param.FakeCloudShadowTex == null ? 0f : settings.FakeCloudStrength
            );
            _param.WaveParameter.Set(
                settings.GrassWaveScaleX,
                settings.GrassWaveScaleY,
                settings.GrassWaveSpeed,
                _param.GrassWaveTex == null ? 0f : settings.GrassWaveStrength
            );
            
            
            
            _param.GrassParam3.Set(
                settings.MaxDisplayRange, 
                settings.GrassHeight, 
                settings.GrassWidth, 
                0f
                );
            _param.GrassParam4.Set(
                settings.FakeCloudPower,
                settings.GrassWavePower,
                settings.LambertStrength,
                0f // w 在root中设置, 控制 shadowmask强度
                );
            
        }

    }
}