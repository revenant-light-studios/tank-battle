using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public abstract class ATankTurret : MonoBehaviour
    {
        public abstract void UpdateTurret(Vector3 angle);
    }
}