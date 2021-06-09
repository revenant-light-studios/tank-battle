using System.Collections.Generic;
using ExtensionMethods;
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
            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _impactEffect.gameObject.SetActive(false);
            _particleSystem = GetComponent<ParticleSystem>();
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
                    _impactEffect.transform.position = transform.InverseTransformVector(collisionEvents[0].intersection);    
                }
                _impactEffect.Play();
                Debug.Log($"ImpactEffect played");
            }
            
        }
    }
}