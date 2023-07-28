using System.Collections.Generic;
using GamePlus.Urp.Terrain.Grass.Runtime.Comp;
using GamePlus.Urp.Terrain.Grass.Runtime.Data;
using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comm
{
    public class RootChunkFilter
    {
        private Dictionary<GrassRoot, List<GrassChunk>> _data = new Dictionary<GrassRoot, List<GrassChunk>>();
        private List<GrassChunk> _empty = new List<GrassChunk>();

        public void Clear()
        {
            foreach (var v in _data)
            {
                v.Value?.Clear();
            }
            _data.Clear();
        }

        public void Generate(Camera camera)
        {
            Clear();
            var objs = Object.FindObjectsOfType<GrassRoot>();
            if (objs.Length == 0)
            {
                return;
            }
            
            foreach (var grassRoot in objs)
            {
                if(!grassRoot.gameObject.activeSelf || !grassRoot.isActiveAndEnabled) continue;

                var chunks = grassRoot.GetChunks(camera);

                if (chunks.Count > 0)
                {
                    _data.Add(grassRoot, chunks);
                }
            }
        }

        public ICollection<GrassRoot> GetAllRoot()
        {
            return _data.Keys;
        }
        
        public List<GrassChunk> GetChunksByKey(GrassRoot root)
        {
            if (!_data.ContainsKey(root))
            {
                return _empty;
            }

            var chunks = _data[root];
            if (chunks == null || chunks.Count == 0)
            {
                return _empty;
            }

            return chunks;
        }

        public bool IsEmpty()
        {
            return _data.Count == 0;
        }
    }
}