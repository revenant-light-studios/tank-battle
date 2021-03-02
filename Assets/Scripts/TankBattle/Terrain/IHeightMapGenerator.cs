using UnityEngine;

namespace TankBattle.Terrain
{
    public interface IHeightMapGenerator
    {
        float[,] GenerateTerrainHeightMap();
    }
}