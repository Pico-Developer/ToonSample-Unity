using System.Collections.Generic;
using GamePlus.Urp.Terrain.Grass.Runtime.Comm;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using GamePlus.Urp.Terrain.Grass.Runtime.Feature;
using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comp
{
    [ExecuteInEditMode]
    public class GrassRoot : MonoBehaviour
    {
        // TODO DEBUG
        private GrassGizmos _gizmos = new GrassGizmos();

        [Header("Basic")]
        // 场地的大小, 单位米
        [Range(1, 400)] public float MaxGrassSize = 1;
        // 草的高度图, 最大高度是多少米
        public float TerrainHeightMax = 1;
        
        // 高度图
        public Texture2D TerrainHeightNormalTex;

        [Header("Data")]
        public Texture2D GrassDataTex;
        
        [Header("Clip")] 
        // public Texture2D MaskTex;
        [Range(0, 1)] public float MaskClip = 0;
        [Range(0, 1)] public float SlopeClip = 0;

        // public Texture2D ShadowMaskTex;
        [Range(0, 1)] public float ShadowMaskStrength = 1;

        [Header("Advanced")]
        
        [Range(1, 20)] public int DepthLevel = 1;
        
        
        
        
        private void Start()
        {
            FlushChunks();
        }

        private void OnEnable()
        {
            FlushChunks();
        }

        private void Awake()
        {
            FlushChunks();
        }


        void Update()
        {
            if (transform.hasChanged)
            {
                UpdateChunksBounds();
                transform.hasChanged = false;
            }
        }



        private void OnValidate()
        {
            FlushChunks();
        }

        private int _nowChunkCount = -1;

        private readonly List<GrassChunk> _chunks = new List<GrassChunk>();

        private void FlushChunks()
        {
            if(_nowChunkCount == DepthLevel && _chunks.Count != 0) return;
            _nowChunkCount = DepthLevel;
            _chunks.Clear();
            
            if(GrassParamManage.USE_GIZMOS)
                _gizmos.Clear();

            var chunkSize = MaxGrassSize / DepthLevel;
            var halfChunkSize = chunkSize / 2.0f;
            
            
            for (int x = 0; x < DepthLevel; x++)
            {
                for (int y = 0; y < DepthLevel; y++)
                {
                    GrassChunk chunk = new GrassChunk();
                    chunk.Position = new Vector3(x*chunkSize + halfChunkSize, 0, y*chunkSize + halfChunkSize);
                    chunk.Points = new[]
                    {
                        new Vector3(x*chunkSize + halfChunkSize - halfChunkSize, 0, y*chunkSize + halfChunkSize + halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize - halfChunkSize, 0, y*chunkSize + halfChunkSize - halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize + halfChunkSize, 0, y*chunkSize + halfChunkSize - halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize + halfChunkSize, 0, y*chunkSize + halfChunkSize + halfChunkSize),
                        
                        new Vector3(x*chunkSize + halfChunkSize - halfChunkSize, TerrainHeightMax, y*chunkSize + halfChunkSize + halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize - halfChunkSize, TerrainHeightMax, y*chunkSize + halfChunkSize - halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize + halfChunkSize, TerrainHeightMax, y*chunkSize + halfChunkSize - halfChunkSize),
                        new Vector3(x*chunkSize + halfChunkSize + halfChunkSize, TerrainHeightMax, y*chunkSize + halfChunkSize + halfChunkSize),
                    };
                    chunk.Size = chunkSize;
                    chunk.UpdateBounds(transform.localToWorldMatrix);
                    
                    // TODO DEBUG
                    if (GrassParamManage.USE_GIZMOS)
                    {
                        _gizmos.AddDebugPoint(chunk.Position, 5.0f, Color.green);
                        for (int i = 0; i < 4; i++)
                            _gizmos.AddDebugPoint(chunk.Points[i], 1f, Color.red);      
                    }
                    _chunks.Add(chunk);
                }
            }
        }

        private void UpdateChunksBounds()
        {
            foreach (var chunk in _chunks)
            {
                chunk.UpdateBounds(transform.localToWorldMatrix);
            }
        }

        private Camera _camera;

        public List<GrassChunk> GetChunks(Camera cam)
        {
            if(GrassParamManage.USE_GIZMOS)
                _gizmos.Clear();
            _camera = cam;
            var cameraPositionWS = cam.transform.position;
            /*
             * planes[0]：近平面（Near Plane）
             * planes[1]：远平面（Far Plane）
             * planes[2]：左平面（Left Plane）
             * planes[3]：右平面（Right Plane）
             * planes[4]：上平面（Top Plane）
             * planes[5]：下平面（Bottom Plane）
             */
            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            
            List<GrassChunk> list = new List<GrassChunk>();

            if(GrassParamManage.USE_GIZMOS)
                _gizmos.Clear();
            foreach (var chunk in _chunks)
            {
                bool inFrustum = GeometryUtility.TestPlanesAABB(planes, chunk.Bounds);
                bool isNear = chunk.ClosestPoint(cameraPositionWS) < GrassParamManage.MAX_DISPLAY_RANGE;
                
                if (inFrustum & isNear)
                {
                    list.Add(chunk);
                    if(GrassParamManage.USE_GIZMOS)
                        _gizmos.AddDebugCube(chunk.Position, chunk.Size, Color.blue);
                }
            }
            list.Sort((x, y) => x.CompareTo(y));

            return list;
        }

        

        // TODO DEBUG
        private void OnDrawGizmos()
        {
            if (!GrassParamManage.USE_GIZMOS)
            {
                _gizmos.Clear();
                return;
            }
            _gizmos.DrawGrassGizmos(transform.localToWorldMatrix, null);
            
            
            // Debug Camera Frustum
            if(_camera != null)
            {
                var color = Gizmos.color;
                Matrix4x4 oldMatrix = Gizmos.matrix;
            
                Gizmos.matrix = _camera.transform.localToWorldMatrix;
            
                Gizmos.DrawFrustum(Vector3.zero, _camera.fieldOfView, _camera.farClipPlane, _camera. nearClipPlane, _camera.aspect);
            
                Gizmos.color = color;
                Gizmos.matrix = oldMatrix;
            }
            
        }

    }
}
