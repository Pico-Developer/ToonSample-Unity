using GamePlus.Urp.Terrain.Grass.Resources.Shader;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Data
{
    public class GrassBuffer
    {
        private ComputeBuffer _instanceBuffer;
        private ComputeBuffer _argsBuffer;

        // private int _size;

        private int _size = 1;

        private uint[] _args;
        
        private static readonly int ArgsLen = 5;

        ~GrassBuffer()
        {
            Release();
        }

        public void Init(int size)
        {
            Release();
            _args = new uint[ArgsLen];
            _size = size;
            _instanceBuffer = new ComputeBuffer(size, GrassData.Size, ComputeBufferType.Append);
            _argsBuffer = new ComputeBuffer(ArgsLen, sizeof(uint), ComputeBufferType.IndirectArguments);
        }
        
        public bool BufferSizeComp(int size)
        {
            return size == _size;
        }
        

        public void Reset(CommandBuffer cmd, uint indexCount)
        {
            // 重置buffer计数器, 清空所有buffer内容
            cmd.SetBufferCounterValue(_instanceBuffer, 0);
            
            _args[0] = indexCount;
            _args[1] = 0;
            cmd.SetBufferData(_argsBuffer, _args);
        }
        
        public ComputeBuffer GetInstanceBuffer()
        {
            return _instanceBuffer;
        }

        public ComputeBuffer GetArgsBuffer()
        {
            return _argsBuffer;
        }
        
        public void Release()
        {
            _argsBuffer?.Release();
            _instanceBuffer?.Release();
            _argsBuffer = null;
            _instanceBuffer = null;
        }
    }
}