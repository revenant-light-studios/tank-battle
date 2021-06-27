using TankBattle.Terrain.Noise;
using UnityEngine;

namespace TankBattle.Terrain
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshTerrain : MonoBehaviour
    {
        [SerializeField] private PerlinNoiseParameters _parameters;

        public float heightMultiplier;
        public AnimationCurve heightCurve;
        
        public bool autoUpdate;
        
        private PerlinHeightMapGenerator _heightMapGenerator;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private PerlinNoiseRenderer _renderer;

        private MeshData _meshData;
        
        public MeshData TerrainMeshData => _meshData;
        public PerlinNoiseParameters TerrainParameters => _parameters;

        public void Generate(int seed = -1)
        {
            if(seed != -1)
            {
                _parameters.seed = seed;
            }
            
            // Debug.Log($"Generating terrain with seed {_parameters.seed}");
            _heightMapGenerator = new PerlinHeightMapGenerator(_parameters);
            float[,] heights = _heightMapGenerator.GenerateTerrainHeightMap();

            _meshData = TerrainMeshGenerator.GenerateTerrainMesh(heights, heightMultiplier, heightCurve);
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = _meshData.CreateMesh();
            _meshFilter.sharedMesh.RecalculateBounds();
            
            // _renderer = GetComponent<PerlinNoiseRenderer>();
            // _renderer.DrawNoiseMap(heights);
            
            _meshCollider = GetComponent<MeshCollider>();
            _meshCollider.sharedMesh = _meshFilter.mesh;

            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            // Debug.Log($"World size ({bounds.size})");
        }
        
        public float GetHeight(int x, int z)
        {
            Vector3 vertex = _meshData.vertices[x + z * _parameters.zSize];
            return vertex.y;
        }
    }
}