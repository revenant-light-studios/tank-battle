using UnityEditor;
using UnityEngine;

namespace TankBattle.InGameGUI.Editor
{
    [CustomEditor(typeof(HitImage))]
    public class HitImageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Simulate hit"))
            {
                HitImage hitImage = (HitImage)target;
                hitImage.HitFlash();
            }
        }
    }
}