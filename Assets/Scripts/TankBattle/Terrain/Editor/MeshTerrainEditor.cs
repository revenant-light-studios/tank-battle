using UnityEditor;
using UnityEngine;

namespace TankBattle.Terrain.Editor
{
    [CustomEditor(typeof(MeshTerrain))]
    public class MeshTerrainEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MeshTerrain _terrain = (MeshTerrain)target;
            
            if (DrawDefaultInspector())
            {
                if (_terrain.autoUpdate)
                {
                    _terrain.Generate();
                }
            }

            if (GUILayout.Button("Generate terrain"))
            {
                _terrain.Generate();
            }
        }
    }
}