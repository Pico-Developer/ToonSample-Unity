using GamePlus.Urp.Terrain.Grass.Runtime.Comm;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Feature
{
    public class DrawProceduralPass: ScriptableRenderPass
    {
        
        private readonly GrassBuffer _buffer;
        
        private readonly Material _material;

        private readonly MaterialPropertyBlock _block;

        private readonly GrassParamManage _paramManage;

        public DrawProceduralPass(GrassBuffer buffer, GrassParamManage paramManage)
        {
            _buffer = buffer;
            
            _material = CoreUtils.CreateEngineMaterial("Sample/GrassShader");

            _block = new MaterialPropertyBlock();

            _paramManage = paramManage;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Grass Draw");
            
            var instanceBuffer = _buffer.GetInstanceBuffer();
            var argsBuffer = _buffer.GetArgsBuffer();


            

            var param = _paramManage.GetParam();
                
            _block.Clear();
            _block.SetBuffer(GrassShaderId.InstanceData, instanceBuffer);
            _block.SetMatrix(GrassShaderId.LocalToWorldMatrix, param.LocalToWorld);
            _block.SetMatrix(GrassShaderId.WorldToLocalMatrix, param.WorldToLocal);
            _block.SetColor(GrassShaderId.GrassColorTop, param.Top);
            _block.SetColor(GrassShaderId.GrassColorBottom, param.Bottom);
            _block.SetColor(GrassShaderId.ShadowColorTop, param.ShadowTop);
            _block.SetColor(GrassShaderId.ShadowColorBottom, param.ShadowBottom);
            _block.SetColor(GrassShaderId.WaveColorTop, param.WaveTop);
            _block.SetColor(GrassShaderId.WaveColorBottom, param.WaveBottom);
            _block.SetVector(GrassShaderId.ShaderParam0, param.ShaderParam0);
            _block.SetVectorArray(GrassShaderId.Position, _paramManage.GetGlassBlade().GetPosition());
        
            cmd.DrawProceduralIndirect(
                _paramManage.GetGlassBlade().GetIndexBuffer(), 
                Matrix4x4.identity, 
                _material, 
                0, 
                MeshTopology.Triangles, 
                argsBuffer,
                0,
                _block);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }
}