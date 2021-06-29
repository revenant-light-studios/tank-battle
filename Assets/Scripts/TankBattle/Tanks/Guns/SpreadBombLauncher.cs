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
            LastFired += Time.deltaTime;
            
            if(!CanFire && LastFired >= _firingRate)
            {
                CanFire = _currentNumberOfBullets > 0;
                // Debug.Log($"Canfire: {CanFire} {_currentNumberOfBullets}");
            }

            if (CanFire)
            {
                if (!_aim) return;  // Safety check
                
                if (TriggerPressed)
                {
                    ShowAim(true);
                    Vector3 aimPosition = transform.position;
                    aimPosition.y = 0;
                    _aim.transform.position = aimPosition;
                }
                else
                {
                    if (_aim.activeSelf)
                    {
                        Fire();
                        CanFire = false;
                        LastFired = 0.0f;
                        CurrentNumberOfBullets--;
                        ShowAim(false);                        
                    }
                }
            }
        }

        private void OnTrigger2Released()
        {
            TriggerPressed = false;
        }
        private void OnTrigger2Pressed()
        {
            TriggerPressed = true;
        }

        private void ShowAim(bool show)
        {
            if(_aim) _aim.SetActive(show);
        }

        [PunRPC]
        public override void NetworkFire()
        {
            Transform launchTransform = _launchPoint ? _launchPoint : transform;
            _bullet = Instantiate(TankBullet);
            _bullet.transform.position = launchTransform.position;
            _bullet.OnBulletHit = OnBulletHit;
            _bullet.Fire(transform);
        }
    }
}