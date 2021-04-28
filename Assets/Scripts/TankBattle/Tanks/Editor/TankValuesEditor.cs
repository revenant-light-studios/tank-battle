using UnityEditor;
using UnityEngine;

namespace TankBattle.Tanks.Editor
{
    [CustomEditor(typeof(TankValues))]
    public class TankValuesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Take hit"))
            {
                TankValues values = (TankValues)target;
                values.WasHit();
            }
        }
    }
}