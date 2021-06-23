using ExtensionMethods;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class FinishGame : MonoBehaviour
    {
        private Text _winnerName;
        private Text _winnerStacks;

        private Button _exitRoomBtn;

        private void Awake()
        {
            _winnerName = transform.FirstOrDefault(t => t.name == "WinnerNameText").GetComponent<Text>();
            _winnerStacks = transform.FirstOrDefault(t => t.name == "WinnerStacksText").GetComponent<Text>();

            _exitRoomBtn = transform.FirstOrDefault(t => t.name == "ExitBtn").GetComponent<Button>();

            _exitRoomBtn.onClick.AddListener(LeaveRoom);
        }

        public void InitEndPanel(Tanks.TankManager tankManager)
        {
            _winnerName.text = tankManager.name;
            _winnerStacks.text = $"{tankManager.TankValues.TotalHits}";
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}

