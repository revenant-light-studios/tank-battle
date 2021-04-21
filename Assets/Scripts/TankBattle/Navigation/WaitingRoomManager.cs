using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Players;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation2
{
    [RequireComponent(typeof(PhotonView))]
    public class WaitingRoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerElemPrefab;
        private PhotonView _pView;
        private float MAX_TIME_TO_START = 30f;
        private float _timeToStart;
        private bool _startCountdown = false;

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


        private void Awake()
        {
            _pView = PhotonView.Get(this);
            _timeToStart = MAX_TIME_TO_START;


            Transform playerList = transform.FirstOrDefault(t=>t.name== "PlayerList");
            _playersList = playerList.FirstOrDefault(t => t.name == "Content");
            _navBtns = FindObjectOfType<NavigationsButtons>();
            _leaveRoomBtn = transform.FirstOrDefault(t => t.name == "LeaveBtn").GetComponent<Button>();
            _startBtn = transform.FirstOrDefault(t => t.name == "StartBtn").GetComponent<Button>();
            _timeToStartText = transform.FirstOrDefault(t => t.name == "TimeToStartText").GetComponent<Text>();
            _roomKey = transform.FirstOrDefault(t => t.name == "RoomKey").GetComponent<Text>();
            _numPlayersText = transform.FirstOrDefault(t => t.name == "NumPlayersText").GetComponent<Text>();

            _leaveRoomBtn.onClick.AddListener(LeaveRoom);
            _startBtn.onClick.AddListener(StartGame);

            _navBtns.OnSettings += () => OnGoSettings?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();

            
        }
        private void Start()
        {
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Menu);

        }

        private void Update()
        {
            //StartContdown();
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log(_timeToStart);
                if (_startCountdown)
                {
                    _timeToStart -= Time.deltaTime;
                    Debug.Log("Resta: " + _timeToStart);
                   
                    int sec = (int)_timeToStart;
                    if (_timeToStart > 0)
                    {
                        _pView.RPC("UpdateCountDown", RpcTarget.All, sec);
                    }
                    else
                    {
                        StartGame();
                    }
                    
                }
                
            }
            
        }

        [PunRPC]
        private void UpdateCountDown(int time)
        {
            _timeToStartText.text = time + "\nsegundos...";
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _roomKey.text = "KEY: " + PhotonNetwork.CurrentRoom.Name;
            _timeToStartText.text = _timeToStart + "\nsegundos...";

            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                _roomKey.gameObject.SetActive(false);
                _startBtn.gameObject.SetActive(false);
                _timeToStartText.gameObject.SetActive(true);
            }
            else
            {
                _roomKey.gameObject.SetActive(true);
                _timeToStartText.gameObject.SetActive(false);
                _startBtn.gameObject.SetActive(true);
                if (!PhotonNetwork.IsMasterClient)
                {
                    _startBtn.interactable = false;
                }
            }
            InitPlayersList();
            GetNumPlayers();
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
            GetNumPlayers();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            RemovePlayerFromList(otherPlayer);
            GetNumPlayers();
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

        private void GetNumPlayers()
        {
            int nPlayers = PhotonNetwork.CurrentRoom.Players.Count;
            if (nPlayers == 1)
            {
                _startCountdown = false;
                _timeToStart = 30;
                _timeToStartText.text = _timeToStart + " segundos..."; 
            }
            else if(nPlayers > 1)
            {
                _startCountdown = true;
            }
            _numPlayersText.text = nPlayers + "/20 JUGADORES";
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