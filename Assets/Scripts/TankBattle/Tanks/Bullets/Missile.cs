using ExtensionMethods;
using TankBattle.Global;
using TankBattle.Tanks.Bullets.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Bullets
{
    public class Missile : ATankBullet
    {
        [SerializeField, FormerlySerializedAs("MissileSpeed")]
        private float _speed = 5000f;

        [SerializeField, FormerlySerializedAs("RotationSpeed")]
        private float _rotationSpeed = 200f;

        [SerializeField, FormerlySerializedAs("Target")]
        public GameObject target;

        private Rigidbody _rigidbody;
        private bool _started = false;
        private Impact _impactEffect;
        private GameObject _projectile;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _impactEffect = transform.FirstOrDefault(t => t.name == "Impact")?.GetComponent<Impact>();
            _projectile = transform.FirstOrDefault(t => t.name == "Projectile")?.gameObject;
        }

        public override void Fire(Transform parent)
        {
            _started = true;
            Destroy(gameObject, 10f);
        }

        private void FixedUpdate()
        {
            if (_started)
            {
                if (target)
                {
                    Vector3 targetCenter = target.GetComponent<DetectableObject>().Bounds.center;
                    Vector3 direction = (targetCenter - transform.position).normalized;
                    Vector3 rotation = Vector3.Cross(transform.forward, direction);
                    // float rotationAmount = rotation.magnitude;
                    _rigidbody.angularVelocity = rotation * _rotationSpeed;
                }

                _rigidbody.velocity = transform.forward * _speed;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            OnBulletHit?.Invoke(other.gameObject);
            float timeToDestroy = 0f;
            if(_projectile) _projectile.SetActive(false);
            
            if (_impactEffect)
            {
                _impactEffect.Play();
                timeToDestroy = _impactEffect.Duration;
            }

            Destroy(gameObject, timeToDestroy);
        }
    }
}