using ExtensionMethods;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankBattle.Navigation.Desktop
{
    public class WaitingRoomDesktop : WaitingRoomManager
    {
        GameSettingsRoom _gameSettings;

        public override void Awake()
        {
            base.Awake();
            _gameSettings = transform.FirstOrDefault(t => t.name == "GameSettings").GetComponent<GameSettingsRoom>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.IsMasterClient)
            {
                _gameSettings.SetSettingsVisible(true);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if (!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.LocalPlayer == newMasterClient)
            {
                _gameSettings.SetSettingsVisible(true);
            }
        }
    }
}
