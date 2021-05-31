using UnityEngine;

namespace TankBattle.Tanks.Camera
{
    /// <summary>
    /// Inpired by https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/player-camera-work
    /// </summary>
    public class CameraFollow : ATankCamera
    {
        [Tooltip("Transform to follow")]
        [SerializeField]
        private Transform _followTransform;
        
        [Tooltip("Distance from the camera to the target")]
        [SerializeField]
        private float _distance;
        
        [Tooltip("Height from the camera to the floor")]
        [SerializeField]
        private float _height;

        [Tooltip("Offset for the camera")]
        [SerializeField]
        private Vector3 _centerOffset;
        
        [Tooltip("The Smoothing for the camera to follow the target")]
        [SerializeField]
        private float _smoothSpeed = 0.125f;
        
        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool _followOnStart = false;

        private bool _isFollowing;
        private Transform _cameraTransform;
        Vector3 _cameraOffset = Vector3.zero;
        
        public Transform CameraTransform { get; private set; }
        
        
        private void Start()
        {
            if (_followTransform == null)
            {
                _followTransform = transform;
            }
        }

        private void LateUpdate()
        {
            if (_isFollowing)
            {
                Follow();
            }
        }

        public override void StartFollowing()
        {
            // _cameraTransform = Camera.main.transform;
            _cameraTransform = GameObject.Find("Camera Position")?.transform;

            if (_cameraTransform)
            {
                    
                _isFollowing = true;    
            }
            
            Cut();
        }
        
        private void Follow()
        {
            _cameraOffset.z = -_distance;
            _cameraOffset.y = _height;

            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _followTransform.position + _followTransform.TransformVector(_cameraOffset), _smoothSpeed * Time.deltaTime);
            _cameraTransform.LookAt(_followTransform.position + _centerOffset);
        }

        private void Cut()
        {
            _cameraOffset.z = -_distance;
            _cameraOffset.y = _height;

            _cameraTransform.position = _followTransform.position + _followTransform.TransformVector(_cameraOffset);
            _cameraTransform.LookAt(_followTransform.position + _centerOffset);
        }

    }
}