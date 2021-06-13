using System;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public class SpreadBombLauncher : ATankGun
    {
        private float _lastFire;
        private bool _canFire;
        
        private ATankBullet _bullet;
        private Transform _launchPoint;

        protected override void Awake()
        {
            base.Awake();
            
            _launchPoint = transform.FirstOrDefault(t => t.name == "LaunchPoint");
            _bullet = Instantiate(TankBullet, _launchPoint ? _launchPoint : transform);
            _bullet.OnBulletHit = OnBulletHit;
        }
        
        [PunRPC]
        public override void NetworkFire()
        {
            _bullet.Fire(transform);    
        }
    }
}