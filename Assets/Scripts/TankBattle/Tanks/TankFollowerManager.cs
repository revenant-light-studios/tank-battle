using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Networking.Utilities;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TankBattle.Tanks
{
    public class TankFollowerManager : MonoBehaviourPunCallbacks
    {
        private int _currentPosCamera;
        private TankManager _currentTankFollow;
        private bool _canChangeCamera;

        public TankManager CurrentTankFollow { get => _currentTankFollow; }

        public delegate void OnChangeTankFollowDelegate();
        public OnChangeTankFollowDelegate OnChangeTankFollow;
        
        private void Awake()
        {
            _currentPosCamera = 0;
            _currentTankFollow = null;
        }

        public void InitTankFollower(TankManager tankManager)
        {
            TankValues tankValues = tankManager.GetComponent<TankValues>();
            tankValues.OnTankWasDestroyed += TankWasDestroyed;

            TankInput inputManager = tankManager.GetComponent<TankInput>();
            inputManager.SwitchTankTrigger.OnTriggerPressed += OnSwitchTriggerPressed;
        }
        private void OnSwitchTriggerPressed()
        {
            if (gameObject.activeSelf)
            {
                FollowNextTank();
            }
        }

        private void TankWasDestroyed(TankValues values)
        {
            Debug.Log($"TankFollowerManager: {gameObject.name} TankWasDestroyed called");
            TankManager tankManager = GetComponent<TankManager>();
            gameObject.SetActive(true);
            FollowNextTank();
        }
        
        private void FollowNextTank()
        {
            List<Player> players = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values.ToArray());
            int currentTank = _currentTankFollow != null ? players.IndexOf(_currentTankFollow.photonView.Owner) : -1;
            TankManager nextTankToFollow = null;
            
            for (int i = 0; i < players.Count; i++)
            {
                int nextTank = (currentTank + i) % players.Count;
                
                Debug.Log($"{players[nextTank].NickName} IsAlive: {players[nextTank].IsAlive()}");
                
                if (players[nextTank].IsLocal) continue;
                
                if (players[nextTank].IsAlive())
                {
                    nextTankToFollow = players[nextTank].GetTank();
                }
            }

            if (nextTankToFollow != null && nextTankToFollow != _currentTankFollow)
            {
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
            foreach (DictionaryEntry changedProp in changedProps)
            {
                Debug.Log($"{targetPlayer.NickName} changed IsAlive {targetPlayer.IsAlive()}");   
            }
            
            if (changedProps.ContainsKey(PlayerExtensions.PlayerAlive))
            {
                // FollowNextTank();
            }
        }
    }
}

