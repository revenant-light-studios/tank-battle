using UnityEngine;

namespace TankBattle.InGameGUI
{
    public abstract class ATankHud : MonoBehaviour
    {
        public abstract void RegisterTank(GameObject tank);
    }
}