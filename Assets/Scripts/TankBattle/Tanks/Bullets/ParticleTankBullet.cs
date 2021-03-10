using System;
using ExtensionMethods;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class ParticleTankBullet : ATankBullet
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = transform.FirstOrDefault(t => t.name == "Particle System").GetComponent<ParticleSystem>();
        }

        public override void Fire(Transform parent)
        {
            _particleSystem.Play();
        }
    }
}