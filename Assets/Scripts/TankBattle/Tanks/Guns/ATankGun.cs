using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public abstract class ATankGun : MonoBehaviour
    {
        public abstract float FiringRate { get; }
        public abstract void Fire();
    }
}