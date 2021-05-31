using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Camera
{
    public class CinemachineFollow : ATankCamera
    {
        [Tooltip("Transform to follow")]
        [SerializeField, FormerlySerializedAs("CinemachineFollow")]
        private Transform _followTransform;

        [Tooltip("Transform to look at")]
        [SerializeField, FormerlySerializedAs("CinemachineLookAt")]
        private Transform _lookAtTransform;

        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        public override void StartFollowing()
        {
            _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            _cinemachineVirtualCamera.Follow = _followTransform;
            _cinemachineVirtualCamera.LookAt = _lookAtTransform;
            _cinemachineVirtualCamera.transform.position = _followTransform.transform.position;
        }
    }
}