using TankBattle.Tanks.Bullets;
using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public abstract class ATankGun : MonoBehaviour
    {
        [SerializeField] public ATankBullet TankBullet;
        public abstract float FiringRate { get; }
        public abstract void Fire();
        
        public delegate void OnTankHitDelegate(TankValues other, ATankBullet bullet);
        public OnTankHitDelegate OnTankHit;
        
        protected void OnBulletHit(GameObject other)
        {
            TankValues tankValues = other.GetComponent<TankValues>();
            if (tankValues != null)
            {
                Debug.Log($"Hit {other.name}");
                if (OnTankHit != null)
                {
                    OnTankHit?.Invoke(tankValues, TankBullet);    
                }
                else
                {
                    tankValues.WasHit(TankBullet);
                }
            }
        }
    }
}