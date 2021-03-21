using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public abstract class ATankBullet : MonoBehaviour, ITankBullet
    {
        public delegate void OnBulletHitDelegate(GameObject other);
        public OnBulletHitDelegate OnBulletHit;
        
        public abstract void Fire(Transform parent);
    }
}