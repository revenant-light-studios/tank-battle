
using UnityEditor;
using UnityEngine;

namespace TankBattle.Terrain.Editor
{
    [CustomEditor(typeof(TerrainBehaviour))]
    public class TerrainEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainBehaviour terrainBehaviour = (TerrainBehaviour)target;

            if (DrawDefaultInspector())
            {
                if (terrainBehaviour.autoUpdate)
                {
                    terrainBehaviour.Generate();
                }
            }

            if (GUILayout.Button("Generate terrain"))
            {
                terrainBehaviour.Generate();
            }
        }
    }
}