using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankBattle.Tanks
{
    public class TankViewerManager : MonoBehaviourPunCallbacks, IPointerDownHandler
    {
        private TankManager[] _tankManagersList;
        private int _currentPosCamera;
        private int _livingPlayers;
        private TankManager _currentTankFollow;
        private bool _followMode;
        private bool _canChangeCamera;

        public TankManager CurrentTankFollow { get => _currentTankFollow; }

        public delegate void OnChangeTankFollowDelegate();
        public OnChangeTankFollowDelegate OnChangeTankFollow;

        private void Awake()
        {
            _currentPosCamera = 0;
            _livingPlayers = -1;
            _currentTankFollow = null;
            _followMode = false;
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            UpdateFollowTankList();
            SelectTankToFollow();
        }

        private void OnDisable()
        {
           
        }


        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(RoomExtension.LivingPlayersPropertyName))
            {
                var currentLivingPlayers = PhotonNetwork.CurrentRoom.GetLivingPlayers();
                if(currentLivingPlayers != _livingPlayers && _followMode)
                {
                    
                    UpdateFollowTankList();
                    SelectTankToFollow();
                }
                _livingPlayers = currentLivingPlayers;
            }

        }

        private void UpdateFollowTankList()
        {
            Debug.Log($"Update Viewer");
            List<TankManager> newTankList = new List<TankManager>();
            var allTankList = FindObjectsOfType<TankManager>();
            foreach(var tank in allTankList)
            {
                if (tank.Turret.gameObject.activeSelf)
                {
                    newTankList.Add(tank);
                }
            }

            _tankManagersList = newTankList.ToArray();
        }

        private void SelectTankToFollow()
        {
            if (_currentTankFollow == null)
            {
                _currentTankFollow = _tankManagersList[0];
                _currentTankFollow.CameraFollow.StartFollowing();
                _currentPosCamera = 0;
                OnChangeTankFollow?.Invoke();
            }
            else
            {
                var continueMyFollowTank = false;
                
                for(var i = 0; i < _tankManagersList.Length; i++) 
                {
                    if(_tankManagersList[i].gameObject.name == _currentTankFollow.gameObject.name)
                    {
                        continueMyFollowTank = true;
                        _currentPosCamera = i;
                        break;
                    }
                }
                if (!continueMyFollowTank)
                {
                    _currentTankFollow = _tankManagersList[0];
                    _currentTankFollow.CameraFollow.StartFollowing();
                    _currentPosCamera = 0;
                    OnChangeTankFollow?.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Click");
        }
    }
}

