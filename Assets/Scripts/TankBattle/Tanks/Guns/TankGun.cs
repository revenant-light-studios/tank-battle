using ExtensionMethods;
using TankBattle.Tanks.Bullets;
using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public class TankGun : ATankGun
    {
        [SerializeField] public ATankBullet TankBullet;
        [SerializeField] private float _firingRate = 2f;

        public override float FiringRate => _firingRate;

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

        public override void Fire()
        {
            _bullet?.Fire(_cannonTransform);
            _muzzleParticleSystem.Play();
        }
    }
}