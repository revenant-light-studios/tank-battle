using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation2
{
    public class MainMenuManager : MonoBehaviour
    {
        private string[] _modes = { "PUBLIC", "PRIVATE" };
        private int _modeSelected = 0;

        private Button _playBtn;
        private Button _joinBtn;
        private Button _modeBtn;
        private NavigationsButtons _navBtns;
        private InputField _nickname;
        private int _nicknameLength = 18;

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
            _joinBtn = transform.FirstOrDefault(t => t.name == "JoinBtn").GetComponent<Button>();
            _modeBtn = transform.FirstOrDefault(t => t.name == "ModeBtn").GetComponent<Button>();
            _navBtns = FindObjectOfType<NavigationsButtons>();

            _nickname.onValueChanged.AddListener(delegate { UpdateNickname(); });
            _playBtn.onClick.AddListener(OnClickPlay);
            _joinBtn.onClick.AddListener(OnClickJoin);
            _modeBtn.onClick.AddListener(OnClickMode);

            _navBtns.OnSettings += () => aux();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();
        }

        void aux()
        {
            Debug.Log("In Menu: Settings");
            OnGoSettings?.Invoke();
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
            }
            else
            {
                OnPlayPrivate?.Invoke();
                Debug.Log("Private");
            }
            
        }

        void OnClickJoin()
        {
            Debug.Log("Join");
        }

    }
}