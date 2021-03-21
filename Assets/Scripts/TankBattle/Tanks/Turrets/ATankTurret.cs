using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public abstract class ATankTurret : MonoBehaviour
    {
        public abstract Vector3 MousePosition { set; }
        public abstract void UpdateTurret();
    }
}