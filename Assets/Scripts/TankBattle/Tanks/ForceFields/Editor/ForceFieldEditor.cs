using UnityEditor;
using UnityEngine;

namespace TankBattle.Tanks.ForceFields.Editor
{
    [CustomEditor(typeof(ForceField))]
    public class ForceFieldEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Simulate hit"))
            {
                ForceField _forceField = (ForceField)target;
                _forceField.ForceFieldHit();
            }
        }
    }
}