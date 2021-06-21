using TankBattle.Tanks;
using UnityEngine;

namespace TankBattle.InGameGUI
{
    public abstract class ATankHud : MonoBehaviour
    {
        public abstract void RegisterTank(TankManager tankManager);

        public abstract void UpdateLivingPlayersText(int LivingPlayers);

        public abstract void ShowEndPanel(TankManager tankManager);

        public abstract void StartViewerMode();
    }
}