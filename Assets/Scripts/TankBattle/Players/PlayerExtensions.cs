using System;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TankBattle.Players
{
    public static class PlayerExtensions
    {
        public static readonly string ColorPropertyName = "player-color";
        
        public static Color GetColor(this Player player)
        {
            if (player.CustomProperties.TryGetValue(ColorPropertyName, out object colorStringObject))
            {
                String colorString = (string) colorStringObject;
                if(ColorUtility.TryParseHtmlString("#" + colorString, out Color color))
                {
                    return color;
                }
                
                // Debug.Log($"Unable to parse color #{colorString}");
            }

            return Color.black;
        }
        
        public static void SetColor(this Player player, Color color)
        {
            string colorString = ColorUtility.ToHtmlStringRGBA(color);
            Hashtable hashtable = new Hashtable();
            hashtable[ColorPropertyName] = colorString;
            player.SetCustomProperties(hashtable);
        }
    }
}