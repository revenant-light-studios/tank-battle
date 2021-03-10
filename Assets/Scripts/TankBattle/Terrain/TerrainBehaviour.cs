using TankBattle.Terrain.Noise;
using UnityEngine;

namespace TankBattle.Terrain
{
    public class TerrainBehaviour : MonoBehaviour
    {
        public int width = 256;
        public int depth = 256;
        public int height = 20;

        public Vector2 offset;

        [Header("Perlin parameters")]
        public int seed = 1234;
        public float scale = 20f;
        public int octaves;
        [Range(0,1)] public float persistance;
        public float lacunarity;

        [SerializeReference] private IHeightMapGenerator _heightMapGenerator;
        private UnityEngine.Terrain _terrain;
        private PerlinNoiseRenderer _renderer;
        private PerlinNoiseParameters _perlinNoiseParameters = new PerlinNoiseParameters();

        public bool autoUpdate = false;
        
        public void Generate()
        {
            _renderer = GetComponent<PerlinNoiseRenderer>();
            _terrain = GetComponent<UnityEngine.Terrain>();
            _terrain.terrainData = GenerateTerrain(_terrain.terrainData);
        }
        
        public TerrainData GenerateTerrain(TerrainData terrainData)
        {
            terrainData.heightmapResolution = width + 1;
            terrainData.size = new Vector3(width, height, depth);
            
            _perlinNoiseParameters.xSize = width;
            _perlinNoiseParameters.zSize = depth;
            _perlinNoiseParameters.offset = offset;
            _perlinNoiseParameters.seed = seed;
            _perlinNoiseParameters.scale = scale;
            _perlinNoiseParameters.octaves = octaves;
            _perlinNoiseParameters.persistance = persistance;
            _perlinNoiseParameters.lacunarity = lacunarity;
            
            _heightMapGenerator = new PerlinHeightMapGenerator(_perlinNoiseParameters);
            float[,] heights = _heightMapGenerator.GenerateTerrainHeightMap();
            
            terrainData.SetHeights(0, 0, heights);
            _renderer.DrawNoiseMap(heights);
            return terrainData;
        }
    }
}