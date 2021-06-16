using System.Collections.Generic;
using ExtensionMethods;
using TankBattle.Navigation;
using TankBattle.Tanks.Bullets.Effects;
using TankBattle.Tanks.ForceFields;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class ParticleTankBullet : ATankBullet
    {
        private Impact _impactEffect;
        
        private ParticleSystem _particleSystem;
        public List<ParticleCollisionEvent> collisionEvents;
        public ParticleSystem.Particle[] particles;

        private void Start()
        {
            // The particle system must be deterministic to work well in networked environment
            _particleSystem = GetComponent<ParticleSystem>();
            PlayRoomManager roomManager = FindObjectOfType<PlayRoomManager>();
            _particleSystem.randomSeed = (uint)roomManager.RandomSeed;

            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _impactEffect.gameObject.SetActive(false);
            collisionEvents = new List<ParticleCollisionEvent>();
            particles = new ParticleSystem.Particle[1];
        }

        public override void Fire(Transform parent)
        {
            _particleSystem.Play(false);
            _impactEffect.gameObject.SetActive(true);
        }   

        private void OnParticleCollision(GameObject other)
        {
            bool hitSelf = false;
            
            if (OnBulletHit != null)
            {
                hitSelf = OnBulletHit.Invoke(other);
            }

            if (!hitSelf)
            {
                _particleSystem.GetParticles(particles);
                particles[0].remainingLifetime = -1;
                _particleSystem.SetParticles(particles);
                
                if (_impactEffect)
                {
                    int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);
                    if (numCollisionEvents > 0)
                    {
                        _impactEffect.transform.position = collisionEvents[0].intersection;
                    }
                    _impactEffect.Play();
                }
            }
            else
            {
                Debug.Log($"{name} detected hit with self tank");
            }
        }
    }
}