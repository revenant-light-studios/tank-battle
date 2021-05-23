using System;
using System.Collections.Generic;
using System.Xml.Schema;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Guns;
using TankBattle.Navigation;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    [RequireComponent(typeof(PhotonView)), 
     RequireComponent(typeof(CameraMovement)), 
     RequireComponent(typeof(PlayerInput)), 
     RequireComponent(typeof(DetectableObject))]
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private CameraMovement _cameraMovement;
        //private CameraFollow _cameraFollow;
        private PlayerInput _playerInput;
        private TankHud _tankHud;
        private DetectableObject _detectableObject;
        

        public bool IsDummy = false;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _cameraMovement = GetComponent<CameraMovement>();
            //_cameraFollow = GetComponent<CameraFollow>();
            
            _detectableObject = GetComponent<DetectableObject>();

            GameObject userUI = GameObject.Find("UserUI");
            GameObject userUIMobile = GameObject.Find("UserUIMobile");

            bool _isDesktop = GameObject.Find("GameManager").GetComponent<PlayRoomManager>().IsDesktop;
            Debug.Log("ISDesktop: " + _isDesktop);

            if (_isDesktop)
            {
                _tankHud = userUI.transform.GetComponentInChildren<TankHud>();
                _playerInput = GetComponent<PlayerInputDesktop>();
            }
            else
            {
                _tankHud = userUIMobile.transform.GetComponentInChildren<TankHudMobile>();
                _playerInput = GetComponent<PlayerInputMobile>();
            }

            InitEnemyTracker();
        }

        private void Start()
        {

            if ((_photonView.IsMine || !PhotonNetwork.IsConnected) && !IsDummy)
            {
                //_cameraFollow.StartFollowing();
                _cameraMovement.StartFollowing();
                _playerInput.enabled = true;
                _tankHud.RegisterTank(gameObject);
            }
            else
            {
                Radar.Instance.AddDetectableObject(_detectableObject);
                
                _playerInput.enabled = false;
                //_cameraFollow.enabled = false;

                if (IsDummy)
                {
                    GetComponent<TankGun>().enabled = false;
                    GetComponent<AudioSource>().enabled = false;
                }
            }
        }

        private Transform _launchPointTransform;
        private Transform _cameraTransform;
        private Camera _camera;

        private void InitEnemyTracker()
        {
            _cameraTransform = GameObject.Find("Camera Position")?.transform;
            _camera = _cameraTransform.FirstOrDefault(t => t.name == "Main Camera").GetComponent<Camera>();
            _launchPointTransform = transform.FirstOrDefault(t => t.name == "FirePoint");
        }

        private void Update()
        {
            if (_photonView.IsMine || (!PhotonNetwork.IsConnected && !IsDummy))
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
                                // Debug.LogFormat("DetectableObject {0} entered camera", Radar.Instance.DetectableObjects[i].name);   
                            }
                            
                            dObj.IsVisibleInCamera = true;

                            Vector3 direction = dObj.Bounds.center - _detectableObject.Bounds.center;
                            bool tracked = false;

                            if (Physics.Raycast(_detectableObject.Bounds.center, direction, out RaycastHit hit))
                            {
                                if (hit.transform.gameObject == gameObject)
                                {
                                    Debug.LogFormat("Hitting myself {0}", transform.name);
                                }
                                else if (hit.transform.tag == "Tank" && hit.transform.gameObject == dObj.gameObject)
                                {
                                    tracked = true;
                                }
                            }

                            if (tracked)
                            {
                                dObj.ShowTrackerImage(_tankHud.transform);
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