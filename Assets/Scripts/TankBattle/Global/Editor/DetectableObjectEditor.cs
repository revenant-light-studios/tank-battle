using UnityEditor;
using UnityEngine;

namespace TankBattle.Global.Editor
{
    [CustomEditor(typeof(DetectableObject))]
    public class DetectableObjectEditor : UnityEditor.Editor
    {
        private DetectableObject obj;
        GUIStyle style;
        private void Awake()
        {
            obj = (DetectableObject)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            style = new GUIStyle(GUI.skin.textField);
            EditorGUILayout.LabelField("Tracker: ", obj.RadarTrack != null ? obj.RadarTrack.gameObject.name : "Not assigned", style);
        }
    }
}