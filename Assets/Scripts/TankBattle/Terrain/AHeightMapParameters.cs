using System;
using UnityEngine;

namespace TankBattle.Terrain
{
    [Serializable]
    public class AHeightMapParameters
    {
        public int xSize;
        public int zSize;
        public Vector2 offset;
        public int seed;
    }
}