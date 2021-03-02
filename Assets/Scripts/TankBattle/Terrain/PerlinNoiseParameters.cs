using System;
using UnityEngine;

namespace TankBattle.Terrain
{
    [Serializable]
    public class PerlinNoiseParameters : AHeightMapParameters
    {
        public float scale;
        public int octaves;
        [Range(0.01f, 1f)]public float persistance;
        public float lacunarity;
    }
}