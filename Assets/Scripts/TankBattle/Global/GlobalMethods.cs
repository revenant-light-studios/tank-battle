using TankBattle.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankBattle.Global
{
    public static class GlobalMethods
    {
        private const string _settingsPath = "Settings/GameSettings";
        
        private const string _tutorialAlreadyPlayedKey = "tutorial-already-played";
        public static int TutorialAlreadyPlayed
        {
            get => PlayerPrefs.GetInt(_tutorialAlreadyPlayedKey, 0);
            set => PlayerPrefs.SetInt(_tutorialAlreadyPlayedKey, value);
        }

        private const string _numberOfSecondaryGuns = "number-of-secondary-guns";
        public static int NumberOfSecondaryGuns
        {
            get => PlayerPrefs.GetInt(_numberOfSecondaryGuns, 0);
            set => PlayerPrefs.SetInt(_numberOfSecondaryGuns, value);
        }

        private const string _numberOfDummies = "number-of-dummies";
        public static int NumberOfDummies
        {
            get => PlayerPrefs.GetInt(_numberOfDummies, 0);
            set => PlayerPrefs.SetInt(_numberOfDummies, value);
        }

        private const string _keyboardBindings = "keyboard-bindings";
        public static string KeyboardBindings
        {
            get => PlayerPrefs.GetString(_keyboardBindings, string.Empty);
            set => PlayerPrefs.SetString(_keyboardBindings, value);
        }

        public delegate void OnVolumeChangedDelegate(SoundManager.SoundTypes type);
        public static event OnVolumeChangedDelegate OnVolumeChanged;

        private const string _generalVolume = "general-volume";
        public static float GeneralVolume
        {
            get => PlayerPrefs.GetFloat(_generalVolume, 0.5f);
            set
            {
                PlayerPrefs.SetFloat(_generalVolume, value);
                OnVolumeChanged?.Invoke(SoundManager.SoundTypes.Global);
            }
        }

        public static event OnVolumeChangedDelegate OnMusicVolumeChanged;

        private const string _musicVolume = "music-volume";
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(_musicVolume, 0.15f);
            set
            {
                PlayerPrefs.SetFloat(_musicVolume, value);
                OnVolumeChanged.Invoke(SoundManager.SoundTypes.Music);
            }
        }

        private const string _effectsVolume = "effects-volume";
        public static float EffectsVolume
        {
            get => PlayerPrefs.GetFloat(_effectsVolume, 0.5f);
            set
            {
                PlayerPrefs.SetFloat(_effectsVolume, value);
                OnVolumeChanged.Invoke(SoundManager.SoundTypes.Effects);
            }
        }

        private const string _nickName = "nick-name";
        public static string NickName
        {
            get => PlayerPrefs.GetString(_nickName, string.Empty);
            set => PlayerPrefs.SetString(_nickName, value);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool IsMobile();
#endif

        public static bool IsDesktop()
        {
            GameSettings settings = GameSettings;
            
#if !UNITY_EDITOR && UNITY_WEBGL
            return !IsMobile();
#endif
            
            return settings ? !settings.forceMobile : true;
        }

        public static bool IsForceMobile()
        {
            GameSettings settings = GameSettings;
            return settings.forceMobile;
        }

        private static GameSettings _gameSettings;

        public static GameSettings GameSettings
        {
            get
            {
                if (_gameSettings == null)
                {
                    _gameSettings = Resources.Load<GameSettings>(_settingsPath);
                }

                return _gameSettings;
            }
        }
    }
}