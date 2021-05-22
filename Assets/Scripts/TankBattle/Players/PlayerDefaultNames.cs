using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TankBattle.Players
{
    [CreateAssetMenu(fileName = "NamesList", menuName = "TankBattle/NamesList", order = 0)]
    public class PlayerDefaultNames : ScriptableObject
    {
        public List<string> Names;

        public void SortNames()
        {
            Names.Sort();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerDefaultNames))]
    public class PlayerDefaultNamesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Sort list"))
            {
                PlayerDefaultNames names = (PlayerDefaultNames)target;
                names.SortNames();
            }
        }
    }
#endif
}