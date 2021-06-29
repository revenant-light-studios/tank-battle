using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Global;
using TankBattle.Navigation.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    [RequireComponent(typeof(PhotonView))]
    public class WaitingRoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerElemPrefab;

        [SerializeField, FormerlySerializedAs("GameSettings")]
        private GameSettings _settings;
        
        private float _maxWaitTime = 15.0f;
        private int _minNumberOfPlayers = 2;
        private int _maxNumberOfPlayers = 20;

        private TimeoutText _timerController;

        private Transform _playersList;
        private NavigationsButtons _navBtns;
        private Button _leaveRoomBtn;
        private Button _startBtn;
        private Text _timeToStartText;
        private Text _roomKey;
        private Text _numPlayersText;
        
        public delegate void OnExitWaitingRoomDelegate();
        public OnExitWaitingRoomDelegate OnExitWaitingRoom;

        public delegate void OnStartGameDelegate();
        public OnStartGameDelegate OnStartGame;

        //Navigation
        public delegate void OnGoSettingsDelegate();
        public OnGoSettingsDelegate OnGoSettings;

        public delegate void OnGoCreditsDelegate();
        public OnGoCreditsDelegate OnGoCredits;

        public virtual void Awake()
        {
            if (_settings != null)
            {
                _maxWaitTime = _settings.startWaitTime;
                _minNumberOfPlayers = _settings.minimumNumberOfPlayers;
                _maxNumberOfPlayers = _settings.maximumNumberOfPlayers;
            }

            Transform playerList = transform.FirstOrDefault(t=>t.name== "PlayerList");
            _playersList = playerList.FirstOrDefault(t => t.name == "Content");
            
            _navBtns = transform.FirstOrDefault(t => t.name == "NavigationBtns").GetComponent<NavigationsButtons>();
            _leaveRoomBtn = transform.FirstOrDefault(t => t.name == "LeaveBtn").GetComponent<Button>();
            _startBtn = transform.FirstOrDefault(t => t.name == "StartBtn").GetComponent<Button>();
            _timeToStartText = transform.FirstOrDefault(t => t.name == "TimeToStartText").GetComponent<Text>();
            _roomKey = transform.FirstOrDefault(t => t.name == "RoomKey").GetComponent<Text>();
            _numPlayersText = transform.FirstOrDefault(t => t.name == "NumPlayersText").GetComponent<Text>();

            _leaveRoomBtn.onClick.AddListener(LeaveRoom);
            _startBtn.onClick.AddListener(StartGame);

            _navBtns.OnSettings += () => OnGoSettings?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();

            _timerController = transform.FirstOrDefault(t => t.name == "TimeToStartText").GetComponent<TimeoutText>();
        }
        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Menu);
        }
        
        private void StartTimer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable hashtable = new Hashtable {
                    { RoomOptionsKeys.TimeToStart, (double)_maxWaitTime },
                    { RoomOptionsKeys.StartTime, PhotonNetwork.Time },
                    { RoomOptionsKeys.TimerStarted, true }
                };

                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                
                _timerController.OnTimerFinished += OnTimerFinished;
            }
        }

        private void StopTimer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable hashtable = new Hashtable
                {
                    {
                        RoomOptionsKeys.TimerStarted, false
                    }
                };

                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                
                _timerController.OnTimerFinished -= OnTimerFinished;
            }
        }
        
        private void OnTimerFinished()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartGame();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _roomKey.text = "KEY: " + PhotonNetwork.CurrentRoom.Name;
            
            _timeToStartText.gameObject.SetActive(false);

            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                _roomKey.gameObject.SetActive(false);
                _startBtn.gameObject.SetActive(false);
                _timerController.gameObject.SetActive(true);
            }
            else
            {
                _roomKey.gameObject.SetActive(true);
                _startBtn.gameObject.SetActive(true);
                if (!PhotonNetwork.IsMasterClient)
                {
                    _startBtn.interactable = false;
                }
            }
            
            InitPlayersList();
            CheckNumPlayers();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            ClearPlayersList();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if(!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.LocalPlayer == newMasterClient)
            {
                _startBtn.interactable = true;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerToList(newPlayer);
            CheckNumPlayers();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            RemovePlayerFromList(otherPlayer);
            CheckNumPlayers();
        }

        private void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnStartGame?.Invoke();    
            }
        }

        private void InitPlayersList()
        {
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerToList(playerInfo.Value);
            }
        }

        private void CheckNumPlayers()
        {
            int nPlayers = PhotonNetwork.CurrentRoom.Players.Count;
            
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsVisible)
            {
                if (nPlayers == _maxNumberOfPlayers)
                {
                    StartGame();
                } else if (nPlayers < _minNumberOfPlayers && _timerController.TimerStarted)
                {
                    StopTimer();
                }
                else if(nPlayers >= _minNumberOfPlayers && !_timerController.TimerStarted)
                {
                    StartTimer();
                } 
            }
            
            _numPlayersText.text = $"{nPlayers}/{_maxNumberOfPlayers} Jugadores en linea";
        }

        private void ClearPlayersList()
        {
            List<GameObject> elemenstsToDelete = new List<GameObject>();
            foreach (Transform child in _playersList.transform)
            {
                elemenstsToDelete.Add(child.gameObject);
            }
            elemenstsToDelete.ForEach((child) => Destroy(child));
        }

        private void AddPlayerToList(Player player)
        {
            if (string.IsNullOrEmpty(player.NickName))
            {
                string[] defaultNames = (string[]) PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.DefaultPlayerNames];
                player.NickName = defaultNames[player.ActorNumber];
            }

            GameObject element = Instantiate(_playerElemPrefab, _playersList);
            element.name = player.UserId;

            PlayerElem playerElement = element.GetComponent<PlayerElem>();
            playerElement.PlayerName = string.IsNullOrEmpty(player.NickName) ? "--anonimo--" : player.NickName;
        }
        private void RemovePlayerFromList(Player player)
        {
            Transform elementTransform = GetPlayerElement(player);
            if (elementTransform != null)
            {
                Destroy(elementTransform.gameObject);
            }
        }

        private Transform GetPlayerElement(Player player)
        {
            Transform elementTransform = _playersList.FirstOrDefault(t => t.name == player.UserId);
            return elementTransform;
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}