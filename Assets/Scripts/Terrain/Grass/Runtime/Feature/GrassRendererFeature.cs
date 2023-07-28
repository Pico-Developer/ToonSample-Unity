using GamePlus.Urp.Terrain.Grass.Runtime.Comm;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Feature
{
    public class GrassRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private ComputeShader compute;

        [SerializeField]
        private GrassSettings settings;

        private PrepareDataPass _prepareDataPass;
        private DrawProceduralPass _drawProceduralPass;

        private GrassBuffer _buffer;

        private GrassParamManage _paramManage;
        

        private GrassBladeLevel _bladeLevel;

        private GrassTimeCtl _timeCtl;

        private RootChunkFilter _filter;

        ~GrassRendererFeature()
        {
            _buffer?.Release();
            _bladeLevel?.Release();
        }
        

        /// <inheritdoc/>
        public override void Create()
        {
            
            _paramManage ??= new GrassParamManage();
            _bladeLevel ??= new GrassBladeLevel();
            _timeCtl ??= new GrassTimeCtl();
            _filter ??= new RootChunkFilter();

            _buffer ??= new GrassBuffer();
            
            _buffer.Init(settings.MaxGrassCount);
            
            
            _paramManage.SetGlassBlade(_bladeLevel);
            
            
            _prepareDataPass = new PrepareDataPass(compute, _buffer, _paramManage)
            {
                renderPassEvent = RenderPassEvent.BeforeRendering
            };
            
            _drawProceduralPass = new DrawProceduralPass(_buffer, _paramManage)
            {
                // renderPassEvent = (RenderPassEvent)251
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
            
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
#endif
            

            
        }

        private void OnAssemblyReload()
        {
            _buffer?.Release();
            _bladeLevel?.Release();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera)
            {
                return;
            }
#if UNITY_EDITOR
            var isPrefabPreview = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if(isPrefabPreview != null) return;
#endif
            
            _filter.Generate(renderingData.cameraData.camera);

            
            
            _prepareDataPass.SetupFilter(_filter);
            
            
            if (_bladeLevel.GetLevel() != settings.GrassBladeLevel)
            {
                _bladeLevel.SetupData(settings.GrassBladeLevel);
            }
            _paramManage.SetupGlobal(settings);
            _paramManage.SetTime(_timeCtl.Update(settings.WindStrength));
            
            
            renderer.EnqueuePass(_prepareDataPass);
            renderer.EnqueuePass(_drawProceduralPass);
        }
    }
}


