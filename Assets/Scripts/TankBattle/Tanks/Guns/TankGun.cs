using System;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.UI;

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

        private PhotonView _photonView;
        private Image _crossHairImage;
        
        private void Start()
        {
            _cannonTransform = transform.FirstOrDefault(t => t.name == "FirePoint");
            _muzzleParticleSystem = transform.FirstOrDefault(t => t.name == "TankMuzzleFlash").GetComponent<ParticleSystem>();
            _photonView = GetComponent<PhotonView>();
            
            _bullet = Instantiate(TankBullet, _cannonTransform);

            Canvas canvas = FindObjectOfType<Canvas>();
            _crossHairImage = canvas.transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<Image>();
        }

        private void Update()
        {
            if (_photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                if (Physics.Raycast(_cannonTransform.position, _cannonTransform.forward, out RaycastHit hit))
                {
                    Vector3 position = Camera.main.WorldToScreenPoint(hit.point);
                    _crossHairImage.transform.position = position;
                }
            }
        }

        public override void Fire()
        {
            _bullet?.Fire(_cannonTransform);
            _muzzleParticleSystem.Play();
        }
    }
}