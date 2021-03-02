using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExtensionMethods;
using HightTide.Players;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Players;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject roomPlayerListChildPrefab;

        protected Text _title;
        protected Text _roomKey;
        protected Toggle _privateRoom;
        protected InputField _roomName;
        protected Button _loadBtn;
        protected Button _startBtn;
        protected Transform _playersList;
        protected Button _returnBtn;

        public delegate void OnStartGameDelegate();
        public OnStartGameDelegate OnStartGame;
        
        public delegate void OnLoadGameDelgate();
        public OnLoadGameDelgate OnLoadGame;

        private PlayerColorList _playerColorList;

        private void Awake()
        {
            _title = transform.FirstOrDefault(t => t.name == "Title").GetComponent<Text>();
            _roomKey = transform.FirstOrDefault(t => t.name == "RoomKey").GetComponent<Text>();
            _privateRoom = transform.FirstOrDefault(t => t.name == "PrivateToggle").GetComponent<Toggle>();
            _roomName = transform.FirstOrDefault(t => t.name == "RoomNameInput").GetComponent<InputField>();
            _loadBtn = transform.FirstOrDefault(t => t.name == "LoadBtn").GetComponent<Button>();
            _startBtn = transform.FirstOrDefault(t => t.name == "StartBtn").GetComponent<Button>();
            _returnBtn = transform.FirstOrDefault(t => t.name == "ReturnBtn").GetComponent<Button>();

            Transform playersList = transform.FirstOrDefault(t => t.name == "PlayerList");
            _playersList = playersList.FirstOrDefault(t => t.name == "Content");

            _roomName.onValueChanged.AddListener(NameChange);
            _privateRoom.onValueChanged.AddListener(PrivateChange);
            _startBtn.onClick.AddListener(StartClick);
            _loadBtn.onClick.AddListener(LoadClick);
            _returnBtn.onClick.AddListener(BackClick);

            _playerColorList = Resources.Load<PlayerColorList>("PlayerColors");
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            
            SetPlayerColor();
            InitPlayersList();
            InitRoomProperties();
            SetRoomMode();
        }

        private void SetPlayerColor()
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Color color = _playerColorList.Colors[index];
            PhotonNetwork.LocalPlayer.SetColor(color);
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            ClearPlayersList();
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerToList(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            RemovePlayerFromList(otherPlayer);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            SetRoomProperties();   
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerExtensions.ColorPropertyName))
            {
                SetPlayerColor(targetPlayer);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
        }
        
        private void LoadClick()
        {
            throw new NotImplementedException();
        }

        private void StartClick()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnStartGame?.Invoke();
            }
        }
        
        public void BackClick()
        {
            PhotonNetwork.LeaveRoom(true);
        }

        private void NameChange(string name)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Name] = name;
                Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
                properties[RoomOptionsKeys.Name] = name;
                PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            }
        }

        private void PrivateChange(bool isPrivate)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsVisible = !isPrivate;
            }
        }

        protected virtual void SetRoomMode()
        {
        }

        private void InitRoomProperties()
        {
            _title.text = (string) PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Name];
            _roomName.text = _title.text;
            _roomKey.text = $"Clave: {PhotonNetwork.CurrentRoom.Name}";
            _privateRoom.isOn = !PhotonNetwork.CurrentRoom.IsVisible;
        }

        
        private void SetRoomProperties()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                _title.text = (string) PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Name];
            }
            else
            {
                _title.text = (string) PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.Name];
                _roomName.text = _title.text;
                _roomKey.text = $"Clave: {PhotonNetwork.CurrentRoom.Name}";
                _privateRoom.isOn = !PhotonNetwork.CurrentRoom.IsVisible;
            }
        }

        private void InitPlayersList()
        {
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerToList(playerInfo.Value);
            }
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
            GameObject element = Instantiate(roomPlayerListChildPrefab, _playersList);
            element.name = player.UserId;
            
            RoomPlayerElement playerElement = element.GetComponent<RoomPlayerElement>();
            playerElement.PlayerName = string.IsNullOrEmpty(player.NickName) ? "--anonimo--" : player.NickName;
            SetPlayerColor(player);
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

        private void SetPlayerColor(Player player)
        {
            if(player.CustomProperties.ContainsKey(PlayerExtensions.ColorPropertyName))
            {
                Transform playerElement = GetPlayerElement(player);
                if (playerElement != null)
                {
                    RawImage image = playerElement.GetComponent<RawImage>();
                    image.color = player.GetColor();
                }
            }
        }
    }
}
