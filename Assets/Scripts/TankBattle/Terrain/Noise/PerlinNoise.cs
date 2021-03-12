using System;
using UnityEngine;

namespace TankBattle.Terrain.Noise
{
    public static class PerlinNoise
    {
        public static float[,] GenerateNoiseMap(int width, int height, Vector2 offset, int seed, float scale, int octaves, float persistance, float lacunarity)
        {
            float[,] noiseMap = new float[width, height];
            float maxNoise = float.MinValue;
            float minNoise = float.MaxValue;
            float halfWidth = 0.5f * width;
            float halfHeight = 0.5f * height;

            Debug.Log($"Generating perlin noise map with {seed}");
            System.Random perlinRandom = new System.Random(seed != 0 ? seed : Guid.NewGuid().GetHashCode());
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = perlinRandom.Next(-100000, 100000) + offset.x;
                float offsetY = perlinRandom.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            
            scale = Mathf.Max(0.0001f, scale);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }                    
                    
                    maxNoise = (noiseHeight > maxNoise) ? noiseHeight : maxNoise;
                    minNoise = (noiseHeight < minNoise) ? noiseHeight : minNoise;
                    
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
                }
            }
            
            return noiseMap;
        }
    }
}