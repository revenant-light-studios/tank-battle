using UnityEngine;
using UnityEngine.InputSystem;

namespace TankBattle.Global
{
    public static class GlobalMethods
    {
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

        private const string _generalVolume = "general-volume";
        public static float GeneralVolume
        {
            get => PlayerPrefs.GetFloat(_generalVolume, 1);
            set => PlayerPrefs.SetFloat(_generalVolume, value);
        }

        private const string _musicVolume = "music-volume";
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(_musicVolume, 1);
            set => PlayerPrefs.SetFloat(_musicVolume, value);
        }

        private const string _effectsVolume = "effects-volume";
        public static float EffectsVolume
        {
            get => PlayerPrefs.GetFloat(_effectsVolume, 1);
            set => PlayerPrefs.SetFloat(_effectsVolume, value);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool IsMobile();
#endif

        public static bool IsDesktop()
        {
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");
            
#if !UNITY_EDITOR && UNITY_WEBGL
            return !IsMobile();
#endif
            
            return settings ? !settings.forceMobile : true;
        }

        public static bool IsForceMobile()
        {
            GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");
            return settings.forceMobile;
        }
    }
}