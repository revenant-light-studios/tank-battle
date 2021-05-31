using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Camera;
using TankBattle.Tanks.Guns;
using UnityEngine;
using UnityEngine.AI;

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
        }

        private void Update()
        {
            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                List<DetectableObject> objects = new List<DetectableObject>(Radar.Instance.DetectableObjects);
                
                for (int i = 0; i < objects.Count; i++)
                {
                    DetectableObject dObj = objects[i];

                    if (dObj && dObj.gameObject != gameObject)
                    {
                        if (GeometryUtility.TestPlanesAABB(planes, dObj.Bounds))
                        {
                            if (!dObj.IsVisibleInCamera)
                            {
                                // Debug.LogFormat("DetectableObject {0} entered camera", dObj.name);   
                            }
                            
                            dObj.IsVisibleInCamera = true;

                            Vector3 direction = dObj.Bounds.center - _detectableObject.Bounds.center;
                            bool tracked = false;

                            if (Physics.Raycast(_detectableObject.Bounds.center, direction, out RaycastHit hit))
                            {
                                if (hit.transform.gameObject == dObj.gameObject)
                                {
                                    // Debug.Log($"{dObj.name} tracked");
                                    tracked = true;
                                }
                            }

                            if (tracked)
                            {
                                dObj.ShowTrackerImage(_tankHud.transform, dObj.gameObject);
                                dObj.UpdateTrackerImage(_camera);
                            }
                            else
                            {
                                dObj.HideTrackerImage();
                            }
                        }
                        else
                        {
                            if (dObj.IsVisibleInCamera)
                            {
                                // Debug.LogFormat("DetectableObject {0} exit camera", Radar.Instance.DetectableObjects[i].name);
                            }

                            dObj.IsVisibleInCamera = false;
                            dObj.HideTrackerImage();
                        }
                    }
                }
                
            }
        }
    }
}