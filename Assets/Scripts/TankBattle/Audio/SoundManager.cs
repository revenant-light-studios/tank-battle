using System;
using TankBattle.Navigation;
using UnityEngine;

namespace TankBattle.Audio
{
    public class SoundManager : MonoBehaviour
    {
        private CustomSettings _customSettings;
        [SerializeField] private SoundPrefab[] soundPrefabArray;
        private GameObject _currentTheme;

        public enum Sound
        {
            Enter,
            NewGame,
            Click,
            Back,
            BackgroundMenu,
            BackgroundGame
        }

        private void Start()
        {
            _customSettings = FindObjectOfType<CustomSettings>();
            DontDestroyOnLoad(gameObject);

            _customSettings.OnChangeVolume += () => VolumeChange();
        }

        private void VolumeChange()
        {
            // Debug.Log("New Volume");
            if(_currentTheme != null)
            {
                _currentTheme.GetComponent<AudioSource>().volume = _customSettings.musicVolume * _customSettings.globalVolume;
            }
        }

        public void PlayBackground(Sound sound)
        {
            if(_currentTheme != null)
            {
                Destroy(_currentTheme);
            }
            _currentTheme = Instantiate(GetSounPrefab(sound));
            _currentTheme.GetComponent<AudioSource>().volume = _customSettings.musicVolume * _customSettings.globalVolume;
        }

        public void PlaySoundEffect(Sound sound)
        {
            GameObject _sound = Instantiate(GetSounPrefab(sound));
            DontDestroyOnLoad(_sound);
            Destroy(_sound, 2.0f) ;
            _sound.GetComponent<AudioSource>().volume = _customSettings.effectsVolume * _customSettings.globalVolume;
        }

        private GameObject GetSounPrefab(Sound sound)
        {
            foreach(SoundPrefab soundPrefab in soundPrefabArray)
            {
                if(soundPrefab.sound == sound)
                {
                    return soundPrefab.soundGameObject;
                }
            }
            // Debug.LogError("Sound " + sound + " is not found!");
            return null;
        }


        [Serializable]
        public class SoundPrefab
        {
            public Sound sound;
            public GameObject soundGameObject;
        }
    }   
}
