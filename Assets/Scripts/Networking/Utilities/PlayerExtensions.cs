using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Navigation;
using TankBattle.Tanks;
using UnityEngine.Assertions;

namespace Networking.Utilities
{
    public static class PlayerExtensions
    {
        public static string PlayerAlive = "playerAlive";
        public static string PlayerTank = "playerTankViewId";

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
    }
}