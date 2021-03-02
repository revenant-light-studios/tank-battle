using UnityEngine;

namespace TankBattle.Terrain.Noise
{
    public class PerlinNoiseRenderer : MonoBehaviour
    {
        public Renderer textureRenderer;

        public void DrawNoiseMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Texture2D texture2D = new Texture2D(width, height);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[x + y * width] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }
            
            texture2D.SetPixels(colorMap);
            texture2D.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture2D;
        }
    }
}