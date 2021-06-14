using System;
using ExtensionMethods;
using Photon.Pun;
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

        private Transform _launcher;

        protected override void Awake()
        {
            base.Awake();
            
            if (!_missile)
            {
                _missile = Resources.Load<Missile>("Bullets/Missile");    
            }

            _launcher = transform.FirstOrDefault(t => t.name == "Launcher");
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

        public override void RegisterInput(PlayerInput input)
        {
            _playerInput = input;
            _playerInput.OnTrigger2Pressed += () => TriggerPressed = true;
            _playerInput.OnTrigger2Released += () => TriggerPressed = false;
        }

        [PunRPC]
        public override void NetworkFire()
        {
            if (!_missile) return;
            
            Missile missileInstance = (Missile)Instantiate(_missile);
            
            if(_trackedTank) missileInstance.target = _trackedTank;
            missileInstance.OnBulletHit = OnBulletHit;
            missileInstance.transform.position = _launcher.transform.position;
            missileInstance.transform.rotation = _launcher.transform.rotation;
            missileInstance.Fire(transform);

            if (_launchSound)
            {
                _launchSound.PlayOneShot(_launchSound.clip);
            }
        }
    }
}