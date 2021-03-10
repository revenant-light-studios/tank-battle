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

        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            _heightMapGenerator = new PerlinHeightMapGenerator(_parameters);
            float[,] heights = _heightMapGenerator.GenerateTerrainHeightMap();

            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = TerrainMeshGenerator.GenerateTerrainMesh(heights, heightMultiplier, heightCurve).CreateMesh();
            _meshFilter.sharedMesh.RecalculateBounds();
            
            _renderer = GetComponent<PerlinNoiseRenderer>();
            _renderer.DrawNoiseMap(heights);
            
            _meshCollider = GetComponent<MeshCollider>();
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }
    }
}