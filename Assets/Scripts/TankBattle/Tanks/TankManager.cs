using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Bullets;
using TankBattle.Tanks.Camera;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    [RequireComponent(typeof(PhotonView)),
     RequireComponent(typeof(ATankCamera)), 
     RequireComponent(typeof(PlayerInput)),
     RequireComponent(typeof(TankValues)),
     RequireComponent(typeof(DetectableObject))]
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private ATankCamera _cameraFollow;
        private PlayerInput _playerInput;
        private ATankHud _tankHud;
        private ATankTurret _turret;
        private TankValues _tankValues;
        private DetectableObject _detectableObject;
        
        public bool IsDummy = false;
        
        #region Public properties

        public ATankTurret Turret
        {
            get => _turret;
        }
        #endregion

        private void Awake()
        {
            _tankValues = GetComponent<TankValues>();
            _photonView = GetComponent<PhotonView>();
            _cameraFollow = GetComponent<ATankCamera>();
            _playerInput = GetComponent<PlayerInput>();
            _detectableObject = GetComponent<DetectableObject>();
            _turret = GetComponentInChildren<ATankTurret>();
        }

        private void Start()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                InitTankGunsFromPrefabs();
                InitEnemyTracker();
                InitInput();
                InitCamera();
                InitUI();

                // Put local tank in non collission layer
                gameObject.SetLayerRecursively(12);
            }
            else
            {
                Radar.Instance.AddDetectableObject(_detectableObject);
                
                _playerInput.enabled = false;
                _cameraFollow.enabled = false;

                if (IsDummy)
                {
                    GetComponent<AudioSource>().enabled = false;
                }
            }
        }

        private void Update()
        {
            UpdateEnemyTracker();
            
            // Testing
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SelectNextEnemy();
                }
            }
        }

        #region UI Management
        private void InitUI()
        {
            GameObject userUI = GameObject.FindGameObjectWithTag("UserGameUI");
            if (userUI)
            {
                _tankHud = userUI.transform.GetComponentInChildren<ATankHud>();
                _tankHud.RegisterTank(this);

            }
        }
        #endregion
        
        #region Camera management

        private void InitCamera()
        {
            _cameraFollow.StartFollowing();
        }
        #endregion

        #region Input management

        private void InitInput()
        {
            // Init input
            _playerInput.enabled = true;
            
            if (GlobalMethods.IsDesktop())
            {
                _playerInput.InitInput(null, null, null, null);
            }
            else
            {
                TankHudMobile tankHudMobile = (TankHudMobile)_tankHud;
                _playerInput.InitInput(tankHudMobile.MovementJoystick, tankHudMobile.AimJoystick, tankHudMobile.ShootBtn, tankHudMobile.SpecialShootBtn);
            }
        }
        #endregion
        
        #region Enemy tracking
        private List<DetectableObject> _inScreenTanks;
        private Transform _cameraTransform;
        private UnityEngine.Camera _camera;
        private Transform _launchPointTransform;
        private DetectableObject _trackedTank;
        
        private void InitEnemyTracker()
        {
            _cameraTransform = GameObject.Find("Camera Position")?.transform;
            _camera = _cameraTransform.FirstOrDefault(t => t.name == "Main Camera").GetComponent<UnityEngine.Camera>();
            _launchPointTransform = transform.FirstOrDefault(t => t.name == "FirePoint");
            
            _inScreenTanks = new List<DetectableObject>(Radar.Instance.DetectableObjects);
            foreach (DetectableObject tank in _inScreenTanks)
            {
                tank.InitTrackerImage(_tankHud.transform, _camera);
            }
            
            Radar.Instance.OnDetectableObjectAdded = o =>
            {
                _inScreenTanks.Add(o);
                o.InitTrackerImage(_tankHud.transform, _camera);
            };
            
            Radar.Instance.OnDetectableObjectRemoved = o => _inScreenTanks.Remove(o);
        }

        private void UpdateEnemyTracker()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                
                
                for (int i = _inScreenTanks.Count - 1; i >= 0 ; i--)
                {
                    DetectableObject dObj = _inScreenTanks[i];

                    if (dObj && dObj.gameObject != gameObject)
                    {
                        if (GeometryUtility.TestPlanesAABB(planes, dObj.Bounds))
                        {
                            if (!dObj.Visible)
                            {
                                // Debug.LogFormat("DetectableObject {0} entered camera", dObj.name);   
                            }
                            
                            dObj.Visible = true;

                            Vector3 direction = dObj.Bounds.center - _detectableObject.Bounds.center;
                            bool tracked = false;

                            if (Physics.Raycast(_detectableObject.Bounds.center, direction, out RaycastHit hit))
                            {
                                dObj.Tracked = (hit.transform.gameObject == dObj.gameObject);
                            }
                        }
                        else
                        {
                            if (dObj.Visible)
                            {
                                // Debug.LogFormat("DetectableObject {0} exit camera", Radar.Instance.DetectableObjects[i].name);
                            }

                            dObj.Visible = false;
                        }
                    }
                }
            }            
        }

        private void SelectNextEnemy()
        {
            int currentTankIndex = 0;
            DetectableObject currentTrackedTank = _trackedTank;
            DetectableObject tank;
            
            if (_trackedTank != null)
            {
                currentTankIndex = _inScreenTanks.IndexOf(_trackedTank);
            }
            
            for (int i = 0; i < _inScreenTanks.Count - 1; i++)
            {
                currentTankIndex = (currentTankIndex + 1) % _inScreenTanks.Count;
                tank = _inScreenTanks[currentTankIndex];
                
                if (tank.Visible && tank.Tracked)
                {
                    _trackedTank = tank;
                    _trackedTank.Locked = true;
                    if (_secondaryGun is MissileLauncher) ((MissileLauncher)_secondaryGun).TrackedTank = _trackedTank.gameObject;
                    
                    if (currentTrackedTank) currentTrackedTank.Locked = false;
                    break;
                }
            }
        }
        #endregion
        
        #region Weapons management

        private void InitTankGunsFromPrefabs()
        {
            if (_primaryGunPrefab)
            {
                PrimaryGun = _primaryGunPrefab;
            }

            if (_secondaryGunPrefab)
            {
                SecondaryGun = _secondaryGunPrefab;
            }
        }
        
        public enum TankWeapon
        {
            Primary,
            Secondary
        }
        
        [SerializeField, FormerlySerializedAs("PrimaryGun"), InspectorName("Primary gun Prefab")]
        private ATankGun _primaryGunPrefab;
        
        private ATankGun _primaryGun;
        public ATankGun PrimaryGun
        {
            get => _primaryGun;
            set
            {
                Transform firePoint = transform.FirstOrDefault(t => t.name == "FirePoint");
                _primaryGun = Instantiate(value, firePoint ? firePoint : transform);
                _onTankWeaponEnabled?.Invoke(_primaryGun, TankWeapon.Primary);
            }
        }
        
        [SerializeField, FormerlySerializedAs("SecondaryGun"), InspectorName("Secondary gun Prefab")]
        private ATankGun _secondaryGunPrefab;
        
        private ATankGun _secondaryGun;
        public ATankGun SecondaryGun
        {
            get => _secondaryGun;
            set
            {
                Transform missilePoint = transform.FirstOrDefault(t => t.name == "LaunchPoint");
                _secondaryGun = Instantiate(value, missilePoint ? missilePoint : transform);
                _onTankWeaponEnabled?.Invoke(_secondaryGun, TankWeapon.Secondary);
            }
        }
        
        public delegate void OnTankWeaponEnabledDelegate(ATankGun gun, TankWeapon weapon);
        private OnTankWeaponEnabledDelegate _onTankWeaponEnabled;
        public event OnTankWeaponEnabledDelegate OnTankWeaponEnabled
        {
            add
            {
                _onTankWeaponEnabled += value;

                if (_primaryGun != null)
                {
                    _onTankWeaponEnabled.Invoke(_primaryGun, TankWeapon.Primary);
                }

                if (_secondaryGun != null)
                {
                    _onTankWeaponEnabled.Invoke(_secondaryGun, TankWeapon.Secondary);
                }
            }

            remove
            {
                _onTankWeaponEnabled -= value;
            }
        }
        #endregion
    }
}