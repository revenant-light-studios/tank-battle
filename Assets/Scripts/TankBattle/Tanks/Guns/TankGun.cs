using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public class TankGun : ATankGun
    {
        [SerializeField, FormerlySerializedAs("FiringRate"), InspectorName("Fire rate"), Tooltip("Seconds between consecutive shots")] 
        private float _firingRate = 2f;
     
        [SerializeField, FormerlySerializedAs("MaxEnergy"), InspectorName("Maximum energy"), Tooltip("Maximum capacity of gun")]
        private float _maxEnergy = 1.0f;

        [SerializeField, FormerlySerializedAs("UnloadRate"), InspectorName("UnloadRate"), Tooltip("Amount of energy consumed by each shot")] 
        private float _unloadRate = 0.1f;

        [SerializeField, FormerlySerializedAs("ReloadRate"), InspectorName("ReloadRate"),  Tooltip("Time to fully reload the weapon in seconds")]
        private float _reloadTime = 3.0f;
        
        private float _energy;
        private float _lastFired;
        private bool _canFire;
        private float _reloadRate;

        public override float FiringRate => _firingRate;

        private Transform _cannonTransform;
        private ParticleSystem _muzzleParticleSystem;
        private ATankBullet _bullet;
        private PhotonView _photonView;
        private CrossHair _crossHair;
        private AudioSource _gunAudio;

        private void Awake()
        {
            _cannonTransform = transform.FirstOrDefault(t => t.name == "FirePoint");
            _muzzleParticleSystem = transform.FirstOrDefault(t => t.name == "TankMuzzleFlash").GetComponent<ParticleSystem>();
            _photonView = GetComponent<PhotonView>();
            
            _bullet = Instantiate(TankBullet, _cannonTransform);
            _bullet.OnBulletHit = OnBulletHit;
                
            _gunAudio = GetComponent<AudioSource>();

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas)
            {
                _crossHair = canvas.transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<CrossHair>();    
            }
        }

        private void OnEnable()
        {
            _energy = _maxEnergy;
        }
        
        private void Update()
        {
            if (_photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                if (_crossHair != null && Physics.Raycast(_cannonTransform.position, _cannonTransform.forward, out RaycastHit hit))
                {
                    Vector3 position = UnityEngine.Camera.main.WorldToScreenPoint(hit.point);
                    _crossHair.transform.position = position;
                }
            }
            
            _lastFired += Time.deltaTime;
            
            if (_lastFired > 2 * _firingRate)
            {
                if (_energy < _maxEnergy)
                {
                    _reloadRate = _maxEnergy / _reloadTime;
                    UpdateEnergy(_energy + _reloadRate * Time.deltaTime);
                }
            }
            
            if (!_canFire && _lastFired >= _firingRate)
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

            _lastFired = 0.0f;
            _canFire = false;
        }

        [PunRPC]
        void NetworkFire()
        {
            if (_energy >= _unloadRate)
            {
                _bullet?.Fire(_cannonTransform);
                _muzzleParticleSystem.Play();
                _gunAudio?.PlayOneShot(_gunAudio.clip);
                UpdateEnergy(_energy - _unloadRate);
            }
        }

        private void UpdateEnergy(float value)
        {
            _energy = Mathf.Clamp(value, 0.0f, 1.0f);
            if(_crossHair) _crossHair.UpdateEnergy(_energy / _maxEnergy, _unloadRate);
        }
    }
}