using System;
using System.IO;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Tanks;
using TankBattle.Terrain;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class PlayRoomManager : MonoBehaviourPunCallbacks
    {
        private MeshTerrain _terrain;
        private int _randomSeed;
        private System.Random _randomGenerator;
        
        private Vector3[] _spawnPoints;

        [SerializeField] private GameObject _tankPrefab;

        public System.Random RandomGenerator
        {
            get => _randomGenerator;
        }

        private Text _debugSeedText;
        
        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _randomSeed = (PhotonNetwork.InRoom) ? (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Seed] : Guid.NewGuid().GetHashCode();
            _randomGenerator = new System.Random(_randomSeed);
            
            Canvas canvas = FindObjectOfType<Canvas>();
            _debugSeedText = canvas.transform.FirstOrDefault(t => t.name == "DebugSeed").GetComponent<Text>();

            _terrain = FindObjectOfType<MeshTerrain>();
            _terrain.Generate(_randomSeed);
            _debugSeedText.text = $"Terrain random seed: {_terrain.TerrainParameters.seed}";
            
            if (PhotonNetwork.IsConnected)
            {
                GenerateSpawnPoints(PhotonNetwork.CurrentRoom.PlayerCount);
                InstantiatePlayers();
            }
            else
            {
                GenerateSpawnPoints(1);
                Vector3 position = _spawnPoints[0];
                Instantiate(_tankPrefab, position, Quaternion.identity);
                // Debug.Log($"Instantiated tank at position {position}");
            }
        }

        private void InstantiatePlayers()
        {
            Player player = PhotonNetwork.LocalPlayer;
            Vector3 position = _spawnPoints[player.ActorNumber - 1];
            PhotonNetwork.Instantiate(Path.Combine("Tanks", _tankPrefab.name), position, Quaternion.identity);
        }

        private void GenerateSpawnPoints(int playerCount)
        {
            System.Random generator = new System.Random(_randomSeed);
            _spawnPoints = new Vector3[playerCount];
            int sectors = 360 / playerCount;
            int xCenter = _terrain.TerrainParameters.xSize / 2;
            int zCenter = _terrain.TerrainParameters.zSize / 2;

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                float x;
                float z;
                
                do
                {
                    float randomAngle = generator.Next(i * sectors, i * sectors + sectors) * Mathf.Deg2Rad;
                    float randomRadius = generator.Next((int)(xCenter * 0.5), (int)(xCenter * 0.9));
                    x = xCenter + Mathf.Cos(randomAngle) * randomRadius;
                    z = zCenter + Mathf.Sin(randomAngle) * randomRadius;
                    // Debug.Log($"Checking if valid spawn point at ({x},{z})");
                } while (_terrain.GetHeight((int)x, (int)z) > 0.0f || !IsFreeSpot(x, z));

                _spawnPoints[i] = new Vector3(x, 0.2f, z);
            }
        }

        private bool IsFreeSpot(float x, float z)
        {
            Vector3 center = new Vector3(x, 0.5f, z);
            Vector3 extents = new Vector3(10f, 0.2f, 10f);
            Collider[] hits = Physics.OverlapBox(center, extents, Quaternion.identity);
            return hits.Length == 0;
        }
    }
}