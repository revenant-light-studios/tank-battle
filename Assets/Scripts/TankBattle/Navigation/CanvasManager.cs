using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using UnityEngine;

namespace TankBattle.Navigation
{
    [RequireComponent(typeof(PhotonView))]
    public class CanvasManager : MonoBehaviourPunCallbacks
    {
        enum navScreen
        {
            MainMenu,
            Credits,
            Settings,
            WaitingRoom
        }


        private CreditsManager _credits;
        private MainMenuManager _mainMenu;
        private SettingsManager _settings;
        private WaitingRoomManager _waiting;

        private GameObject _desktop;
        private GameObject _mobile;

        private CustomSettings _globalSettings;
        
        private void Awake()
        {
            _desktop = transform.FirstOrDefault(t => t.name == "Desktop").gameObject;
            _mobile = transform.FirstOrDefault(t => t.name == "Mobile").gameObject;

            if (GlobalMethods.IsDesktop())
            {
                _desktop.SetActive(true);
                _mobile.SetActive(false);
                _mainMenu = _desktop.transform.FirstOrDefault(t => t.name == "MainMenu").GetComponent<MainMenuManager>();
                _credits = _desktop.transform.FirstOrDefault(t => t.name == "Credits").GetComponent<CreditsManager>();
                _settings = _desktop.transform.FirstOrDefault(t => t.name == "Settings").GetComponent<SettingsManager>();
                _waiting = _desktop.transform.FirstOrDefault(t => t.name == "WaitingRoom").GetComponent<WaitingRoomManager>();
            }
            else
            {
                _desktop.SetActive(false);
                _mobile.SetActive(true);
                _mainMenu = _mobile.transform.FirstOrDefault(t => t.name == "MainMenu").GetComponent<MainMenuManager>();
                _credits = _mobile.transform.FirstOrDefault(t => t.name == "Credits").GetComponent<CreditsManager>();
                _settings = _mobile.transform.FirstOrDefault(t => t.name == "Settings").GetComponent<SettingsManager>();
                _waiting = _mobile.transform.FirstOrDefault(t => t.name == "WaitingRoom").GetComponent<WaitingRoomManager>();
            }
            
            _credits.OnGoMenu += () => SelectMenu();
            _credits.OnGoSettings += () => Navigate(navScreen.Settings);
            _settings.OnGoCredits += () => Navigate(navScreen.Credits);
            _settings.OnGoMenu += () => SelectMenu();
            _mainMenu.OnGoCredits += () => Navigate(navScreen.Credits);
            _mainMenu.OnGoSettings += () => Navigate(navScreen.Settings);
            _waiting.OnGoCredits += () => Navigate(navScreen.Credits);
            _waiting.OnGoSettings += () => Navigate(navScreen.Settings);
            _waiting.OnStartGame += () => StartGame();
        }

        void Start()
        {
            _mainMenu.gameObject.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            Navigate(navScreen.WaitingRoom);
        }

        public override void OnLeftRoom()
        {
            Navigate(navScreen.MainMenu);
        }

        [PunRPC]
        private void StartGame()
        {
            // Debug.Log("Start game");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.LoadLevel("GamePlay");
            }
        }

        void SelectMenu()
        {
            if (PhotonNetwork.InRoom)
            {
                Navigate(navScreen.WaitingRoom);
            }
            else
            {
                Navigate(navScreen.MainMenu);
            }
        }

        void Navigate(navScreen next)
        {
            HideAllCanvas();
            switch (next)
            {
                case navScreen.MainMenu:
                    _mainMenu.gameObject.SetActive(true);
                    break;

                case navScreen.Credits:
                    _credits.gameObject.SetActive(true);
                    break;

                case navScreen.Settings:
                    _settings.gameObject.SetActive(true);
                    break;

                case navScreen.WaitingRoom:
                    _waiting.gameObject.SetActive(true);
                    break;
            }
        }

        void HideAllCanvas()
        {
            _mainMenu.gameObject.SetActive(false);
            _credits.gameObject.SetActive(false);
            _settings.gameObject.SetActive(false);
            _waiting.gameObject.SetActive(false);
        }
    }
}