using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Data
{
    
    public interface IGrassBladeLevel
    {
        // 获取索引大小
        public uint GetIndexCount();
        // 获取三角形索引顺序
        public GraphicsBuffer GetIndexBuffer();

        public int GetLevel();
    
        // 释放
        public void Release();

        public Vector4[] GetPosition();
    }
    
    class GrassBladeLevel : IGrassBladeLevel
    {

        private GraphicsBuffer _indexBuffer;

        // 所有点的坐标
        private Vector4[] _position;

        // 顶点数
        private uint _indexCount;

        private int _level = 0;

        public GrassBladeLevel(int n = 3)
        {
            SetupData(n);
        }

        ~GrassBladeLevel()
        {
            Release();
        }

        public void SetupData(int n)
        {
            // 面数 n * 2 - 1
            // 顶点数 n * 2 + 1
            int faceCount = n * 2 - 1;
            int vertCount = faceCount + 2;
            _indexCount = (uint)(faceCount * 3);

            CalcVertPosition(vertCount);

            CalcIndexes(faceCount);
            _level = n;
        }

        private void CalcIndexes(int faceCount)
        {
            Release();
            var indexCount = faceCount * 3;
            this._indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Index, indexCount, sizeof(uint));
            var indexBuffer = new uint[indexCount];
            for (uint i = 0; i < faceCount; i++)
            {
                indexBuffer[i * 3 + 0] = (i + 1) ^ ((i + 1) & 1);
                indexBuffer[i * 3 + 1] = (i >> 1) << 1 | 1;
                indexBuffer[i * 3 + 2] = i + 2;
            }
            this._indexBuffer.SetData(indexBuffer);
        }

        private void CalcVertPosition(int vertCount)
        {
            _position = new Vector4[vertCount];
            const float w = 0.5f, h = 1.0f;
            int splitCount = (vertCount + 1) / 2;
            float diffW = w / splitCount;
            float diffH = h / splitCount;
            for (int i = 0, len = (vertCount - 1) / 2; i < len; i++)
            {
                float nw = w - i * diffW;
                float nh = i * diffH;
                _position[i * 2 + 0] = new Vector4(-nw, nh, 0f, 1.0f);
                _position[i * 2 + 1] = new Vector4(nw, nh, 0f, 1.0f);
            }
            _position[vertCount - 1] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        }

        public Vector4[] GetPosition()
        {
            return _position;
        }
        
        
        // 获取索引数量大小
        public uint GetIndexCount()
        {
            return _indexCount;
        }

        // 获取三角形索引顺序
        public GraphicsBuffer GetIndexBuffer()
        {
            return _indexBuffer;
        }

        public int GetLevel()
        {
            return _level;
        }

        public void Release()
        {
            _indexBuffer?.Release();
        }
    }
}