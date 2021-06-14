using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Navigation;
using TankBattle.Tanks.Bullets.Effects;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class ParticleTankBullet : ATankBullet
    {
        private Impact _impactEffect;
        
        private ParticleSystem _particleSystem;
        public List<ParticleCollisionEvent> collisionEvents;

        private void Start()
        {
            // The particle system must be deterministic to work well in networked environment
            _particleSystem = GetComponent<ParticleSystem>();
            PlayRoomManager roomManager = FindObjectOfType<PlayRoomManager>();
            _particleSystem.randomSeed = (uint)roomManager.RandomSeed;

            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _impactEffect.gameObject.SetActive(false);
            collisionEvents = new List<ParticleCollisionEvent>();
        }

        public override void Fire(Transform parent)
        {
            _particleSystem.Play(false);
            _impactEffect.gameObject.SetActive(true);
        }   

        private void OnParticleCollision(GameObject other)
        {
            OnBulletHit?.Invoke(other);
            if (_impactEffect)
            {
                int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);
                if (numCollisionEvents > 0)
                {
                    _impactEffect.transform.position = collisionEvents[0].intersection;
                    // _impactEffect.transform.rotation = Quaternion.Euler(collisionEvents[0].normal);
                }
                _impactEffect.Play();
                // Debug.Log($"ImpactEffect played");
            }
            
        }
    }
}