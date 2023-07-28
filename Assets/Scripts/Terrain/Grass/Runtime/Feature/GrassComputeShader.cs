using System.Collections.Generic;
using GamePlus.Urp.Terrain.Grass.Runtime.Comm;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Feature
{
    public class GrassComputeShader
    {
        private readonly ComputeShader _cs;

        private readonly int _computeShaderKernelId;
        
        private Camera _camera;

        private Matrix4x4 _vpMatrix;
        private Matrix4x4 _gpuVpMatrix;
        
        private readonly GrassBuffer _buffer;
        
        private Vector4 _param0 = new Vector4();

        public GrassComputeShader(ComputeShader cs, GrassBuffer buffer)
        {
            _cs = cs;
            if (_cs == null)
            {
                return;
            }
            _computeShaderKernelId = cs.FindKernel("CSMain");
            _buffer = buffer;
        }
        
        public void SetCamera(Camera camera)
        {
            _camera = camera;
            var gpuProj = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true);
            var view = camera.worldToCameraMatrix;
            var gpuVp = gpuProj * view;

            var vp = camera.projectionMatrix * view;

            _gpuVpMatrix = gpuVp;
            _vpMatrix = vp;
        }
        
        public bool IsOk()
        {
            return _cs != null;
        }

        public void Run(
            CommandBuffer cmd,
            List<GrassChunk> list,
            GrassParam param)
        {
            int grassCount = param.MaxGrassCount;

            if (!_buffer.BufferSizeComp(grassCount))
            {
                _buffer.Init(grassCount);
            }
            
            cmd.SetComputeMatrixParam(_cs, GrassShaderId.LocalToWorldMatrix, param.LocalToWorld);
            cmd.SetComputeMatrixParam(_cs, GrassShaderId.WorldToLocalMatrix, param.WorldToLocal);
            cmd.SetComputeMatrixParam(_cs, GrassShaderId.VpMatrix, _gpuVpMatrix);
            // cmd.SetComputeVectorParam(_cs, GrassShaderId.WindDir, _windDir);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.WindParameter, param.WindParameter);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassParam1, param.GrassParam1);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassParam2, param.GrassParam2);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassParam3, param.GrassParam3);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassParam4, param.GrassParam4);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.FakeCloudParameter, param.FakeCloudParameter);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassWaveParameter, param.WaveParameter);

            cmd.SetComputeTextureParam(_cs, _computeShaderKernelId, GrassShaderId.TerrainHeightNormalMap, param.TerrainHeightNormalTex);
            cmd.SetComputeTextureParam(_cs, _computeShaderKernelId, GrassShaderId.GrassDataMap, param.GrassDataTex);
            cmd.SetComputeTextureParam(_cs, _computeShaderKernelId, GrassShaderId.FakeShadowMap, param.FakeCloudShadowTex);
            cmd.SetComputeTextureParam(_cs, _computeShaderKernelId, GrassShaderId.GrassWaveMap, param.GrassWaveTex);
            // cmd.SetComputeTextureParam(_cs, _computeShaderKernelId, GrassShaderId.ShadowMaskMap, param.ShadowMaskTex);
            cmd.SetComputeVectorParam(_cs, GrassShaderId.CameraPositionWs, _camera.gameObject.transform.position);
            cmd.SetComputeFloatParam(_cs, GrassShaderId.GrassNullParam, param.GrassNullParam);

            var instanceBuffer = _buffer.GetInstanceBuffer();
            var argsBuffer = _buffer.GetArgsBuffer();
            
            // 设置compute buffer
            cmd.SetComputeBufferParam(_cs, _computeShaderKernelId, GrassShaderId.InstanceData, instanceBuffer);
            // 设置 args 缓冲区
            cmd.SetComputeBufferParam(_cs, _computeShaderKernelId, GrassShaderId.IndirectArgsBuffer, argsBuffer);

            
            foreach (var data in list)
            {
                var position = data.Position;
                var size = data.Size;
                _param0.Set(position.x, position.y, position.z, size);
                cmd.SetComputeVectorParam(_cs, GrassShaderId.GrassParam0, _param0);
                
                cmd.DispatchCompute(_cs ,_computeShaderKernelId, 13, 13,1);
            }

        }
        
    }
}