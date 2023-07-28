using System.Collections.Generic;
using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comm
{
    public class GrassGizmos
    {
        private readonly List<Vector3> _line = new List<Vector3>();
        private readonly List<Vector3> _lineDirection = new List<Vector3>();
        private readonly List<Color> _lineColor = new List<Color>();

        private readonly List<Vector3> _sphere = new List<Vector3>();
        private readonly List<float> _sphereSize = new List<float>();
        private readonly List<Color> _sphereColor = new List<Color>();

        private readonly List<Vector3> _cube = new List<Vector3>();
        private readonly List<float> _cubeSize = new List<float>();
        private readonly List<Color> _cubeColor = new List<Color>();

        public void AddDebugPoint(Vector3 point, float radius, Color color)
        {
            _sphere.Add(point);
            _sphereSize.Add(radius);
            _sphereColor.Add(color);
        }

        public void AddDebugPoint(Vector3 point)
        {
            AddDebugPoint(point, 1f, Color.white);
        }

        public void AddDebugPoint(Vector3 point, float radius)
        {
            AddDebugPoint(point, radius, Color.white);
        }
        
        public void AddDebugPoint(Vector3 point, Color color)
        {
            AddDebugPoint(point, 1f, Color.white);
        }
        
        public void AddDebugCube(Vector3 point, float size, Color color)
        {
            _cube.Add(point);
            _cubeSize.Add(size);
            _cubeColor.Add(color);
        }

        public void AddDebugCube(Vector3 point)
        {
            AddDebugCube(point, 1f, Color.white);
        }

        public void AddDebugCube(Vector3 point, float size)
        {
            AddDebugCube(point, size, Color.white);
        }
        
        public void AddDebugCube(Vector3 point, Color color)
        {
            AddDebugCube(point, 1f, Color.white);
        }

        public void AddDebugLine(Vector3 point, Vector3 normal, Color color)
        {
            _line.Add(point);
            _lineDirection.Add(normal);
            _lineColor.Add(color);
        }
        
        public void AddDebugLine(Vector3 point, Vector3 normal)
        {
            AddDebugLine(point, normal, Color.magenta);
        }

        public void Clear()
        {
            _line.Clear();
            _lineDirection.Clear();
            _lineColor.Clear();
            _sphere.Clear();
            _sphereSize.Clear();
            _sphereColor.Clear();
            _cube.Clear();
            _cubeSize.Clear();
            _cubeColor.Clear();
        }

        private void DrawDebugLine()
        {
            for (int i = 0; i < _line.Count; i++)
            {
                Gizmos.color = (Color)_lineColor[i];
                Gizmos.DrawLine((Vector3)_line[i], (Vector3)_line[i] + 0.5f * (Vector3)_lineDirection[i]);
            }
        }
        private void DrawDebugSphere()
        {
            for (int i = 0; i < _sphere.Count; i++)
            {
                Gizmos.color = _sphereColor[i];
                Gizmos.DrawSphere(_sphere[i], _sphereSize[i]);
            }
        }
        
        private void DrawDebugCube()
        {
            for (int i = 0; i < _cube.Count; i++)
            {
                Gizmos.color = _cubeColor[i];
                Gizmos.DrawWireCube(_cube[i], new Vector3(_cubeSize[i], 1, _cubeSize[i]));
            }
        }

        private void DrawMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                return;
            }
            Gizmos.color = Color.black;
            Gizmos.DrawWireMesh(mesh);
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(mesh.vertices[i], 0.01f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(mesh.vertices[i], mesh.vertices[i] + 0.5f * mesh.normals[i]);
            }
        }


        public void DrawGrassGizmos(Matrix4x4 localToWorldMatrix, Mesh mesh)
        {
            var color = Gizmos.color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            
            Gizmos.matrix = localToWorldMatrix;




            DrawMesh(mesh);
            DrawDebugLine();
            DrawDebugSphere();
            DrawDebugCube();
            
            Gizmos.color = color;
            Gizmos.matrix = oldMatrix;
        }
    }
    
    
}