using UnityEngine;

namespace TankBattle.Tanks
{
    /// <summary>
    /// Inpired by https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/player-camera-work
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
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
        
        private void Start()
        {
            if (_followOnStart)
            {
                StartFollowing();
            }    
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null && _isFollowing)
            {
                StartFollowing();
            }

            if (_isFollowing)
            {
                Follow();
            }
        }

        private void StartFollowing()
        {
            _cameraTransform = Camera.main.transform;
            _isFollowing = true;
            
                        
            Cut();
        }
        
        private void Follow()
        {
            _cameraOffset.z = -_distance;
            _cameraOffset.y = _height;

            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, transform.position + transform.TransformVector(_cameraOffset), _smoothSpeed * Time.deltaTime);
            _cameraTransform.LookAt(transform.position + _centerOffset);
        }

        private void Cut()
        {
            _cameraOffset.z = -_distance;
            _cameraOffset.y = _height;

            _cameraTransform.position = transform.position + transform.TransformVector(_cameraOffset);
            _cameraTransform.LookAt(transform.position + _centerOffset);
        }

    }
}