using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.UI;
using System;

namespace TankBattle.Navigation2
{
    public class SettingsManager : MonoBehaviour
    {
        private CustomSettings _customSettings;
        private NavigationsButtons _navBtns;
        public Slider _globalSound { get; private set; }
        public Slider _musicSound { get; private set; }
        public Slider _effectsSound { get; private set; }
        public Button _shootBtn { get; private set; }

        private Event _keyEvent;
        private bool _waitingForKey = false;
        private string _pressedButton;

        //Navigation
        public delegate void OnGoMenuDelegate();
        public OnGoMenuDelegate OnGoMenu;

        public delegate void OnGoCreditsDelegate();
        public OnGoCreditsDelegate OnGoCredits;

        private void Awake()
        {
            _customSettings = FindObjectOfType<CustomSettings>();

            _navBtns = FindObjectOfType<NavigationsButtons>();

            Transform soundTransform = transform.FirstOrDefault(t => t.name == "Sound").transform;
            _globalSound = soundTransform.FirstOrDefault(t => t.name == "GlobalVolume").GetComponentInChildren<Slider>();
            _musicSound = soundTransform.FirstOrDefault(t => t.name == "MusicVolume").GetComponentInChildren<Slider>();
            _effectsSound = soundTransform.FirstOrDefault(t => t.name == "EffectsVolume").GetComponentInChildren<Slider>();

            Transform controllersTransform = transform.FirstOrDefault(t => t.name == "Controllers").transform;
            _shootBtn = controllersTransform.FirstOrDefault(t => t.name == "ShootBtn").GetComponent<Button>();

            _navBtns.OnMenu += () => OnGoMenu?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();
           

            _shootBtn.onClick.AddListener(KeyChange);
        }
        // Start is called before the first frame update
        void Start()
        {
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Settings);

            //init values
            _shootBtn.GetComponentInChildren<Text>().text = _customSettings.shootBtn.ToString();
           
            _globalSound.value = _customSettings.globalVolume;
            _musicSound.value = _customSettings.musicVolume;
            _effectsSound.value = _customSettings.effectsVolume;

            //callbacks
            _globalSound.onValueChanged.AddListener(GlobalVolumeChange);
            _musicSound.onValueChanged.AddListener(MusicVolumeChange);
            _effectsSound.onValueChanged.AddListener(EffectsVolumeChange);
        }
        private void Update()
        {
            if (_waitingForKey)
            {
                foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(vKey))
                    {
                        switch (_pressedButton)
                        {
                            case "ShootBtn":
                                //_customSettings.shootBtn = vKey;
                                _shootBtn.GetComponentInChildren<Text>().text = vKey.ToString();
                                break;
                        }
                        _waitingForKey = false;
                        _shootBtn.interactable = true;
                    }
                }
            }
        }

        private void GlobalVolumeChange(float volume)
        {
            _customSettings.globalVolume = volume;
        }

        private void MusicVolumeChange(float volume)
        {
            _customSettings.musicVolume = volume;
        }

        private void EffectsVolumeChange(float volume)
        {
            _customSettings.effectsVolume = volume;
        }
        private void KeyChange()
        {
            _waitingForKey = true;
            if(EventSystem.current.currentSelectedGameObject != null)
            {
                _pressedButton = EventSystem.current.currentSelectedGameObject.name;
                _shootBtn.interactable = false;
            }           
        }
    }
}