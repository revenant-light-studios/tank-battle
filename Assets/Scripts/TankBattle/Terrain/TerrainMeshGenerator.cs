using UnityEngine;

namespace TankBattle.Terrain
{
    public static class TerrainMeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve curve)
        {
            int width = heightMap.GetLength(0);
            int depth = heightMap.GetLength(1);

            float topLeftX = (width - 1) * -0.5f;
            float topLeftZ = (depth - 1) * 0.5f;

            MeshData meshData = new MeshData(width, depth);
            int vertexIndex = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float height = curve.Evaluate(heightMap[x, z]) * heightMultiplier;

                    if (x < 5f || x > width - 5f)
                    {
                        height = heightMultiplier;
                    } else if (z < 5f || z > depth - 5f)
                    {
                        height = heightMultiplier;
                    }
                    
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, height, topLeftZ - z);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)width, z / (float)depth);

                    if (x < width - 1 && z < depth - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int triangleIndex;

        public MeshData(int meshWidth, int meshDepth)
        {
            vertices = new Vector3[meshWidth * meshDepth];
            uvs = new Vector2[meshWidth * meshDepth];
            triangles = new int[(meshWidth - 1) * (meshDepth - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex+1] = b;
            triangles[triangleIndex+2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}