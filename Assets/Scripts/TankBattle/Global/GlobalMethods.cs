using UnityEngine;

namespace TankBattle.Global
{
    public static class GlobalMethods
    {
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
       
    }
}