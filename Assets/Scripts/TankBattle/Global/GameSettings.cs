using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Global
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TankBattle/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Public game options")]
        [FormerlySerializedAs("MinPlayers"), InspectorName("Minimum players to start"), Tooltip("Minimum number of players to start")]
        public int minimumNumberOfPlayers;

        [FormerlySerializedAs("MaxPlayers"), InspectorName("Maximum players"), Tooltip("Maximum number of players per room")]
        public int maximumNumberOfPlayers;

        [FormerlySerializedAs("StartWaitTime"), InspectorName("Time to start"), Tooltip("Timer value when minimum number of players reached")]
        public float startWaitTime;
    }
}