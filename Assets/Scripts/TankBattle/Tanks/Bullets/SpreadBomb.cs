using System;
using System.Collections.Generic;
using ExtensionMethods;
using TankBattle.Navigation;
using TankBattle.Tanks.Bullets.Effects;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class SpreadBomb : ATankBullet
    {
        private ParticleSystem _spreadBombParticleSystem;
        private Impact _impactEffect;
        
        private List<ParticleCollisionEvent> _collisionEvents;

        private void Start()
        {
            _spreadBombParticleSystem = GetComponent<ParticleSystem>();
            PlayRoomManager roomManager = FindObjectOfType<PlayRoomManager>();
            _spreadBombParticleSystem.randomSeed = (uint)roomManager.RandomSeed;
            
            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _impactEffect.gameObject.SetActive(false);
            _collisionEvents = new List<ParticleCollisionEvent>();
        }

        public override void Fire(Transform parent)
        {
            _spreadBombParticleSystem?.Play();
            _impactEffect.gameObject.SetActive(true);
        }

        private void OnParticleCollision(GameObject other)
        {
            OnBulletHit?.Invoke(other);

            if (_impactEffect)
            {
                int numCollisionEvents = _spreadBombParticleSystem.GetCollisionEvents(other, _collisionEvents);
                if (numCollisionEvents > 0)
                {
                    _impactEffect.transform.position = _collisionEvents[0].intersection;
                }
                _impactEffect.Play();
            }
        }
    }
}