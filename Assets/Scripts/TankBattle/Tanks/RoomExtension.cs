using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace TankBattle.Tanks
{
    public static class RoomExtension
    {
        public static string LivingPlayersPropertyName = "livingPlayers";

        public static int GetLivingPlayers(this Room room)
        {
            if (room.CustomProperties.TryGetValue(LivingPlayersPropertyName, out object livingPlayers))
            {
                return (int)livingPlayers;
            }

            return -1;
        }

        public static void UpdateLivingPlayers(this Room room, int livingPlayersValue)
        {
            Hashtable hashtable = new Hashtable();
            hashtable[LivingPlayersPropertyName] = livingPlayersValue;
            room.SetCustomProperties(hashtable);
        }
    }
}
