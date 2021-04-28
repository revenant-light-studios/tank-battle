using System;
using Photon.Pun;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Guns;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class TankManager : MonoBehaviour
    {
        private PhotonView _photonView;
        private CameraFollow _cameraFollow;
        private PlayerInput _playerInput;
        private TankHud _tankHud;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _cameraFollow = GetComponent<CameraFollow>();
            _playerInput = GetComponent<PlayerInput>();
            
            GameObject userUI = GameObject.Find("UserUI");
            if (userUI)
            {
                _tankHud = userUI.transform.GetComponentInChildren<TankHud>();
            }
        }

        private void Start()
        {
            if (_photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                _cameraFollow.StartFollowing();
                _playerInput.enabled = true;
                _tankHud.RegisterTank(gameObject);
            }
            else
            {
                _playerInput.enabled = false;
            }
        }
    }
}