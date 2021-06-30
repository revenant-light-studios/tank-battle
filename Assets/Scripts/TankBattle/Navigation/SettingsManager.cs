using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;
using TankBattle.Global;

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

            _globalSound.value = GlobalMethods.GeneralVolume;
            _musicSound.value = GlobalMethods.MusicVolume;
            _effectsSound.value = GlobalMethods.EffectsVolume;

            _navBtns.OnMenu += () => OnGoMenu?.Invoke();
            _navBtns.OnCredits += () => OnGoCredits?.Invoke();
            _globalSound.onValueChanged.AddListener(GlobalVolumeChange);
            _musicSound.onValueChanged.AddListener(MusicVolumeChange);
            _effectsSound.onValueChanged.AddListener(EffectsVolumeChange);

            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Settings);
        }     

        private void GlobalVolumeChange(float volume)
        {
            Debug.Log($"G: {volume}");
            GlobalMethods.GeneralVolume = volume;
        }

        private void MusicVolumeChange(float volume)
        {
            GlobalMethods.MusicVolume = volume;
            Debug.Log($"M: {GlobalMethods.MusicVolume}");
            
        }

        private void EffectsVolumeChange(float volume)
        {
            GlobalMethods.EffectsVolume = volume;
            Debug.Log($"E: {volume}");
        }
        
    }
}