using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public class SpreadBombLauncher : ATankGun
    {
        private ATankBullet _bullet;
        private Transform _launchPoint;
        private GameObject _aim;
        
        protected override void Awake()
        {
            base.Awake();
            
            _launchPoint = transform.FirstOrDefault(t => t.name == "LaunchPoint");
        }

        private void OnDestroy()
        {
            if(_aim) Destroy(_aim.gameObject);
        }

        public override void RegisterInput(TankInput input)
        {
            _aim = transform.FirstOrDefault(t => t.name == "Aim")?.gameObject;
            _aim.transform.SetParent(null, true);

            TankInput = input;
            TankInput.Trigger2.OnTriggerPressed += OnTrigger2Pressed;
            TankInput.Trigger2.OnTriggerReleased += OnTrigger2Released;
        }

        protected override void Update()
        {
            if(_aim)
            {
                if (_aim.activeSelf)
                {
                    Vector3 aimPosition = transform.position;
                    aimPosition.y = 0;
                    _aim.transform.position = aimPosition;
                }
            }
            
            if (TriggerPressed)
            {
                CanFire = true;
                TriggerPressed = false;
                Fire();
            }
        }

        private void OnTrigger2Released()
        {
            TriggerPressed = true;
            ShowAim(false);
        }
        private void OnTrigger2Pressed()
        {
            ShowAim(true);
        }

        private void ShowAim(bool show)
        {
            if(_aim) _aim.SetActive(show);
        }

        [PunRPC]
        public override void NetworkFire()
        {
            _bullet = Instantiate(TankBullet, _launchPoint ? _launchPoint : transform);
            _bullet.OnBulletHit = OnBulletHit;
            _bullet.Fire(transform);
            CurrentNumberOfBullets--;
        }
    }
}