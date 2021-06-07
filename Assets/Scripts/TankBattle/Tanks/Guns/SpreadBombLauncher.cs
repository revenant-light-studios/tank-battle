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
        
        [SerializeField, FormerlySerializedAs("FiringRate"), InspectorName("Fire rate"), Tooltip("Seconds between consecutive shots")] 
        private float _firingRate = 2f;

        private float _lastFire;
        private bool _canFire;
        
        public override float FiringRate => _firingRate;
        
        private PhotonView _photonView;
        private ATankBullet _bullet;
        private Transform _launchPoint;

        private void Awake()
        {
            _launchPoint = transform.FirstOrDefault(t => t.name == "LaunchPoint");
            _photonView = GetComponent<PhotonView>();
            _bullet = Instantiate(TankBullet, _launchPoint ? _launchPoint : transform);
            _bullet.OnBulletHit = OnBulletHit;
        }

        private void Update()
        {
            _lastFire += Time.deltaTime;
            
            if(!_canFire && _lastFire >= _firingRate)
            {
                _canFire = true;
            }
        }

        public override void Fire()
        {
            if (!_canFire) return;
            

            if(PhotonNetwork.IsConnected && _photonView.IsMine)
            {
                _photonView.RPC("NetworkFire", RpcTarget.All);
            }
            else
            {
                NetworkFire();
            }

            _canFire = false;
            _lastFire = 0.0f;
        }

        [PunRPC]
        void NetworkFire()
        {
            _bullet.Fire(transform);    
        }
    }
}