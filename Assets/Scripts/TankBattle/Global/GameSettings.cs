using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Global
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TankBattle/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Public game settings")]
        [FormerlySerializedAs("MinPlayers"), InspectorName("Minimum players to start"), Tooltip("Minimum number of players to start")]
        public int minimumNumberOfPlayers;

        [FormerlySerializedAs("MaxPlayers"), InspectorName("Maximum players"), Tooltip("Maximum number of players per room")]
        public int maximumNumberOfPlayers;

        [FormerlySerializedAs("StartWaitTime"), InspectorName("Time to start"), Tooltip("Timer value when minimum number of players reached")]
        public float startWaitTime;

        [Header("Dummy tanks settings")]
        [FormerlySerializedAs("SpawnDummyTanks"), InspectorName("Spawn dummy tanks"), Tooltip("Spawn some dummy tanks in game ")]
        public bool spawnDummyTanks;

        [FormerlySerializedAs("NumberOfDummies"), InspectorName("Number of dummies to spawn"), Tooltip("Number of dummies to spawn")]
        public int numberOfDummies;

        [Header("Other settings")]
        [FormerlySerializedAs("ForceMobileLayout"), InspectorName("Force mobile"), Tooltip("Force mobile layout")]
        public bool forceMobile;
    }
}