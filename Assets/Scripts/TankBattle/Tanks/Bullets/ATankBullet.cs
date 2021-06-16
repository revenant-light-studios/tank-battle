using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Bullets
{
    public abstract class ATankBullet : MonoBehaviour, ITankBullet
    {
        [SerializeField, FormerlySerializedAs("Damage"), InspectorName("Bullet damage"), Tooltip("Damage caused by this bullet")]
        protected float _damage;
        public delegate bool OnBulletHitDelegate(GameObject other);
        public OnBulletHitDelegate OnBulletHit;
        
        public abstract void Fire(Transform parent);
        
        public float Damage { get => _damage; }
    }
}