using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class SettingsManager : MonoBehaviour
    {
        protected CustomSettings _customSettings;

        public InputField _nickname { get; private set; }
        public Slider _globalSound { get; private set; }
        public Slider _musicSound { get; private set; }
        public Slider _effectsSound { get; private set; }
        public Toggle _isDaltonic { get; private set; }
        
        private Button _returnBtn;

        public delegate void OnReturnMainMenuDelegate();
        public OnReturnMainMenuDelegate OnReturnMainMenu;

        private void Awake()
        {
            _customSettings = FindObjectOfType<CustomSettings>();

            Transform nicknameTransform = transform.FirstOrDefault(t => t.name == "Nickname").transform;
            _nickname = nicknameTransform.FirstOrDefault(t => t.name == "NicknameInput").GetComponent<InputField>();

            Transform soundTransform = transform.FirstOrDefault(t => t.name == "Sound").transform;
            _globalSound = soundTransform.FirstOrDefault(t => t.name == "GlobalSound").GetComponentInChildren<Slider>();
            _musicSound = soundTransform.FirstOrDefault(t => t.name == "MusicSound").GetComponentInChildren<Slider>();
            _effectsSound = soundTransform.FirstOrDefault(t => t.name == "EffectsSound").GetComponentInChildren<Slider>();

            Transform accesibilityTransform = transform.FirstOrDefault(t => t.name == "Accesibility").transform;
            _isDaltonic = accesibilityTransform.FirstOrDefault(t => t.name == "Daltonic").GetComponent<Toggle>();

            _returnBtn = transform.FirstOrDefault(t => t.name == "ReturnBtn").GetComponent<Button>();

            //init values
            _nickname.text = _customSettings.nickname;

            _globalSound.value = _customSettings.globalVolume;
            _musicSound.value = _customSettings.musicVolume;
            _effectsSound.value = _customSettings.effectsVolume;

            _isDaltonic.isOn = _customSettings.isDaltonic;

           
            //callbacks
            _returnBtn.onClick.AddListener(ReturnMainMenu);

            _nickname.onValueChanged.AddListener(NameChange);
            _globalSound.onValueChanged.AddListener(GlobalVolumeChange);
            _musicSound.onValueChanged.AddListener(MusicVolumeChange);
            _effectsSound.onValueChanged.AddListener(EffectsVolumeChange);
            _isDaltonic.onValueChanged.AddListener(DaltonicChange);
        }

        private void DaltonicChange(bool isDaltonic)
        {
            _customSettings.isDaltonic = isDaltonic;
        }

        private void NameChange(string name)
        {
            //put name in photon player
            Debug.Log(name);
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

        public void ReturnMainMenu()
        {
            OnReturnMainMenu?.Invoke();
        }
    }
}
