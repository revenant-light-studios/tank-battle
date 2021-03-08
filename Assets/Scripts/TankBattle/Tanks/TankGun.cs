using System;
using ExtensionMethods;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    public class TankGun : MonoBehaviour
    {
        private float _lastFired = 0f;
        private bool _fired = false;
        
        [SerializeField] private float _firingRate = 2f;
        [SerializeField] private float _fireForce = 100f;
        [SerializeField] private Transform _cannonTransformOffset;

        private Transform _cannonTransform;
        private ParticleSystem _muzzleParticleSystem;

        private void Start()
        {
            _cannonTransform = transform.FirstOrDefault(t => t.name == "MuzzleFlash01");
            _muzzleParticleSystem = transform.FirstOrDefault(t => t.name == "MuzzleFlash01").GetComponent<ParticleSystem>();
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
            GameObject bullet = Instantiate(Resources.Load<GameObject>("TankBullet"));
            bullet.GetComponent<TankBullet>().Fire(_cannonTransform, _fireForce);
            _muzzleParticleSystem.Play();
        }
    }
}