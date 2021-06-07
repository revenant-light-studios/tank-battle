using System;
using ExtensionMethods;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class SpreadBomb : ATankBullet
    {
        private ParticleSystem _spreadBombParticleSystem;

        private void Start()
        {
            _spreadBombParticleSystem = GetComponent<ParticleSystem>();
        }

        public override void Fire(Transform parent)
        {
            _spreadBombParticleSystem?.Play();
        }

        private void OnParticleCollision(GameObject other)
        {
            OnBulletHit?.Invoke(other);
        }
    }
}