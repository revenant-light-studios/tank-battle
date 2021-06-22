using System.Collections.Generic;
using ExtensionMethods;
using TankBattle.Global;
using TankBattle.Navigation;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class SpreadBomb : ATankBullet
    {
        private Transform _bombTransform;
        private ParticleCollisionDelegate _childParticleCollision;
        private ParticleSystem _spreadBombParticleSystem;

        private bool _started;
        private bool _detonated;

        public float BombSpeed;
        public float BombAltitude;

        private List<ParticleCollisionEvent> _collisionEvents;

        private void Start()
        {
            _bombTransform = transform.FirstOrDefault(t => t.name == "Bomb");
            _spreadBombParticleSystem = transform.FirstOrDefault(t => t.name=="Projectile")?.GetComponent<ParticleSystem>();
            
            _childParticleCollision = transform.FirstOrDefault(t => t.name == "Projectile")?.GetComponent<ParticleCollisionDelegate>();
            if (_childParticleCollision) _childParticleCollision.OnChildParticleCollision = OnParticleCollision;
            

            PlayRoomManager roomManager = FindObjectOfType<PlayRoomManager>();
            _spreadBombParticleSystem.randomSeed = (uint)roomManager.RandomSeed;
            
            _collisionEvents = new List<ParticleCollisionEvent>();
        }

        public override void Fire(Transform parent)
        {
            _started = true;
        }

        private void Update()
        {
            if(!_started) return;
            
            if(!_detonated)
            {
                Vector3 position = _bombTransform.position;
                position.y += BombSpeed * Time.deltaTime;
                _bombTransform.position = position;

                if (position.y >= BombAltitude)
                {
                    _detonated = true;
                    _bombTransform.gameObject.SetActive(false);
                    _spreadBombParticleSystem.transform.position = _bombTransform.position;
                    _spreadBombParticleSystem.Play();
                }
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            // Debug.Log($"{name}: Particle collided with {other.name}");
            OnBulletHit?.Invoke(other);
        }
    }
}