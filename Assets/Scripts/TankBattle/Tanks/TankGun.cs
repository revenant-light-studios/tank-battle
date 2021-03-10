using ExtensionMethods;
using TankBattle.Tanks.Bullets;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class TankGun : MonoBehaviour
    {
        private float _lastFired = 0f;
        private bool _fired = false;
        
        [SerializeField] public ATankBullet TankBullet;
        [SerializeField] private float _firingRate = 2f;

        private Transform _cannonTransform;
        private ParticleSystem _muzzleParticleSystem;
        private ATankBullet _bullet;

        private void Start()
        {
            _cannonTransform = transform.FirstOrDefault(t => t.name == "FirePoint");
            _muzzleParticleSystem = transform.FirstOrDefault(t => t.name == "TankMuzzleFlash").GetComponent<ParticleSystem>();

            _bullet = Instantiate(TankBullet, _cannonTransform);
            _bullet.transform.localPosition = Vector3.zero;
            _bullet.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            if (Input.GetButton("Fire1"))
            {
                if(!_fired)
                {
                    _lastFired = _firingRate;
                    _fired = true;
                    Fire();
                }
            }
            
            if (_fired)
            {
                _lastFired -= Time.deltaTime;
                
                if (_lastFired <= 0f)
                {
                    _fired = false;
                }
            }
        }

        private void Fire()
        {
            _bullet?.Fire(_cannonTransform);
            _muzzleParticleSystem.Play();
        }
    }
}