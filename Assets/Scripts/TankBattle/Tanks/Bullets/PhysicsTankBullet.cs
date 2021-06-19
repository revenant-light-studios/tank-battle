using ExtensionMethods;
using TankBattle.Tanks.Bullets.Effects;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class PhysicsTankBullet : ATankBullet
    {
        [SerializeField] private float _force = 1f;
        private Rigidbody _rigidBody;
        
        private Impact _impactEffect;
        private GameObject _projectile;
        private bool _started;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _projectile = transform.FirstOrDefault(t => t.name == "Projectile")?.gameObject;        
        }

        public override void Fire(Transform parent)
        {
            _started = true;
        }

        private void FixedUpdate()
        {
            if(_started)
            {
                _rigidBody.velocity = transform.forward * _force;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log($"{name}: collided with {other.gameObject.name}");
            if(_projectile) _projectile.SetActive(false);
            
            OnBulletHit?.Invoke(other.gameObject);
            
            float timeToDestroy = 0f;
            if (_impactEffect)
            {
                _impactEffect.Play();
                timeToDestroy = _impactEffect.Duration;
            }

            Destroy(gameObject, timeToDestroy);
        }
    }
}