using System;
using System.Collections;
using System.IO;
using ExitGames.Client.Photon;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Items;
using TankBattle.Tanks;
using TankBattle.Terrain;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace TankBattle.Navigation
{
    public class PlayRoomManager : MonoBehaviourPunCallbacks
    {
        #region public usefull stuff
        public static byte LoadingEvent = 100;

        private static PlayRoomManager _playRoomManagerInstance;

        public static PlayRoomManager Instance
        {
            get
            {
                if (_playRoomManagerInstance == null)
                {
                    throw new Exception("You cannot use the playroom manager without an instance in the scene");
                }
                
                return _playRoomManagerInstance;
            }
        }

        private GameObject _userUI;

        public GameObject UserUI
        {
            get => _userUI;
        }
        #endregion
        
        
        private MeshTerrain _terrain;
        private int _randomSeed;
        private Random _randomGenerator;
        
        private Vector3[] _spawnPoints;

        [SerializeField] private LoadingUI _LoadingUI;
        [SerializeField] private GameObject _tankPrefab;
        [SerializeField] private GameObject[] _tankAddOns;

        private int _numberOfDummies = 10;
        private bool _spawnDummies = false;
        private int _numberOfSecondaryWeapons = 10;
        private bool _spawnSecondaryWeapons = false;
        private PickableItem[] _secondaryWeaponTypes;
        
        public Random RandomGenerator
        {
            get
            {
                if (_randomGenerator == null)
                {
                    _randomGenerator = new Random(RandomSeed);        
                }

                return _randomGenerator;
            }
        }

        public int RandomSeed
        {
            get
            {
                if (_randomSeed==0)
                {
                    _randomSeed = (PhotonNetwork.InRoom) ? (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Seed] : Guid.NewGuid().GetHashCode();
                }

                return _randomSeed;
            }
        }

        private Text _debugSeedText;

        private void Awake()
        {
            _playRoomManagerInstance = this;
        }

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
            LoadScene();
        }

        #region Scene Loading

        private IEnumerator _loadCoroutine;
        private int _loadingProgess = 0;

        private void LoadScene()
        {
            _loadCoroutine = LoadSceneCoroutine();
            StartCoroutine(_loadCoroutine);
        }
        
        private IEnumerator LoadSceneCoroutine()
        {
            ShowInGameUI();
            
            _LoadingUI?.Show();
            
            yield return SendLoadingMessage(_loadingProgess, "Generando arena");
            
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");
            if (settings)
            {
                _spawnDummies = settings.spawnDummyTanks;
                _numberOfDummies = settings.numberOfDummies;
                _spawnSecondaryWeapons = settings.spawnSecondaryWeapons;
                _numberOfSecondaryWeapons = settings.numberOfSecondaryWeapons;
                _secondaryWeaponTypes = settings.secondaryWeapons;

                if (_secondaryWeaponTypes.Length == 0)
                {
                    _spawnSecondaryWeapons = false;
                }
            }
            
            _terrain = FindObjectOfType<MeshTerrain>();
            _terrain.Generate(RandomSeed);
            _debugSeedText.text = $"Terrain random seed: {_terrain.TerrainParameters.seed}";

            _loadingProgess += 50;
            yield return SendLoadingMessage(_loadingProgess, "Generando puntos de spawn");

            int numberOfDummySpawnPoints = _spawnDummies ? _numberOfDummies : 0;
            int numberOfSecondaryWeapons = _spawnSecondaryWeapons ? _numberOfSecondaryWeapons : 0;
            int numberOfPlayers = PhotonNetwork.IsConnected ? PhotonNetwork.CurrentRoom.PlayerCount : 1;
            int totalElementsToSpawn = numberOfPlayers + numberOfSecondaryWeapons + numberOfDummySpawnPoints;

            GenerateSpawnPoints(numberOfPlayers + numberOfDummySpawnPoints);
            _loadingProgess += (int)((numberOfPlayers + numberOfDummySpawnPoints) / totalElementsToSpawn * 0.5);
            yield return SendLoadingMessage(_loadingProgess, "Instanciando tanques");
                
            if (PhotonNetwork.IsConnected)
            {
                InstantiatePlayers();
            }
            else
            {
                Vector3 position = _spawnPoints[0];
                Instantiate(_tankPrefab, position, Quaternion.identity);
            }

            if (_spawnDummies)
            {
                SpawnDummyTanks(_numberOfDummies);
            }
            
            
            yield return SendLoadingMessage(_loadingProgess, "Generando armas secundarias");
            
            if (_spawnSecondaryWeapons)
            {
                SpawnSecondaryWeapons(numberOfSecondaryWeapons);
            }
            
            yield return SendLoadingMessage(1, "Comenzando partida");
            yield return new WaitForSeconds(1.0f);
            
            _LoadingUI.Hide();
        }

        private void ShowInGameUI()
        {
            Transform desktopUI = transform.FirstOrDefault(t => t.name == "UserUIDesktop");
            Transform mobileUI = transform.FirstOrDefault(t => t.name == "UserUIMobile");
            
            if (GlobalMethods.IsDesktop())
            {
                desktopUI.gameObject.SetActive(true);
                mobileUI.gameObject.SetActive(false);
                _userUI = desktopUI.gameObject;
            }
            else
            {
                desktopUI.gameObject.SetActive(false);
                mobileUI.gameObject.SetActive(true);
                _userUI = mobileUI.gameObject;
            }
            
            _debugSeedText = _userUI.transform.FirstOrDefault(t => t.name == "DebugSeed").GetComponent<Text>();
        }

        private YieldInstruction SendLoadingMessage(int progress, string message = "")
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return new WaitForEndOfFrame();

            if (PhotonNetwork.IsConnected)
            {
                object[] content = new object[] { progress, message };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(LoadingEvent, content, options, SendOptions.SendReliable);
            }
            else
            {
                _LoadingUI.ShowProgress(progress, message);
            }

            return new WaitForEndOfFrame();
        }

        #endregion
        
        #region Players instantiation
        private void InstantiatePlayers()
        {
            Player player = PhotonNetwork.LocalPlayer;
            Vector3 position = _spawnPoints[player.ActorNumber - 1];
            PhotonNetwork.Instantiate(Path.Combine("Tanks", _tankPrefab.name), position, Quaternion.identity);
        }

        private void GenerateSpawnPoints(int playerCount)
        {
            Random generator = RandomGenerator;
            _spawnPoints = new Vector3[playerCount];
            int sectors = 360 / playerCount;
            int xCenter = _terrain.TerrainParameters.xSize / 2;
            int zCenter = _terrain.TerrainParameters.zSize / 2;
            
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                int maxProbes = 2000;
                float x;
                float z;
                float terrainHeight = 0f;
                
                do
                {
                    float randomAngle = generator.Next(i * sectors, i * sectors + sectors) * Mathf.Deg2Rad;
                    float randomRadius = generator.Next((int)(xCenter * 0.5), (int)(xCenter * 0.9));
                    x = Mathf.Cos(randomAngle) * randomRadius;
                    z = Mathf.Sin(randomAngle) * randomRadius;
                    
                    // Debug.Log($"Trying spawnpoint {i} at angle: {randomAngle * Mathf.Rad2Deg}, radius: {randomRadius}");
                } while (!IsFreeSpot(x, z, xCenter, zCenter, out terrainHeight) && --maxProbes > 0);

                Vector3 spawnPosition = new Vector3(x * _terrain.transform.localScale.x, 1f, z * _terrain.transform.localScale.z); 
                
                // DEBUG CODE
                // Debug.Log($"Spawnpoint {i} [probes {maxProbes}, terrainHeigh {terrainHeight}] at ({x},1f,{z})");
                // GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // s.transform.position = spawnPosition; 
                // s.transform.localScale = Vector3.one * 30;
                // s.name = $"TankSpawn{i}";
                // s.GetComponent<Renderer>().material.color = Color.green;
                
                _spawnPoints[i] = spawnPosition;
            }
        }

        private bool IsFreeSpot(float x, float z, int xCenter, int zCenter, out float terrainHeight, float tolerance = 15f, float minDistance = 15f)
        {
            Vector3 center = new Vector3(x * _terrain.transform.localScale.x , 1f, z * _terrain.transform.localScale.z);
            Vector3 extents = new Vector3(tolerance, 0.2f, tolerance);
            Collider[] hits = Physics.OverlapBox(center, extents, Quaternion.identity);

            bool nearSpawnPoint = false;
             
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                if (Vector3.Distance(_spawnPoints[i], center) < minDistance)
                {
                    nearSpawnPoint = true;
                    break;
                }
            }

            terrainHeight = _terrain.GetHeight((int)x + xCenter, (int)z + zCenter);
            
            return !nearSpawnPoint && hits.Length == 0 && terrainHeight <= 0.01f;
        }

        private void SpawnDummyTanks(int numberOfTanks)
        {
            // Only the master client spawns dummies
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
            
            for (int i = 0; i < numberOfTanks; i++)
            {
                Vector3 position = _spawnPoints[_spawnPoints.Length - 1 - i];

                GameObject dummyTank;
                if (PhotonNetwork.IsConnected)
                {
                    dummyTank = PhotonNetwork.InstantiateRoomObject(Path.Combine("Tanks", _tankPrefab.name), position, Quaternion.identity);
                }
                else
                {
                    dummyTank = Instantiate(_tankPrefab, position, Quaternion.identity);    
                }
                
                dummyTank.name = $"Dummy{i}";
                dummyTank.GetComponent<TankManager>().IsDummy = true;
            }
        }
        #endregion

        #region Pickable items instantiation

        private void SpawnSecondaryWeapons(int numberOfSecondaryWeapons = 0)
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
            
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");

            if (!settings.spawnSecondaryWeapons) return;
            
            PickableItem[] items = settings.secondaryWeapons;

            Random generator = RandomGenerator;
            int sectors = 360 / numberOfSecondaryWeapons;
            int xCenter = _terrain.TerrainParameters.xSize / 2;
            int zCenter = _terrain.TerrainParameters.zSize / 2;
            
            for (int i = 0; i < numberOfSecondaryWeapons; i++)
            {
                int maxProbes = 1000;
                float x;
                float z;
                float terrainHeight = 0f;
                
                do
                {
                    float randomAngle = generator.Next(i * sectors, i * sectors + sectors) * Mathf.Deg2Rad;
                    float randomRadius = generator.Next((int)(xCenter * 0.3), (int)(xCenter * 0.9));
                    x = Mathf.Cos(randomAngle) * randomRadius;
                    z = Mathf.Sin(randomAngle) * randomRadius;
                } while (!IsFreeSpot(x, z, xCenter, zCenter, out terrainHeight, 25f, 25f) && --maxProbes > 0);

                Vector3 spawnPosition = new Vector3(x * _terrain.transform.localScale.x, 1f, z * _terrain.transform.localScale.z);
                
                // DEBUG CODE
                // Debug.Log($"Weapon Spawnpoint {i} [probes {maxProbes}, terrainHeigh {terrainHeight}] at ({x},1f,{z})");
                // GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // s.transform.position = spawnPosition; 
                // s.transform.localScale = Vector3.one * 5;
                // s.name = $"SecondaryWeapon{i}";
                // s.GetComponent<Renderer>().material.color = Color.red;

                int weponType = generator.Next(0, _secondaryWeaponTypes.Length);
                SpawnSecondaryWeapon(_secondaryWeaponTypes[weponType], spawnPosition);
            }
        }

        private void SpawnSecondaryWeapon(PickableItem item, Vector3 position)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.InstantiateRoomObject("Items/" + item.name, position, Quaternion.identity);
            }
            else
            {
                Instantiate(item, position, Quaternion.identity);
            }
        }
        #endregion
    }
}