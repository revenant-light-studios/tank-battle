using System.Collections.Generic;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    [RequireComponent(typeof(PhotonView))]
    public class RoomListManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject roomListChildPrefab;
        
        public Button _createRoomButton { get; private set; }
        private Button _joinButton;
        public Button _returnButton { get; private set; }
        private InputField _roomKeyInput;
        private Transform _roomList;

        public delegate void OnReturnMainMenuDelegate();
        public OnReturnMainMenuDelegate OnReturnMainMenu;

        public void Awake()
        {
            _createRoomButton = transform.FirstOrDefault(t => t.name == "CreateRoomBtn").GetComponent<Button>();
            _joinButton = transform.FirstOrDefault(t => t.name == "JoinRoomBtn").GetComponent<Button>();
            _returnButton = transform.FirstOrDefault(t => t.name == "ReturnBtn").GetComponent<Button>();
            _roomKeyInput = transform.FirstOrDefault(t => t.name == "RoomKeyInput").GetComponent<InputField>();

            
            Transform roomList = transform.FirstOrDefault(t => t.name == "RoomList");
            _roomList = roomList.FirstOrDefault(t => t.name == "Content");

            _createRoomButton.onClick.AddListener(CreateRoom);
            _joinButton.onClick.AddListener(JoinRoomWithSecret);
            _returnButton.onClick.AddListener(ReturnMainMenu);
        }

        public override void OnEnable()
        {
            base.OnEnable(); 
        }

        public override void OnDisable()
        {
            base.OnDisable();
            ClearRoomList();
        }
        public void ReturnMainMenu()
        {
            OnReturnMainMenu?.Invoke();
        }

        private void JoinRoomWithSecret()
        {
            string secret = _roomKeyInput.text;
            JoinRoom(secret);
        }

        private void JoinRoom(string key)
        {
            if (!PhotonNetwork.IsConnected || string.IsNullOrEmpty(key))
                return;
            
            PhotonNetwork.JoinRoom(key);
        }

        private void CreateRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            
            string roomKey = RoomFactory.Instance.GenerateRoomKey();
            RoomOptions options = RoomFactory.Instance.CreateRoomProperties(roomKey,false);
            PhotonNetwork.CreateRoom(roomKey, options);
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                UpdateRoomList(info);
            }
        }

        private void UpdateRoomList(RoomInfo info)
        {
            Transform listElement = _roomList.FirstOrDefault(t => t.name == info.Name);
            RoomListElement element = null;

            if (listElement == null && !info.RemovedFromList)
            { 
                Debug.Log($"Room {info.Name} not found in list");
                GameObject gameObject = Instantiate(roomListChildPrefab, _roomList.transform); 
                element = gameObject.GetComponent<RoomListElement>();
                element.name = info.Name;
                
                gameObject.GetComponent<Button>().onClick.AddListener(() => RoomClick(gameObject));
            } else if (info.RemovedFromList)
            {
                if (listElement != null)
                {
                    Destroy(listElement.gameObject);       
                }
            }
            else
            {
                element = listElement.GetComponent<RoomListElement>();
            }

            if (element != null)
            {
                UpdateListElement(element, info);
            }
        }

        private void RoomClick(GameObject clickedGameObject)
        {
            JoinRoom(clickedGameObject.name);
        }

        private void UpdateListElement(RoomListElement element, RoomInfo info)
        {
            element.RoomName = (string) info.CustomProperties[RoomOptionsKeys.Name];
            element.NumberOfPlayers = $"{info.PlayerCount} / {info.MaxPlayers}";
        }
        
        private void ClearRoomList()
        {
            List<GameObject> elemenstsToDelete = new List<GameObject>();
            foreach (Transform child in _roomList.transform)
            {
                elemenstsToDelete.Add(child.gameObject);
            }
            elemenstsToDelete.ForEach((child) => Destroy(child));
        }

    }
}
