using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Tanks;
using UnityEngine.Assertions;

namespace Networking.Utilities
{
    public static class PlayerExtensions
    {
        public static string PlayerAlive = "playerAlive";
        public static string PlayerTank = "playerTankViewId";
        
        // Stats
        public static string DeadTimestamp = "dead-timestamp";
        public static string TotalHits = "total-hits";
        public static string TotalBulletsFired = "total-bullets-fired";
        public static string TotalDamage = "total-damaget";
        public static string EnemiesKilled = "enemies-killed";

        public static bool IsAlive(this Player player)
        {
            if (player.CustomProperties.TryGetValue(PlayerAlive, out object alive))
            {
                return (bool)alive;
            }
            
            return false;
        }

        public static void SetAlive(this Player player, bool alive)
        {
            Hashtable propertyHashTable = new Hashtable();
            propertyHashTable[PlayerAlive] = alive;
            if (!alive)
            {
                propertyHashTable[DeadTimestamp] = PhotonNetwork.Time;
            }
            player.SetCustomProperties(propertyHashTable);
        }

        public static void SetTank(this Player player, TankManager tank)
        {
            PhotonView view = tank.photonView;
            Assert.IsTrue(view.IsMine);
            Hashtable propertiesHashTable = new Hashtable();
            propertiesHashTable[PlayerTank] = view.ViewID;
            player.SetCustomProperties(propertiesHashTable);
        }

        public static TankManager GetTank(this Player player)
        {
            TankManager tank = null;
            if (player.CustomProperties.TryGetValue(PlayerTank, out object playerTankViewId))
            {
                PhotonView view = PhotonNetwork.GetPhotonView((int)playerTankViewId);
                if (view)
                {
                    tank = view.GetComponent<TankManager>();
                }
            }
            return tank;
        }

        public static void SetStat(this Player player, string stat, float value)
        {
            // Debug.Log($"{player.NickName}: stat change {stat}: {value}");   
            Hashtable statHashtable = new Hashtable();
            statHashtable[stat] = value;
            player.SetCustomProperties(statHashtable);
        }

        public static float GetStat(this Player player, string stat)
        {
            if (player.CustomProperties.TryGetValue(stat, out object value))
            {
                return (float)value;
            }

            return 0f;
        }
    }
}