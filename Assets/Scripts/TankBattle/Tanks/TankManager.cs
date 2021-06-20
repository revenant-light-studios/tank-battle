using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.InGameGUI.Hud;
using TankBattle.Navigation;
using TankBattle.Tanks.Camera;
using TankBattle.Tanks.ForceFields;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    [RequireComponent(typeof(PhotonView)),
     RequireComponent(typeof(ATankCamera)), 
     RequireComponent(typeof(TankInput)),
     RequireComponent(typeof(TankValues)),
     RequireComponent(typeof(DetectableObject))]
    public class TankManager : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private PhotonView _photonView;
        private ATankCamera _cameraFollow;
        private TankInput _tankInput;
        private ATankHud _tankHud;
        private ATankTurret _turret;
        private TankValues _tankValues;
        private DetectableObject _detectableObject;
        
        [SerializeField, FormerlySerializedAs("ForceField")] private ForceField _forceFieldPrefab;
        private ForceField _forceField;
        public ForceField ForceField => _forceField;

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
            _tankInput = GetComponent<TankInput>();
            _detectableObject = GetComponent<DetectableObject>();
            _turret = GetComponentInChildren<ATankTurret>();
            _tankHud = PlayRoomManager.Instance.UserUI.GetComponentInChildren<ATankHud>();
            
            if (!_forceFieldPrefab)
            {
                _forceFieldPrefab = Resources.Load<ForceField>("Tanks/ForceFields/ForceField");
            }
        }

        private void Start()
        {
            // All tanks
            if (_forceFieldPrefab)
            {
                _forceField = Instantiate(_forceFieldPrefab);
                _forceField.ParentTank = this;
                _tankValues.ForceField = _forceField.GetComponent<ForceField>();
            }
            
            // All network tanks
            if (PhotonNetwork.IsConnected)
            {
                if (_photonView && _photonView.Owner != null)
                {
                    gameObject.name = _photonView.Owner.NickName;    
                }
            }
            
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                // Only local player tank
                InitUI();
                InitEnemyTracker();
                InitInput();
                InitCamera();
                InitTankGunsFromPrefabs();
                
                // Put local tank in non collission layer
                // gameObject.SetLayerRecursively(12);
            }
            else
            {
                // All non local tanks
                Radar.Instance.AddDetectableObject(_detectableObject);

                _tankInput.enabled = false;
                _cameraFollow.enabled = false;

                if (IsDummy)
                {
                    GetComponent<AudioSource>().enabled = false;
                }
            }
        }

        private void Update()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                UpdateEnemyTracker();
            }
        }

        private void FixedUpdate()
        {
            Vector3 position = transform.position;
            position.y = 0f;
            _forceField.transform.position = position;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
        }

        #region UI Management
        private void InitUI()
        {
            _tankHud.RegisterTank(this);
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
            _tankInput.enabled = true;
            _tankInput.InitInput();
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
            _tankInput.LockTrigger.OnTriggerPressed += () => SelectNextEnemy();
            
            _cameraTransform = GameObject.Find("Camera Position")?.transform;
            _camera = _cameraTransform.FirstOrDefault(t => t.name == "Main Camera").GetComponent<UnityEngine.Camera>();
            _launchPointTransform = transform.FirstOrDefault(t => t.name == "FirePoint");

            _inScreenTanks = new List<DetectableObject>();
            
            Debug.Log($"InitEnemyTracker, tanks: {Radar.Instance.DetectableObjects.Count}");
            foreach (DetectableObject tank in Radar.Instance.DetectableObjects)
            {
                tank.InitTrackerImage(_tankHud.transform, _camera);
            }
            
            Radar.Instance.OnDetectableObjectAdded = o =>
            {
                if (!_inScreenTanks.Contains(o))
                {
                    _inScreenTanks.Add(o);
                    o.InitTrackerImage(_tankHud.transform, _camera);
                }
            };
            
            Radar.Instance.OnDetectableObjectRemoved = o => _inScreenTanks.Remove(o);
        }

        private void UpdateEnemyTracker()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                int layerMask = LayerMask.GetMask("SelfTank", "Ground");

                if (!_camera)
                {
                    Debug.Log($"{name}: Camera is null");
                    return;
                }
                
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

                            if (Physics.Raycast(_detectableObject.Bounds.center, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
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
            int currentTankIndex = -1;
            DetectableObject tank;
            
            if (_trackedTank != null)
            {
                currentTankIndex = _inScreenTanks.IndexOf(_trackedTank);
            }
            
            for (int i = 0; i < _inScreenTanks.Count; i++)
            {
                currentTankIndex = (currentTankIndex + 1) % _inScreenTanks.Count;
                tank = _inScreenTanks[currentTankIndex];
                
                // Debug.Log($"Trying to track {tank.name}: Visible({tank.Visible}) Tracked({tank.Tracked})");
                
                if (tank.Visible && tank.Tracked)
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        int tankViewId = tank.GetComponent<PhotonView>().ViewID;
                        // Debug.Log($"Locking tank {tank.name} with id {tankViewId}");
                        _photonView.RPC("RemoteTankLock", RpcTarget.All, tankViewId);
                    }
                    else
                    {
                        TankLock(tank);
                    }
                    break;
                }
            }
        }

        [PunRPC]
        public void RemoteTankLock(int tankViewId)
        {
            // Debug.Log($"{gameObject.name} recevied message to lock view {tankViewId}");
            PhotonView tankView = PhotonNetwork.GetPhotonView(tankViewId);
            DetectableObject tank = tankView.GetComponent<DetectableObject>();
            TankLock(tank);
        }
        
        public void TankLock(DetectableObject tank)
        {
            if (_trackedTank) _trackedTank.Locked = false;
            _trackedTank = tank;
            _trackedTank.Locked = true;
            
            if (_secondaryGun is MissileLauncher) ((MissileLauncher)_secondaryGun).TrackedTank = _trackedTank.gameObject;
        }
        #endregion
        
        #region Weapons management

        private void InitTankGunsFromPrefabs()
        {
            if (_primaryGunPrefab)
            {
                PickWeapon(_primaryGunPrefab.name, TankWeapon.Primary);
            }

            if (_secondaryGunPrefab)
            {
                PickWeapon(_secondaryGunPrefab.name, TankWeapon.Secondary);
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
                if (_primaryGun != null)
                {
                    Destroy(_primaryGun.gameObject);
                    _onTankWeaponEnabled?.Invoke(null, TankWeapon.Primary);   
                }
                
                _primaryGun = value;
                
                if(value != null)
                {
                    Debug.Log($"{name}: Set gun {value.name}");
                    Transform firePoint = transform.FirstOrDefault(t => t.name == "FirePoint");
                    _primaryGun.transform.SetParent(firePoint, false);
                    _primaryGun.RegisterInput(_tankInput);
                    _primaryGun.ParentTank = this;
                    _primaryGun.OnNumberOfBulletsChange += bullets => WeaponNumberOfBulletsChange(bullets, TankWeapon.Primary);
                    _onTankWeaponEnabled?.Invoke(_primaryGun, TankWeapon.Primary);
                }
            }
        }

        private void WeaponNumberOfBulletsChange(int numberOfBullets, TankWeapon weapon)
        {
            if (PhotonNetwork.IsConnected && !_photonView.IsMine) return;

            if (numberOfBullets == 0)
            {
                DropWeapon(weapon);
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
                if (_secondaryGun != null)
                {
                    Destroy(_secondaryGun.gameObject);
                    _onTankWeaponEnabled?.Invoke(null, TankWeapon.Secondary);
                }
                
                _secondaryGun = value;
                
                if(value != null)
                {
                    Transform missilePoint = transform.FirstOrDefault(t => t.name == "LaunchPoint");
                    _secondaryGun.transform.SetParent(missilePoint, false);
                    _secondaryGun.RegisterInput(_tankInput);
                    _secondaryGun.ParentTank = this;
                    _secondaryGun.OnNumberOfBulletsChange += bullets => WeaponNumberOfBulletsChange(bullets, TankWeapon.Secondary);
                    _onTankWeaponEnabled?.Invoke(_secondaryGun, TankWeapon.Secondary);
                }
                
                if (_trackedTank) _trackedTank.Locked = false;
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
        
        public void PickWeapon(string weaponName, TankWeapon weapon)
        {
            string resource = $"Guns/{weaponName}";

            if (PhotonNetwork.IsConnected)
            {
                GameObject gun = PhotonNetwork.Instantiate(resource, Vector3.zero, Quaternion.identity, 0, 
                    new object[] { _photonView.ViewID, weapon });
            }
            else
            {
                ATankGun gunPrefab = Resources.Load<ATankGun>(resource);
                ATankGun gun = Instantiate(gunPrefab);
                
                if (weapon == TankWeapon.Primary)
                {
                    PrimaryGun = gun;
                }
                else
                {
                    SecondaryGun = gun;
                }
            }
        }

        public void DropWeapon(TankWeapon weapon)
        {
            if (PhotonNetwork.IsConnected)
            {
                _photonView.RPC("NetworkDropWeapon", RpcTarget.All, weapon);
            }
            else
            {
                NetworkDropWeapon(weapon);
            }
        }

        [PunRPC]
        public void NetworkDropWeapon(TankWeapon weapon)
        {
            if (weapon == TankWeapon.Primary) PrimaryGun = null;
            else SecondaryGun = null;
        }
        #endregion
    }
}