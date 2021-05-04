using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TankBattle.Tanks.ForceField.Editor
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