using System;
using System.Collections;
using System.Collections.Generic;
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
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace TankBattle.Navigation
{
    public class PlayRoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        #region public usefull static stuff
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

        private TankManager _userTank;
        public TankManager UserTank
        {
            get => _userTank;
        }

        public void RegisterLocalTank(TankManager _tankManager)
        {
            _userTank = _tankManager;
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
        private bool _loadingFinished = false;

        private void LoadScene()
        {
            _loadCoroutine = LoadSceneCoroutine();
            StartCoroutine(_loadCoroutine);
        }
        
        private IEnumerator LoadSceneCoroutine()
        {
            ShowInGameUI();
            _LoadingUI.gameObject.SetActive(true);
            
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");

            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                
                _spawnDummies = settings.spawnDummyTanks;
                _numberOfDummies = settings.numberOfDummies;
                _spawnSecondaryWeapons = settings.spawnSecondaryWeapons;
                _numberOfSecondaryWeapons = settings.numberOfSecondaryWeapons;
            }
            else
            {
                _numberOfDummies = GlobalMethods.NumberOfDummies;
                _spawnDummies = _numberOfDummies > 0;
                _numberOfSecondaryWeapons = GlobalMethods.NumberOfSecondaryGuns;
                _spawnSecondaryWeapons = _numberOfSecondaryWeapons > 0;
            }
            
            _secondaryWeaponTypes = settings.secondaryWeapons;

            if (_secondaryWeaponTypes.Length == 0)
            {
                _spawnSecondaryWeapons = false;
            }

            int numberOfSecondaryWeapons = _spawnSecondaryWeapons ? _numberOfSecondaryWeapons : 0;
            int numberOfDummies = _spawnDummies ? _numberOfDummies : 0;
            int numberOfPlayers = PhotonNetwork.IsConnected ? PhotonNetwork.CurrentRoom.PlayerCount : 1;
            int totalElementsToLoad = numberOfSecondaryWeapons + numberOfDummies + numberOfPlayers + 1;
            
            _LoadingUI?.Show(totalElementsToLoad);
            
            yield return SendLoadingMessage(_loadingProgess, "Generando arena");
            _terrain = FindObjectOfType<MeshTerrain>();
            _terrain.Generate(RandomSeed);
            if(_debugSeedText != null) _debugSeedText.text = $"Terrain random seed: {_terrain.TerrainParameters.seed}";
            _loadingProgess++;

            if (PhotonNetwork.IsMasterClient)
            {
                // Master client is responsible for all object spawning
                yield return StartCoroutine(SpawnTanks());
                yield return StartCoroutine(SpawnSecondaryWeapons(numberOfSecondaryWeapons));
                yield return SendLoadingMessage(_loadingProgess, "Comenzando partida");

                // Comunicate all clients that loading has finished
                object[] content = new object[] {};
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(LoadingFinished, content, options, SendOptions.SendReliable);
            }
            else
            {
                yield return WaitForEndOfLoad();
            }
            
            yield return new WaitForSeconds(1.0f);
            _LoadingUI.Hide();
        }
        
        

        private IEnumerator WaitForEndOfLoad()
        {
            while (!_loadingFinished)
            {
                yield return new WaitForEndOfFrame();
            }
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
            
            _debugSeedText = _userUI.transform.FirstOrDefault(t => t.name == "DebugSeed")?.GetComponent<Text>();
        }

        public static byte LoadingEvent = 100;
        public static byte LoadingFinished = 101;

        private YieldInstruction SendLoadingMessage(int progress, string message = "")
        {
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
        public const byte SpawnPlayer = 102;

        private IEnumerator SpawnTanks()
        {
            int numberOfDummySpawnPoints = _spawnDummies ? _numberOfDummies : 0;
            int numberOfPlayers = PhotonNetwork.IsConnected ? PhotonNetwork.CurrentRoom.PlayerCount : 1;
            int totalTanksToSpawn = numberOfPlayers + numberOfDummySpawnPoints;
            int degrees = 360 / totalTanksToSpawn;
            
            yield return StartCoroutine(SpawnPlayerTanks(degrees));
            yield return StartCoroutine(SpawnDummyTanks(numberOfDummySpawnPoints, degrees, numberOfPlayers));
        }
        
        private IEnumerator SpawnDummyTanks(int numberOfDummys, int sectorDegrees, int startSector = 0)
        {
            if (numberOfDummys > 0)
            {
                yield return SendLoadingMessage(_loadingProgess, "Generando tanques dummy");
            }
            
            Debug.Log($"Start spawning Dummy tanks");
            for (int i = 0; i < numberOfDummys; i++)
            {
                int mySectorStart = sectorDegrees * (i + startSector);
                int mySectorEnd = mySectorStart + sectorDegrees;
                Vector3 spawnPosition = GenerateSpawnPoint(mySectorStart, mySectorEnd);
                string name = $"Dummy{i}";
                Debug.Log($"Spawning dummy[{i}] {name} in sector ({mySectorStart},{mySectorEnd}) position {spawnPosition}");
                
                GameObject dummyTank = PhotonNetwork.InstantiateRoomObject(Path.Combine("Tanks", _tankPrefab.name), spawnPosition, Quaternion.identity);
                dummyTank.name = name;
                dummyTank.GetComponent<TankManager>().IsDummy = true;
                
                _loadingProgess += 1;
                yield return SendLoadingMessage(_loadingProgess, $"Generado dummy {name}");
            }

            yield return new WaitForSeconds(1);
        }

        private IEnumerator SpawnPlayerTanks(int sectorDegrees, int startSector = 0)
        {
            Debug.Log($"Start spawning Player tanks");
            yield return SendLoadingMessage(_loadingProgess, "Generando tanques de los jugadores");
            
            int i = startSector;

            foreach (KeyValuePair<int,Player> valuePair in PhotonNetwork.CurrentRoom.Players)
            {
                Player player = valuePair.Value;
                int mySectorStart = sectorDegrees * i;
                int mySectorEnd = mySectorStart + sectorDegrees;
                Vector3 spawnPosition = GenerateSpawnPoint(mySectorStart, mySectorEnd);
                Debug.Log($"Spawning player {player.NickName} tank in sector ({mySectorStart},{mySectorEnd}) position {spawnPosition}");
                
                object[] content = new object[] { spawnPosition };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ActorNumber }
                };
                PhotonNetwork.RaiseEvent(SpawnPlayer, content, raiseEventOptions, SendOptions.SendReliable);
                i++;
                
                _loadingProgess++;
                yield return SendLoadingMessage(_loadingProgess, $"Generado tanque del jugador {player.NickName}");
            }
            
            yield return new WaitForSeconds(1);
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == SpawnPlayer)
            {
                object[] data = (object[])photonEvent.CustomData;
                Vector3 spawnPosition = (Vector3)data[0];

                Player player = PhotonNetwork.LocalPlayer;
                PhotonNetwork.Instantiate(Path.Combine("Tanks", _tankPrefab.name), spawnPosition, Quaternion.identity);
            } else if (eventCode == LoadingFinished)
            {
                _loadingFinished = true;
            }
        }
        
        private Vector3 GenerateSpawnPoint(int sectorStart, int sectorEnd, int maxProbes = 2000, float tolerance = 15f, float minDistance = 15f)
        {
            Random generator = RandomGenerator;
            int xCenter = _terrain.TerrainParameters.xSize / 2;
            int zCenter = _terrain.TerrainParameters.zSize / 2;
            float x, z;

            do
            {
                float randomAngle = generator.Next(sectorStart, sectorEnd) * Mathf.Deg2Rad;
                float randomRadius = generator.Next((int)(xCenter * 0.5), (int)(xCenter * 0.9));
                x = Mathf.Cos(randomAngle) * randomRadius;
                z = Mathf.Sin(randomAngle) * randomRadius;
                    
                // Debug.Log($"Trying spawnpoint {i} at angle: {randomAngle * Mathf.Rad2Deg}, radius: {randomRadius}");
            } while (!IsFreeSpot(x, z, xCenter, zCenter, out float terrainHeight, tolerance, minDistance) && --maxProbes > 0);
            
            Vector3 spawnPosition = new Vector3(x * _terrain.transform.localScale.x, 1f, z * _terrain.transform.localScale.z);
            
            // DEBUG CODE
            // Debug.Log($"Weapon Spawnpoint {i} [probes {maxProbes}, terrainHeigh {terrainHeight}] at ({x},1f,{z})");
            // GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // s.transform.position = spawnPosition; 
            // s.transform.localScale = Vector3.one * 5;
            // s.name = $"SecondaryWeapon{i}";
            // s.GetComponent<Renderer>().material.color = Color.red;
            
            return spawnPosition;
        }
        
        private bool IsFreeSpot(float x, float z, int xCenter, int zCenter, out float terrainHeight, float tolerance = 15f, float minDistance = 15f)
        {
            Vector3 center = new Vector3(x * _terrain.transform.localScale.x , 1f, z * _terrain.transform.localScale.z);
            Vector3 extents = new Vector3(tolerance, 0.2f, tolerance);
            Collider[] hits = Physics.OverlapBox(center, extents, Quaternion.identity);
            terrainHeight = _terrain.GetHeight((int)x + xCenter, (int)z + zCenter);
            return hits.Length == 0 && terrainHeight <= 0.01f;
        }
        #endregion

        #region Pickable items instantiation

        private IEnumerator SpawnSecondaryWeapons(int numberOfSecondaryWeapons = 0)
        {
            if (numberOfSecondaryWeapons > 0)
            {
                yield return SendLoadingMessage(_loadingProgess, "Generando armas secundarias");
            }
            
            Random generator = RandomGenerator;
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");
            PickableItem[] items = settings.secondaryWeapons;

            int sectorDegrees = 360 / numberOfSecondaryWeapons;
            
            for (int i = 0; i < numberOfSecondaryWeapons; i++)
            {
                int mySectorStart = sectorDegrees * i;
                int mySectorEnd = mySectorStart + sectorDegrees;
                Vector3 spawnPosition = GenerateSpawnPoint(mySectorStart, mySectorEnd, 1000, 25f, 25f);
                spawnPosition.y = 0.1f;
                
                int weaponType = generator.Next(0, _secondaryWeaponTypes.Length);
                SpawnSecondaryWeapon(_secondaryWeaponTypes[weaponType], spawnPosition);
                
                Debug.Log($"Spawning weapon {_secondaryWeaponTypes[weaponType].ToString()} tank in sector ({mySectorStart},{mySectorEnd}) position {spawnPosition}");
                
                _loadingProgess++;
                yield return SendLoadingMessage(_loadingProgess, $"Generando {_secondaryWeaponTypes[weaponType].name}");
            }
            
            yield return new WaitForSeconds(1);
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
        
        #region End scene management
        public void ExitPlay()
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene("Lobby");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyPlayerObjects(otherPlayer);   
            }
        }
        #endregion
    }
}