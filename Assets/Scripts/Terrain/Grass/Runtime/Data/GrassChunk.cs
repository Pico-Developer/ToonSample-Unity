using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Data
{
    public class GrassChunk
    {
        // 位置信息, 中心点位置
        public Vector3 Position;

        // 范围大小
        public float Size;
        
        // 四周的四个点
        public Vector3[] Points;

        public Bounds Bounds;

        private float _closestPointDistance;

        public void UpdateBounds(Matrix4x4 local2World)
        {
            Bounds = GeometryUtility.CalculateBounds(Points, local2World);
        }

        public float ClosestPoint(Vector3 point)
        {
            Vector3 p = Bounds.ClosestPoint(point);
            p.y = 0;
            point.y = 0;
            _closestPointDistance = Vector3.Distance(point, p);
            return _closestPointDistance;
        }

        public int CompareTo(GrassChunk chunk)
        {
            return chunk._closestPointDistance > _closestPointDistance ? -1 : 1;
        }
    }
}