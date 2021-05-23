using System;
using System.IO;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Compression;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Tanks;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Guns;
using TankBattle.Terrain;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class PlayRoomManager : MonoBehaviourPunCallbacks
    {
        private GameObject _MobileUI;
        private GameObject _DesktopUI;
        
        [SerializeField, FormerlySerializedAs("IsDesktop")]
        private bool _isDesktop = false;

        public bool IsDesktop
        {
            get { return _isDesktop; }
        }

        private MeshTerrain _terrain;
        private int _randomSeed;
        private System.Random _randomGenerator;
        
        private Vector3[] _spawnPoints;

        [SerializeField] private GameObject _tankPrefab;
        [SerializeField] private int _numberOfDummies;
        [SerializeField] private bool _spawnDummies;

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

        private void Awake()
        {
            _DesktopUI = GameObject.Find("UserUI");
            _MobileUI = GameObject.Find("UserUIMobile");
        }

        private void Start()
        {
            if (_isDesktop)
            {
                _MobileUI.SetActive(false);
            }
            else
            {
                _DesktopUI.SetActive(false);
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Cursor.lockState = CursorLockMode.Confined;
            
            _randomSeed = (PhotonNetwork.InRoom) ? (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Seed] : Guid.NewGuid().GetHashCode();
            _randomGenerator = new System.Random(_randomSeed);
            
            Canvas canvas = FindObjectOfType<Canvas>();
            _debugSeedText = canvas.transform.FirstOrDefault(t => t.name == "DebugSeed").GetComponent<Text>();

            _terrain = FindObjectOfType<MeshTerrain>();
            _terrain.Generate(_randomSeed);
            _debugSeedText.text = $"Terrain random seed: {_terrain.TerrainParameters.seed}";

            int numberOfDummySpawnPoints = _spawnDummies ? _numberOfDummies : 0; 
            
            if (PhotonNetwork.IsConnected)
            {
                
                GenerateSpawnPoints(PhotonNetwork.CurrentRoom.PlayerCount + numberOfDummySpawnPoints);
                InstantiatePlayers();
            }
            else
            {
                GenerateSpawnPoints(1 + numberOfDummySpawnPoints);
                Vector3 position = _spawnPoints[0];
                Instantiate(_tankPrefab, position, Quaternion.identity);
                // Debug.Log($"Instantiated tank at position {position}");
            }

            if (_spawnDummies)
            {
                SpawnDummyTanks(_numberOfDummies);
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
                int maxProbes = 1000;
                float x;
                float z;
                
                do
                {
                    float randomAngle = generator.Next(i * sectors, i * sectors + sectors) * Mathf.Deg2Rad;
                    float randomRadius = generator.Next((int)(xCenter * 0.5), (int)(xCenter * 0.9));
                    x = Mathf.Cos(randomAngle) * randomRadius;
                    z = Mathf.Sin(randomAngle) * randomRadius;
                    Debug.Log($"Trying spawnpoint {i} at angle: {randomAngle * Mathf.Rad2Deg}, radius: {randomRadius}");
                } while (!IsFreeSpot(x, z, xCenter, zCenter) && --maxProbes > 0);

                // TODO: Spawn point y depends on height?
                Debug.Log($"Spawning {i} at ({x},1f,{z})");
                _spawnPoints[i] = new Vector3(x * 2, 1f, z * 2);
            }
        }

        private bool IsFreeSpot(float x, float z, int xCenter, int zCenter)
        {
            Vector3 center = new Vector3(x * 2, 1f, z * 2);
            Vector3 extents = new Vector3(15f, 0.2f, 15f);
            Collider[] hits = Physics.OverlapBox(center, extents, Quaternion.identity);
            return hits.Length == 0 && _terrain.GetHeight((int)x + xCenter, (int)z + zCenter) <= 0.0f;
        }

        private void SpawnDummyTanks(int numberOfTanks)
        {
            for (int i = 0; i < numberOfTanks; i++)
            {
                Vector3 position = _spawnPoints[_spawnPoints.Length - 1 - i];
                GameObject dummyTank = Instantiate(_tankPrefab, position, Quaternion.identity);
                dummyTank.name = $"Dummy{i}";
                dummyTank.GetComponent<TankManager>().IsDummy = true;
                
                // dummyTank.GetComponent<PlayerInput>().enabled = false;
                // dummyTank.GetComponent<CameraFollow>().enabled = false;
                // dummyTank.GetComponent<TankGun>().enabled = false;
            }
        }
    }
}