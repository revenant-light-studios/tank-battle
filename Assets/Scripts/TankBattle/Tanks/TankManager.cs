using System;
using Photon.Pun;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private CameraFollow _cameraFollow;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _cameraFollow = GetComponent<CameraFollow>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            if (_photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                _cameraFollow.StartFollowing();
                _playerInput.enabled = true;
            }
            else
            {
                _playerInput.enabled = false;
            }
        }
    }
}