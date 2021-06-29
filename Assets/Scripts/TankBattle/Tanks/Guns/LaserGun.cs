using ExtensionMethods;
using Photon.Pun;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public class LaserGun : ATankGun
    {
        [SerializeField, FormerlySerializedAs("MaxEnergy"), InspectorName("Maximum energy"), Tooltip("Maximum capacity of gun")]
        private float _maxEnergy = 1.0f;

        [SerializeField, FormerlySerializedAs("UnloadRate"), InspectorName("UnloadRate"), Tooltip("Amount of energy consumed by each shot")] 
        private float _unloadRate = 0.1f;

        [SerializeField, FormerlySerializedAs("ReloadRate"), InspectorName("ReloadRate"),  Tooltip("Time to fully reload the weapon in seconds")]
        private float _reloadTime = 3.0f;
        
        private float _energy;
        private float _reloadRate;
        
        private ParticleSystem _muzzleParticleSystem;
        private ATankBullet _bullet;
        private CrossHair _crossHair;
        private AudioSource _gunAudio;

        protected override void Awake()
        { 
            base.Awake();
            _muzzleParticleSystem = transform.FirstOrDefault(t => t.name == "TankMuzzleFlash").GetComponent<ParticleSystem>();
            _gunAudio = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _energy = _maxEnergy;
        }
        
        protected override void Update()
        {
            LastFired += Time.deltaTime;
            
            if(!CanFire && LastFired >= _firingRate)
            {
                CanFire = _energy >= _unloadRate;
                // Debug.Log($"LastFired: {LastFired} -> CanFire: {_energy >= _unloadRate}");
            }
            
            if (TriggerPressed && CanFire)
            {
                Fire();
                CanFire = false;
                LastFired = 0.0f;
                UpdateEnergy(_energy - _unloadRate);
            }        
            
            if (!TriggerPressed && LastFired > 2 * _firingRate)
            {
                if (_energy < _maxEnergy)
                {
                    _reloadRate = _maxEnergy / _reloadTime;
                    UpdateEnergy(_energy + _reloadRate * Time.deltaTime);
                }
            }
        }
        
        private void UpdateEnergy(float value)
        {
            _energy = Mathf.Clamp(value, 0.0f, 1.0f);
            OnEnergyUpdate?.Invoke(_energy / _maxEnergy, _unloadRate);
        }

        [PunRPC]
        public override void NetworkFire()
        {
                if (!TankBullet) return;

                ATankBullet bulletInstance = Instantiate(TankBullet, transform.position, transform.rotation);
                Collider bulletCollider = bulletInstance.GetComponentInChildren<Collider>();
                
                if (_parentTank)
                {
                    if (_parentTank.ForceField)
                    {
                        Physics.IgnoreCollision(bulletCollider, _parentTank.ForceField.GetComponent<Collider>());    
                    }
                    
                    foreach (Collider col in _parentTank.gameObject.GetAllColliders())
                    {
                        Physics.IgnoreCollision(bulletCollider, col);
                    }
                }
                
                bulletInstance.OnBulletHit = OnBulletHit;
                bulletInstance.Fire(transform);

                _muzzleParticleSystem.Play();
                _gunAudio?.PlayOneShot(_gunAudio.clip);
        }
    }
}