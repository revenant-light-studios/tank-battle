using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
            _canTrack = true;
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

        public override void RegisterInput(TankInput input)
        {
            TankInput = input;
            TankInput.Trigger2.OnTriggerPressed += () => TriggerPressed = true;
            TankInput.Trigger2.OnTriggerReleased += () => TriggerPressed = false;
        }
        
        protected virtual void Update()
        {
            LastFired += Time.deltaTime;
            
            if(!CanFire && LastFired >= _firingRate)
            {
                CanFire = _currentNumberOfBullets > 0;
                // Debug.Log($"Canfire: {CanFire} {_currentNumberOfBullets}");
            }
         
            if (TriggerPressed && CanFire)
            {
                Fire();
                CanFire = false;
                LastFired = 0.0f;
                CurrentNumberOfBullets--;
            }
        }

        [PunRPC]
        public override void NetworkFire()
        {
            if (!_missile) return;
            
            Missile missileInstance = (Missile)Instantiate(_missile);
            if(_parentTank)
            {
                // Debug.Log($"Ignore self collissions");
                Physics.IgnoreCollision(missileInstance.GetComponentInChildren<Collider>(), _parentTank.ForceField.GetComponent<Collider>());
                
            }
            
            if(_trackedTank) missileInstance.target = _trackedTank;
            missileInstance.transform.position = _launcher.transform.position;
            missileInstance.transform.rotation = _launcher.transform.rotation;
            missileInstance.Fire(transform);
            missileInstance.OnBulletHit = OnBulletHit;

            if (_launchSound)
            {
                _launchSound.PlayOneShot(_launchSound.clip);
            }
        }
    }
}