using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExtensionMethods;
using TankBattle.Navigation;

namespace HightTide.UI.Mobile
{
    public class RoomManagerMobile : RoomManager
    {
        protected override void SetRoomMode()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _loadBtn.gameObject.SetActive(true);
                _roomName.gameObject.SetActive(true);
                _title.gameObject.SetActive(false);

                _privateRoom.interactable = true;
                _roomName.interactable = true;
                _loadBtn.interactable = true;
            }
            else
            {
                _loadBtn.gameObject.SetActive(false);
                _roomName.gameObject.SetActive(false);
                _privateRoom.interactable = false;
            }
        }
    }
}
