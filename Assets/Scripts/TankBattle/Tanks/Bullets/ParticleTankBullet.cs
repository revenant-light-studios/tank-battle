using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class ParticleTankBullet : ATankBullet
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public override void Fire(Transform parent)
        {
            _particleSystem.Play();
        }   

        private void OnParticleCollision(GameObject other)
        {
            OnBulletHit?.Invoke(other);
            // Debug.Log($"Bullet hit {other.name}");
        }
    }
}