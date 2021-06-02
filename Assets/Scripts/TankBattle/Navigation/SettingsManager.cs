using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class SettingsManager : MonoBehaviour
    {
        protected CustomSettings _customSettings;
        protected NavigationsButtons _navBtns;
        public Slider _globalSound { get; private set; }
        public Slider _musicSound { get; private set; }
        public Slider _effectsSound { get; private set; }
        
        //Navigation
        public delegate void OnGoMenuDelegate();
        public OnGoMenuDelegate OnGoMenu;

        public delegate void OnGoCreditsDelegate();
        public OnGoCreditsDelegate OnGoCredits;

        public virtual void Awake()
        {
            _customSettings = FindObjectOfType<CustomSettings>();
            
            _navBtns = transform.FirstOrDefault(t => t.name == "NavigationBtns").GetComponent<NavigationsButtons>();

            Transform soundTransform = transform.FirstOrDefault(t => t.name == "Sound").transform;
            _globalSound = soundTransform.FirstOrDefault(t => t.name == "GlobalVolume").GetComponentInChildren<Slider>();
            _musicSound = soundTransform.FirstOrDefault(t => t.name == "MusicVolume").GetComponentInChildren<Slider>();
            _effectsSound = soundTransform.FirstOrDefault(t => t.name == "EffectsVolume").GetComponentInChildren<Slider>();

            _navBtns.OnMenu += () => OnGoMenu?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();
        }
        // Start is called before the first frame update
        public virtual void Start()
        {
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Settings);

           
            _globalSound.value = _customSettings.globalVolume;
            _musicSound.value = _customSettings.musicVolume;
            _effectsSound.value = _customSettings.effectsVolume;

            //callbacks
            _globalSound.onValueChanged.AddListener(GlobalVolumeChange);
            _musicSound.onValueChanged.AddListener(MusicVolumeChange);
            _effectsSound.onValueChanged.AddListener(EffectsVolumeChange);
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
        
    }
}