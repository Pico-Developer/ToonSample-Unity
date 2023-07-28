using GamePlus.Urp.Terrain.Grass.Runtime.Comm;
using GamePlus.Urp.Terrain.Grass.Runtime.Comp;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Feature
{
    public class PrepareDataPass : ScriptableRenderPass
    {

        private readonly GrassBuffer _buffer;

        private readonly GrassComputeShader _compute;

        private readonly GrassParamManage _paramManage;

        private RootChunkFilter _filter;

        public PrepareDataPass(ComputeShader cs, GrassBuffer buffer, GrassParamManage paramManage)
        {
            _buffer = buffer;
            _compute = new GrassComputeShader(cs, buffer);
            _paramManage = paramManage;
        }
        
        private Camera GetCamera(ref RenderingData renderingData)
        {
            return renderingData.cameraData.camera;
        }

        public void SetupFilter(RootChunkFilter filter)
        {
            _filter = filter;
        }
        
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!_compute.IsOk())
            {
                return;
            }
            
            Camera camera = GetCamera(ref renderingData);
            if (camera == null)
            {
                return;
            }

            

            _compute.SetCamera(camera);

            var cmd = CommandBufferPool.Get("Grass Compute");
            
            var glassBlade = _paramManage.GetGlassBlade();

            // 重置buffer计数器, 清空所有buffer内容
            var instanceBuffer = _buffer.GetInstanceBuffer();
            if (instanceBuffer != null)
            {
                _buffer.Reset(cmd, glassBlade.GetIndexCount());
            }

            var roots = _filter.GetAllRoot();
            foreach (var grassRoot in roots)
            {
                if(!grassRoot.gameObject.activeSelf || !grassRoot.isActiveAndEnabled) continue;
                _paramManage.SetupWithRoot(grassRoot);
                var param = _paramManage.GetParam();

                var chunks = _filter.GetChunksByKey(grassRoot);
                
                _compute.Run(
                    cmd,
                    chunks,
                    param
                );
                
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);

        }
    }
}