using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Networking.Utilities;
using Photon.Realtime;
using TankBattle.InGameGUI.Hud;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TankBattle.Tanks
{
    public class TankFollowerManager : MonoBehaviourPunCallbacks
    {
        private int _currentPosCamera;
        private TankManager _currentTankFollow;
        private bool _isRunning;

        public TankManager CurrentTankFollow { get => _currentTankFollow; }

        public delegate void OnChangeTankFollowDelegate();
        public OnChangeTankFollowDelegate OnChangeTankFollow;

        private TankInput _tankInput;
        private ATankHud _tankHud;
        
        private void Awake()
        {
            _currentPosCamera = 0;
            _currentTankFollow = null;
            _isRunning = false;
        }

        public void InitTankFollower(TankManager tankManager)
        {
            TankValues tankValues = tankManager.GetComponent<TankValues>();
            tankValues.OnTankWasDestroyed += TankWasDestroyed;

            _tankInput = tankManager.GetComponent<TankInput>();
            _tankInput.SwitchTankTrigger.OnTriggerPressed += OnSwitchTriggerPressed;

            _tankHud = tankManager.TankHud;
        }

        private void SetHudFollowingMode()
        {
            
        }
        
        private void OnSwitchTriggerPressed()
        {
            if (_isRunning)
            {
                FollowNextTank();
            }
        }

        private void TankWasDestroyed(TankValues values)
        {
            Debug.Log($"TankFollowerManager: {gameObject.name} TankWasDestroyed called");
            TankManager tankManager = GetComponent<TankManager>();
            
            _tankInput.SwitchActionMap(TankInput.TankInputMaps.DeadPlayer);
            _isRunning = true;
            FollowNextTank();
        }
        
        private void FollowNextTank()
        {
            List<Player> players = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values.ToArray());
            int currentTank = _currentTankFollow != null ? players.IndexOf(_currentTankFollow.photonView.Owner) : 0;
            TankManager nextTankToFollow = null;
            
            for (int i = 0; i < players.Count; i++)
            {
                int nextTank = (currentTank + i) % players.Count;
                
                Debug.Log($"nextTank={nextTank}");
                
                if (players[nextTank].IsLocal) continue;
                
                if (players[nextTank].IsAlive())
                {
                    nextTankToFollow = players[nextTank].GetTank();
                }
            }

            if (nextTankToFollow != null && nextTankToFollow != _currentTankFollow)
            {
                Debug.Log($"Camera following {_currentTankFollow}");
                _currentTankFollow = nextTankToFollow;
                _currentTankFollow.CameraFollow.StartFollowing();    
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} TankFollowManager: No more alive tanks to follow");
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerExtensions.PlayerAlive) && _isRunning)
            {
                Debug.Log($"{targetPlayer.NickName} changed IsAlive {targetPlayer.IsAlive()}");
                FollowNextTank();
            }
        }
    }
}

