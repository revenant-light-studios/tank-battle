using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation2
{
    public class MainMenuManager : MonoBehaviourPunCallbacks
    {
        private string[] _modes = { "PUBLIC", "PRIVATE" };
        private int _modeSelected = 0;

        private Button _playBtn;
        private Button _openJoinBtn;
        private Button _modeBtn;
        private NavigationsButtons _navBtns;
        private InputField _nickname;
        private int _nicknameLength = 18;

        private Transform _joinPanel;
        private Button _joinBtn;
        private Button _closeJoinPanel;
        private InputField _keyRoom;

        public delegate void OnPlayPublicDelegate();
        public OnPlayPublicDelegate OnPlayPublic;
        
        public delegate void OnPlayPrivateDelegate();
        public OnPlayPrivateDelegate OnPlayPrivate;

        public delegate void OnJoinDelegate();
        public OnJoinDelegate OnJoin;

        //Navigation
        public delegate void OnGoSettingsDelegate();
        public OnGoSettingsDelegate OnGoSettings;

        public delegate void OnGoCreditsDelegate();
        public OnGoCreditsDelegate OnGoCredits;

        private void Awake()
        {
            _nickname = transform.FirstOrDefault(t => t.name == "NicknameInput").GetComponent<InputField>();
            _playBtn = transform.FirstOrDefault(t => t.name == "PlayBtn").GetComponent<Button>();
            _openJoinBtn = transform.FirstOrDefault(t => t.name == "OpenJoinBtn").GetComponent<Button>();
            _modeBtn = transform.FirstOrDefault(t => t.name == "ModeBtn").GetComponent<Button>();
            _navBtns = FindObjectOfType<NavigationsButtons>();
            _joinPanel = transform.FirstOrDefault(t => t.name == "JoinPanel").transform;
            _joinBtn = _joinPanel.FirstOrDefault(t => t.name == "JoinBtn").GetComponent<Button>();
            _closeJoinPanel = _joinPanel.FirstOrDefault(t => t.name == "ExitJoinPanel").GetComponent<Button>();
            _keyRoom = _joinPanel.FirstOrDefault(t => t.name == "InputKeyRoom").GetComponent<InputField>();

            _nickname.onValueChanged.AddListener(delegate { UpdateNickname(); });
            _playBtn.onClick.AddListener(OnClickPlay);
            _openJoinBtn.onClick.AddListener(OnClickOpenJoin);
            _modeBtn.onClick.AddListener(OnClickMode);
            _joinBtn.onClick.AddListener(OnClickJoin);
            _closeJoinPanel.onClick.AddListener(OnClickCloseJoinPanel);

            _navBtns.OnSettings += () => OnGoSettings?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();
        }

        void Start()
        {
            _nickname.characterLimit = _nicknameLength;
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Menu);
            _modeBtn.GetComponent<Text>().text = _modes[_modeSelected];
        }

        void UpdateNickname()
        {
            PhotonNetwork.NickName = _nickname.text;
        }

        void OnClickMode()
        {
            _modeSelected++;
            if (_modeSelected == _modes.Length)
            {
                _modeSelected = 0;
            }
            _modeBtn.GetComponent<Text>().text = _modes[_modeSelected];
        }

        void OnClickPlay()
        {
            if (_modes[_modeSelected] == "PUBLIC")
            {
                OnPlayPublic?.Invoke();
                Debug.Log("Public");
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                OnPlayPrivate?.Invoke();
                Debug.Log("Private");
                CreateRoom(false);
            }
            
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateRoom(true);
        }

        void OnClickOpenJoin()
        {
            _joinPanel.gameObject.SetActive(true);
            _openJoinBtn.gameObject.SetActive(false);
        }

        void OnClickJoin()
        {
            string secret = _keyRoom.text;
            JoinRoom(secret);
        }

        void OnClickCloseJoinPanel()
        {
            _joinPanel.gameObject.SetActive(false);
            _openJoinBtn.gameObject.SetActive(true);
        }

        private void CreateRoom(bool isPublic)
        {
            if (!PhotonNetwork.IsConnected)
                return;

            string roomKey = RoomFactory.Instance.GenerateRoomKey();
            RoomOptions options = RoomFactory.Instance.CreateRoomProperties(roomKey,isPublic);
            PhotonNetwork.CreateRoom(roomKey, options);
        }

        private void JoinRoom(string key)
        {
            if (!PhotonNetwork.IsConnected || string.IsNullOrEmpty(key))
                return;

            key = key.ToUpper();
            PhotonNetwork.JoinRoom(key);
        }

    }
}