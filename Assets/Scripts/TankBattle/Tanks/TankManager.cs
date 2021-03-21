using System;
using Photon.Pun;
using TankBattle.Tanks.Guns;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private CameraFollow _cameraFollow;
        private PlayerInput _playerInput;
        private ATankGun _tankGun;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _cameraFollow = GetComponent<CameraFollow>();
            _playerInput = GetComponent<PlayerInput>();
            _tankGun = GetComponent<TankGun>();
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