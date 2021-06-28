using UnityEngine;

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