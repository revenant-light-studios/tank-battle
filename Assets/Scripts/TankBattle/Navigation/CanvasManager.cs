using ExtensionMethods;
using Photon.Pun;
using UnityEngine;

namespace TankBattle.Navigation
{
    [RequireComponent(typeof(PhotonView))]
    public class CanvasManager : MonoBehaviourPunCallbacks
    {
        private bool _isDesktop = true;

        private RoomListManager _roomList;
        private RoomManager _room;
        private CreditsManager _credits;
        private MainMenuManager _mainMenu;
        private SettingsManager _settings;

        private GameObject _desktop;
        private GameObject _mobile;

        private void Awake()
        {
            _desktop = transform.FirstOrDefault(t => t.name == "Desktop").gameObject;
            _mobile = transform.FirstOrDefault(t => t.name == "Mobile").gameObject;

            if (_isDesktop)
            {
                _roomList = _desktop.transform.FirstOrDefault(t => t.name == "RoomList").GetComponent<RoomListManager>();
                _room = _desktop.transform.FirstOrDefault(t => t.name == "Room").GetComponent<RoomManager>();
                _credits = _desktop.transform.FirstOrDefault(t => t.name == "Credits").GetComponent<CreditsManager>();
                _mainMenu = _desktop.transform.FirstOrDefault(t => t.name == "MainMenu").GetComponent<MainMenuManager>();
                _settings = _desktop.transform.FirstOrDefault(t => t.name == "Settings").GetComponent<SettingsManager>();
            }
            else
            {
                _roomList = _mobile.transform.FirstOrDefault(t => t.name == "RoomList").GetComponent<RoomListManager>();
                _room = _mobile.transform.FirstOrDefault(t => t.name == "Room").GetComponent<RoomManager>();
                _credits = _mobile.transform.FirstOrDefault(t => t.name == "Credits").GetComponent<CreditsManager>();
                _mainMenu = _mobile.transform.FirstOrDefault(t => t.name == "MainMenu").GetComponent<MainMenuManager>();
                _settings = _mobile.transform.FirstOrDefault(t => t.name == "Settings").GetComponent<SettingsManager>();
            }

           

            _room.OnStartGame += () => photonView.RPC("StartGame", RpcTarget.AllBuffered);
            _credits.OnReturnMainMenu += () => ReturnToMainMenu("credits");
            _roomList.OnReturnMainMenu += () => ReturnToMainMenu("roomList");
            _settings.OnReturnMainMenu += () => ReturnToMainMenu("settings");
            _mainMenu.OnPlay += () => LeaveMainMenu("play");
            _mainMenu.OnCredits += () => LeaveMainMenu("credits");
            _mainMenu.OnSettings += () => LeaveMainMenu("settings");            
        }
        
        private void Start()
        {
            _roomList.gameObject.SetActive(false);
            _room.gameObject.SetActive(false);
            _credits.gameObject.SetActive(false);
            _settings.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            _roomList.gameObject.SetActive(false);
            _room.gameObject.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            _roomList.gameObject.SetActive(true);
            _room.gameObject.SetActive(false);
        }

        [PunRPC]
        private void StartGame()
        {
            Debug.Log("Start game");
            PhotonNetwork.LoadLevel("GameScene");
        }

        private void ReturnToMainMenu(string screen)
        {
            switch (screen)
            {
                case "credits":
                    _credits.gameObject.SetActive(false);
                    break;
                case "settings":
                    _settings.gameObject.SetActive(false);
                    break;
                case "roomList":
                    _roomList.gameObject.SetActive(false);
                    break;
            }

            _mainMenu.gameObject.SetActive(true);
        }

        private void LeaveMainMenu(string screen)
        {
            switch (screen)
            {
                case "play":
                    _roomList.gameObject.SetActive(true);
                    break;
                case "credits":
                    _credits.gameObject.SetActive(true);
                    break;
                case "settings":
                    _settings.gameObject.SetActive(true);
                    break;
            }
            _mainMenu.gameObject.SetActive(false);
        }
    }
}
