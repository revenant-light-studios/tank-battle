using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Camera;
using TankBattle.Tanks.Guns;
using UnityEngine;

namespace TankBattle.Tanks
{
    [RequireComponent(typeof(PhotonView)), 
     RequireComponent(typeof(ATankCamera)), 
     RequireComponent(typeof(PlayerInput)), 
     RequireComponent(typeof(DetectableObject))]
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private ATankCamera _cameraFollow;
        private PlayerInput _playerInput;
        private ATankHud _tankHud;
        private DetectableObject _detectableObject;

        private List<DetectableObject> _inScreenTanks;

        public bool IsDummy = false;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _cameraFollow = GetComponent<ATankCamera>();
            _playerInput = GetComponent<PlayerInput>();
            _detectableObject = GetComponent<DetectableObject>();
        }

        private void Start()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                InitLocalTank();
                InitEnemyTracker();

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
                    GetComponent<TankGun>().enabled = false;
                    GetComponent<AudioSource>().enabled = false;
                }
            }
        }
        
        private Transform _launchPointTransform;
        private Transform _cameraTransform;
        private UnityEngine.Camera _camera;

        private void InitLocalTank()
        {
            GameObject userUI = GameObject.FindGameObjectWithTag("UserGameUI");
            if (userUI)
            {
                _tankHud = userUI.transform.GetComponentInChildren<ATankHud>();
                _tankHud.RegisterTank(gameObject);

            }

            _cameraFollow.StartFollowing();
            _playerInput.enabled = true;
                
            if (_tankHud is TankHudMobile)
            {
                TankHudMobile tankHudMobile = (TankHudMobile)_tankHud;
                _playerInput.InitInput(tankHudMobile.MovementJoystick, tankHudMobile.AimJoystick);    
            }
            else
            {
                _playerInput.InitInput(null, null);
            }
        }

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
        
        private void Update()
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
    }
}