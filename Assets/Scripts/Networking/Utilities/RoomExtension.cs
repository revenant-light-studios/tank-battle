using System.Collections.Generic;
using Photon.Realtime;
using TankBattle.Tanks;

namespace Networking.Utilities
{
    public static class RoomExtension
    {
        public static int GetAlivePlayersCount(this Room room)
        {
            int alivePlayers = 0;
            
            foreach (Player player in room.Players.Values)
            {
                if (player.IsAlive())
                {
                    alivePlayers++;
                }
            }

            return alivePlayers;
        }

        public static List<Player> GetAlivePlayers(this Room room)
        {
            List<Player> alivePlayers = new List<Player>();
            foreach (Player player in room.Players.Values)
            {
                if (player.IsAlive())
                {
                    alivePlayers.Add(player);
                }
            }
            return alivePlayers;
        }

        public static List<TankManager> GetAlivePlayersTanks(this Room room)
        {
            List<TankManager> alivePlayersTanks = new List<TankManager>();

            foreach (Player player in room.Players.Values)
            {
                if (player.IsAlive())
                {
                    TankManager playerTank = player.GetTank();
                    if (playerTank != null)
                    {
                        alivePlayersTanks.Add(playerTank);
                    }
                }
            }

            return alivePlayersTanks;
        }

        public static List<TankManager> GetPlayersTanks(this Room room)
        {
            List<TankManager> playersTanks = new List<TankManager>();

            foreach (Player player in room.Players.Values)
            {
                TankManager playerTank = player.GetTank();
                if (playerTank != null)
                {
                    playersTanks.Add(playerTank);
                }
            }

            return playersTanks;
        }
    }
}
