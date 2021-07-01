using TankBattle.Items;
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

        [FormerlySerializedAs("NumberOfDummies"), InspectorName("Number of dummies to spawn"), Tooltip("Number of dummies to spawn"), Range(1, 10)]
        public int numberOfDummies;

        [Header("Secondary weapons settings")]
        [FormerlySerializedAs("SpawnSecondaryWeapons"), InspectorName("Spawn secondary weapons")]
        public bool spawnSecondaryWeapons;

        [FormerlySerializedAs("NumberOfSecondaryWeapons"), InspectorName("Number of secondary weapons"),
         Tooltip("Number of secondary weapons per player to spawn"), Range(1, 20)]
        public int numberOfSecondaryWeapons;

        [FormerlySerializedAs("SecondaryWeapons"), InspectorName("Secondary weapons"), Tooltip("Allowed secondary weapons")]
        public PickableItem[] secondaryWeapons;

        [Header("Tank settings")]
        [FormerlySerializedAs("TankSpeedMultiplier"), InspectorName("Tank speed multiplier"), Tooltip("Tank speed multiplier"), Range(1.0f, 2.0f)]
        public float TankSpeedMultiplier;
        
        [Header("Other settings")]
        [FormerlySerializedAs("ForceMobileLayout"), InspectorName("Force mobile"), Tooltip("Force mobile layout")]
        public bool forceMobile;
    }
}