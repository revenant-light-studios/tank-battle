using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankBattle.Navigation
{
    public class TutorialManager : MonoBehaviour, IPointerDownHandler
    {
        private int _tutorialPhase = 0;
        private List<GameObject> _tutorialTexts = new List<GameObject>();
        private GameObject _tutorialPanel, _navButtons, _nickname, _playButton, _modeButton, _joinButton;

        private void Awake()
        {
            _tutorialPanel = transform.FirstOrDefault(t => t.name == "TutorialPanel").gameObject;

            _navButtons = _tutorialPanel.transform.FirstOrDefault(t => t.name == "NavigationBtnsTutorial").gameObject;
            _nickname = _tutorialPanel.transform.FirstOrDefault(t => t.name == "NicknameInputTutorial").gameObject;
            _playButton = _tutorialPanel.transform.FirstOrDefault(t => t.name == "PlayBtnTutorial").gameObject;
            _modeButton = _tutorialPanel.transform.FirstOrDefault(t => t.name == "ModeBtnTutorial").gameObject;
            _joinButton = _tutorialPanel.transform.FirstOrDefault(t => t.name == "OpenJoinBtnTutorial").gameObject;

            _tutorialTexts.Add(_tutorialPanel.transform.FirstOrDefault(t => t.name == "WelcomeText").gameObject);
            _tutorialTexts.Add(_tutorialPanel.transform.FirstOrDefault(t => t.name == "NavButtonsText").gameObject);
            _tutorialTexts.Add(_tutorialPanel.transform.FirstOrDefault(t => t.name == "NicknameText").gameObject);
            _tutorialTexts.Add(_tutorialPanel.transform.FirstOrDefault(t => t.name == "JoinButtonText").gameObject);
            _tutorialTexts.Add(_tutorialPanel.transform.FirstOrDefault(t => t.name == "PlayButtonText").gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            changeTexts();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tutorialPhase++;

            if (_tutorialPhase < _tutorialTexts.Count)
            {
                changeTexts();
            }
            updatePhase();
        }

        void changeTexts()
        {
            if (_tutorialPhase > 0)
            {
                _tutorialTexts[_tutorialPhase - 1].SetActive(false);
            }
            if (_tutorialPhase < _tutorialTexts.Count)
            {
                _tutorialTexts[_tutorialPhase].SetActive(true);
            }
        }

        void updatePhase()
        {
            switch (_tutorialPhase)
            {
                //Welcome
                case 0:
                    break;
                //NavButtons
                case 1:
                    _navButtons.SetActive(true);
                    break;
                //Nickname
                case 2:
                    _navButtons.SetActive(false);
                    _nickname.SetActive(true);
                    break;
                //JoinBtn
                case 3:
                    _nickname.SetActive(false);
                    _joinButton.SetActive(true);
                    break;
                //PlayBtns
                case 4:
                    _joinButton.SetActive(false);
                    _playButton.SetActive(true);
                    _modeButton.SetActive(true);
                    break;
                //End
                case 5:
                    _playButton.SetActive(false);
                    _modeButton.SetActive(false);
                    _tutorialPanel.SetActive(false);
                    this.enabled = false;
                    break;
                    
            }
        }
    }
}
