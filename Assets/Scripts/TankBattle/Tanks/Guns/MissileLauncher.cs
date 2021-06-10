using System;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Bullets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public class MissileLauncher : ATankGun
    {
        private ATankBullet _missile;
        private AudioSource _launchSound;
        
        [SerializeField, FormerlySerializedAs("TrackedTank")]
        private GameObject _trackedTank;

        private void Awake()
        {
            if (!_missile)
            {
                _missile = Resources.Load<Missile>("Bullets/Missile");    
            }

            _launchSound = GetComponent<AudioSource>();
        }
        
        public GameObject TrackedTank
        {
            get => _trackedTank;

            set
            {
                _trackedTank = value;
                // Debug.Log($"Locked {_trackedTank.name}");
            }
        }

        public override void NetworkFire()
        {
            if (!_missile) return;
            
            Missile missileInstance = (Missile)Instantiate(_missile);
            
            if(_trackedTank) missileInstance.target = _trackedTank;
            missileInstance.OnBulletHit = OnBulletHit;
            missileInstance.transform.position = transform.position;
            missileInstance.transform.rotation = transform.rotation;
            missileInstance.Fire(transform);

            if (_launchSound)
            {
                _launchSound.PlayOneShot(_launchSound.clip);
            }
        }
    }
}