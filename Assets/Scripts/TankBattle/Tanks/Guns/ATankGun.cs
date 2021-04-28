using TankBattle.Tanks.Bullets;
using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public abstract class ATankGun : MonoBehaviour
    {
        [SerializeField] public ATankBullet TankBullet;
        public abstract float FiringRate { get; }
        public abstract void Fire();
        
        public delegate void OnTankHitDelegate(TankValues other);
        public OnTankHitDelegate OnTankHit;
    }
}