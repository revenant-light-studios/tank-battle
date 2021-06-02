using TankBattle.Terrain.Noise;

namespace TankBattle.Terrain
{
    public class PerlinHeightMapGenerator : IHeightMapGenerator
    {
        private PerlinNoiseParameters parameters;

        public PerlinHeightMapGenerator(PerlinNoiseParameters parameters)
        {
            this.parameters = parameters;
        }

        public float[,] GenerateTerrainHeightMap()
        {
            return PerlinNoise.GenerateNoiseMap(
                parameters.xSize, 
                parameters.zSize, 
                parameters.offset, 
                parameters.seed, 
                parameters.scale, 
                parameters.octaves, 
                parameters.persistance, 
                parameters.lacunarity);
        }
    }
}